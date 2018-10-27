using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Aveva.ApplicationFramework;
using Aveva.ApplicationFramework.Presentation;
using Moq;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(ShowTestRunnerCommand))]
    class ShowTestRunnerCommandTest
    {
        [Test]
        public void Constructor_QueriesForCorrectServiceTypes()
        {
            // Arrange
            var windowManagerMock = SetupWindowManager();
            var serviceManagerMock = new Mock<ServiceManager>();
            serviceManagerMock.Setup(manager => manager.GetService(typeof(WindowManager)))
                .Returns(windowManagerMock.Object);
            serviceManagerMock.Setup(manager => manager.GetService(typeof(TestRunner)))
                .Returns(Mock.Of<TestRunner>());
            serviceManagerMock.Setup(manager => manager.GetService(typeof(TestCaseProvider)))
                .Returns(Mock.Of<TestCaseProvider>());
            // Act
            new ShowTestRunnerCommand(serviceManagerMock.Object);
            // Assert
            serviceManagerMock.VerifyAll();
        }

        [Test]
        public void Constructor_ChecksForNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null));

            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null, null, Mock.Of<TestRunner>()));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null, Mock.Of<TestCaseProvider>(), null));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(Mock.Of<WindowManager>(), null, null));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null, Mock.Of<TestCaseProvider>(), Mock.Of<TestRunner>()));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(Mock.Of<WindowManager>(), null, Mock.Of<TestRunner>()));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(Mock.Of<WindowManager>(), Mock.Of<TestCaseProvider>(), null));
        }

        [Test]
        public void Constructor_CreatesRunnerControlWithSpecifiedTestProviderAndRunner()
        {
            // Arrange
            var windowManagerMock = SetupWindowManager();
            var expectedProvider = Mock.Of<TestCaseProvider>();
            var expectedRunner = Mock.Of<TestRunner>();
            // Act
            var command = new ShowTestRunnerCommand(
                windowManagerMock.Object, expectedProvider, expectedRunner
            );
            // Assert
            var window = command.GetType().GetField(
                "Window", BindingFlags.NonPublic | BindingFlags.Instance
            ).GetValue(command) as DockedWindow;
            var control = window.Control;
            var actualProvider = control.GetType().GetField(
                "Provider", BindingFlags.NonPublic | BindingFlags.Instance
            ).GetValue(control);
            var actualRunner = control.GetType().GetField(
                "Runner", BindingFlags.NonPublic | BindingFlags.Instance
            ).GetValue(control);
            Assert.AreSame(expectedProvider, actualProvider);
            Assert.AreSame(expectedRunner, actualRunner);
        }

        [Test]
        public void Constructor_CreatesWindowWithRunnerControl()
        {
            // Arrange
            var windowManagerMock = SetupWindowManager();
            // Act
            var command = new ShowTestRunnerCommand(
                windowManagerMock.Object, Mock.Of<TestCaseProvider>(), Mock.Of<TestRunner>()
            );
            // Assert
            windowManagerMock.VerifyAll();
        }

        [Test]
        public void Execute_ShowsOrHidesWindow()
        {
            // Arrange
            var windowMock = new Mock<DockedWindow>();
            var windowManagerMock = SetupWindowManager(windowMock);
            // Act & Assert
            var command = new ShowTestRunnerCommand(
                windowManagerMock.Object, Mock.Of<TestCaseProvider>(), Mock.Of<TestRunner>()
            );
            command.Checked = true;
            command.Execute();
            windowMock.Verify(window => window.Show());

            command.Checked = false;
            command.Execute();
            windowMock.Verify(window => window.Hide());
        }

        [Test]
        public void Checked_ChangesWhenWindowIsShownOrHidden()
        {
            // Arrange
            var windowMock = new Mock<DockedWindow>();
            var windowManagerMock = SetupWindowManager(windowMock);
            var providerMock = new Mock<TestCaseProvider>();
            providerMock.Setup(provider => provider.GetTestCases())
                .Returns(new List<TestCase>());
            // Act
            var command = new ShowTestRunnerCommand(
                windowManagerMock.Object, providerMock.Object, Mock.Of<TestRunner>()
            );
            // Assert
            command.Checked = false;
            windowMock.Raise(window => window.Shown += null, windowMock.Object, EventArgs.Empty);
            Assert.IsTrue(command.Checked);

            windowMock.Raise(window => window.Closed += null, windowMock.Object, EventArgs.Empty);
            Assert.IsFalse(command.Checked);
        }

        [Test]
        public void LoadsTestsWhenWindowOpens()
        {
            // Arrange
            var windowMock = new Mock<DockedWindow>();
            var windowManagerMock = SetupWindowManager(windowMock);
            var providerMock = new Mock<TestCaseProvider>();
            providerMock.Setup(provider => provider.GetTestCases())
                .Returns(new List<TestCase>());
            // Act
            var command = new ShowTestRunnerCommand(
                windowManagerMock.Object, providerMock.Object, Mock.Of<TestRunner>()
            );
            // Assert
            providerMock.Verify(provider => provider.GetTestCases(), Times.Never);
            windowMock.Raise(window => window.Shown += null, windowMock.Object, EventArgs.Empty);
            providerMock.Verify(provider => provider.GetTestCases(), Times.Once);
        }

        private static Mock<WindowManager> SetupWindowManager()
        {
            return SetupWindowManager(new Mock<DockedWindow>());
        }

        private static Mock<WindowManager> SetupWindowManager(Mock<DockedWindow> windowMock)
        {
            var result = new Mock<WindowManager>();
            result.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Control>(), It.IsAny<DockedPosition>()
            )).Returns((string key, string title, Control control, DockedPosition position) => {
                windowMock.SetupGet(window => window.Control).Returns(control);
                return windowMock.Object;
            });
            return result;
        }
    }
}
