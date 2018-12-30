using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PmlUnit.Properties;

namespace PmlUnit
{
    partial class TestListView : UserControl
    {
        [Category("Behavior")]
        public event EventHandler SelectionChanged;

        public TestListView()
        {
            InitializeComponent();
            ResetColumnWidths();

            TestStatusImageList.Images.Add("Unknown", Resources.Unknown);
            TestStatusImageList.Images.Add("Failure", Resources.Failure);
            TestStatusImageList.Images.Add("Success", Resources.Success);
        }

        public void SetTests(IEnumerable<Test> tests)
        {
            TestList.Items.Clear();
            TestList.Groups.Clear();

            foreach (var testGroup in tests.GroupBy(test => test.TestCase))
            {
                var groupName = string.Format(CultureInfo.CurrentCulture, "{0} ({1})", testGroup.Key.Name, testGroup.Count());
                var group = TestList.Groups.Add(testGroup.Key.Name, groupName);
                foreach (var test in testGroup)
                {
                    var item = TestList.Items.Add(test.Name);
                    item.Group = group;
                    item.SubItems.Add("");
                    item.Tag = new Entry(test, item);
                }
            }

            ResetColumnWidths();
        }

        public void ResetColumnWidths()
        {
            ExecutionTimeColumn.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            TestNameColumn.Width = Math.Max(0, TestList.ClientSize.Width - ExecutionTimeColumn.Width);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> AllTests => FilterTests(entry => true);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SucceededTests => FilterTests(entry => entry.Succeeded);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> FailedTests => FilterTests(entry => entry.Failed);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> NotExecutedTests => FilterTests(entry => !entry.HasRun);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SelectedTests => FilterTests(entry => entry.Selected);

        private List<TestListEntry> FilterTests(Func<Entry, bool> predicate)
        {
            var result = new List<TestListEntry>();
            foreach (ListViewItem item in TestList.Items)
            {
                var entry = item.Tag as Entry;
                if (entry != null && predicate(entry))
                    result.Add(entry);
            }
            return result;
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            ResetColumnWidths();
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        private class Entry : TestListEntry
        {
            public Test Test { get; }

            private readonly ListViewItem Item;
            private TestResult ResultField;

            public Entry(Test test, ListViewItem item)
            {
                if (test == null)
                    throw new ArgumentNullException(nameof(test));
                if (item == null)
                    throw new ArgumentNullException(nameof(item));

                Test = test;
                Item = item;
                Item.ImageKey = GetImageKey();
            }

            public TestResult Result
            {
                get { return ResultField; }
                set
                {
                    ResultField = value;
                    Item.ImageKey = GetImageKey();
                    Item.SubItems[1].Text = FormatDuration();
                }
            }

            public bool Succeeded => Result != null && Result.Success;

            public bool Failed => Result != null && !Result.Success;

            public bool HasRun => Result != null;

            public bool Selected
            {
                get { return Item.Selected; }
                set { Item.Selected = value; }
            }

            private string GetImageKey()
            {
                if (Result == null)
                    return "Unknown";
                else if (Result.Success)
                    return "Success";
                else
                    return "Failure";
            }

            private string FormatDuration()
            {
                if (Result == null)
                    return "";
                else
                    return Result.Duration.Format();
            }
        }
    }

    interface TestListEntry
    {
        Test Test { get; }
        TestResult Result { get; set; }
        bool Selected { get; set; }
    }
}
