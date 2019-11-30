// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Diagnostics.CodeAnalysis;

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
            Assert.That(tests.Count, Is.EqualTo(5));
            Assert.That(tests, Contains.Item(First.Tests["one"]));
            Assert.That(tests, Contains.Item(First.Tests["two"]));
            Assert.That(tests, Contains.Item(First.Tests["three"]));
            Assert.That(tests, Contains.Item(Second.Tests["four"]));
            Assert.That(tests, Contains.Item(Second.Tests["five"]));
        }

        [Test]
        public void PassedTests_OnlyReturnsPassedTests()
        {
            // Arrange
            First.Tests["three"].Result = new TestResult(TimeSpan.FromSeconds(1));
            Second.Tests["five"].Result = new TestResult(TimeSpan.FromSeconds(1));
            // Act
            var tests = TestList.PassedTests;
            // Assert
            Assert.That(tests.Count, Is.EqualTo(2));
            Assert.That(tests, Contains.Item(First.Tests["three"]));
            Assert.That(tests, Contains.Item(Second.Tests["five"]));
        }

        [Test]
        public void FailedTests_OnlyReturnsFailedTests()
        {
            // Arrange
            First.Tests["one"].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("foo"));
            First.Tests["three"].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("bar"));
            Second.Tests["four"].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("baz"));
            // Act
            var tests = TestList.FailedTests;
            // Assert
            Assert.That(tests.Count, Is.EqualTo(3));
            Assert.That(tests, Contains.Item(First.Tests["one"]));
            Assert.That(tests, Contains.Item(First.Tests["three"]));
            Assert.That(tests, Contains.Item(Second.Tests["four"]));
        }

        [Test]
        public void NotExecutedTests_OnlyReturnsNotExecutedTests()
        {
            // Arrange
            First.Tests["two"].Result = new TestResult(TimeSpan.FromSeconds(1));
            Second.Tests["four"].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("error"));
            // Act
            var tests = TestList.NotExecutedTests;
            // Assert
            Assert.That(tests.Count, Is.EqualTo(3));
            Assert.That(tests, Contains.Item(First.Tests["one"]));
            Assert.That(tests, Contains.Item(First.Tests["three"]));
            Assert.That(tests, Contains.Item(Second.Tests["five"]));
        }

        [Test]
        public void SetTests_ClearsResultOfNewTests()
        {
            foreach (var test in TestList.AllTests)
                Assert.That(test.Result, Is.Null);
        }
    }
}
