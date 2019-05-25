// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                DisposeChildControls();
            }
            base.Dispose(disposing);
        }

        private void DisposeChildControls()
        {
            var children = Controls.OfType<Control>().ToList();
            Controls.Clear();
            foreach (var child in children)
                child.Dispose();
        }

        public void SetTests(IEnumerable<Test> tests)
        {
            DisposeChildControls();

            var minimumSize = new Size();

            foreach (var testGroup in tests.GroupBy(test => test.TestCase))
            {
                var group = new TestListGroupEntry(testGroup.Key.Name);
                try
                {
                    Controls.Add(group);
                    group.Top = minimumSize.Height;
                    group.Width = Width;
                    group.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    group.ImageList = TestStatusImageList;

                    foreach (var test in testGroup)
                        group.Add(test);

                    minimumSize.Width = Math.Max(minimumSize.Width, group.MinimumSize.Width);
                    minimumSize.Height += group.Height;
                }
                catch
                {
                    group.Dispose();
                    throw;
                }
            }

            Height = minimumSize.Height;
            MinimumSize = minimumSize;
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
            foreach (Control child in Controls)
            {
                var group = child as TestListGroupEntry;
                if (group != null)
                    result.AddRange(group.Entries.Where(predicate));
            }
            return result;
        }
    }
}
