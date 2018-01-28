﻿using System;
using System.Drawing;
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
            TestView.Items.Clear();
            foreach (var testCase in Provider.GetTestCases())
            {
                var group = TestView.Groups.Add(testCase.Name, testCase.Name);
                foreach (var test in testCase.Tests)
                {
                    var item = TestView.Items.Add(test.Name);
                    item.SubItems.Add("");
                    item.Tag = test;
                    item.Group = group;
                }
            }
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

        private void OnSizeChanged(object sender, EventArgs e)
        {
            ExecutionTimeColumn.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            TestNameColumn.Width = Math.Max(0, TestView.ClientSize.Width - ExecutionTimeColumn.Width);
        }

        private void OnRunAllLinkClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (ListViewItem item in TestView.Items)
            {
                var test = item.Tag as Test;
                if (test == null)
                    continue;

                var result = Runner.Run(test);
                item.BackColor = result.Success ? Color.Green : Color.Red;
                item.SubItems[1].Text = FormatDuration(result.Duration);
            }

            OnSizeChanged(sender, e);
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
    }
}
