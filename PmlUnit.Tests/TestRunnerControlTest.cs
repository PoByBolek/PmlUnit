// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Moq;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(PmlTestRunner))]
    public class TestRunnerControlTest
    {
        [Test]
        public void Constructor_ShouldCheckForNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new TestRunnerControl(null, null));
            Assert.Throws<ArgumentNullException>(() => new TestRunnerControl(Mock.Of<TestCaseProvider>(), null));
            Assert.Throws<ArgumentNullException>(() => new TestRunnerControl(null, Mock.Of<TestRunner>()));
        }

        [Test]
        public void Dispose_DisposesTestRunner()
        {
            var runnerMock = new Mock<TestRunner>();
            var control = new TestRunnerControl(Mock.Of<TestCaseProvider>(), runnerMock.Object);
            // Act
            control.Dispose();
            // Assert
            runnerMock.Verify(runner => runner.Dispose());
        }
    }

    [TestFixture]
    [TestOf(typeof(PmlTestRunner))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TestRunnerControlProviderTest
    {
        private List<TestCase> TestCases;
        private Mock<TestCaseProvider> ProviderMock;
        private TestRunnerControl RunnerControl;
        private TestListView TestList;

        [SetUp]
        public void Setup()
        {
            TestCases = new List<TestCase>();
            var first = new TestCase("Foo");
            first.Tests.Add("one");
            first.Tests.Add("two");
            TestCases.Add(first);
            var second = new TestCase("Bar");
            second.Tests.Add("three");
            second.Tests.Add("four");
            second.Tests.Add("five");
            TestCases.Add(second);

            ProviderMock = new Mock<TestCaseProvider>();
            ProviderMock.Setup(provider => provider.GetTestCases()).Returns(TestCases);

            RunnerControl = new TestRunnerControl(ProviderMock.Object, Mock.Of<TestRunner>());
            TestList = RunnerControl.FindControl<TestListView>("TestList");
        }

        [TearDown]
        public void TearDown()
        {
            RunnerControl.Dispose();
        }

        [Test]
        public void LoadTests_LoadsTestsFromTheTestCaseProvider()
        {
            // Act
            RunnerControl.LoadTests();
            // Assert
            ProviderMock.Verify(provider => provider.GetTestCases());
        }

        [Test]
        public void LoadTests_AddsTestsToList()
        {
            // Act
            RunnerControl.LoadTests();
            // Assert
            var allTests = TestList.AllTests;
            Assert.AreEqual(5, allTests.Count);
            Assert.AreSame(TestCases[0].Tests["one"], allTests[0]);
            Assert.AreSame(TestCases[0].Tests["two"], allTests[1]);
            Assert.AreSame(TestCases[1].Tests["three"], allTests[2]);
            Assert.AreSame(TestCases[1].Tests["four"], allTests[3]);
            Assert.AreSame(TestCases[1].Tests["five"], allTests[4]);
        }
    }

    [TestFixture]
    [TestOf(typeof(PmlTestRunner))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TestRunnerControlRunTest
    {
        private TestCase TestCase;
        private Mock<TestRunner> RunnerMock;
        private TestRunnerControl RunnerControl;
        private TestListView TestList;
        private TestSummaryView TestSummary;

        [SetUp]
        public void Setup()
        {
            TestCase = new TestCase("TestCase");
            TestCase.Tests.Add("one").Result = new TestResult(TimeSpan.FromSeconds(1));
            TestCase.Tests.Add("two").Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("error"));
            TestCase.Tests.Add("three").Result = new TestResult(TimeSpan.FromSeconds(1));
            TestCase.Tests.Add("four");

            RunnerMock = new Mock<TestRunner>();
            RunnerMock.Setup(runner => runner.Run(It.IsAny<Test>())).Returns(new TestResult(TimeSpan.FromSeconds(1)));

            RunnerControl = new TestRunnerControl(Mock.Of<TestCaseProvider>(), RunnerMock.Object);
            TestSummary = RunnerControl.FindControl<TestSummaryView>("TestSummary");
            TestList = RunnerControl.FindControl<TestListView>("TestList");
            TestList.SetTests(TestCase.Tests);

            var entries = TestList.AllTestEntries;
            entries[1].Selected = true;
            entries[3].Selected = true;
        }

        [TearDown]
        public void TearDown()
        {
            RunnerControl.Dispose();
        }

        [Test]
        public void RunAllLinkClick_RunsAllTests()
        {
            // Act
            RunEventHandler("OnRunAllLinkClick");
            // Assert
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["one"]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["two"]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["three"]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["four"]), Times.Once());
        }

        [Test]
        public void RunSucceededLinkClick_RunsSucceededTests()
        {
            // Act
            RunEventHandler("OnRunSucceededTestsMenuItemClick");
            // Assert
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["one"]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["two"]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["three"]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["four"]), Times.Never());
        }

        [Test]
        public void RunFailedLinkClick_RunsFailedTests()
        {
            // Act
            RunEventHandler("OnRunFailedTestsMenuItemClick");
            // Assert
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["one"]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["two"]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["three"]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["four"]), Times.Never());
        }

        [Test]
        public void RunNotExecutedLinkClick_RunsNotExecutedTests()
        {
            // Act
            RunEventHandler("OnRunNotExecutedTestsMenuItemClick");
            // Assert
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["one"]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["two"]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["three"]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["four"]), Times.Once());
        }

        [Test]
        public void RunSelectedLinkClick_RunsSelectedTests()
        {
            // Act
            RunEventHandler("OnRunSelectedTestsMenuItemClick");
            // Assert
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["one"]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["two"]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["three"]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests["four"]), Times.Once());
        }

        private void RunEventHandler(string handler)
        {
            var method = RunnerControl.GetType().GetMethod(
                handler, BindingFlags.Instance | BindingFlags.NonPublic, null,
                new Type[] { typeof(object), typeof(EventArgs) }, null
            );
            if (method == null)
            {
                Assert.Fail("Unable to find {0} event handler.", handler);
            }

            method.Invoke(RunnerControl, new object[] { null, EventArgs.Empty });
        }
    }


    [TestFixture]
    [TestOf(typeof(PmlTestRunner))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TestRunnerControlSelectionTest
    {
        private TestCase TestCase;
        private TestRunnerControl RunnerControl;
        private TestListView TestList;
        private TestSummaryView TestSummary;
        private TestDetailsView TestDetails;

        [SetUp]
        public void Setup()
        {
            TestCase = new TestCase("Test");
            TestCase.Tests.Add("one");
            TestCase.Tests.Add("two");
            TestCase.Tests.Add("three");
            RunnerControl = new TestRunnerControl(Mock.Of<TestCaseProvider>(), Mock.Of<TestRunner>());
            TestSummary = RunnerControl.FindControl<TestSummaryView>("TestSummary");
            TestDetails = RunnerControl.FindControl<TestDetailsView>("TestDetails");
            TestList = RunnerControl.FindControl<TestListView>("TestList");
            TestList.SetTests(TestCase.Tests);
        }

        [TearDown]
        public void TearDown()
        {
            RunnerControl.Dispose();
        }

        [Test]
        public void SelectionChanged_AssignsTestOfSingleSelectedEntryToTestDetails()
        {
            // Arrange
            var entries = TestList.AllTestEntries;
            // Act & Assert
            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].Selected = true;
                Assert.AreSame(TestDetails.Test, entries[i].Test, "Should assign test {0}.", i);
                entries[i].Selected = false;
            }
        }

        [Test]
        public void SelectionChanged_ShowsTestDetailsIfExactlyOneEntryIsSelected()
        {
            // Arrange
            var entries = TestList.AllTestEntries;
            // Act & Assert
            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].Selected = true;
                Assert.IsTrue(TestDetails.Visible, "Should show test details for test {0}", i);
                Assert.IsFalse(TestSummary.Visible, "Should not show test summary for test {0}", i);
                entries[i].Selected = false;
            }
        }

        [Test]
        public void SelectionChanged_ShowsTestSummaryUnlessExactlyOneEntryIsSelected()
        {
            // Arrange
            var entries = TestList.AllTestEntries;
            // Act & Assert
            Assert.IsFalse(TestDetails.Visible);
            Assert.IsTrue(TestSummary.Visible);

            entries[0].Selected = true;
            entries[1].Selected = true;
            entries[2].Selected = true;
            Assert.IsFalse(TestDetails.Visible);
            Assert.IsTrue(TestSummary.Visible);

            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].Selected = false;
                Assert.IsFalse(TestDetails.Visible, "Should not show test details for test {0}", i);
                Assert.True(TestSummary.Visible, "Should show test summary for test {0}", i);
                entries[i].Selected = true;
            }
        }
    }
}
