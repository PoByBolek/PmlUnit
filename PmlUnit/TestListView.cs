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

        private bool IgnoreSelectionChanged;

        public TestListView()
        {
            InitializeComponent();

            TestStatusImageList.Images.Add(TestListViewEntry.NotExecutedImageKey, Resources.Unknown);
            TestStatusImageList.Images.Add(TestListViewEntry.FailureImageKey, Resources.Failure);
            TestStatusImageList.Images.Add(TestListViewEntry.SuccessImageKey, Resources.Success);
        }

        public void SetTests(IEnumerable<Test> tests)
        {
            GroupPanel.Clear();

            foreach (var testGroup in tests.GroupBy(test => test.TestCase))
            {
                var group = new TestListGroupEntry(testGroup.Key.Name);
                try
                {
                    foreach (var test in testGroup)
                        group.Add(test);

                    group.ImageList = TestStatusImageList;
                    group.SizeChanged += OnGroupSizeChanged;
                    group.EntryClick += OnEntryClick;
                    group.SelectionChanged += OnSelectionChanged;

                    GroupPanel.Controls.Add(group);
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
        public List<TestListEntry> AllTests => Entries.ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SucceededTests => Entries.Where(entry => entry.Result != null && entry.Result.Error == null).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> FailedTests => Entries.Where(entry => entry.Result?.Error != null).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> NotExecutedTests => Entries.Where(entry => entry.Result == null).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SelectedTests => Entries.Where(entry => entry.Selected).ToList();

        private IEnumerable<TestListEntry> Entries => Groups.SelectMany(group => group.Entries);

        private IEnumerable<TestListGroupEntry> Groups => GroupPanel.Controls.OfType<TestListGroupEntry>();

        private void OnGroupSizeChanged(object sender, EventArgs e)
        {
            GroupPanel.Height = GroupPanel.Controls.OfType<Control>().Sum(c => c.Height);
        }

        private void OnEntryClick(object sender, EntryClickEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                e.Entry.Selected = !e.Entry.Selected;
            }
            else if (ModifierKeys == Keys.None)
            {
                try
                {
                    IgnoreSelectionChanged = true;
                    foreach (var entry in Entries)
                        entry.Selected = false;
                }
                finally
                {
                    IgnoreSelectionChanged = false;
                }

                e.Entry.Selected = true;
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!IgnoreSelectionChanged)
                SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
