using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class TestRunnerControl : UserControl
    {
        private readonly TestCaseProvider Provider;
        private readonly TestRunner Runner;

        public TestRunnerControl(TestCaseProvider provider, TestRunner runner)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));

            Provider = provider;
            Runner = runner;
            InitializeComponent();
            ResetTestViewColumnWidths();
            ResetSplitContainerOrientation();
        }

        public void LoadTests()
        {
            SetTests(Provider.GetTestCases());
        }

        private void SetTests(IEnumerable<TestCase> testCases)
        {
            TestView.Items.Clear();
            TestView.Groups.Clear();
            foreach (var testCase in testCases)
            {
                var groupName = string.Format(CultureInfo.CurrentCulture, "{0} ({1})", testCase.Name, testCase.Tests.Count);
                var group = TestView.Groups.Add(testCase.Name, groupName);
                foreach (var test in testCase.Tests)
                {
                    var item = TestView.Items.Add(test.Name);
                    var entry = new TestListEntry(test, item);
                    item.Tag = entry;
                    item.ImageKey = entry.ImageKey;
                    item.SubItems.Add("");
                    item.Group = group;
                }
            }

            ResetTestViewColumnWidths();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                Runner.Dispose();
            }
            base.Dispose(disposing);
        }

        private void OnRunAllLinkClick(object sender, EventArgs e)
        {
            Run(FilterTests(entry => true));
        }

        private void OnRunLinkClicked(object sender, EventArgs e)
        {
            RunContextMenu.Show(RunLinkLabel, new Point(0, RunLinkLabel.Height));
        }

        private void OnRunFailedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(FilterTests(entry => entry.Failed));
        }

        private void OnRunNotExecutedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(FilterTests(entry => !entry.HasRun));
        }

        private void OnRunSucceededTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(FilterTests(entry => entry.Succeeded));
        }

        private void OnRunSelectedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(FilterTests(entry => entry.Item.Selected));
        }

        private ICollection<TestListEntry> FilterTests(Func<TestListEntry, bool> predicate)
        {
            var result = new List<TestListEntry>();
            foreach (ListViewItem item in TestView.Items)
            {
                var entry = item.Tag as TestListEntry;
                if (entry != null && predicate(entry))
                    result.Add(entry);
            }
            return result;
        }

        private void Run(ICollection<TestListEntry> entries)
        {
            Enabled = false;
            try
            {
                RunInternal(entries);
            }
            finally
            {
                Enabled = true;
                ResetTestViewColumnWidths();
            }
        }

        private void RunInternal(ICollection<TestListEntry> entries)
        {
            ExecutionProgressBar.Value = 0;
            ExecutionProgressBar.Maximum = entries.Count;
            ExecutionProgressBar.Color = Color.Green;

            foreach (var entry in entries)
            {
                entry.Result = Runner.Run(entry.Test);
                entry.Item.ImageKey = entry.ImageKey;
                entry.Item.SubItems[1].Text = entry.FormatDuration();

                ExecutionProgressBar.Increment(1);
                if (entry.Failed)
                    ExecutionProgressBar.Color = Color.Red;

                Application.DoEvents();
            }
        }

        private void OnRefreshLinkClick(object sender, EventArgs e)
        {
            Runner.RefreshIndex();
            SetTests(Provider.GetTestCases().Select(testCase => Reload(testCase)));
        }

        private TestCase Reload(TestCase testCase)
        {
            try
            {
                Runner.Reload(testCase);
            }
            catch (PmlException e)
            {
                Console.WriteLine("Failed to reload test case {0}", testCase.Name);
                Console.WriteLine(e);
            }

            return testCase;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in TestView.SelectedItems)
            {
                var result = item.SubItems[0].Tag as TestResult;
                TestResultLabel.Text = result?.Error?.Message ?? "";
            }
        }

        private void OnTestViewSizeChanged(object sender, EventArgs e)
        {
            ResetTestViewColumnWidths();
        }

        private void ResetTestViewColumnWidths()
        {
            ExecutionTimeColumn.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            TestNameColumn.Width = Math.Max(0, TestView.ClientSize.Width - ExecutionTimeColumn.Width);
        }

        private void OnSplitContainerSizeChanged(object sender, EventArgs e)
        {
            ResetSplitContainerOrientation();
        }

        private void ResetSplitContainerOrientation()
        {
            var size = TestResultSplitContainer.Size;
            var orientation = size.Width > size.Height ? Orientation.Vertical : Orientation.Horizontal;
            TestResultSplitContainer.Orientation = orientation;
        }

        private class TestListEntry
        {
            public Test Test { get; }
            public TestResult Result { get; set; }
            public ListViewItem Item { get; }

            public TestListEntry(Test test, ListViewItem item)
            {
                if (test == null)
                    throw new ArgumentNullException(nameof(test));
                if (item == null)
                    throw new ArgumentNullException(nameof(item));

                Test = test;
                Item = item;
            }

            public bool Succeeded
            {
                get { return Result != null && Result.Success; }
            }

            public bool Failed
            {
                get { return Result != null && !Result.Success; }
            }

            public bool HasRun
            {
                get { return Result != null; }
            }

            public string ImageKey
            {
                get
                {
                    if (Result == null)
                        return "Unknown";
                    else if (Result.Success)
                        return "Success";
                    else
                        return "Failure";
                }
            }

            public string FormatDuration()
            {
                if (Result == null)
                    return "";

                var millis = Result.Duration.TotalMilliseconds;
                if (millis < 1)
                    return "< 1 ms";
                else
                    return Convert.ToInt64(millis) + " ms";
            }
        }
    }
}
