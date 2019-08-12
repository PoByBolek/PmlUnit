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
            TestCases.Add(new TestCaseBuilder("Foo").AddTest("testOne").AddTest("testTwo").Build());
            TestCases.Add(new TestCaseBuilder("Bar").AddTest("testThree").AddTest("testFour").AddTest("testFive").Build());

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
            Assert.AreEqual("testOne", allTests[0].Test.Name);
            Assert.AreEqual("testTwo", allTests[1].Test.Name);
            Assert.AreEqual("testThree", allTests[2].Test.Name);
            Assert.AreEqual("testFour", allTests[3].Test.Name);
            Assert.AreEqual("testFive", allTests[4].Test.Name);
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
            TestCase = new TestCaseBuilder("TestCase").AddTest("one").AddTest("two").AddTest("three").AddTest("four").Build();

            RunnerMock = new Mock<TestRunner>();
            RunnerMock.Setup(runner => runner.Run(It.IsAny<Test>())).Returns(new TestResult(TimeSpan.FromSeconds(1)));

            RunnerControl = new TestRunnerControl(Mock.Of<TestCaseProvider>(), RunnerMock.Object);
            TestSummary = RunnerControl.FindControl<TestSummaryView>("TestSummary");
            TestList = RunnerControl.FindControl<TestListView>("TestList");
            TestList.SetTests(TestCase.Tests);

            var tests = TestList.AllTests;
            tests[0].Result = new TestResult(TimeSpan.FromSeconds(1));
            tests[1].Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException("error"));
            tests[2].Result = new TestResult(TimeSpan.FromSeconds(1));

            tests[1].Selected = true;
            tests[3].Selected = true;
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
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[0]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[1]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[2]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[3]), Times.Once());
        }

        [Test]
        public void RunSucceededLinkClick_RunsSucceededTests()
        {
            // Act
            RunEventHandler("OnRunSucceededTestsMenuItemClick");
            // Assert
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[0]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[1]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[2]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[3]), Times.Never());
        }

        [Test]
        public void RunFailedLinkClick_RunsFailedTests()
        {
            // Act
            RunEventHandler("OnRunFailedTestsMenuItemClick");
            // Assert
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[0]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[1]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[2]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[3]), Times.Never());
        }

        [Test]
        public void RunNotExecutedLinkClick_RunsNotExecutedTests()
        {
            // Act
            RunEventHandler("OnRunNotExecutedTestsMenuItemClick");
            // Assert
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[0]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[1]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[2]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[3]), Times.Once());
        }

        [Test]
        public void RunSelectedLinkClick_RunsSelectedTests()
        {
            // Act
            RunEventHandler("OnRunSelectedTestsMenuItemClick");
            // Assert
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[0]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[1]), Times.Once());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[2]), Times.Never());
            RunnerMock.Verify(runner => runner.Run(TestCase.Tests[3]), Times.Once());
        }

        [Test]
        public void Run_AssignsTestResultsToListEntries()
        {
            // Arrange
            var results = new List<TestResult> {
                new TestResult(TimeSpan.FromSeconds(1), new PmlException("error")), null,
                new TestResult(TimeSpan.FromSeconds(3)), new TestResult(TimeSpan.FromSeconds(4))
            };
            RunnerMock.Reset();
            for (int i = 0; i < results.Count; i++)
                SetupTestResult(TestCase.Tests[i], results[i]);
            // Act
            RunEventHandler("OnRunAllLinkClick");
            // Assert
            var tests = TestList.AllTests;
            for (int i = 0; i < results.Count; i++)
                Assert.AreSame(results[i], tests[i].Result);
        }

        private void SetupTestResult(Test test, TestResult result)
        {
            RunnerMock.Setup(runner => runner.Run(test)).Returns(result);
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
            TestCase = new TestCaseBuilder("Test").AddTest("one").AddTest("two").AddTest("three").Build();
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
            var entries = TestList.AllTests;
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
            var entries = TestList.AllTests;
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
            var entries = TestList.AllTests;
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
