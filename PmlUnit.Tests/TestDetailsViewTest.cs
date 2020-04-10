// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestDetailsView))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TestDetailsViewTest
    {
        private TestDetailsView TestDetails;
        private Label TestNameLabel;
        private IconLabel StatusLabel;
        private Label ElapsedTimeLabel;
        private Label StackTraceLabel;

        [SetUp]
        public void Setup()
        {
            TestDetails = new TestDetailsView();
            TestDetails.Test = new Test(new TestCase("Foo", "foo.pmlobj"), "bar");
            TestNameLabel = TestDetails.FindControl<Label>("TestNameLabel");
            StatusLabel = TestDetails.FindControl<IconLabel>("TestResultIconLabel");
            ElapsedTimeLabel = TestDetails.FindControl<Label>("ElapsedTimeLabel");
            StackTraceLabel = TestDetails.FindControl<Label>("StackTraceLabel");
        }

        [TestCase("Test")]
        [TestCase("foobar")]
        [TestCase("longTestNameWithDigitsInIt123123")]
        public void Test_AssignsTestNameToLabel(string testName)
        {
            // Arrange
            var test = new Test(new TestCase("Foo", "foo.pmlobj"), testName);
            // Act
            TestDetails.Test = test;
            // Assert
            Assert.AreEqual(testName, TestNameLabel.Text);
        }

        [TestCase(null, "Not executed")]
        [TestCase(false, "Failed")]
        [TestCase(true, "Passed")]
        public void Result_SetsStatusLabelText(bool? passed, string expected)
        {
            // Act
            TestDetails.Test.Result = CreateTestResult(passed, TimeSpan.FromSeconds(0));
            // Assert
            Assert.AreEqual(expected, StatusLabel.Text);
        }

        [SetCulture("en-US")]
        [TestCase(0, "< 1 ms")]
        [TestCase(1, "1 ms")]
        [TestCase(42, "42 ms")]
        [TestCase(123, "123 ms")]
        [TestCase(999, "999 ms")]
        [TestCase(1000, "1.0 s")]
        [TestCase(1499, "1.4 s")]
        [TestCase(3123, "3.1 s")]
        [TestCase(9923, "9.9 s")]
        [TestCase(9999, "9.9 s")]
        [TestCase(10000, "10 s")]
        [TestCase(25467, "25 s")]
        [TestCase(456789, "457 s")]
        public void Result_SetsElapsedTimeLabelText(int milliseconds, string expected)
        {
            ElapsedTimeLabel.Text = "";
            TestDetails.Test.Result = new TestResult(TimeSpan.FromMilliseconds(milliseconds));
            Assert.AreEqual("Elapsed time: " + expected, ElapsedTimeLabel.Text);

            ElapsedTimeLabel.Text = "";
            TestDetails.Test.Result = new TestResult(TimeSpan.FromMilliseconds(milliseconds), new PmlError("error"));
            Assert.AreEqual("Elapsed time: " + expected, ElapsedTimeLabel.Text);
        }

        [Test]
        public void Result_NoResultClearsElapsedTimeLabelText()
        {
            // Arrange
            TestDetails.Test.Result = new TestResult(TimeSpan.FromSeconds(1));
            // Act
            TestDetails.Test.Result = null;
            // Assert
            Assert.AreEqual("", ElapsedTimeLabel.Text);
        }

        [Test]
        public void Result_SetsStackTraceLabelText()
        {
            var error = new PmlError("This is a test");
            TestDetails.Test.Result = new TestResult(TimeSpan.FromSeconds(0), error);
            Assert.AreEqual(error.Message, StackTraceLabel.Text);

            TestDetails.Test.Result = new TestResult(TimeSpan.FromSeconds(0));
            Assert.AreEqual("", StackTraceLabel.Text);

            TestDetails.Test.Result = new TestResult(TimeSpan.FromSeconds(0), error);
            Assert.AreEqual(error.Message, StackTraceLabel.Text);

            TestDetails.Test.Result = null;
            Assert.AreEqual("", StackTraceLabel.Text);
        }

        private static TestResult CreateTestResult(bool? passed, TimeSpan duration)
        {
            if (passed == null)
                return null;
            else if (passed.Value)
                return new TestResult(duration);
            else
                return new TestResult(duration, new PmlError("error"));
        }
    }
}
