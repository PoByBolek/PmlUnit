// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using Aveva.ApplicationFramework.Presentation;
using Moq;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(ShowTestRunnerCommand))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    class ShowTestRunnerCommandTest
    {
        private Mock<WindowManager> WindowManagerMock;
        private Mock<DockedWindow> WindowMock;
        private TestRunnerControl Control;
        private ShowTestRunnerCommand Command;

        [SetUp]
        public void Setup()
        {
            WindowMock = new Mock<DockedWindow>();
            WindowManagerMock = new Mock<WindowManager>();
            WindowManagerMock.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Control>(), It.IsAny<DockedPosition>()
            )).Returns((string key, string title, Control control, DockedPosition position) => {
                WindowMock.SetupGet(window => window.Control).Returns(control);
                return WindowMock.Object;
            });

            Control = new TestRunnerControl(Mock.Of<TestCaseProvider>(), Mock.Of<AsyncTestRunner>());
            Command = new ShowTestRunnerCommand(WindowManagerMock.Object, Control);
        }

        [TearDown]
        public void TearDown()
        {
            Control.Dispose();
        }

        [Test]
        public void Constructor_ChecksForNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null, null));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(null, Control));
            Assert.Throws<ArgumentNullException>(() => new ShowTestRunnerCommand(Mock.Of<WindowManager>(), null));
        }

        [Test]
        public void Constructor_CreatesWindowWithRunnerControl()
        {
            // Arrange
            WindowManagerMock.Invocations.Clear();
            // Act
            var command = new ShowTestRunnerCommand(WindowManagerMock.Object, Control);
            // Assert
            WindowManagerMock.Verify(
                manager => manager.CreateDockedWindow(
                    It.IsAny<string>(), It.IsAny<string>(), Control, It.IsAny<DockedPosition>()
                )
            );
        }

        [Test]
        public void Execute_ShowsOrHidesWindow()
        {
            Command.Checked = true;
            Command.Execute();
            WindowMock.Verify(window => window.Show());

            Command.Checked = false;
            Command.Execute();
            WindowMock.Verify(window => window.Hide());
        }

        [Test]
        public void Checked_ChangesWhenWindowIsHidden()
        {
            // Act
            Command.Checked = true;
            WindowMock.Raise(window => window.Closed += null, WindowMock.Object, EventArgs.Empty);
            // Assert
            Assert.IsFalse(Command.Checked);
        }
    }
}
