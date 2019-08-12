using System;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(Test))]
    public class TestTest
    {
        private Test Test;

        [SetUp]
        public void Setup()
        {
            Test = new TestCaseBuilder("Foo").AddTest("bar").Build().Tests[0];
        }

        [Test]
        public void Result_DefaultsToNull()
        {
            Assert.AreEqual(null, Test.Result);
        }

        [Test]
        public void Result_RaisesResultChanged()
        {
            TestResult expected = null;
            bool eventRaised;

            Test.ResultChanged += (object sender, EventArgs e) =>
            {
                eventRaised = true;
                Assert.AreEqual(Test, sender);
                Assert.AreEqual(expected, Test.Result);
            };


            expected = new TestResult(TimeSpan.FromSeconds(1));
            eventRaised = false;
            Test.Result = expected;
            Assert.IsTrue(eventRaised);
            Assert.AreEqual(expected, Test.Result);

            eventRaised = false;
            Test.Result = expected;
            Assert.IsFalse(eventRaised);
            Assert.AreEqual(expected, Test.Result);

            expected = new TestResult(TimeSpan.FromSeconds(1));
            eventRaised = false;
            Test.Result = expected;
            Assert.IsTrue(eventRaised);
            Assert.AreEqual(expected, Test.Result);

            eventRaised = false;
            expected = null;
            Test.Result = null;
            Assert.IsTrue(eventRaised);
            Assert.AreEqual(expected, Test.Result);

            eventRaised = false;
            expected = null;
            Test.Result = null;
            Assert.IsFalse(eventRaised);
            Assert.AreEqual(expected, Test.Result);
        }

        [Test]
        public void Status_IsNotExecutedWhenResultIsNull()
        {
            Test.Result = null;
            Assert.AreEqual(TestStatus.NotExecuted, Test.Status);
        }

        [Test]
        public void Status_IsSuccessfulWhenResultHasNoError()
        {
            Test.Result = new TestResult(TimeSpan.FromSeconds(1));
            Assert.AreEqual(TestStatus.Successful, Test.Status);
        }

        [Test]
        public void Status_IsFailedWhenResultHasError()
        {
            Test.Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException());
            Assert.AreEqual(TestStatus.Failed, Test.Status);
        }
    }
}
