﻿// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Forms;
using Aveva.ApplicationFramework.Presentation;
using Moq;
using NUnit.Framework;

#if PDMS || E3D_11
using ICommandManager = Aveva.ApplicationFramework.Presentation.CommandManager;
using IWindowManager = Aveva.ApplicationFramework.Presentation.WindowManager;
#endif

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(PmlUnitAddin))]
    [Apartment(ApartmentState.STA)]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    class PmlUnitAddinTest
    {
        private Mock<ICommandManager> CommandManagerMock;
        private Mock<ServiceProvider> ServiceProviderMock;
        private Mock<AsyncTestRunner> TestRunnerMock;
        private Mock<TestCaseProvider> TestCaseProviderMock;
        private PmlUnitAddin Addin;

        [SetUp]
        public void Setup()
        {
            CommandManagerMock = new Mock<ICommandManager>();
            CommandManagerMock.Setup(manager => manager.Commands.Add(It.IsAny<Command>()));

            var windowManagerMock = new Mock<IWindowManager>();
            windowManagerMock.Setup(manager => manager.CreateDockedWindow(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Control>(), It.IsAny<DockedPosition>()
            )).Returns(Mock.Of<DockedWindow>());

            ServiceProviderMock = new Mock<ServiceProvider>();
            ServiceProviderMock.Setup(provider => provider.GetService<ICommandManager>())
                .Returns(CommandManagerMock.Object);
            ServiceProviderMock.Setup(provider => provider.GetService<IWindowManager>())
                .Returns(windowManagerMock.Object);

            TestCaseProviderMock = new Mock<TestCaseProvider>();
            TestCaseProviderMock.Setup(provider => provider.GetTestCases())
                .Returns(new List<TestCase>());
            TestRunnerMock = new Mock<AsyncTestRunner>();
            Addin = new PmlUnitAddin(TestCaseProviderMock.Object, TestRunnerMock.Object, Mock.Of<SettingsProvider>());
        }

        [TearDown]
        public void TearDown()
        {
            Addin.Dispose();
        }

        [Test]
        public void Start_AddsTestRunnerCommand()
        {
            // Act
            Addin.Start(ServiceProviderMock.Object);
            // Assert
            CommandManagerMock.Verify(
                manager => manager.Commands.Add(It.IsNotNull<ShowTestRunnerCommand>())
            );
        }

        [Test]
        public void Start_LoadsTestCases()
        {
            // Act
            Addin.Start(ServiceProviderMock.Object);
            // Assert
            TestCaseProviderMock.Verify(
                provider => provider.GetTestCases(), Times.Once()
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
            Assert.IsFalse(Addin.GetField<Control>("TestRunnerControl").IsDisposed);
        }
    }
}
