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
        public const int EntryHeight = 20;

        [Category("Behavior")]
        public event EventHandler SelectionChanged;

        private readonly List<TestListGroupEntry> Groups;
        private bool IgnoreSelectionChanged;
        private TestListBaseEntry SelectionStartEntry;
        private TestListBaseEntry FocusedEntry;

        public TestListView()
        {
            InitializeComponent();

            DoubleBuffered = true;
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

            FocusedEntry = null;
            SelectionStartEntry = Groups.FirstOrDefault();
            AutoScrollMinSize = new Size(0, EntryHeight * Groups.Sum(group => 1 + group.Entries.Count));

            Invalidate();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> AllTests => AllTestEntries.ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SucceededTests => AllTestEntries.Where(entry => entry.Result != null && entry.Result.Error == null).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> FailedTests => AllTestEntries.Where(entry => entry.Result?.Error != null).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> NotExecutedTests => AllTestEntries.Where(entry => entry.Result == null).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SelectedTests => AllTestEntries.Where(entry => entry.Selected).ToList();

        private IEnumerable<TestListEntry> AllTestEntries => Groups.SelectMany(group => group.Entries);

        private IEnumerable<TestListBaseEntry> AllEntries
        {
            get
            {
                foreach (var group in Groups)
                {
                    yield return group;
                    foreach (var entry in group.Entries)
                        yield return entry;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            using (var brush = new SolidBrush(BackColor))
            {
                g.FillRectangle(new SolidBrush(BackColor), e.ClipRectangle);
            }

            using (var options = new TestListPaintOptions(this, e.ClipRectangle, StatusImageList, ExpanderImageList))
            {
                int width = ClientSize.Width;
                int minY = e.ClipRectangle.Top;
                int maxY = e.ClipRectangle.Bottom;
                int y = -VerticalScroll.Value;

                foreach (var group in Groups)
                {
                    if (y > maxY)
                        break;

                    int height = group.Height;
                    if (y + height >= minY)
                    {
                        var bounds = new Rectangle(0, y, width, height);
                        group.Paint(g, bounds, options);
                    }
                    y += height;
                }
            }
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            Invalidate(); // We need to repaint everything because of the ellipsis characters in too long test names
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Select();

            var clicked = FindEntry(e.Location);
            bool left = e.Button == MouseButtons.Left;
            bool right = e.Button == MouseButtons.Right;

            if ((left || right) && ModifierKeys == Keys.None)
            {
                foreach (var entry in AllEntries)
                    entry.Selected = false;

                if (clicked != null)
                {
                    clicked.Selected = true;
                    FocusedEntry = clicked;
                    SelectionStartEntry = clicked;
                }

                Invalidate();
            }
            else if (left && ModifierKeys == Keys.Control && clicked != null)
            {
                clicked.Selected = !clicked.Selected;
                FocusedEntry = clicked;
                SelectionStartEntry = clicked;

                Invalidate();
            }
            else if (left && ModifierKeys == Keys.Shift && clicked != null)
            {
                bool selected = false;
                foreach (var entry in AllEntries)
                {
                    if (entry == clicked || entry == SelectionStartEntry)
                    {
                        entry.Selected = true;
                        selected = clicked == SelectionStartEntry ? false : !selected;
                    }
                    else
                    {
                        entry.Selected = selected;
                    }
                }

                Invalidate();
            }
        }

        private TestListBaseEntry FindEntry(Point location)
        {
            int y = -VerticalScroll.Value;
            int target = location.Y;

            foreach (var group in Groups)
            {
                if (target < y)
                    return null;
                if (target >= y && target < y + EntryHeight)
                    return group;
                y += EntryHeight;

                if (group.IsExpanded)
                {
                    int height = group.Entries.Count * EntryHeight;
                    if (target >= y && target < y + height)
                    {
                        int index = (target - y) / EntryHeight;
                        return group.Entries[index];
                    }
                    y += height;
                }
            }
            return null;
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

    interface TestListBaseEntry
    {
        bool Selected { get; set; }
        void Paint(Graphics g, Rectangle bounds, TestListPaintOptions options);
    }

    class TestListPaintOptions : IDisposable
    {
        public Rectangle ClipRectangle { get; }
        public Brush NormalTextBrush { get; }
        public Brush SelectedTextBrush { get; }
        public Brush SelectedBackBrush { get; }
        public ImageList StatusImageList { get; }
        public ImageList ExpanderImageList { get; }
        public Font EntryFont { get; }
        public Font HeaderFont { get; }
        public StringFormat EntryFormat { get; }

        public TestListPaintOptions(TestListView view, Rectangle clipRectangle, ImageList statusImageList, ImageList expanderImageList)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (statusImageList == null)
                throw new ArgumentNullException(nameof(statusImageList));
            if (expanderImageList == null)
                throw new ArgumentNullException(nameof(expanderImageList));

            try
            {
                ClipRectangle = clipRectangle;
                StatusImageList = statusImageList;
                ExpanderImageList = expanderImageList;
                EntryFont = view.Font;
                NormalTextBrush = new SolidBrush(view.ForeColor);
                SelectedTextBrush = SystemBrushes.HighlightText; // don't dispose the system brushes
                SelectedBackBrush = SystemBrushes.Highlight; // don't dispose the system brushes
                HeaderFont = new Font(view.Font, FontStyle.Bold);
                EntryFormat = new StringFormat(StringFormatFlags.NoWrap);
                EntryFormat.Trimming = StringTrimming.EllipsisCharacter;
            }
            catch
            {
                if (NormalTextBrush != null)
                    NormalTextBrush.Dispose();
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
                NormalTextBrush.Dispose();
                HeaderFont.Dispose();
                EntryFormat.Dispose();
            }
        }
    }
}
