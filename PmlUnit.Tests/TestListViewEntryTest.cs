// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Forms;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestListViewEntry))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TestListViewEntryTest
    {
        private CultureInfo InitialCulture;

        private ImageList StatusImageList;
        private TestListViewEntry Entry;
        private Label ImageLabel;
        private Label NameLabel;
        private Label DurationLabel;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            InitialCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [OneTimeTearDown]
        public void ClassTearDown()
        {
            CultureInfo.CurrentCulture = InitialCulture;
        }

        [SetUp]
        public void Setup()
        {
            StatusImageList = new ImageList();
            var testCase = new TestCaseBuilder("Test").AddTest("one").Build();
            Entry = new TestListViewEntry(testCase.Tests[0]);
            Entry.ImageList = StatusImageList;
            ImageLabel = Entry.FindControl<Label>(nameof(ImageLabel));
            NameLabel = Entry.FindControl<Label>(nameof(NameLabel));
            DurationLabel = Entry.FindControl<Label>(nameof(DurationLabel));
        }

        [TearDown]
        public void TearDown()
        {
            Entry.Dispose();
            StatusImageList.Dispose();
        }

        [TestCase("one")]
        [TestCase("two")]
        [TestCase("three")]
        [TestCase("testSomethingOrSomethingElse")]
        public void SetsNameLabelText(string testName)
        {
            var testCase = new TestCaseBuilder("Test").AddTest(testName).Build();
            using (var entry = new TestListViewEntry(testCase.Tests[0]))
            {
                var label = entry.FindControl<Label>(nameof(NameLabel));
                Assert.AreEqual(testName, label.Text);
            }
        }

        [Test]
        public void ImageList_AssignsToStatusLabel()
        {
            Entry.ImageList = null;
            Assert.AreSame(null, ImageLabel.ImageList);

            Entry.ImageList = StatusImageList;
            Assert.AreSame(StatusImageList, ImageLabel.ImageList);
        }

        [Test]
        public void Results_SetsImageKeyOfImageLabel()
        {
            Assert.AreEqual("Unknown", ImageLabel.ImageKey);

            Entry.Result = new TestResult(TimeSpan.FromSeconds(1));
            Assert.AreEqual("Success", ImageLabel.ImageKey);

            Entry.Result = new TestResult(TimeSpan.FromSeconds(1), new Exception());
            Assert.AreEqual("Failure", ImageLabel.ImageKey);

            Entry.Result = null;
            Assert.AreEqual("Unknown", ImageLabel.ImageKey);
        }

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
        public void Result_FormatsDurationToDurationLabel(int milliseconds, string expected)
        {
            Entry.Result = new TestResult(TimeSpan.FromMilliseconds(milliseconds));
            Assert.AreEqual(expected, DurationLabel.Text);

            Entry.Result = new TestResult(TimeSpan.FromMilliseconds(milliseconds), new PmlException("foobar"));
            Assert.AreEqual(expected, DurationLabel.Text);

            Entry.Result = null;
            Assert.AreEqual("", DurationLabel.Text);
        }
    }
}
