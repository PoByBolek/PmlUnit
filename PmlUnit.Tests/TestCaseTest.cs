// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.IO;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestCase))]
    public class TestCaseTest
    {
        [Test]
        public void Constructor_ChecksForNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new TestCase(null, null));
            Assert.Throws<ArgumentNullException>(() => new TestCase(null, ""));
            Assert.Throws<ArgumentNullException>(() => new TestCase("", null));
            Assert.Throws<ArgumentNullException>(() => new TestCase("", ""));

            Assert.Throws<ArgumentNullException>(() => new TestCase(null, "not.null"));
            Assert.Throws<ArgumentNullException>(() => new TestCase("", "not.null"));
            Assert.Throws<ArgumentNullException>(() => new TestCase("NotNull", null));
            Assert.Throws<ArgumentNullException>(() => new TestCase("NotNull", ""));
        }

        [TestCase("hello world")]
        [TestCase("   asdf")]
        [TestCase("test.case")]
        [TestCase("foo!")]
        [TestCase("bar()")]
        public void Constructor_ShouldCheckForNonAlphaNumericCharacters(string name)
        {
            Assert.Throws<ArgumentException>(() => new TestCase(name, "dummy.pmlobj"), "Should raise ArgumentException for \"{0}\".", name);
        }

        [TestCase("Test")]
        [TestCase("SomethingElse")]
        public void Constructor_ShouldSetNameProperty(string name)
        {
            var testCase = new TestCase(name, "dummy.pmlobj");
            Assert.AreEqual(name, testCase.Name);
        }
        
        [TestCase("C:\\absolute\\paths\\encouraged.pmlobj", "C:\\absolute\\paths\\encouraged.pmlobj")]
        [TestCase("C:/using/forward/slashes.pmlobj", "C:\\using\\forward\\slashes.pmlobj")]
        [TestCase("Y:\\extensions\\dont.matter", "Y:\\extensions\\dont.matter")]
        [TestCase("\\relative\\to\\current\\drive.pmlobj", "\\relative\\to\\current\\drive.pmlobj")]
        [TestCase("../../../../../../../../../../../relative/to/working/directory.pmlobj", "\\relative\\to\\working\\directory.pmlobj")]
        public void Constructor_ShouldSetFileNameProperty(string fileName, string expected)
        {
            expected = Path.GetFullPath(expected);
            var testCase = new TestCase("HelloWorld", fileName);
            Assert.AreEqual(expected, testCase.FileName);
        }
    }
}
