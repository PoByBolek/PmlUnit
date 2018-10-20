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
            var windowMock = new Mock<WindowManager>();
            windowMock.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Control>(), It.IsAny<DockedPosition>()
            )).Returns(Mock.Of<DockedWindow>());
            var serviceMock = new Mock<ServiceManager>();
            serviceMock.Setup(manager => manager.GetService(typeof(WindowManager)))
                .Returns(windowMock.Object);
            serviceMock.Setup(manager => manager.GetService(typeof(TestRunner)))
                .Returns(new TestRunner(Mock.Of<ObjectProxy>()));
            serviceMock.Setup(manager => manager.GetService(typeof(TestCaseProvider)))
                .Returns(Mock.Of<TestCaseProvider>());
            // Act
            new ShowTestRunnerCommand(serviceMock.Object);
            // Assert
            serviceMock.VerifyAll();
        }

        [Test]
        public void Constructor_ChecksForNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null));

            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null, null, new TestRunner(Mock.Of<ObjectProxy>())));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null, Mock.Of<TestCaseProvider>(), null));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(Mock.Of<WindowManager>(), null, null));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null, Mock.Of<TestCaseProvider>(), new TestRunner(Mock.Of<ObjectProxy>())));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(Mock.Of<WindowManager>(), null, new TestRunner(Mock.Of<ObjectProxy>())));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(Mock.Of<WindowManager>(), Mock.Of<TestCaseProvider>(), null));
        }

        [Test]
        public void Constructor_CreatesRunnerControlWithSpecifiedTestProviderAndRunner()
        {
            // Arrange
            var windowMock = new Mock<WindowManager>();
            windowMock.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Control>(), It.IsAny<DockedPosition>()
            )).Returns(Mock.Of<DockedWindow>());
            var expectedProvider = Mock.Of<TestCaseProvider>();
            var expectedRunner = new TestRunner(Mock.Of<ObjectProxy>());
            // Act
            var command = new ShowTestRunnerCommand(windowMock.Object, expectedProvider, expectedRunner);
            // Assert
            var control = command.GetType().GetField(
                "RunnerControl", BindingFlags.NonPublic | BindingFlags.Instance
            ).GetValue(command);
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
            var windowMock = new Mock<WindowManager>();
            windowMock.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsNotNull<TestRunnerControl>(), It.IsAny<DockedPosition>()
            )).Returns(Mock.Of<DockedWindow>());
            // Act
            var command = new ShowTestRunnerCommand(
                windowMock.Object, Mock.Of<TestCaseProvider>(),
                new TestRunner(Mock.Of<ObjectProxy>())
            );
            // Assert
            windowMock.VerifyAll();
        }

        [Test]
        public void Execute_ShowsOrHidesWindow()
        {
            // Arrange
            var windowMock = new Mock<DockedWindow>();
            var managerMock = new Mock<WindowManager>();
            managerMock.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Control>(), It.IsAny<DockedPosition>()
            )).Returns(windowMock.Object);
            // Act & Assert
            var command = new ShowTestRunnerCommand(
                managerMock.Object, Mock.Of<TestCaseProvider>(),
                new TestRunner(Mock.Of<ObjectProxy>())
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
            var managerMock = new Mock<WindowManager>();
            managerMock.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Control>(), It.IsAny<DockedPosition>()
            )).Returns(windowMock.Object);

            var providerMock = new Mock<TestCaseProvider>();
            providerMock.Setup(provider => provider.GetTestCases())
                .Returns(new List<TestCase>());
            // Act
            var command = new ShowTestRunnerCommand(
                managerMock.Object, providerMock.Object,
                new TestRunner(Mock.Of<ObjectProxy>())
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
            var managerMock = new Mock<WindowManager>();
            managerMock.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Control>(), It.IsAny<DockedPosition>()
            )).Returns(windowMock.Object);

            var providerMock = new Mock<TestCaseProvider>();
            providerMock.Setup(provider => provider.GetTestCases())
                .Returns(new List<TestCase>());
            // Act
            var command = new ShowTestRunnerCommand(
                managerMock.Object, providerMock.Object,
                new TestRunner(Mock.Of<ObjectProxy>())
            );
            // Assert
            providerMock.Verify(provider => provider.GetTestCases(), Times.Never);
            windowMock.Raise(window => window.Shown += null, windowMock.Object, EventArgs.Empty);
            providerMock.Verify(provider => provider.GetTestCases(), Times.Once);
        }
    }
}
