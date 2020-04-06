// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
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
            First = new TestCase("TestCaseOne", "one.pmlobj");
            First.Tests.Add("one");
            First.Tests.Add("two");
            First.Tests.Add("three");
            Second = new TestCase("TestCaseTwo", "two.pmlobj");
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
            // Assert
            var expected = First.Tests.Concat(Second.Tests);
            Assert.That(TestList.AllTests, Is.EquivalentTo(expected));
        }

        [Test]
        public void PassedTests_OnlyReturnsPassedTests()
        {
            // Arrange
            var first = First.Tests["three"];
            first.Result = new TestResult(TimeSpan.FromSeconds(1));
            var second = Second.Tests["five"];
            second.Result = new TestResult(TimeSpan.FromSeconds(1));
            // Assert
            var expected = new List<Test>() { first, second };
            Assert.That(TestList.PassedTests, Is.EquivalentTo(expected));
        }

        [Test]
        public void FailedTests_OnlyReturnsFailedTests()
        {
            // Arrange
            var first = First.Tests["one"];
            first.Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("foo"));
            var second = First.Tests["three"];
            second.Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("bar"));
            var third = Second.Tests["four"];
            third.Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("baz"));
            // Assert
            var expected = new List<Test>() { first, second, third };
            Assert.That(TestList.FailedTests, Is.EquivalentTo(expected));
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
            var expected = new List<Test>() { First.Tests["one"], First.Tests["three"], Second.Tests["five"] };
            Assert.That(TestList.NotExecutedTests, Is.EquivalentTo(expected));
        }
    }
}
