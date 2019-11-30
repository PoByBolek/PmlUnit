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
            TestList.TestCases.Clear();
            TestList.TestCases.AddRange(Provider.GetTestCases());
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

        private void OnRunPassedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(TestList.PassedTests);
        }

        private void OnRunSelectedTestsMenuItemClick(object sender, EventArgs e)
        {
            Run(TestList.SelectedTests);
        }

        private void Run(ICollection<Test> tests)
        {
            Enabled = false;
            try
            {
                RunInternal(tests);
            }
            finally
            {
                Enabled = true;
                TestSummary.UpdateSummary(tests);
            }
        }

        private void RunInternal(ICollection<Test> tests)
        {
            ExecutionProgressBar.Value = 0;
            ExecutionProgressBar.Maximum = tests.Count;
            ExecutionProgressBar.Color = Color.Green;

            foreach (var test in tests)
            {
                Runner.Run(test);
                ExecutionProgressBar.Increment(1);
                if (test.Status == TestStatus.Failed)
                    ExecutionProgressBar.Color = Color.Red;

                Application.DoEvents();
            }
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

        private void OnTestListSelectionChanged(object sender, EventArgs e)
        {
            var selected = TestList.SelectedTests;
            TestDetails.Test = selected.FirstOrDefault();
            TestSummary.Visible = selected.Count != 1;
            TestDetails.Visible = selected.Count == 1;
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
