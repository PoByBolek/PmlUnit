using System;
using System.Collections.Generic;

using Moq;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestRunner))]
    public class TestRunnerTest
    {
        [Test]
        public void Constructor_ShouldCheckForNullProxy()
        {
            Assert.Throws<ArgumentNullException>(() => new TestRunner(null));
        }

        [Test]
        public void Dispose_ShouldDisposeTheProxy()
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new TestRunner(proxy.Object);
            runner.Dispose();
            proxy.Verify(p => p.Dispose());
        }

        [Test]
        public void Dispose_ShouldOnlyDisposeTheProxyOnce()
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new TestRunner(proxy.Object);
            runner.Dispose();
            runner.Dispose();
            proxy.Verify(p => p.Dispose(), Times.Once());
        }

        [Test]
        public void Run_ShouldInvokeTheProxysRunMethod()
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new TestRunner(proxy.Object);
            runner.Run(new TestCaseBuilder("Test").AddTest("one").Build().Tests[0]);
            proxy.Verify(p => p.Invoke("run", "Test", "one", false, false));
        }

        [Test]
        public void Run_ShouldPassWhetherTestCaseHasASetUpMethod()
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new TestRunner(proxy.Object);
            var builder = new TestCaseBuilder("Test").AddTest("one");
            builder.HasSetUp = true;
            runner.Run(builder.Build().Tests[0]);
            proxy.Verify(p => p.Invoke("run", "Test", "one", true, false));
        }

        [Test]
        public void Run_ShouldPassWhetherTestCaseHasATearDownMethod()
        {
            var proxy = new Mock<ObjectProxy>();
            var runner = new TestRunner(proxy.Object);
            var builder = new TestCaseBuilder("Test").AddTest("one");
            builder.HasTearDown = true;
            runner.Run(builder.Build().Tests[0]);
            proxy.Verify(p => p.Invoke("run", "Test", "one", false, true));
        }
    }
}
