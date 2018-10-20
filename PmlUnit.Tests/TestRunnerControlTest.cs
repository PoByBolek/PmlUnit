using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
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
            // Arrange
            var runnerMock = new Mock<TestRunner>();
            var control = new TestRunnerControl(Mock.Of<TestCaseProvider>(), runnerMock.Object);
            // Act
            control.Dispose();
            // Assert
            runnerMock.Verify(runner => runner.Dispose());
        }

        [Test]
        public void LoadTests_LoadsTestsFromTheTestCaseProvider()
        {
            // Arrange
            var providerMock = new Mock<TestCaseProvider>();
            providerMock.Setup(provider => provider.GetTestCases())
                .Returns(new List<TestCase>());
            var control = new TestRunnerControl(providerMock.Object, Mock.Of<TestRunner>());
            // Act
            control.LoadTests();
            // Assert
            providerMock.Verify(provider => provider.GetTestCases());
        }

        [Test]
        public void LoadTests_AddsTestsToList()
        {
            // Arrange
            var testCases = new List<TestCase>();
            testCases.Add(new TestCaseBuilder("Foo").AddTest("testOne").AddTest("testTwo").Build());
            testCases.Add(new TestCaseBuilder("Bar").AddTest("testThree").AddTest("testFour").AddTest("testFive").Build());
            var providerMock = new Mock<TestCaseProvider>();
            providerMock.Setup(provider => provider.GetTestCases())
                .Returns(testCases);
            var control = new TestRunnerControl(providerMock.Object, Mock.Of<TestRunner>());
            // Act
            control.LoadTests();
            // Assert
            var list = control.GetType().GetField(
                "TestView", BindingFlags.NonPublic | BindingFlags.Instance
            ).GetValue(control) as ListView;
            Assert.AreEqual(5, list.Items.Count);
            Assert.AreEqual("testOne", list.Items[0].Text);
            Assert.AreEqual("testTwo", list.Items[1].Text);
            Assert.AreEqual("testThree", list.Items[2].Text);
            Assert.AreEqual("testFour", list.Items[3].Text);
            Assert.AreEqual("testFive", list.Items[4].Text);
        }
    }
}
