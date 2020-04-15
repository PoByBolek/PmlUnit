// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class TestRunnerControl : UserControl
    {
        private delegate void RunDelegate(IList<Test> tests, int index);

        private readonly TestCaseProvider Provider;
        private readonly AsyncTestRunner Runner;

        public TestRunnerControl(TestCaseProvider provider, AsyncTestRunner runner)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));

            Provider = provider;
            Runner = runner;
            Runner.TestCompleted += OnTestCompleted;
            Runner.RunCompleted += OnRunCompleted;
            InitializeComponent();
            ResetSplitContainerOrientation();
        }

        public void LoadTests()
        {
            TestList.TestCases.Clear();
            TestList.TestCases.AddRange(Provider.GetTestCases());
            TestSummary.UpdateSummary(TestList.AllTests);
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
            Run(TestList.AllTests);
        }

        private void OnRunLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RunContextMenu.Show(RunLinkLabel, new Point(0, RunLinkLabel.Height));
        }

        private void OnRunFailedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(TestList.FailedTests);
        }

        private void OnRunNotExecutedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(TestList.NotExecutedTests);
        }

        private void OnRunPassedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(TestList.PassedTests);
        }

        private void OnRunSelectedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(TestList.SelectedTests);
        }

        private void Run(IList<Test> tests)
        {
            Enabled = false;

            ExecutionProgressBar.Value = 0;
            ExecutionProgressBar.Maximum = tests.Count;
            ExecutionProgressBar.Color = Color.Green;

            try
            {
                Runner.RunAsync(tests);
            }
            catch (Exception error)
            {
                Enabled = true;
                MessageBox.Show(error.ToString(), "Test run failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnTestCompleted(object sender, TestCompletedEventArgs e)
        {
            ExecutionProgressBar.Increment(1);
            if (e.Test.Status == TestStatus.Failed)
                ExecutionProgressBar.Color = Color.Red;

            Update();
        }

        private void OnRunCompleted(object sender, TestRunCompletedEventArgs e)
        {
            Enabled = true;
            TestSummary.UpdateSummary(e.Tests.ToList());
            if (e.Error != null)
                MessageBox.Show(e.Error.ToString(), "Test run failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OnRefreshLinkClick(object sender, EventArgs e)
        {
            Runner.RefreshIndex();
            TestList.TestCases.Clear();
            TestList.TestCases.AddRange(Provider.GetTestCases().Select(Reload));
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

        private void OnGroupByLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GroupByMenu.Show(GroupByLinkLabel, new Point(0, GroupByLinkLabel.Height));
        }

        private void OnGroupByTestResultMenuItemClick(object sender, EventArgs e)
        {
            TestList.Grouping = TestListGrouping.Result;
        }

        private void OnGroupByTestCaseNameMenuItemClick(object sender, EventArgs e)
        {
            TestList.Grouping = TestListGrouping.TestCase;
        }

        private void OnTestListGroupingChanged(object sender, EventArgs e)
        {
            GroupByTestResultToolStripMenuItem.Checked = TestList.Grouping == TestListGrouping.Result;
            GroupByTestCaseNameToolStripMenuItem.Checked = TestList.Grouping == TestListGrouping.TestCase;
        }

        private void OnTestListSelectionChanged(object sender, EventArgs e)
        {
            var selected = TestList.SelectedTests;
            TestDetails.Test = selected.FirstOrDefault();
            TestSummary.Visible = selected.Count != 1;
            TestDetails.Visible = selected.Count == 1;
        }

        private void OnTestListTestActivate(object sender, TestEventArgs e)
        {
            OpenFile(e.Test.FileName, e.Test.LineNumber);
        }

        private void OnTestDetailsFileActivate(object sender, FileEventArgs e)
        {
            OpenFile(e.FileName, e.LineNumber);
        }

        private void OpenFile(string fileName, int lineNumber)
        {
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                var url = new StringBuilder("vscode://file/");
                url.Append(Uri.EscapeUriString(fileName.Replace('\\', '/')));
                if (lineNumber > 0)
                    url.Append(':').Append(lineNumber);
                Process.Start(url.ToString());
            }
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
