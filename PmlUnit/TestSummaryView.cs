// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Data;
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
        }

        public void UpdateSummary(ICollection<TestListEntry> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            int failedTests = entries.Count(entry => entry.Result != null && !entry.Result.Success);
            FailedTestCountLabel.Text = Pluralize(failedTests, "failed test");
            FailedTestCountLabel.Visible = failedTests > 0;

            int successfulTests = entries.Count(entry => entry.Result != null && entry.Result.Success);
            SuccessfulTestCountLabel.Text = Pluralize(successfulTests, "successful test");
            SuccessfulTestCountLabel.Visible = successfulTests > 0;

            int notExecutedTests = entries.Count(entry => entry.Result == null);
            NotExecutedTestCountLabel.Text = Pluralize(notExecutedTests, "not executed test");
            NotExecutedTestCountLabel.Visible = notExecutedTests > 0;

            if (failedTests > 0)
                ResultLabel.Text = "Last test run: Failed";
            else if (successfulTests > 0)
                ResultLabel.Text = "Last test run: Successful";
            else
                ResultLabel.Text = "Last test run: Unknown";

            TimeSpan totalRuntime = TimeSpan.FromSeconds(Math.Round(
                entries.Where(entry => entry.Result != null)
                .Sum(entry => entry.Result.Duration.TotalSeconds)
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
