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
        public void ValidatesArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null));

            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, null));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(null, Mock.Of<TestCaseParser>()));
            Assert.Throws<ArgumentNullException>(() => new IndexFileTestCaseProvider(new IndexFile(), null));

            new IndexFileTestCaseProvider(new IndexFile(), Mock.Of<TestCaseParser>());
        }

        [Test]
        public void ReturnsEmptyCollectionFromEmptyIndexFile()
        {
            var provider = new IndexFileTestCaseProvider(new IndexFile());
            var result = provider.GetTestCases();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void CallsTestCaseParserWithObjectName()
        {
            var parser = new Mock<TestCaseParser>(MockBehavior.Strict);
            parser.Setup(mock => mock.Parse(@"C:\testing\path\to\some\tests\pmlrandomtest.pmlobj")).Returns(TestCase);

            var index = new IndexFile();
            index.Files.Add(@"C:\testing\path\to\some\tests\pmlrandomtest.pmlobj");

            var provider = new IndexFileTestCaseProvider(index, parser.Object);
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

            var index = new IndexFile();
            index.Files.Add(@"C:\testing\path\to\tests\pmlfirsttest.pmlobj");
            index.Files.Add(@"C:\testing\path\to\tests\pmlsecondtest.pmlobj");
            index.Files.Add(@"C:\testing\path\to\tests\pmlthirdtest.pmlobj");
            
            var prodivder = new IndexFileTestCaseProvider(index, parser.Object);
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

            var index = new IndexFile();
            index.Files.Add(@"C:\some\other\testing\path\somefunc.pmlfnc");
            index.Files.Add(@"C:\some\other\testing\path\anyform.pmlfrm");
            index.Files.Add(@"C:\some\other\testing\path\pmltest.pmlobj");
            index.Files.Add(@"C:\some\other\testing\path\nested\PmlCamelTest.PmlObj");
            index.Files.Add(@"C:\some\other\testing\path\nested\PMLOTHERTEST.PMLOBJ");
            index.Files.Add(@"C:\some\other\testing\path\nested\somecommand.pmlcmd");
            index.Files.Add(@"C:\some\other\testing\path\nested\otherobject.pmlobj");
            index.Files.Add(@"C:\some\other\testing\path\image.png");
            index.Files.Add(@"C:\some\other\testing\path\somethingelse.txt");
            index.Files.Add(@"C:\some\other\testing\path\finaltest.PmLObJ");
            index.Files.Add(@"C:\some\other\testing\path\IGNORED.PMLOBJ");

            var provider = new IndexFileTestCaseProvider(index, parser.Object);
            var result = provider.GetTestCases();
            Assert.That(result, Is.EquivalentTo(Enumerable.Repeat(TestCase, 4)));

            parser.Verify();
        }
    }
}
