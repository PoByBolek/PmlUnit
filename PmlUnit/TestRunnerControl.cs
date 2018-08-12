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

        public TestRunnerControl(TestCaseProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            Provider = provider;
            Runner = new TestRunner();
            InitializeComponent();
            ResetTestViewColumnWidths();
            ResetSplitContainerOrientation();
        }

        public TestRunnerControl(TestCaseProvider provider, TestRunner runner)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));

            Provider = provider;
            Runner = runner;
            InitializeComponent();
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
                    item.ImageKey = "Unknown";
                    item.SubItems.Add("");
                    item.Tag = test;
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
            Enabled = false;

            ExecutionProgressBar.Value = 0;
            ExecutionProgressBar.Maximum = TestView.Items.Count;
            ExecutionProgressBar.Color = Color.Green;

            foreach (ListViewItem item in TestView.Items)
            {
                var test = item.Tag as Test;
                if (test == null)
                    continue;

                var result = Runner.Run(test);
                item.ImageKey = result.Success ? "Success" : "Failure";
                item.SubItems[0].Tag = result;
                item.SubItems[1].Text = FormatDuration(result.Duration);

                ExecutionProgressBar.Increment(1);
                if (!result.Success)
                    ExecutionProgressBar.Color = Color.Red;

                Application.DoEvents();
            }

            Enabled = true;
            ResetTestViewColumnWidths();
        }

        private static string FormatDuration(TimeSpan? duration)
        {
            if (duration == null)
                return "";

            var millis = duration.Value.TotalMilliseconds;
            if (millis < 1)
                return "< 1 ms";
            else
                return Convert.ToInt64(millis) + " ms";
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
    }
}
