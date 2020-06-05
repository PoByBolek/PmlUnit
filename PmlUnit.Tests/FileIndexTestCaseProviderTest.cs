// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.IO;
using System.Linq;
using Moq;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(FileIndexTestCaseProvider))]
    public class FileIndexTestCaseProviderTest
    {
        private TestCase TestCase;

        [SetUp]
        public void Setup()
        {
            TestCase = new TestCase("dummy", "dummy.pmlobj");
        }

        [Test]
        public void ValidatesArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new FileIndexTestCaseProvider(null));

            Assert.Throws<ArgumentNullException>(() => new FileIndexTestCaseProvider(null, null));
            Assert.Throws<ArgumentNullException>(() => new FileIndexTestCaseProvider(null, Mock.Of<TestCaseParser>()));
            Assert.Throws<ArgumentNullException>(() => new FileIndexTestCaseProvider(new FileIndex(), null));

            new FileIndexTestCaseProvider(new FileIndex(), Mock.Of<TestCaseParser>());
        }

        [Test]
        public void ReturnsEmptyCollectionFromEmptyIndexFile()
        {
            var index = new FileIndex(Enumerable.Empty<IndexFile>());
            var provider = new FileIndexTestCaseProvider(index);
            var result = provider.GetTestCases();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void CallsTestCaseParserWithObjectName()
        {
            var parser = new Mock<TestCaseParser>(MockBehavior.Strict);
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\some\tests\pmlrandomtest.pmlobj")).Returns(TestCase);

            var indexFile = new IndexFile();
            indexFile.Files.Add(@"C:\testing\path\to\some\tests\pmlrandomtest.pmlobj");
            var index = new FileIndex(indexFile);

            var provider = new FileIndexTestCaseProvider(index, parser.Object);
            var result = provider.GetTestCases();
            Assert.That(result, Is.EquivalentTo(Enumerable.Repeat(TestCase, 1)));

            parser.Verify();
        }

        [Test]
        public void IgnoresTestFilesThatCannotBeParsed()
        {
            var parser = new Mock<TestCaseParser>(MockBehavior.Strict);
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\tests\pmlfirsttest.pmlobj")).Throws<ParserException>();
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\tests\pmlsecondtest.pmlobj")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\tests\pmlthirdtest.pmlobj")).Throws<FileNotFoundException>();

            var indexFile = new IndexFile();
            indexFile.Files.Add(@"C:\testing\path\to\tests\pmlfirsttest.pmlobj");
            indexFile.Files.Add(@"C:\testing\path\to\tests\pmlsecondtest.pmlobj");
            indexFile.Files.Add(@"C:\testing\path\to\tests\pmlthirdtest.pmlobj");
            var index = new FileIndex(indexFile);

            var prodivder = new FileIndexTestCaseProvider(index, parser.Object);
            var result = prodivder.GetTestCases();
            Assert.That(result, Is.EquivalentTo(Enumerable.Repeat(TestCase, 1)));

            parser.Verify();
        }

        [Test]
        public void OnlyAttemptsToParseObjectFilesThatEndInTest()
        {
            var parser = new Mock<TestCaseParser>(MockBehavior.Strict);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\pmltest.pmlobj")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\nested\PmlCamelTest.PmlObj")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\nested\PMLOTHERTEST.PMLOBJ")).Returns(TestCase);
            parser.Setup(mock => mock.Parse(@"C:\some\other\testing\path\finaltest.PmLObJ")).Returns(TestCase);

            var indexFile = new IndexFile();
            indexFile.Files.Add(@"C:\some\other\testing\path\somefunc.pmlfnc");
            indexFile.Files.Add(@"C:\some\other\testing\path\anyform.pmlfrm");
            indexFile.Files.Add(@"C:\some\other\testing\path\pmltest.pmlobj");
            indexFile.Files.Add(@"C:\some\other\testing\path\nested\PmlCamelTest.PmlObj");
            indexFile.Files.Add(@"C:\some\other\testing\path\nested\PMLOTHERTEST.PMLOBJ");
            indexFile.Files.Add(@"C:\some\other\testing\path\nested\somecommand.pmlcmd");
            indexFile.Files.Add(@"C:\some\other\testing\path\nested\otherobject.pmlobj");
            indexFile.Files.Add(@"C:\some\other\testing\path\image.png");
            indexFile.Files.Add(@"C:\some\other\testing\path\somethingelse.txt");
            indexFile.Files.Add(@"C:\some\other\testing\path\finaltest.PmLObJ");
            indexFile.Files.Add(@"C:\some\other\testing\path\IGNORED.PMLOBJ");
            var index = new FileIndex(indexFile);

            var provider = new FileIndexTestCaseProvider(index, parser.Object);
            var result = provider.GetTestCases();
            Assert.That(result, Is.EquivalentTo(Enumerable.Repeat(TestCase, 4)));

            parser.Verify();
        }
    }
}
