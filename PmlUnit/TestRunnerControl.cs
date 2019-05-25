// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
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
            ResetSplitContainerOrientation();
        }

        public void LoadTests()
        {
            SetTests(Provider.GetTestCases());
        }

        private void SetTests(IEnumerable<TestCase> testCases)
        {
            TestList.SetTests(testCases.SelectMany(testCase => testCase.Tests));
            TestSummary.UpdateSummary(TestList.AllTests);
            ResetTestDetails(TestList.SelectedTests.FirstOrDefault());
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

        private void OnRunLinkClicked(object sender, EventArgs e)
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

        private void OnRunSucceededTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(TestList.SucceededTests);
        }

        private void OnRunSelectedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(TestList.SelectedTests);
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
                TestSummary.UpdateSummary(entries);
                ResetTestDetails(entries.FirstOrDefault());
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
                ExecutionProgressBar.Increment(1);
                if (entry.Result != null && !entry.Result.Success)
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

        private void OnTestListSelectionChanged(object sender, EventArgs e)
        {
            var selected = TestList.SelectedTests;
            ResetTestDetails(selected.FirstOrDefault());
            TestSummary.Visible = selected.Count != 1;
            TestDetails.Visible = selected.Count == 1;
        }

        private void ResetTestDetails(TestListEntry selected)
        {
            if (selected == null)
                return;

            TestDetails.Test = selected.Test;
            TestDetails.Result = selected.Result;
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
