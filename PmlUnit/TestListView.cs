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
    partial class TestListView : ScrollableControl
    {
        [Category("Behavior")]
        public event EventHandler SelectionChanged;

        private bool IgnoreSelectionChanged;
        private TestListEntry FocusedEntry;
        private readonly List<TestListViewEntry> Entries;

        private const int EntryHeight = 20;

        public TestListView()
        {
            InitializeComponent();

            Entries = new List<TestListViewEntry>();

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
            Entries.Clear();
            Entries.AddRange(tests.Select(test => new TestListViewEntry(test, this)));
            AutoScrollMinSize = new Size(0, EntryHeight * Entries.Count);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> AllTests => AllEntries.ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SucceededTests => AllEntries.Where(entry => entry.Result != null && entry.Result.Error == null).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> FailedTests => AllEntries.Where(entry => entry.Result?.Error != null).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> NotExecutedTests => AllEntries.Where(entry => entry.Result == null).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SelectedTests => AllEntries.Where(entry => entry.Selected).ToList();

        private IEnumerable<TestListGroupEntry> Groups => Enumerable.Empty<TestListGroupEntry>();

        private IEnumerable<TestListEntry> AllEntries => Entries.OfType<TestListEntry>();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            using (var brush = new SolidBrush(BackColor))
            {
                g.FillRectangle(new SolidBrush(BackColor), e.ClipRectangle);
            }

            using (var brush = new SolidBrush(ForeColor))
            using (var format = new StringFormat(StringFormatFlags.NoWrap))
            {
                format.Trimming = StringTrimming.EllipsisCharacter;
                
                int offset = VerticalScroll.Value;
                int width = ClientSize.Width;
                int startIndex = Math.Max(0, (e.ClipRectangle.Top + offset) / EntryHeight);
                int endIndex = Math.Min((e.ClipRectangle.Bottom + offset) / EntryHeight + 1, Entries.Count);

                for (int i = startIndex; i < endIndex; i++)
                {
                    var bounds = new Rectangle(0, i * EntryHeight - offset, width, EntryHeight);
                    Entries[i].Paint(g, bounds, StatusImageList, brush, format);
                }
            }
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            Invalidate(); // We need to repaint everything because of the ellipsis characters in too long test names
        }

        private void OnGroupSizeChanged(object sender, EventArgs e)
        {
        }

        private void OnGroupClick(object sender, EventArgs e)
        {
            var group = sender as TestListGroupEntry;
            if (group == null)
                return;

            try
            {
                IgnoreSelectionChanged = true;
                foreach (var entry in Entries)
                    entry.Selected = false;
                foreach (var entry in group.Entries)
                    entry.Selected = true;
                FocusedEntry = group.Entries.FirstOrDefault();
            }
            finally
            {
                IgnoreSelectionChanged = false;
                OnSelectionChanged(this, EventArgs.Empty);
            }
        }

        private void OnEntryClick(object sender, EntryClickEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                e.Entry.Selected = !e.Entry.Selected;
                if (e.Entry.Selected)
                    FocusedEntry = e.Entry;
            }
            else if (ModifierKeys == Keys.Shift)
            {
                try
                {
                    IgnoreSelectionChanged = true;
                    if (FocusedEntry == null)
                        FocusedEntry = Entries.FirstOrDefault();

                    var selected = false;
                    foreach (var entry in Entries)
                    {
                        if (entry == e.Entry || entry == FocusedEntry)
                        {
                            entry.Selected = true;
                            selected = e.Entry == FocusedEntry ? false : !selected;
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
                FocusedEntry = e.Entry;
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!IgnoreSelectionChanged)
                SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
