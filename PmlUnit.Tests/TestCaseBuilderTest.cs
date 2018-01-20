using System;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestCaseBuilder))]
    public class TestCaseBuilderTest
    {
        [Test]
        public void Constructor_ShouldCheckForNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new TestCaseBuilder(null));
            Assert.Throws<ArgumentNullException>(() => new TestCaseBuilder(""));
        }

        [TestCase("hello world")]
        [TestCase("   asdf")]
        [TestCase("test.case")]
        [TestCase("foo!")]
        [TestCase("bar()")]
        public void Constructor_ShouldCheckForNonAlphaNumericCharacters(string name)
        {
            Assert.Throws<ArgumentException>(() => new TestCaseBuilder(name), "Should raise ArgumentException for \"{0}\".", name);
        }

        [Test]
        public void Constructor_ShouldSetNameProperty()
        {
            var builder = new TestCaseBuilder("Test");
            Assert.That(builder.Name, Is.EqualTo("Test"));
        }

        [Test]
        public void AddTest_ShouldCheckForNullTestName()
        {
            var builder = new TestCaseBuilder("Test");
            Assert.Throws<ArgumentNullException>(() => builder.AddTest(null));
            Assert.Throws<ArgumentNullException>(() => builder.AddTest(""));
        }

        [TestCase(".firstTest")]
        [TestCase("second test")]
        [TestCase("  thirdTest")]
        [TestCase("!fourthTest")]
        [TestCase("fifthTest()")]
        [TestCase("sixth%test")]
        public void AddTest_ShouldCheckForNonAlphanumericCharacters(string testName)
        {
            var builder = new TestCaseBuilder("Test");
            Assert.Throws<ArgumentException>(() => builder.AddTest(testName), "Should raise ArgumentException for \"{0}\".", testName);
        }

        [Test]
        public void AddTest_ShouldAddTheTestNameToTheBuilder()
        {
            var builder = new TestCaseBuilder("Test");
            builder.AddTest("one").AddTest("two").AddTest("three");
            Assert.That(builder.TestNames.Count, Is.EqualTo(3));
            Assert.That(builder.TestNames, Contains.Item("one"));
            Assert.That(builder.TestNames, Contains.Item("two"));
            Assert.That(builder.TestNames, Contains.Item("three"));
        }

        [Test]
        public void AddTest_ShouldIgnoreTestNameCasing()
        {
            var builder = new TestCaseBuilder("Test");
            builder.AddTest("Test").AddTest("test").AddTest("tEsT").AddTest("TEst");
            Assert.That(builder.TestNames.Count, Is.EqualTo(1));
            Assert.That(builder.TestNames, Contains.Item("Test"));
        }

        [Test]
        public void Build_CreatesTestCaseWithSameName()
        {
            var testCase = new TestCaseBuilder("HelloWorld").Build();
            Assert.That(testCase.Name, Is.EqualTo("HelloWorld"));
        }

        [Test]
        public void Build_CreatesTestCaseWithSameSetUpFlag()
        {
            var builder = new TestCaseBuilder("Test");
            Assert.That(builder.Build().HasSetUp, Is.False);
            builder.HasSetUp = true;
            Assert.That(builder.Build().HasSetUp);
        }

        [Test]
        public void Build_CreatesTestCaseWithSameTearDownFlag()
        {
            var builder = new TestCaseBuilder("Test");
            Assert.That(builder.Build().HasTearDown, Is.False);
            builder.HasTearDown = true;
            Assert.That(builder.Build().HasTearDown);
        }

        [Test]
        public void Build_CreatesTestCaseWithRegisteredTests()
        {
            var testCase = new TestCaseBuilder("Test").AddTest("one").AddTest("two").AddTest("three").Build();
            Assert.That(testCase.Tests.Count, Is.EqualTo(3));
            Assert.That(testCase.Tests, Has.Exactly(1).Matches<Test>(test => test.Name == "one"));
            Assert.That(testCase.Tests, Has.Exactly(1).Matches<Test>(test => test.Name == "two"));
            Assert.That(testCase.Tests, Has.Exactly(1).Matches<Test>(test => test.Name == "three"));
        }
    }
}
