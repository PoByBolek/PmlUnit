using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Moq;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(PmlTestRunner))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TestRunnerControlTest
    {
        private List<TestCase> TestCases;
        private Mock<TestCaseProvider> ProviderMock;
        private Mock<TestRunner> RunnerMock;
        private TestRunnerControl RunnerControl;
        private ListView TestList;
        private Label ResultLabel;

        [SetUp]
        public void Setup()
        {
            TestCases = new List<TestCase>();
            TestCases.Add(new TestCaseBuilder("Foo").AddTest("testOne").AddTest("testTwo").Build());
            TestCases.Add(new TestCaseBuilder("Bar").AddTest("testThree").AddTest("testFour").AddTest("testFive").Build());

            ProviderMock = new Mock<TestCaseProvider>();
            ProviderMock.Setup(provider => provider.GetTestCases()).Returns(TestCases);

            RunnerMock = new Mock<TestRunner>();
            RunnerMock.Setup(runner => runner.Run(It.IsAny<Test>())).Returns(new TestResult(TimeSpan.FromSeconds(1)));

            RunnerControl = new TestRunnerControl(ProviderMock.Object, RunnerMock.Object);
            TestList = FindControl<ListView>("TestView");
            ResultLabel = FindControl<Label>("TestResultLabel");
        }

        [TearDown]
        public void TearDown()
        {
            RunnerControl.Dispose();
        }

        [Test]
        public void Constructor_ShouldCheckForNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new TestRunnerControl(null, null));
            Assert.Throws<ArgumentNullException>(() => new TestRunnerControl(ProviderMock.Object, null));
            Assert.Throws<ArgumentNullException>(() => new TestRunnerControl(null, RunnerMock.Object));
        }

        [Test]
        public void Dispose_DisposesTestRunner()
        {
            // Act
            RunnerControl.Dispose();
            // Assert
            RunnerMock.Verify(runner => runner.Dispose());
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
            Assert.AreEqual(5, TestList.Items.Count);
            Assert.AreEqual("testOne", TestList.Items[0].Text);
            Assert.AreEqual("testTwo", TestList.Items[1].Text);
            Assert.AreEqual("testThree", TestList.Items[2].Text);
            Assert.AreEqual("testFour", TestList.Items[3].Text);
            Assert.AreEqual("testFive", TestList.Items[4].Text);
        }

        [Test]
        public void RunAllLinkClick_RunsAllTests()
        {
            // Arrange
            RunnerControl.LoadTests();
            // Act
            RunEventHandler("OnRunAllLinkClick");
            // Assert
            AssertTestsHaveRun(new HashSet<int> { 0, 1, 2, 3, 4 });
        }

        [TestCase("")]
        [TestCase("0")]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        [TestCase("0,1")]
        [TestCase("1,2")]
        [TestCase("2,3")]
        [TestCase("3,4")]
        [TestCase("1,3")]
        [TestCase("0,4")]
        [TestCase("0,2,4")]
        [TestCase("0,1,2,3,4")]
        public void RunSelectedLinkClick_RunsSelectedTests(string selectedIndices)
        {
            // these are strings so that the Visual Studio Test Runner can display them
            var selected = SplitInts(selectedIndices);

            // Arrange
            RunnerControl.LoadTests();
            SelectTests(selected);
            // Act
            RunEventHandler("OnRunSelectedTestsMenuItemClick");
            // Assert
            AssertTestsHaveRun(selected);
        }

        [TestCase("")]
        [TestCase("0")]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        [TestCase("0,1")]
        [TestCase("1,2")]
        [TestCase("2,3")]
        [TestCase("3,4")]
        [TestCase("1,3")]
        [TestCase("0,4")]
        [TestCase("0,2,4")]
        [TestCase("0,1,2,3,4")]
        public void RunNotExecutedLinkClick_RunsNotExecutedTests(string selectedIndices)
        {
            var selected = SplitInts(selectedIndices);
            // Arrange
            RunnerControl.LoadTests();
            SelectTests(selected);
            // Act
            RunEventHandler("OnRunSelectedTestsMenuItemClick");
            RunnerMock.ResetCalls();
            RunEventHandler("OnRunNotExecutedTestsMenuItemClick");
            // Assert
            var expected = new HashSet<int> { 0, 1, 2, 3, 4 };
            expected.ExceptWith(selected);
            AssertTestsHaveRun(expected);
        }

        [Test]
        public void SelectedIndexChange_DisplaysTestFailureInDetailsPanel()
        {
            // Arrange
            RunnerMock.Reset();
            RunnerMock.Setup(runner => runner.Run(It.IsAny<Test>()))
                .Returns(new TestResult(TimeSpan.FromSeconds(1), new PmlException("An error occurred")));
            RunnerControl.LoadTests();
            // Act
            RunEventHandler("OnRunAllLinkClick");
            TestList.Items[0].Selected = true;
            // Assert
            Assert.AreEqual("An error occurred", ResultLabel.Text);
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

        private void AssertTestsHaveRun(ICollection<int> indicesThatShouldHaveRun)
        {
            var tests = TestCases.SelectMany(testCase => testCase.Tests).ToList();
            for (int i = 0; i < tests.Count; i++)
            {
                var times = indicesThatShouldHaveRun.Contains(i) ? Times.Once() : Times.Never();
                RunnerMock.Verify(runner => runner.Run(tests[i]), times);
            }
        }

        private T FindControl<T>(string name) where T : Control
        {
            var controls = new Stack<Control>(RunnerControl.Controls.OfType<Control>());
            while (controls.Count > 0)
            {
                var control = controls.Pop();
                var casted = control as T;
                if (casted != null && casted.Name == name)
                {
                    return casted;
                }

                foreach (Control child in control.Controls)
                {
                    controls.Push(child);
                }
            }

            Assert.Fail("Unable to find a {0} named \"{1}\".", typeof(T), name);
            return null;
        }

        private void SelectTests(IEnumerable<int> indices)
        {
            foreach (int index in indices)
            {
                TestList.Items[index].Selected = true;
            }
        }

        private static HashSet<int> SplitInts(string values)
        {
            var result = new HashSet<int>();
            foreach (string part in values.Split(','))
            {
                if (!string.IsNullOrEmpty(part))
                {
                    result.Add(int.Parse(part));
                }
            }
            return result;
        }
    }
}
