using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class TestSummaryView : UserControl
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TestListEntryCollection TestEntries { get; }

        public TestSummaryView()
        {
            InitializeComponent();

            TestEntries = new TestListEntryCollection();
            TestEntries.Changed += OnTestsChanged;
        }

        private void OnTestsChanged(object sender, EventArgs e)
        {
            int failedTests = TestEntries.Count(entry => entry.Result != null && !entry.Result.Success);
            FailedTestCountLabel.Text = Pluralize(failedTests, "failed test");
            FailedTestCountLabel.Visible = failedTests > 0;

            int successfulTests = TestEntries.Count(entry => entry.Result != null && entry.Result.Success);
            SuccessfulTestCountLabel.Text = Pluralize(successfulTests, "successful test");
            SuccessfulTestCountLabel.Visible = successfulTests > 0;

            int notExecutedTests = TestEntries.Count(entry => entry.Result == null);
            NotExecutedTestCountLabel.Text = Pluralize(notExecutedTests, "not executed test");
            NotExecutedTestCountLabel.Visible = notExecutedTests > 0;

            if (failedTests > 0)
                ResultLabel.Text = "Last test run: Failed";
            else if (successfulTests > 0)
                ResultLabel.Text = "Last test run: Successful";
            else
                ResultLabel.Text = "Last test run: Unknown";

            TimeSpan totalRuntime = TimeSpan.FromSeconds(Math.Round(
                TestEntries.Where(entry => entry.Result != null)
                .Sum(entry => entry.Result.Duration.TotalSeconds)
            ));
            RuntimeLabel.Text = string.Format("(Total runtime: {0:c})", totalRuntime);
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

    class TestListEntryCollection : ICollection<TestListEntry>
    {
        public event EventHandler Changed;

        private readonly List<TestListEntry> Entries;

        public TestListEntryCollection()
        {
            Entries = new List<TestListEntry>();
        }

        public int Count => Entries.Count;

        public bool IsReadOnly => false;

        public void Add(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Entries.Add(item);
            RaiseChanged();
        }

        public void AddRange(IEnumerable<TestListEntry> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var list = items.ToList(); // only enumerate items once
            if (list.Contains(null))
                throw new ArgumentException("Cannot add null entries", nameof(items));

            Entries.AddRange(list);
            if (list.Count > 0)
                RaiseChanged();
        }

        public bool Remove(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            bool result = Entries.Remove(item);
            if (result)
                RaiseChanged();
            return result;
        }

        public void Clear()
        {
            if (Entries.Count > 0)
            {
                Entries.Clear();
                RaiseChanged();
            }
        }

        public bool Contains(TestListEntry item) => Entries.Contains(item);

        public void CopyTo(TestListEntry[] array, int arrayIndex) => Entries.CopyTo(array, arrayIndex);

        public IEnumerator<TestListEntry> GetEnumerator() => Entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
