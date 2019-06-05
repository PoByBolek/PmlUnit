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
        private TestListEntry LastSelectedEntry;

        public TestListView()
        {
            InitializeComponent();

            ExpanderImageList.Images.Add(TestListGroupEntry.ExpandedImageKey, Resources.Expanded);
            ExpanderImageList.Images.Add(TestListGroupEntry.ExpandedHighlightImageKey, Resources.ExpandedHighlight);
            ExpanderImageList.Images.Add(TestListGroupEntry.CollapsedImageKey, Resources.Collapsed);
            ExpanderImageList.Images.Add(TestListGroupEntry.CollapsedHighlightImageKey, Resources.CollapsedHighlight);

            StatusImageList.Images.Add(TestListViewEntry.NotExecutedImageKey, Resources.Unknown);
            StatusImageList.Images.Add(TestListViewEntry.FailureImageKey, Resources.Failure);
            StatusImageList.Images.Add(TestListViewEntry.SuccessImageKey, Resources.Success);
        }

        public void SetTests(IEnumerable<Test> tests)
        {
            GroupPanel.Clear();
            LastSelectedEntry = null;

            foreach (var testGroup in tests.GroupBy(test => test.TestCase))
            {
                var group = new TestListGroupEntry(testGroup.Key.Name);
                try
                {
                    foreach (var test in testGroup)
                        group.Add(test);

                    group.ExpanderImageList = ExpanderImageList;
                    group.StatusImageList = StatusImageList;
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
                if (e.Entry.Selected)
                    LastSelectedEntry = e.Entry;
            }
            else if (ModifierKeys == Keys.Shift)
            {
                try
                {
                    IgnoreSelectionChanged = true;
                    if (LastSelectedEntry == null)
                        LastSelectedEntry = Entries.FirstOrDefault();

                    var selected = false;
                    foreach (var entry in Entries)
                    {
                        if (entry == e.Entry || entry == LastSelectedEntry)
                        {
                            entry.Selected = true;
                            selected = e.Entry == LastSelectedEntry ? false : !selected;
                        }
                        else
                        {
                            entry.Selected = selected;
                        }
                    }
                }
                finally
                {
                    IgnoreSelectionChanged = false;
                    OnSelectionChanged(this, EventArgs.Empty);
                }
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
                LastSelectedEntry = e.Entry;
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!IgnoreSelectionChanged)
                SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
