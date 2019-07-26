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
        private int IgnoredSelectionChanges;
        private TestListBaseEntry SelectionStartEntry;
        private TestListBaseEntry FocusedEntry;

        public TestListView()
        {
            InitializeComponent();

            DoubleBuffered = true;
            Groups = new List<TestListGroupEntry>();
            IgnoreSelectionChanged = false;
            IgnoredSelectionChanges = 0;

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
                group.SelectionChanged += OnSelectionChanged;
                group.ExpandedChanged += OnGroupExpandedChanged;
                foreach (var test in grouping)
                {
                    var entry = new TestListViewEntry(test);
                    entry.SelectionChanged += OnSelectionChanged;
                    entry.ResultChanged += OnTestResultChanged;
                    group.Add(entry);
                }
                Groups.Add(group);
            }

            FocusedEntry = null;
            SelectionStartEntry = Groups.FirstOrDefault();
            AutoScrollMinSize = new Size(0, Groups.Sum(group => group.Height));

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
        public List<TestListEntry> SelectedTests
        {
            get
            {
                var result = new HashSet<TestListEntry>();
                foreach (var group in Groups)
                {
                    if (group.Selected)
                    {
                        foreach (var entry in group.Entries)
                            result.Add(entry);
                    }
                    else
                    {
                        foreach (var entry in group.Entries)
                        {
                            if (entry.Selected)
                                result.Add(entry);
                        }
                    }
                }
                return result.ToList();
            }
        }

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

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Select();

            var clicked = FindEntry(e.Location);
            bool left = e.Button == MouseButtons.Left;
            bool right = e.Button == MouseButtons.Right;

            BeginUpdate();
            try
            {
                if (ModifierKeys == Keys.None)
                {
                    var group = clicked as TestListGroupEntry;
                    var relativeClickLocation = new Point(e.X, (e.Y + VerticalScroll.Value) % EntryHeight);
                    if (left && group != null && group.IconBounds.Contains(relativeClickLocation))
                    {
                        group.IsExpanded = !group.IsExpanded;
                    }
                    else if (left || right)
                    {
                        foreach (var entry in AllEntries)
                            entry.Selected = false;

                        if (clicked != null)
                        {
                            clicked.Selected = true;
                            FocusedEntry = clicked;
                            SelectionStartEntry = clicked;
                        }
                    }
                }
                else if (left && ModifierKeys == Keys.Control && clicked != null)
                {
                    clicked.Selected = !clicked.Selected;
                    FocusedEntry = clicked;
                    SelectionStartEntry = clicked;
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
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (ModifierKeys == Keys.None && e.Button == MouseButtons.Left)
            {
                var group = FindEntry(e.Location) as TestListGroupEntry;
                if (group != null)
                    group.IsExpanded = !group.IsExpanded;
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

        private void OnGroupExpandedChanged(object sender, EventArgs e)
        {
            AutoScrollMinSize = new Size(0, Groups.Sum(group => group.Height));
            Invalidate();
        }

        private void BeginUpdate()
        {
            IgnoreSelectionChanged = true;
            IgnoredSelectionChanges = 0;
        }

        private void EndUpdate()
        {
            IgnoreSelectionChanged = false;
            if (IgnoredSelectionChanges > 0)
            {
                IgnoredSelectionChanges = 0;
                Invalidate();
                OnSelectionChanged(this, EventArgs.Empty);
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (IgnoreSelectionChanged)
                IgnoredSelectionChanges++;
            else
                SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnTestResultChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }

    interface TestListBaseEntry
    {
        event EventHandler SelectionChanged;

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
                SelectedTextBrush = view.Focused ? SystemBrushes.HighlightText.Clone() as Brush : new SolidBrush(view.ForeColor);
                SelectedBackBrush = view.Focused ? SystemBrushes.Highlight.Clone() as Brush : SystemBrushes.Control.Clone() as Brush;
                HeaderFont = new Font(view.Font, FontStyle.Bold);
                EntryFormat = new StringFormat(StringFormatFlags.NoWrap);
                EntryFormat.Trimming = StringTrimming.EllipsisCharacter;
            }
            catch
            {
                if (NormalTextBrush != null)
                    NormalTextBrush.Dispose();
                if (SelectedTextBrush != null)
                    SelectedTextBrush.Dispose();
                if (SelectedBackBrush != null)
                    SelectedBackBrush.Dispose();
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
                SelectedTextBrush.Dispose();
                SelectedBackBrush.Dispose();
                HeaderFont.Dispose();
                EntryFormat.Dispose();
            }
        }
    }
}
