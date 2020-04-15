// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(PmlTestRunner))]
    public class PmlTestRunnerTest
    {
        private TestCase TestCase;
        private Test Test;
        private Hashtable StackTrace;

        [SetUp]
        public void Setup()
        {
            TestCase = new TestCase("Test", "test.pmlobj");
            Test = TestCase.Tests.Add("one");
            StackTrace = new Hashtable();
            StackTrace[1.0] = "(61,123)   FM: Form FOOBAR not found";
            StackTrace[2.0] = " *** Error Line not available";
            StackTrace[3.0] = " *** Error Command not available";
        }

        [Test]
        public void Constructor_ShouldCheckForNullArguments()
        {
            Clock clock = Mock.Of<Clock>();
            Clock noClock = null;
            MethodInvoker invoker = Mock.Of<MethodInvoker>();
            MethodInvoker noInvoker = null;
            EntryPointResolver resolver = Mock.Of<EntryPointResolver>();
            EntryPointResolver noResolver = null;
            ObjectProxy proxy = Mock.Of<ObjectProxy>();
            ObjectProxy noProxy = null;

            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noInvoker));

            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noInvoker, noClock));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noInvoker, clock));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(invoker, noClock));

            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noInvoker, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(invoker, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noInvoker, resolver));

            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noInvoker, noClock, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noInvoker, noClock, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noInvoker, clock, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noInvoker, clock, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(invoker, noClock, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(invoker, noClock, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(invoker, clock, noResolver));

            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, noInvoker));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, invoker));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, noInvoker));

            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, noInvoker, noClock));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, noInvoker, clock));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, invoker, noClock));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, invoker, clock));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, noInvoker, noClock));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, noInvoker, clock));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, invoker, noClock));

            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, noInvoker, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, noInvoker, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, invoker, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, invoker, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, noInvoker, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, noInvoker, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, invoker, noResolver));

            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, noInvoker, noClock, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, noInvoker, noClock, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, noInvoker, clock, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, noInvoker, clock, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, invoker, noClock, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, invoker, noClock, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, invoker, clock, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(noProxy, invoker, clock, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, noInvoker, noClock, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, noInvoker, noClock, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, noInvoker, clock, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, noInvoker, clock, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, invoker, noClock, noResolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, invoker, noClock, resolver));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(proxy, invoker, clock, noResolver));
        }

        [Test]
        public void Dispose_ShouldDisposeTheProxy()
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new PmlTestRunner(proxy.Object, Mock.Of<MethodInvoker>());
            runner.Dispose();
            proxy.Verify(p => p.Dispose());
        }

        [Test]
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
            Justification = "Tests that the TestRunner does not dispose its proxy multiple times.")]
        public void Dispose_ShouldOnlyDisposeTheProxyOnce()
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new PmlTestRunner(proxy.Object, Mock.Of<MethodInvoker>());
            runner.Dispose();
            runner.Dispose();
            proxy.Verify(p => p.Dispose(), Times.Once());
        }

        [Test]
        public void Run_ShouldFailAfterBeingDisposed()
        {
            var runner = new PmlTestRunner(Mock.Of<ObjectProxy>(), Mock.Of<MethodInvoker>(), Mock.Of<Clock>());
            runner.Dispose();
            Assert.Throws<ObjectDisposedException>(() => runner.Run(TestCase));
            Assert.Throws<ObjectDisposedException>(() => runner.Run(Test));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void Run_ShouldInvokeTheProxysRunMethod(bool hasSetUp, bool hasTearDown)
        {
            // Arrange
            var proxy = new Mock<ObjectProxy>();
            var runner = new PmlTestRunner(proxy.Object, Mock.Of<MethodInvoker>());
            TestCase.HasSetUp = hasSetUp;
            TestCase.HasTearDown = hasTearDown;
            // Act
            runner.Run(Test);
            // Assert
            proxy.Verify(p => p.Invoke("run", "Test", "one", hasSetUp, hasTearDown), Times.Exactly(1));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void Run_ShouldInvokeTheProxyForAllTestsInTheTestCase(bool hasSetUp, bool hasTearDown)
        {
            // Arrange
            var proxy = new Mock<ObjectProxy>();
            var runner = new PmlTestRunner(proxy.Object, Mock.Of<MethodInvoker>());
            TestCase.Tests.Add("two");
            TestCase.Tests.Add("three");
            TestCase.HasSetUp = hasSetUp;
            TestCase.HasTearDown = hasTearDown;
            // Act
            runner.Run(TestCase);
            // Assert
            proxy.Verify(p => p.Invoke("run", "Test", "one", hasSetUp, hasTearDown), Times.Exactly(1));
            proxy.Verify(p => p.Invoke("run", "Test", "two", hasSetUp, hasTearDown), Times.Exactly(1));
            proxy.Verify(p => p.Invoke("run", "Test", "three", hasSetUp, hasTearDown), Times.Exactly(1));
        }

        [Test]
        public void Run_ShouldAssignTestResultToAllTestsInTheTestCase()
        {
            TestCase.Tests.Add("two");
            TestCase.Tests.Add("three");

            var runner = new PmlTestRunner(Mock.Of<ObjectProxy>(), Mock.Of<MethodInvoker>());
            runner.Run(TestCase);

            Assert.NotNull(TestCase.Tests["one"].Result);
            Assert.NotNull(TestCase.Tests["two"].Result);
            Assert.NotNull(TestCase.Tests["three"].Result);
        }

        [Test]
        public void Run_ShouldQueryTheClock()
        {
            var clock = new Mock<Clock>();
            var runner = new PmlTestRunner(Mock.Of<ObjectProxy>(), Mock.Of<MethodInvoker>(), clock.Object);
            runner.Run(Test);
            clock.VerifyGet(mock => mock.CurrentInstant, Times.Exactly(2));
        }

        [TestCase(20)]
        [TestCase(12)]
        [TestCase(0)]
        [TestCase(-5)]
        [TestCase(3600)]
        [TestCase(86401)]
        public void Run_ShouldAssignAndReturnTestResultWithElapsedTime(int duration)
        {
            // Arrange
            int seconds = 0;
            var clock = new Mock<Clock>();
            clock.SetupGet(mock => mock.CurrentInstant)
                .Returns(() => Instant.FromSeconds(seconds))
                .Callback(() => seconds = duration);
            var runner = new PmlTestRunner(Mock.Of<ObjectProxy>(), Mock.Of<MethodInvoker>(), clock.Object);
            // Act
            var result = runner.Run(Test);
            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(duration), result.Duration);
            Assert.AreSame(result, Test.Result);
        }

        [TestCase(13)]
        [TestCase(123)]
        [TestCase(128937489)]
        public void Run_ShouldAssignAndReturnTestResultWithElapsedTimeWhenTestFails(int duration)
        {
            // Arrange
            int seconds = 0;
            var proxy = new Mock<ObjectProxy>();
            proxy.Setup(mock => mock.Invoke(It.IsAny<string>(), It.IsAny<object[]>())).Returns(StackTrace);
            var clock = new Mock<Clock>();
            clock.SetupGet(mock => mock.CurrentInstant)
                .Returns(() => Instant.FromSeconds(seconds))
                .Callback(() => seconds = duration);
            var runner = new PmlTestRunner(proxy.Object, Mock.Of<MethodInvoker>(), clock.Object);
            // Act
            var result = runner.Run(Test);
            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(duration), result.Duration);
            Assert.AreSame(result, Test.Result);
        }

        [Test]
        public void Run_ShouldReturnPassedResultWhenTestPasses()
        {
            var runner = new PmlTestRunner(Mock.Of<ObjectProxy>(), Mock.Of<MethodInvoker>());
            var result = runner.Run(Test);
            Assert.IsTrue(result.Passed);
        }

        [Test]
        public void Run_ShouldReturnFailedResultWhenTestReturnsError()
        {
            // Arrange
            var proxy = new Mock<ObjectProxy>();
            proxy.Setup(mock => mock.Invoke(It.IsAny<string>(), It.IsAny<object[]>())).Returns(StackTrace);
            var runner = new PmlTestRunner(proxy.Object, Mock.Of<MethodInvoker>());
            // Act
            var result = runner.Run(Test);
            // Assert
            Assert.IsFalse(result.Passed);
            Assert.NotNull(result.Error);
            Assert.AreEqual(StackTrace[1.0], result.Error.Message);
        }

        [Test]
        public void Run_ShouldDisposeTestReturnValue()
        {
            // Arrange
            var result = new Mock<IDisposable>();
            var proxy = new Mock<ObjectProxy>();
            proxy.Setup(mock => mock.Invoke(It.IsAny<string>(), It.IsAny<object[]>())).Returns(result.Object);
            var runner = new PmlTestRunner(proxy.Object, Mock.Of<MethodInvoker>());
            // Act
            runner.Run(Test);
            // Assert
            result.Verify(mock => mock.Dispose(), Times.Exactly(1));
        }
    }
}
