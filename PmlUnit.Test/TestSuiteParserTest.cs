using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PmlUnit.Test
{
    [TestClass]
    public class TestSuiteParserTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_ShouldCheckForNullReader()
        {
            var parser = new TestSuiteParser();
            parser.Parse(null);
        }

        [TestMethod]
        public void Parse_ShouldFindTestSuiteName()
        {
            var suite = Parse(@"define object TestSuite
endobject");
            Assert.AreEqual("TestSuite", suite.Name);
        }

        [TestMethod]
        public void Parse_ShouldBeCaseInsensitiveWhenSearchingForTestSuiteName()
        {
            var suite = Parse(@"DEfinE OBJeCt CaseInsensitive
endobject");
            Assert.AreEqual("CaseInsensitive", suite.Name);
        }

        [TestMethod]
        public void Parse_ShouldIgnoreAdditionalWhitespaceWhenSearchingForTestSuiteName()
        {
            var suite = Parse(@"   define    object   MoreWhitespace    
endobject");
            Assert.AreEqual("MoreWhitespace", suite.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void Parse_ShouldThrowExceptionWithoutAnObjectDefinition()
        {
            Parse("");
        }

        [TestMethod]
        public void Parse_ShouldIgnoreCommentedObjectDefinitions()
        {
            var suite = Parse(@"$(
define object IgnoreThisOne
$)
define object TakeThisOneInstead
endobject");
            Assert.AreEqual("TakeThisOneInstead", suite.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void Parse_ShouldThrowExceptionWithMultipleObjectDefinitions()
        {
            Parse(@"
define object One
endobject

define object Two
endobject");
        }

        [TestMethod]
        public void Parse_ShouldFindOneTestCase()
        {
            var suite = Parse(@"
define object TestCase
endobject

define method .testMethodA(!assert is PmlAssert)
endmethod");
            Assert.AreEqual(1, suite.TestCases.Count);
            Assert.AreEqual("testMethodA", suite.TestCases[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void Parse_ShouldThrowExceptionForMethodsBeforeObjectDefinition()
        {
            Parse(@"
define method .testMethod(!assert is PmlAssert)
endmethod


define object TestSuite
endobject");
        }

        [TestMethod]
        public void Parse_ShouldIgnoreMethodsThatDoNotStartWithTest()
        {
            var suite = Parse(@"
define object TestSuite
endobject


define method .otherMethod(!assert is PmlAssert)
endmethod");
            Assert.AreEqual(0, suite.TestCases.Count);
        }

        [TestMethod]
        public void Parse_ShouldIgnoreMethodsWithIncompatibleSignature()
        {
            var suite = Parse(@"
define object TestSuite
endobject

define method .testMethod(!foo is Real)
endmethod");
            Assert.AreEqual(0, suite.TestCases.Count);
        }

        [TestMethod]
        public void Parse_ShouldNotConsiderArgumentNamesWhenDeterminingCompatibleMethodSignatures()
        {
            var suite = Parse(@"
define object TestSuite
endobject

define method .testMethod(!somethingNotNamedAssert is PmlAssert)
endmethod");
            Assert.AreEqual(1, suite.TestCases.Count);
            Assert.AreEqual("testMethod", suite.TestCases[0].Name);
        }

        [TestMethod]
        public void Parse_ShouldFindMultipleTestCases()
        {
            var suite = Parse(@"
define object TestSuite
endobject

define method .testMethodA(!assert is PmlAssert)
endmethod

define method .testMethodB(!assert is PmlAssert)
endmethod");
            Assert.AreEqual(2, suite.TestCases.Count);
            Assert.AreEqual("testMethodA", suite.TestCases[0].Name);
            Assert.AreEqual("testMethodB", suite.TestCases[1].Name);
        }

        [TestMethod]
        public void Parse_ShouldFindSetUpMethod()
        {
            var suite = Parse(@"
define object TestSuite
endobject

define method .setUp()
endmethod");
            Assert.IsTrue(suite.HasSetUpMethod);
        }

        [TestMethod]
        public void Parse_ShouldFindTearDownMethod()
        {
            var suite = Parse(@"
define object TestSuite
endobject

define method .tearDown()
endmethod");
            Assert.IsTrue(suite.HasTearDownMethod);
        }

        private static TestSuite Parse(string objectDefinition)
        {
            var parser = new TestSuiteParser();
            using (var reader = new StringReader(objectDefinition))
            {
                return parser.Parse(reader);
            }
        }
    }
}
