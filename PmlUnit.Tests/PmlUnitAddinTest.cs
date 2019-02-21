// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Forms;
using Aveva.ApplicationFramework;
using Aveva.ApplicationFramework.Presentation;
using Moq;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(PmlUnitAddin))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    class PmlUnitAddinTest
    {
        private Mock<CommandManager> CommandManagerMock;
        private Mock<ServiceManager> ServiceManagerMock;
        private Mock<TestRunner> TestRunnerMock;
        private PmlUnitAddin Addin;

        [SetUp]
        public void Setup()
        {
            CommandManagerMock = new Mock<CommandManager>();
            CommandManagerMock.Setup(manager => manager.Commands.Add(It.IsAny<Command>()));

            var windowManagerMock = new Mock<WindowManager>();
            windowManagerMock.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Control>(), It.IsAny<DockedPosition>()
            )).Returns(Mock.Of<DockedWindow>());

            ServiceManagerMock = new Mock<ServiceManager>();
            ServiceManagerMock.Setup(manager => manager.GetService(typeof(CommandManager)))
                .Returns(CommandManagerMock.Object);
            ServiceManagerMock.Setup(manager => manager.GetService(typeof(WindowManager)))
                .Returns(windowManagerMock.Object);

            TestRunnerMock = new Mock<TestRunner>();
            Addin = new PmlUnitAddin(TestRunnerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            Addin.Dispose();
        }

        [Test]
        public void Start_RegistersTestCaseProvider()
        {
            // Act
            Addin.Start(ServiceManagerMock.Object);
            // Assert
            ServiceManagerMock.Verify(
                manager => manager.AddService(typeof(TestCaseProvider), It.IsNotNull<TestCaseProvider>())
            );
        }

        [Test]
        public void Start_RegistersTestRunner()
        {
            // Act
            Addin.Start(ServiceManagerMock.Object);
            // Assert
            ServiceManagerMock.Verify(
                manager => manager.AddService(typeof(TestRunner), TestRunnerMock.Object)
            );
        }

        [Test]
        public void Start_AddsTestRunnerCommand()
        {
            // Act
            Addin.Start(ServiceManagerMock.Object);
            // Assert
            CommandManagerMock.Verify(
                manager => manager.Commands.Add(It.IsNotNull<ShowTestRunnerCommand>())
            );
        }

        [Test]
        public void Stop_DisposesTestRunner()
        {
            // Act
            Addin.Stop();
            // Assert
            TestRunnerMock.Verify(runner => runner.Dispose());
        }

        [Test]
        public void Stop_DoesNotDisposesTestRunnerControl()
        {
            // Act
            Addin.Stop();
            // Assert
            var control = Addin.GetType()
                .GetField("RunnerControl", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(Addin) as Control;
            Assert.IsFalse(control.IsDisposed);
        }
    }
}
