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
        private TestCase First;
        private TestCase Second;

        [SetUp]
        public void Setup()
        {
            First = new TestCase("TestCaseOne");
            First.Tests.Add("one");
            First.Tests.Add("two");
            First.Tests.Add("three");
            Second = new TestCase("TestCaseTwo");
            Second.Tests.Add("four");
            Second.Tests.Add("five");
            TestList = new TestListView();
            TestList.TestCases.Add(First);
            TestList.TestCases.Add(Second);
        }

        [TearDown]
        public void TearDown()
        {
            TestList.Dispose();
        }

        [Test]
        public void AllTests_ReturnsAllRegisteredTests()
        {
            // Act
            var tests = TestList.AllTests;
            // Assert
            Assert.AreEqual(5, tests.Count);
            Assert.AreSame(First.Tests["one"], tests[0]);
            Assert.AreSame(First.Tests["two"], tests[1]);
            Assert.AreSame(First.Tests["three"], tests[2]);
            Assert.AreSame(Second.Tests["four"], tests[3]);
            Assert.AreSame(Second.Tests["five"], tests[4]);
        }

        [Test]
        public void SucceededTests_ReturnsSucceededTests()
        {
            // Arrange
            First.Tests["three"].Result = new TestResult(TimeSpan.FromSeconds(1));
            Second.Tests["five"].Result = new TestResult(TimeSpan.FromSeconds(1));
            // Act
            var tests = TestList.SucceededTests;
            // Assert
            Assert.AreEqual(2, tests.Count);
            Assert.AreSame(First.Tests["three"], tests[0]);
            Assert.AreSame(Second.Tests["five"], tests[1]);
        }

        [Test]
        public void FailedTests_ReturnsFailedTests()
        {
            // Arrange
            First.Tests["one"].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("foo"));
            First.Tests["three"].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("bar"));
            Second.Tests["four"].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("baz"));
            // Act
            var tests = TestList.FailedTests;
            // Assert
            Assert.AreEqual(3, tests.Count);
            Assert.AreSame(First.Tests["one"], tests[0]);
            Assert.AreSame(First.Tests["three"], tests[1]);
            Assert.AreSame(Second.Tests["four"], tests[2]);
        }

        [Test]
        public void NotExecutedTests_ReturnsNotExecutedTests()
        {
            // Arrange
            First.Tests["two"].Result = new TestResult(TimeSpan.FromSeconds(1));
            Second.Tests["four"].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("error"));
            // Act
            var tests = TestList.NotExecutedTests;
            // Assert
            Assert.AreEqual(3, tests.Count);
            Assert.AreSame(First.Tests["one"], tests[0]);
            Assert.AreSame(First.Tests["three"], tests[1]);
            Assert.AreSame(Second.Tests["five"], tests[2]);
        }

        [Test]
        public void SetTests_ClearsResultOfNewTests()
        {
            foreach (var test in TestList.AllTests)
                Assert.IsNull(test.Result);
        }
    }
}
