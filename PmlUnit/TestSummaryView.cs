// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class TestSummaryView : UserControl
    {
        public TestSummaryView()
        {
            InitializeComponent();

            SummaryLabel.Font = new Font(Font.FontFamily, Font.Size * 1.5f, FontStyle.Bold);
            ResultLabel.Font = new Font(Font, FontStyle.Bold);
        }

        public void UpdateSummary(ICollection<Test> tests)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests));

            int failedTests = tests.Count(test => test.Status == TestStatus.Failed);
            FailedTestCountLabel.Text = Pluralize(failedTests, "failed test");
            FailedTestCountLabel.Visible = failedTests > 0;

            int passedTests = tests.Count(test => test.Status == TestStatus.Passed);
            PassedTestCountLabel.Text = Pluralize(passedTests, "passed test");
            PassedTestCountLabel.Visible = passedTests > 0;

            int notExecutedTests = tests.Count(test => test.Status == TestStatus.NotExecuted);
            NotExecutedTestCountLabel.Text = Pluralize(notExecutedTests, "not executed test");
            NotExecutedTestCountLabel.Visible = notExecutedTests > 0;

            if (failedTests > 0)
                ResultLabel.Text = "Last test run: Failed";
            else if (passedTests > 0)
                ResultLabel.Text = "Last test run: Passed";
            else
                ResultLabel.Text = "Last test run: Unknown";

            TimeSpan totalRuntime = TimeSpan.FromSeconds(Math.Round(
                tests.Where(test => test.Result != null)
                .Sum(test => test.Result.Duration.TotalSeconds)
            ));
            RuntimeLabel.Text = string.Format(CultureInfo.CurrentCulture, "(Total runtime: {0:c})", totalRuntime);
            RuntimeLabel.Left = ResultLabel.Right;
        }

        private static string Pluralize(int count, string singular)
        {
            StringBuilder result = new StringBuilder();
            result.Append(count).Append(" ").Append(singular);
            if (count != 1)
                result.Append("s");
            return result.ToString();
        }
    }
}
