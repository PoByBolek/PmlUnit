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
        [Test]
        public void Constructor_ShouldCheckForNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider((TextReader)null, null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider((TextReader)null, ""));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(Mock.Of<TextReader>(), null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(Mock.Of<TextReader>(), ""));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider((TextReader)null, "asdf"));

            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, null, Mock.Of<TestCaseParser>()));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(Mock.Of<TextReader>(), null, null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(Mock.Of<TextReader>(), null, Mock.Of<TestCaseParser>()));

            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, "", null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, "", Mock.Of<TestCaseParser>()));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(Mock.Of<TextReader>(), "", null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(Mock.Of<TextReader>(), "", Mock.Of<TestCaseParser>()));


            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, "foo", null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, "foo", Mock.Of<TestCaseParser>()));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(Mock.Of<TextReader>(), "foo", null));
        }

        [Test]
        public void GetTestCases_ShouldReturnEmptyCollectionFromEmptyIndexFile()
        {
            using (var reader = new StringReader(""))
            {
                var provider = new IndexFileTestCaseProvider(reader, "base/path/");
                var result = provider.GetTestCases();
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void GetTestCases_ShouldCallTestCaseParserWithObjectName()
        {
            var dummy = new TestCaseBuilder("dummy").Build();
            var parser = new Mock<TestCaseParser>();
            parser.Setup(mock => mock.Parse(It.IsAny<string>())).Returns(dummy);

            var indexFile = @"/path/to/some/tests/
pmlrandomtest.pmlobj";
            using (var reader = new StringReader(indexFile))
            {
                var provider = new IndexFileTestCaseProvider(reader, @"C:\testing", parser.Object);
                var result = provider.GetTestCases();
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result, Contains.Item(dummy));
            }

            parser.Verify(mock => mock.Parse(@"C:\testing\path\to\some\tests\pmlrandomtest.pmlobj"));
        }

        [Test]
        public void GetTestCases_ShouldIgnoreTestFilesThatCannotBeParsed()
        {
            var dummy = new TestCaseBuilder("dummy").Build();
            var parser = new Mock<TestCaseParser>();
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\tests\pmlfirsttest.pmlobj")).Throws<ParserException>();
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\tests\pmlsecondtest.pmlobj")).Returns(dummy);
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\tests\pmlthirdtest.pmlobj")).Throws<FileNotFoundException>();

            var indexFile = @"
/path/to/tests/
pmlfirsttest.pmlobj
pmlsecondtest.pmlobj
pmlthirdtest.pmlobj";
            using (var reader = new StringReader(indexFile))
            {
                var prodivder = new IndexFileTestCaseProvider(reader, @"C:\testing", parser.Object);
                var result = prodivder.GetTestCases();
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result, Contains.Item(dummy));
            }
        }

        [Test]
        public void GetTestCases_ShouldOnlyAttemptToParseObjectFiles()
        {
            var dummy = new TestCaseBuilder("dummy").Build();
            var parser = new Mock<TestCaseParser>(MockBehavior.Strict);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\pmltest.pmlobj")).Returns(dummy);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\PmlDuplicateTest.PmlObj")).Returns(dummy);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\PMLOTHERTEST.PMLOBJ")).Returns(dummy);

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
                var provider = new IndexFileTestCaseProvider(reader, @"C:\some\other", parser.Object);
                provider.GetTestCases();
            }
        }

        [Test]
        public void GetTestCases_ShouldOnlyAttemptToParseObjectFilesThatEndInTest()
        {
            var dummy = new TestCaseBuilder("dummy").Build();
            var parser = new Mock<TestCaseParser>(MockBehavior.Strict);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\OTHERTEST.PMLOBJ")).Returns(dummy);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\randomtest.pmlobj")).Returns(dummy);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\FoOtEsT.PmLoBj")).Returns(dummy);

            var indexFile = @"
/testing/path/
pmlstuff.pmlobj
OTHERTEST.PMLOBJ
somehelper.pmlobj
randomtest.pmlobj
FoOtEsT.PmLoBj";
            using (var reader = new StringReader(indexFile))
            {
                var provider = new IndexFileTestCaseProvider(reader, @"C:\some\other", parser.Object);
                provider.GetTestCases();
            }
        }

        [Test]
        public void GetTestCases_ShouldTryToParseFilesFromDifferentDirectories()
        {
            var dummy = new TestCaseBuilder("dummy").Build();
            var parser = new Mock<TestCaseParser>();
            parser.Setup(mock => mock.Parse(@"C:\full\path\to\the\tests\firsttest.pmlobj")).Returns(dummy);
            parser.Setup(mock => mock.Parse(@"C:\full\path\to\some\other\tests\secondtest.pmlobj")).Returns(dummy);
            parser.Setup(mock => mock.Parse(@"C:\full\path\to\some\other\tests\thirdtest.pmlobj")).Returns(dummy);
            parser.Setup(mock => mock.Parse(@"C:\full\path\to\the\tests\fourthtest.pmlobj")).Returns(dummy);

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
                var provider = new IndexFileTestCaseProvider(reader, @"C:\full\path\to", parser.Object);
                provider.GetTestCases();
            }

            parser.VerifyAll();
        }
    }
}
