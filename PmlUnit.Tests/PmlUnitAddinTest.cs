using System.IO;
using System.Windows.Forms;
using Aveva.ApplicationFramework;
using Aveva.ApplicationFramework.Presentation;
using Moq;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(PmlUnitAddin))]
    class PmlUnitAddinTest
    {
        [Test]
        public void Start_RegistersTestCaseProvider()
        {
            // Arrange
            var mock = new Mock<ServiceManager>();
            var addin = new PmlUnitAddin();
            // Act
            addin.Start(mock.Object, Mock.Of<TestRunner>());
            // Assert
            mock.Verify(manager => manager.AddService(
                typeof(TestCaseProvider), It.IsNotNull<TestCaseProvider>()
            ));
        }

        [Test]
        public void Start_RegistersTestRunner()
        {
            // Arrange
            var mock = new Mock<ServiceManager>();
            var runner = Mock.Of<TestRunner>();
            var addin = new PmlUnitAddin();
            // Act
            addin.Start(mock.Object, runner);
            // Assert
            mock.Verify(manager => manager.AddService(typeof(TestRunner), runner));
        }

        [Test]
        public void Start_AddsUiCustomizationFile()
        {
            // Arrange
            var commandBarMock = new Mock<CommandBarManager>();
            var serviceMock = new Mock<ServiceManager>();
            serviceMock.Setup(manager => manager.GetService(typeof(CommandBarManager)))
                .Returns(commandBarMock.Object);
            var addin = new PmlUnitAddin();
            // Act
            addin.Start(serviceMock.Object, Mock.Of<TestRunner>());
            // Assert
            commandBarMock.Verify(
                manager => manager.AddUICustomizationFromStream(It.IsNotNull<Stream>(), "PmlUnit")
            );
        }

        [Test]
        public void Start_AddsTestRunnerCommand()
        {
            // Arrange
            var commandMock = new Mock<CommandManager>();
            commandMock.Setup(manager => manager.Commands.Add(It.IsAny<Command>()));
            var serviceMock = new Mock<ServiceManager>();
            serviceMock.Setup(manager => manager.GetService(typeof(CommandManager)))
                .Returns(commandMock.Object);

            var windowMock = new Mock<WindowManager>();
            windowMock.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Control>(), It.IsAny<DockedPosition>()
            )).Returns(Mock.Of<DockedWindow>());
            serviceMock.Setup(manager => manager.GetService(typeof(WindowManager)))
                .Returns(windowMock.Object);

            serviceMock.Setup(manager => manager.GetService(typeof(TestCaseProvider)))
                .Returns(Mock.Of<TestCaseProvider>());
            serviceMock.Setup(manager => manager.GetService(typeof(TestRunner)))
                .Returns(Mock.Of<TestRunner>());

            var addin = new PmlUnitAddin();
            // Act
            addin.Start(serviceMock.Object, Mock.Of<TestRunner>());
            // Assert
            commandMock.Verify(
                manager => manager.Commands.Add(It.IsNotNull<ShowTestRunnerCommand>())
            );
        }
    }
}
