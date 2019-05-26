// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            TestStatusImageList.Images.Add("Unknown", Resources.Unknown);
            TestStatusImageList.Images.Add("Failure", Resources.Failure);
            TestStatusImageList.Images.Add("Success", Resources.Success);
        }

        public void SetTests(IEnumerable<Test> tests)
        {
            GroupPanel.Clear();

            foreach (var testGroup in tests.GroupBy(test => test.TestCase))
            {
                var group = new TestListGroupEntry(testGroup.Key.Name);
                try
                {
                    GroupPanel.Controls.Add(group);
                    group.ImageList = TestStatusImageList;

                    foreach (var test in testGroup)
                        group.Add(test);

                }
                catch
                {
                    group.Dispose();
                    throw;
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> AllTests => FilterTests(entry => true);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SucceededTests => FilterTests(entry => entry.Result != null && entry.Result.Error == null);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> FailedTests => FilterTests(entry => entry.Result?.Error != null);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> NotExecutedTests => FilterTests(entry => entry.Result == null);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SelectedTests => FilterTests(entry => entry.Selected);

        private List<TestListEntry> FilterTests(Func<TestListEntry, bool> predicate)
        {
            var result = new List<TestListEntry>();
            foreach (Control child in GroupPanel.Controls)
            {
                var group = child as TestListGroupEntry;
                if (group != null)
                    result.AddRange(group.Entries.Where(predicate));
            }
            return result;
        }
    }
}
