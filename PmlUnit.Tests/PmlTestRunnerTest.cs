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
        [Test]
        public void Constructor_ShouldCheckForNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner((ObjectProxy)null));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner((Clock)null));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(Mock.Of<ObjectProxy>(), null));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(null, Mock.Of<Clock>()));
            Assert.Throws<ArgumentNullException>(() => new PmlTestRunner(null, null));
        }

        [Test]
        public void Dispose_ShouldDisposeTheProxy()
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new PmlTestRunner(proxy.Object);
            runner.Dispose();
            proxy.Verify(p => p.Dispose());
        }

        [Test]
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
            Justification = "Tests that the TestRunner does not dispose its proxy multiple times.")]
        public void Dispose_ShouldOnlyDisposeTheProxyOnce()
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new PmlTestRunner(proxy.Object);
            runner.Dispose();
            runner.Dispose();
            proxy.Verify(p => p.Dispose(), Times.Once());
        }

        [Test]
        public void Run_ShouldFailAfterBeingDisposed()
        {
            var runner = new PmlTestRunner(Mock.Of<ObjectProxy>(), Mock.Of<Clock>());
            runner.Dispose();
            var testCase = new TestCaseBuilder("Test").AddTest("one").Build();
            Assert.Throws<ObjectDisposedException>(() => runner.Run(testCase));
            Assert.Throws<ObjectDisposedException>(() => runner.Run(testCase.Tests[0]));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void Run_ShouldInvokeTheProxysRunMethod(bool hasSetUp, bool hasTearDown)
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new PmlTestRunner(proxy.Object);
            var builder = new TestCaseBuilder("Test").AddTest("one");
            builder.HasSetUp = hasSetUp;
            builder.HasTearDown = hasTearDown;
            runner.Run(builder.Build().Tests[0]);
            proxy.Verify(p => p.Invoke("run", "Test", "one", hasSetUp, hasTearDown), Times.Exactly(1));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void Run_ShouldInvokeTheProxyForAllTestsInTheTestCase(bool hasSetUp, bool hasTearDown)
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new PmlTestRunner(proxy.Object);
            var builder = new TestCaseBuilder("Test").AddTest("one").AddTest("two").AddTest("three");
            builder.HasSetUp = hasSetUp;
            builder.HasTearDown = hasTearDown;
            runner.Run(builder.Build());
            proxy.Verify(p => p.Invoke("run", "Test", "one", hasSetUp, hasTearDown), Times.Exactly(1));
            proxy.Verify(p => p.Invoke("run", "Test", "two", hasSetUp, hasTearDown), Times.Exactly(1));
            proxy.Verify(p => p.Invoke("run", "Test", "three", hasSetUp, hasTearDown), Times.Exactly(1));
        }

        public void Run_ShouldAssignTestResultToAllTestsInTheTestCase()
        {
            var testCase = new TestCaseBuilder("Test").AddTest("one").AddTest("two").AddTest("three").Build();
            
            var proxy = new Mock<ObjectProxy>();
            var oneError = new PmlException("one");
            proxy.Setup(p => p.Invoke("run", "Test", "one", It.IsAny<bool>(), It.IsAny<bool>())).Throws(oneError);
            var twoError = new PmlException("two");
            proxy.Setup(p => p.Invoke("run", "Test", "two", It.IsAny<bool>(), It.IsAny<bool>())).Throws(twoError);
            var threeError = new PmlException("three");
            proxy.Setup(p => p.Invoke("run", "Test", "three", It.IsAny<bool>(), It.IsAny<bool>())).Throws(threeError);

            var runner = new PmlTestRunner(proxy.Object);
            runner.Run(testCase);

            Assert.NotNull(testCase.Tests[0].Result);
            Assert.AreSame(oneError, testCase.Tests[0].Result.Error);
            Assert.NotNull(testCase.Tests[1].Result);
            Assert.AreSame(twoError, testCase.Tests[1].Result.Error);
            Assert.NotNull(testCase.Tests[2].Result);
            Assert.AreSame(threeError, testCase.Tests[2].Result.Error);
        }

        [Test]
        public void Run_ShouldQueryTheClock()
        {
            var clock = new Mock<Clock>();
            var runner = new PmlTestRunner(Mock.Of<ObjectProxy>(), clock.Object);
            runner.Run(new TestCaseBuilder("TestCase").AddTest("test").Build().Tests[0]);
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
            int seconds = 0;
            var clock = new Mock<Clock>();
            clock.SetupGet(mock => mock.CurrentInstant)
                .Returns(() => Instant.FromSeconds(seconds))
                .Callback(() => seconds = duration);
            var runner = new PmlTestRunner(Mock.Of<ObjectProxy>(), clock.Object);
            var test = new TestCaseBuilder("Test").AddTest("method").Build().Tests[0];
            var result = runner.Run(test);
            Assert.AreEqual(TimeSpan.FromSeconds(duration), result.Duration);
            Assert.AreSame(result, test.Result);
        }

        [TestCase(13)]
        [TestCase(123)]
        [TestCase(128937489)]
        public void Run_ShouldAssignAndReturnTestResultWithElapsedTimeWhenTestFails(int duration)
        {
            int seconds = 0;
            var proxy = new Mock<ObjectProxy>();
            proxy.Setup(mock => mock.Invoke(It.IsAny<string>(), It.IsAny<object[]>())).Throws<PmlException>();
            var clock = new Mock<Clock>();
            clock.SetupGet(mock => mock.CurrentInstant)
                .Returns(() => Instant.FromSeconds(seconds))
                .Callback(() => seconds = duration);
            var runner = new PmlTestRunner(proxy.Object, clock.Object);
            var test = new TestCaseBuilder("Test").AddTest("method").Build().Tests[0];
            var result = runner.Run(test);
            Assert.AreEqual(TimeSpan.FromSeconds(duration), result.Duration);
            Assert.AreSame(result, test.Result);
        }

        [Test]
        public void Run_ShouldReturnSuccessfulResultWhenTestSucceeds()
        {
            var runner = new PmlTestRunner(Mock.Of<ObjectProxy>());
            var result = runner.Run(new TestCaseBuilder("Test").AddTest("method").Build().Tests[0]);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void Run_ShouldReturnFailedResultWhenTestFails()
        {
            var error = new PmlException();
            var proxy = new Mock<ObjectProxy>();
            proxy.Setup(mock => mock.Invoke(It.IsAny<string>(), It.IsAny<object[]>())).Throws(error);
            var runner = new PmlTestRunner(proxy.Object);
            var result = runner.Run(new TestCaseBuilder("Test").AddTest("test").Build().Tests[0]);
            Assert.IsFalse(result.Success);
            Assert.AreSame(error, result.Error);
        }

        [Test]
        public void Run_ShouldReturnFailedResultWhenTestReturnsError()
        {
            var error = new Hashtable();
            error[1.0] = "This is an error message...";
            error[2.0] = "... which spans two lines";
            var proxy = new Mock<ObjectProxy>();
            proxy.Setup(mock => mock.Invoke(It.IsAny<string>(), It.IsAny<object[]>())).Returns(error);
            var runner = new PmlTestRunner(proxy.Object);
            var result = runner.Run(new TestCaseBuilder("Test").AddTest("test").Build().Tests[0]);
            Assert.IsFalse(result.Success);
            Assert.NotNull(result.Error);
            Assert.AreEqual("This is an error message...\r\n... which spans two lines\r\n", result.Error.Message);
        }

        [Test]
        public void Run_ShouldDisposeTestReturnValue()
        {
            var result = new Mock<IDisposable>();
            var proxy = new Mock<ObjectProxy>();
            proxy.Setup(mock => mock.Invoke(It.IsAny<string>(), It.IsAny<object[]>())).Returns(result.Object);
            var runner = new PmlTestRunner(proxy.Object);
            runner.Run(new TestCaseBuilder("Test").AddTest("test").Build().Tests[0]);
            result.Verify(mock => mock.Dispose(), Times.Exactly(1));
        }
    }
}
