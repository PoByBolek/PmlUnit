using System;
using System.IO;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestCaseParser))]
    public class TestCaseParserTest
    {
        [Test]
        public void Parse_ShouldCheckForNullReader()
        {
            var parser = new TestCaseParser();
            Assert.Throws<ArgumentNullException>(() => parser.Parse(null));
        }

        [Test]
        public void Parse_ShouldFindTestSuiteName()
        {
            var testCase = Parse(@"define object TestCase
endobject");
            Assert.That(testCase.Name, Is.EqualTo("TestCase"));
        }

        [Test]
        public void Parse_ShouldBeCaseInsensitiveWhenSearchingForTestSuiteName()
        {
            var testCase = Parse(@"DEfinE OBJeCt CaseInsensitive
endobject");
            Assert.That(testCase.Name, Is.EqualTo("CaseInsensitive"));
        }

        [Test]
        public void Parse_ShouldIgnoreAdditionalWhitespaceWhenSearchingForTestSuiteName()
        {
            var testCase = Parse(@"   define    object   MoreWhitespace    
endobject");
            Assert.That(testCase.Name, Is.EqualTo("MoreWhitespace"));
        }

        [Test]
        public void Parse_ShouldThrowExceptionWithoutAnObjectDefinition()
        {
            Assert.Throws<ParserException>(() => Parse(""));
        }

        [Test]
        public void Parse_ShouldIgnoreCommentedObjectDefinitions()
        {
            var testCase = Parse(@"$(
define object IgnoreThisOne
$)
define object TakeThisOneInstead
endobject");
            Assert.That(testCase.Name, Is.EqualTo("TakeThisOneInstead"));
        }

        [Test]
        public void Parse_ShouldThrowExceptionWithMultipleObjectDefinitions()
        {
            Assert.Throws<ParserException>(() => Parse(@"
define object One
endobject

define object Two
endobject"));
        }

        [Test]
        public void Parse_ShouldFindOneTestCase()
        {
            var testCase = Parse(@"
define object TestCase
endobject

define method .testMethodA(!assert is PmlAssert)
endmethod");
            Assert.That(testCase.Tests.Count, Is.EqualTo(1));
            Assert.That(testCase.Tests[0].Name, Is.EqualTo("testMethodA"));
        }

        [Test]
        public void Parse_ShouldThrowExceptionForMethodsBeforeObjectDefinition()
        {
            Assert.Throws<ParserException>(() => Parse(@"
define method .testMethod(!assert is PmlAssert)
endmethod


define object TestSuite
endobject"));
        }

        [Test]
        public void Parse_ShouldIgnoreMethodsThatDoNotStartWithTest()
        {
            var testCase = Parse(@"
define object TestSuite
endobject


define method .otherMethod(!assert is PmlAssert)
endmethod");
            Assert.That(testCase.Tests.Count, Is.EqualTo(0));
        }

        [Test]
        public void Parse_ShouldIgnoreMethodsWithIncompatibleSignature()
        {
            var testCase = Parse(@"
define object TestSuite
endobject

define method .testMethod(!foo is Real)
endmethod");
            Assert.That(testCase.Tests.Count, Is.EqualTo(0));
        }

        [Test]
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

        [Test]
        public void Parse_ShouldFindMultipleTestCases()
        {
            var testCase = Parse(@"
define object TestSuite
endobject

define method .testMethodA(!assert is PmlAssert)
endmethod

define method .testMethodB(!assert is PmlAssert)
endmethod");
            Assert.That(testCase.Tests.Count, Is.EqualTo(2));
            Assert.That(testCase.Tests[0].Name, Is.EqualTo("testMethodA"));
            Assert.That(testCase.Tests[1].Name, Is.EqualTo("testMethodB"));
        }

        [Test]
        public void Parse_ShouldFindSetUpMethod()
        {
            var testCase = Parse(@"
define object TestSuite
endobject

define method .setUp()
endmethod");
            Assert.That(testCase.HasSetUp);
        }

        [Test]
        public void Parse_ShouldFindTearDownMethod()
        {
            var testCase = Parse(@"
define object TestSuite
endobject

define method .tearDown()
endmethod");
            Assert.That(testCase.HasTearDown);
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
