using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PmlUnit.Test
{
    [TestClass]
    public class TestCaseParserTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_ShouldCheckForNullReader()
        {
            var parser = new TestCaseParser();
            parser.Parse(null);
        }

        [TestMethod]
        public void Parse_ShouldFindTestSuiteName()
        {
            var testCase = Parse(@"define object TestCase
endobject");
            Assert.AreEqual("TestCase", testCase.Name);
        }

        [TestMethod]
        public void Parse_ShouldBeCaseInsensitiveWhenSearchingForTestSuiteName()
        {
            var testCase = Parse(@"DEfinE OBJeCt CaseInsensitive
endobject");
            Assert.AreEqual("CaseInsensitive", testCase.Name);
        }

        [TestMethod]
        public void Parse_ShouldIgnoreAdditionalWhitespaceWhenSearchingForTestSuiteName()
        {
            var testCase = Parse(@"   define    object   MoreWhitespace    
endobject");
            Assert.AreEqual("MoreWhitespace", testCase.Name);
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
            var testCase = Parse(@"$(
define object IgnoreThisOne
$)
define object TakeThisOneInstead
endobject");
            Assert.AreEqual("TakeThisOneInstead", testCase.Name);
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
            var testCase = Parse(@"
define object TestCase
endobject

define method .testMethodA(!assert is PmlAssert)
endmethod");
            Assert.AreEqual(1, testCase.Tests.Count);
            Assert.AreEqual("testMethodA", testCase.Tests[0].Name);
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
            var testCase = Parse(@"
define object TestSuite
endobject


define method .otherMethod(!assert is PmlAssert)
endmethod");
            Assert.AreEqual(0, testCase.Tests.Count);
        }

        [TestMethod]
        public void Parse_ShouldIgnoreMethodsWithIncompatibleSignature()
        {
            var testCase = Parse(@"
define object TestSuite
endobject

define method .testMethod(!foo is Real)
endmethod");
            Assert.AreEqual(0, testCase.Tests.Count);
        }

        [TestMethod]
        public void Parse_ShouldNotConsiderArgumentNamesWhenDeterminingCompatibleMethodSignatures()
        {
            var testCase = Parse(@"
define object TestSuite
endobject

define method .testMethod(!somethingNotNamedAssert is PmlAssert)
endmethod");
            Assert.AreEqual(1, testCase.Tests.Count);
            Assert.AreEqual("testMethod", testCase.Tests[0].Name);
        }

        [TestMethod]
        public void Parse_ShouldFindMultipleTestCases()
        {
            var testCase = Parse(@"
define object TestSuite
endobject

define method .testMethodA(!assert is PmlAssert)
endmethod

define method .testMethodB(!assert is PmlAssert)
endmethod");
            Assert.AreEqual(2, testCase.Tests.Count);
            Assert.AreEqual("testMethodA", testCase.Tests[0].Name);
            Assert.AreEqual("testMethodB", testCase.Tests[1].Name);
        }

        [TestMethod]
        public void Parse_ShouldFindSetUpMethod()
        {
            var testCase = Parse(@"
define object TestSuite
endobject

define method .setUp()
endmethod");
            Assert.IsTrue(testCase.HasSetUp);
        }

        [TestMethod]
        public void Parse_ShouldFindTearDownMethod()
        {
            var testCase = Parse(@"
define object TestSuite
endobject

define method .tearDown()
endmethod");
            Assert.IsTrue(testCase.HasTearDown);
        }

        private static TestCase Parse(string objectDefinition)
        {
            var parser = new TestCaseParser();
            using (var reader = new StringReader(objectDefinition))
            {
                return parser.Parse(reader);
            }
        }
    }
}
