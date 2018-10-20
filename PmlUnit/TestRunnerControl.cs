using System;
using System.Collections.Generic;
using System.Drawing;
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
                var groupName = string.Format("{0} ({1})", testCase.Name, testCase.Tests.Count);
                var group = TestView.Groups.Add(testCase.Name, groupName);
                foreach (var test in testCase.Tests)
                {
                    var item = TestView.Items.Add(test.Name);
                    var status = new TestStatus(test, item);
                    item.Tag = status;
                    item.ImageKey = status.ImageKey;
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

        private void OnRunAllLinkClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Run(status => true);
        }

        private void OnRunLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RunContextMenu.Show(RunLinkLabel, new Point(0, RunLinkLabel.Height));
        }

        private void OnRunFailedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(status => status.Failed);
        }

        private void OnRunNotExecutedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(status => !status.HasRun);
        }

        private void OnRunSucceededTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(status => status.Succeeded);
        }

        private void OnRunSelectedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(status => status.Item.Selected);
        }

        private void Run(Func<TestStatus, bool> predicate)
        {
            var toRun = new List<TestStatus>();
            foreach (ListViewItem item in TestView.Items)
            {
                var status = item.Tag as TestStatus;
                if (status != null && predicate(status))
                    toRun.Add(status);
            }

            Enabled = false;

            ExecutionProgressBar.Value = 0;
            ExecutionProgressBar.Maximum = toRun.Count;
            ExecutionProgressBar.Color = Color.Green;

            foreach (var status in toRun)
            {
                status.Result = Runner.Run(status.Test);
                status.Item.ImageKey = status.ImageKey;
                status.Item.SubItems[1].Text = status.FormatDuration();

                ExecutionProgressBar.Increment(1);
                if (status.Failed)
                    ExecutionProgressBar.Color = Color.Red;

                Application.DoEvents();
            }

            Enabled = true;
            ResetTestViewColumnWidths();
        }

        private void OnRefreshLinkClick(object sender, LinkLabelLinkClickedEventArgs e)
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
            catch (Exception e)
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

        private class TestStatus
        {
            public Test Test { get; }
            public TestResult Result { get; set; }
            public ListViewItem Item { get; }

            public TestStatus(Test test, ListViewItem item)
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
