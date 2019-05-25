// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestListView))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TestListViewTest
    {
        private TestListView TestList;

        [SetUp]
        public void Setup()
        {
            TestList = new TestListView();
        }

        [TearDown]
        public void TearDown()
        {
            TestList.Dispose();
        }

        [Test]
        public void SetTests_ChecksForNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => TestList.SetTests(null));
        }

        [Test]
        public void SetTests_AssignsTestsToList()
        {
            // Arrange
            var testCase = new TestCaseBuilder("TestCase").AddTest("one").AddTest("two").AddTest("three").Build();
            // Act
            TestList.SetTests(testCase.Tests);
            // Assert
            Assert.AreEqual(1, TestList.Controls.Count);

            var group = TestList.Controls[0] as TestListGroupEntry;
            Assert.AreEqual(testCase.Tests.Count, group.Entries.Count());
            int index = 0;
            foreach (var entry in group.Entries)
                Assert.AreSame(testCase.Tests[index++], entry.Test);
        }

        [Test]
        public void SetTests_GroupsTestsByTestCase()
        {
            // Arrange
            var first = new TestCaseBuilder("TestCaseOne").AddTest("one").AddTest("two").AddTest("three").Build();
            var second = new TestCaseBuilder("TestCaseTwo").AddTest("four").AddTest("five").Build();
            // Act
            TestList.SetTests(first.Tests.Concat(second.Tests));
            // Assert
            Assert.AreEqual(2, TestList.Controls.Count);

            var firstGroup = TestList.Controls[0] as TestListGroupEntry;
            Assert.AreEqual(first.Tests.Count, firstGroup.Entries.Count());
            int index = 0;
            foreach (var entry in firstGroup.Entries)
                Assert.AreSame(first.Tests[index++], entry.Test);

            var secondGroup = TestList.Controls[1] as TestListGroupEntry;
            Assert.AreEqual(second.Tests.Count, secondGroup.Entries.Count());
            index = 0;
            foreach (var entry in firstGroup.Entries)
                Assert.AreSame(first.Tests[index++], entry.Test);
        }

        [Test]
        public void AllTests_ReturnsAllRegisteredTests()
        {
            // Arrange
            var first = new TestCaseBuilder("TestCaseOne").AddTest("one").AddTest("two").AddTest("three").Build();
            var second = new TestCaseBuilder("TestCaseTwo").AddTest("four").AddTest("five").Build();
            TestList.SetTests(first.Tests.Concat(second.Tests));
            // Act
            var tests = TestList.AllTests;
            // Assert
            Assert.AreEqual(5, tests.Count);
            Assert.AreSame(first.Tests[0], tests[0].Test);
            Assert.AreSame(first.Tests[1], tests[1].Test);
            Assert.AreSame(first.Tests[2], tests[2].Test);
            Assert.AreSame(second.Tests[0], tests[3].Test);
            Assert.AreSame(second.Tests[1], tests[4].Test);
        }

        [Test]
        public void SucceededTests_ReturnsSucceededTests()
        {
            // Arrange
            var first = new TestCaseBuilder("TestCaseOne").AddTest("one").AddTest("two").AddTest("three").Build();
            var second = new TestCaseBuilder("TestCaseTwo").AddTest("four").AddTest("five").Build();
            TestList.SetTests(first.Tests.Concat(second.Tests));
            TestList.AllTests[2].Result = new TestResult(TimeSpan.FromSeconds(1));
            TestList.AllTests[4].Result = new TestResult(TimeSpan.FromSeconds(1));
            // Act
            var tests = TestList.SucceededTests;
            // Assert
            Assert.AreEqual(2, tests.Count);
            Assert.AreSame(first.Tests[2], tests[0].Test);
            Assert.AreSame(second.Tests[1], tests[1].Test);
        }

        [Test]
        public void FailedTests_ReturnsFailedTests()
        {
            // Arrange
            var first = new TestCaseBuilder("TestCaseOne").AddTest("one").AddTest("two").AddTest("three").Build();
            var second = new TestCaseBuilder("TestCaseTwo").AddTest("four").AddTest("five").Build();
            TestList.SetTests(first.Tests.Concat(second.Tests));
            TestList.AllTests[0].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("foo"));
            TestList.AllTests[2].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("bar"));
            TestList.AllTests[3].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("baz"));
            // Act
            var tests = TestList.FailedTests;
            // Assert
            Assert.AreEqual(3, tests.Count);
            Assert.AreSame(first.Tests[0], tests[0].Test);
            Assert.AreSame(first.Tests[2], tests[1].Test);
            Assert.AreSame(second.Tests[0], tests[2].Test);
        }

        [Test]
        public void NotExecutedTests_ReturnsNotExecutedTests()
        {
            // Arrange
            var first = new TestCaseBuilder("TestCaseOne").AddTest("one").AddTest("two").AddTest("three").Build();
            var second = new TestCaseBuilder("TestCaseTwo").AddTest("four").AddTest("five").Build();
            TestList.SetTests(first.Tests.Concat(second.Tests));
            TestList.AllTests[1].Result = new TestResult(TimeSpan.FromSeconds(1));
            TestList.AllTests[3].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("error"));
            // Act
            var tests = TestList.NotExecutedTests;
            // Assert
            Assert.AreEqual(3, tests.Count);
            Assert.AreSame(first.Tests[0], tests[0].Test);
            Assert.AreSame(first.Tests[2], tests[1].Test);
            Assert.AreSame(second.Tests[1], tests[2].Test);
        }

        [Test]
        public void SetTests_ClearsResultOfNewTests()
        {
            // Arrange
            var testCase = new TestCaseBuilder("TestCase").AddTest("one").Build();
            // Act
            TestList.SetTests(testCase.Tests);
            // Assert
            var group = TestList.Controls[0] as TestListGroupEntry;
            foreach (var entry in group.Entries)
                Assert.IsNull(entry.Result);
        }
    }
}
