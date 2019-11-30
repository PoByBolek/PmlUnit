using System;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(Test))]
    public class TestTest
    {
        [Test]
        public void Constructor_ChecksForNullArguments()
        {
            var testCase = new TestCase("Foo");
            Assert.Throws<ArgumentNullException>(() => new Test(null, null));
            Assert.Throws<ArgumentNullException>(() => new Test(null, ""));
            Assert.Throws<ArgumentNullException>(() => new Test(testCase, null));
            Assert.Throws<ArgumentNullException>(() => new Test(testCase, ""));
        }

        [TestCase("hello world")]
        [TestCase("   asdf")]
        [TestCase("test.case")]
        [TestCase("foo!")]
        [TestCase("bar()")]
        public void Constructor_ChecksForNonAlphaNumericCharacters(string name)
        {
            var testCase = new TestCase("Foo");
            Assert.Throws<ArgumentException>(() => new Test(testCase, name), "Should raise ArgumentException for \"{0}\".", name);
        }

        [TestCase("Test")]
        [TestCase("SomethingElse")]
        public void Constructor_SetsNameProperty(string name)
        {
            var test = new Test(new TestCase("Foo"), name);
            Assert.AreEqual(name, test.Name);
        }

        [Test]
        public void Constructor_SetsTestCaseProperty()
        {
            var testCase = new TestCase("Foo");
            var test = new Test(testCase, "bar");
            Assert.AreSame(testCase, test.TestCase);
        }

        [Test]
        public void Result_DefaultsToNull()
        {
            var test = new Test(new TestCase("foo"), "bar");
            Assert.AreEqual(null, test.Result);
        }

        [Test]
        public void Result_RaisesResultChanged()
        {
            var test = new Test(new TestCase("foo"), "bar");
            TestResult expected = null;
            bool eventRaised;

            test.ResultChanged += (object sender, EventArgs e) =>
            {
                eventRaised = true;
                Assert.AreEqual(test, sender);
                Assert.AreEqual(expected, test.Result);
            };


            expected = new TestResult(TimeSpan.FromSeconds(1));
            eventRaised = false;
            test.Result = expected;
            Assert.IsTrue(eventRaised);
            Assert.AreEqual(expected, test.Result);

            eventRaised = false;
            test.Result = expected;
            Assert.IsFalse(eventRaised);
            Assert.AreEqual(expected, test.Result);

            expected = new TestResult(TimeSpan.FromSeconds(1));
            eventRaised = false;
            test.Result = expected;
            Assert.IsTrue(eventRaised);
            Assert.AreEqual(expected, test.Result);

            eventRaised = false;
            expected = null;
            test.Result = null;
            Assert.IsTrue(eventRaised);
            Assert.AreEqual(expected, test.Result);

            eventRaised = false;
            expected = null;
            test.Result = null;
            Assert.IsFalse(eventRaised);
            Assert.AreEqual(expected, test.Result);
        }

        [Test]
        public void Status_IsNotExecutedWhenResultIsNull()
        {
            var test = new Test(new TestCase("foo"), "bar");
            test.Result = null;
            Assert.AreEqual(TestStatus.NotExecuted, test.Status);
        }

        [Test]
        public void Status_IsPassedWhenResultHasNoError()
        {
            var test = new Test(new TestCase("foo"), "bar");
            test.Result = new TestResult(TimeSpan.FromSeconds(1));
            Assert.AreEqual(TestStatus.Passed, test.Status);
        }

        [Test]
        public void Status_IsFailedWhenResultHasError()
        {
            var test = new Test(new TestCase("foo"), "bar");
            test.Result = new TestResult(TimeSpan.FromSeconds(1), new PmlException());
            Assert.AreEqual(TestStatus.Failed, test.Status);
        }
    }
}
