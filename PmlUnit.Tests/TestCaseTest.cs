// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestCase))]
    public class TestCaseTest
    {
        [Test]
        public void Constructor_ShouldCheckForNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new TestCase(null));
            Assert.Throws<ArgumentNullException>(() => new TestCase(""));
        }

        [TestCase("hello world")]
        [TestCase("   asdf")]
        [TestCase("test.case")]
        [TestCase("foo!")]
        [TestCase("bar()")]
        public void Constructor_ShouldCheckForNonAlphaNumericCharacters(string name)
        {
            Assert.Throws<ArgumentException>(() => new TestCase(name), "Should raise ArgumentException for \"{0}\".", name);
        }

        [TestCase("Test")]
        [TestCase("SomethingElse")]
        public void Constructor_ShouldSetNameProperty(string name)
        {
            var testCase = new TestCase(name);
            Assert.AreEqual(name, testCase.Name);
        }
    }
}
