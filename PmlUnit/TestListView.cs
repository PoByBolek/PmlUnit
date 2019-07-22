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
        private readonly List<TestListGroupEntry> Groups;

        private const int EntryHeight = 20;

        public TestListView()
        {
            InitializeComponent();

            Groups = new List<TestListGroupEntry>();

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
            Groups.Clear();

            foreach (var grouping in tests.GroupBy(test => test.TestCase.Name))
            {
                var group = new TestListGroupEntry(grouping.Key);
                foreach (var test in grouping)
                    group.Add(new TestListViewEntry(test));
                Groups.Add(group);
            }

            AutoScrollMinSize = new Size(0, EntryHeight * Groups.Sum(group => 1 + group.Entries.Count));
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

        private IEnumerable<TestListEntry> AllEntries => Groups.SelectMany(group => group.Entries);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            using (var brush = new SolidBrush(BackColor))
            {
                g.FillRectangle(new SolidBrush(BackColor), e.ClipRectangle);
            }

            using (var options = new TestListPaintOptions(this, StatusImageList, ExpanderImageList))
            {
                int width = ClientSize.Width;
                int minY = e.ClipRectangle.Top;
                int maxY = e.ClipRectangle.Bottom;
                int y = -VerticalScroll.Value;

                foreach (var group in Groups)
                {
                    if (y > maxY)
                        break;

                    if (y + EntryHeight >= minY)
                    {
                        var headerBounds = new Rectangle(0, y, width, EntryHeight);
                        group.Paint(g, headerBounds, options);
                    }
                    y += EntryHeight;

                    if (group.IsExpanded)
                    {
                        int totalHeight = group.Entries.Count * EntryHeight;
                        if (y + totalHeight < minY)
                        {
                            y += totalHeight;
                            continue;
                        }

                        foreach (var entry in group.Entries)
                        {
                            if (y > maxY)
                                break;

                            if (y + EntryHeight >= minY)
                            {
                                var entryBounds = new Rectangle(20, y, width - 20, EntryHeight);
                                entry.Paint(g, entryBounds, options);
                            }
                            y += EntryHeight;
                        }
                    }
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
                //foreach (var entry in Groups)
                //    entry.Selected = false;
                //foreach (var entry in group.Entries)
                //    entry.Selected = true;
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
                    //IgnoreSelectionChanged = true;
                    //if (FocusedEntry == null)
                    //    FocusedEntry = Groups.FirstOrDefault();

                    //var selected = false;
                    //foreach (var entry in Groups)
                    //{
                    //    if (entry == e.Entry || entry == FocusedEntry)
                    //    {
                    //        entry.Selected = true;
                    //        selected = e.Entry == FocusedEntry ? false : !selected;
                    //    }
                    //    else
                    //    {
                    //        entry.Selected = selected;
                    //    }
                    //}
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
                    //foreach (var entry in Groups)
                    //    entry.Selected = false;
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

    class TestListPaintOptions : IDisposable
    {
        public Brush ForeBrush { get; }
        public ImageList StatusImageList { get; }
        public ImageList ExpanderImageList { get; }
        public Font EntryFont { get; }
        public Font HeaderFont { get; }
        public StringFormat EntryFormat { get; }

        public TestListPaintOptions(TestListView view, ImageList statusImageList, ImageList expanderImageList)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (statusImageList == null)
                throw new ArgumentNullException(nameof(statusImageList));
            if (expanderImageList == null)
                throw new ArgumentNullException(nameof(expanderImageList));

            try
            {
                ForeBrush = new SolidBrush(view.ForeColor);
                StatusImageList = statusImageList;
                ExpanderImageList = expanderImageList;
                EntryFont = view.Font;
                HeaderFont = new Font(view.Font, FontStyle.Bold);
                EntryFormat = new StringFormat(StringFormatFlags.NoWrap);
                EntryFormat.Trimming = StringTrimming.EllipsisCharacter;
            }
            catch
            {
                if (HeaderFont != null)
                    HeaderFont.Dispose();
                if (EntryFormat != null)
                    EntryFormat.Dispose();
                throw;
            }
        }

        ~TestListPaintOptions()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HeaderFont.Dispose();
                EntryFormat.Dispose();
            }
        }
    }
}
