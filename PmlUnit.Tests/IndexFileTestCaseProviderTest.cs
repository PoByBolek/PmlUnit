// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.IO;

using Moq;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(IndexFileTestCaseProvider))]
    public class IndexFileTestCaseProviderTest
    {
        private TestCase TestCase;

        [SetUp]
        public void Setup()
        {
            TestCase = new TestCase("dummy", "dummy.pmlobj");
        }

        [Test]
        public void Constructor_ShouldCheckForNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(""));

            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, (TextReader)null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider("", (TextReader)null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, Mock.Of<TextReader>()));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider("", Mock.Of<TextReader>()));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider("asdf", (TextReader)null));

            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, null, Mock.Of<TestCaseParser>()));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, Mock.Of<TextReader>(), null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, Mock.Of<TextReader>(), Mock.Of<TestCaseParser>()));

            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider("", null, null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider("", null, Mock.Of<TestCaseParser>()));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider("", Mock.Of<TextReader>(), null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider("", Mock.Of<TextReader>(), Mock.Of<TestCaseParser>()));


            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider("foo", null, null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider("foo", null, Mock.Of<TestCaseParser>()));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider("foo", Mock.Of<TextReader>(), null));
        }

        [Test]
        public void GetTestCases_ShouldReturnEmptyCollectionFromEmptyIndexFile()
        {
            using (var reader = new StringReader(""))
            {
                var provider = new IndexFileTestCaseProvider("base/path/", reader);
                var result = provider.GetTestCases();
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void GetTestCases_ShouldCallTestCaseParserWithObjectName()
        {
            var parser = new Mock<TestCaseParser>();
            parser.Setup(mock => mock.Parse(It.IsAny<string>())).Returns(TestCase);

            var indexFile = @"/path/to/some/tests/
pmlrandomtest.pmlobj";
            using (var reader = new StringReader(indexFile))
            {
                var provider = new IndexFileTestCaseProvider(@"C:\testing", reader, parser.Object);
                var result = provider.GetTestCases();
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result, Contains.Item(TestCase));
            }

            parser.Verify(mock => mock.Parse(@"C:\testing\path\to\some\tests\pmlrandomtest.pmlobj"));
        }

        [Test]
        public void GetTestCases_ShouldIgnoreTestFilesThatCannotBeParsed()
        {
            var parser = new Mock<TestCaseParser>();
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\tests\pmlfirsttest.pmlobj")).Throws<ParserException>();
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\tests\pmlsecondtest.pmlobj")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\tests\pmlthirdtest.pmlobj")).Throws<FileNotFoundException>();

            var indexFile = @"
/path/to/tests/
pmlfirsttest.pmlobj
pmlsecondtest.pmlobj
pmlthirdtest.pmlobj";
            using (var reader = new StringReader(indexFile))
            {
                var prodivder = new IndexFileTestCaseProvider(@"C:\testing", reader, parser.Object);
                var result = prodivder.GetTestCases();
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result, Contains.Item(TestCase));
            }
        }

        [Test]
        public void GetTestCases_ShouldOnlyAttemptToParseObjectFiles()
        {
            var parser = new Mock<TestCaseParser>(MockBehavior.Strict);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\pmltest.pmlobj")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\PmlDuplicateTest.PmlObj")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\PMLOTHERTEST.PMLOBJ")).Returns(TestCase);

            var indexFile = @"
/testing/path/
somefunc.pmlfnc
anyform.pmlfrm
pmltest.pmlobj
PmlDuplicateTest.PmlObj
PMLOTHERTEST.PMLOBJ
PmlDuplicateTest.PmlObj
somecommand.pmlcmd
amacro.pmlmac
image.png
somethingelse.txt";
            using (var reader = new StringReader(indexFile))
            {
                var provider = new IndexFileTestCaseProvider(@"C:\some\other", reader, parser.Object);
                provider.GetTestCases();
            }
        }

        [Test]
        public void GetTestCases_ShouldOnlyAttemptToParseObjectFilesThatEndInTest()
        {
            var parser = new Mock<TestCaseParser>(MockBehavior.Strict);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\OTHERTEST.PMLOBJ")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\randomtest.pmlobj")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\FoOtEsT.PmLoBj")).Returns(TestCase);

            var indexFile = @"
/testing/path/
pmlstuff.pmlobj
OTHERTEST.PMLOBJ
somehelper.pmlobj
randomtest.pmlobj
FoOtEsT.PmLoBj";
            using (var reader = new StringReader(indexFile))
            {
                var provider = new IndexFileTestCaseProvider(@"C:\some\other", reader, parser.Object);
                provider.GetTestCases();
            }
        }

        [Test]
        public void GetTestCases_ShouldTryToParseFilesFromDifferentDirectories()
        {
            var parser = new Mock<TestCaseParser>();
            parser.Setup(mock => mock.Parse(@"C:\full\path\to\the\tests\firsttest.pmlobj")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\full\path\to\some\other\tests\secondtest.pmlobj")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\full\path\to\some\other\tests\thirdtest.pmlobj")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\full\path\to\the\tests\fourthtest.pmlobj")).Returns(TestCase);

            var indexFile = @"
/the/tests/
firsttest.pmlobj

/some/other/tests/
secondtest.pmlobj
thirdtest.pmlobj

/the/tests/
fourthtest.pmlobj";
            using (var reader = new StringReader(indexFile))
            {
                var provider = new IndexFileTestCaseProvider(@"C:\full\path\to", reader, parser.Object);
                provider.GetTestCases();
            }

            parser.VerifyAll();
        }
    }
}
