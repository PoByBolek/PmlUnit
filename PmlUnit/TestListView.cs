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
        private TestListBaseEntry FocusedEntryField;

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

        private IEnumerable<TestListBaseEntry> VisibleEntries
        {
            get
            {
                foreach (var group in Groups)
                {
                    yield return group;
                    if (group.IsExpanded)
                    {
                        foreach (var entry in group.Entries)
                            yield return entry;
                    }
                }
            }
        }

        private TestListBaseEntry FocusedEntry
        {
            get { return FocusedEntryField; }
            set
            {
                if (value != FocusedEntryField)
                {
                    FocusedEntryField = value;
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            using (var brush = new SolidBrush(BackColor))
            {
                g.FillRectangle(brush, e.ClipRectangle);
            }

            using (var options = new TestListPaintOptions(this, e.ClipRectangle, FocusedEntry, StatusImageList, ExpanderImageList))
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

        protected override bool IsInputKey(Keys keys)
        {
            switch (keys)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Control | Keys.Up:
                case Keys.Control | Keys.Down:
                case Keys.Control | Keys.Left:
                case Keys.Control | Keys.Right:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Right:
                    return true;
                default:
                    return base.IsInputKey(keys);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (FocusedEntry == null)
                return;

            TestListBaseEntry target = null;

            if (e.KeyCode == Keys.Space)
            {
                if (e.Modifiers == Keys.None)
                    FocusedEntry.Selected = true;
                else if (e.Modifiers == Keys.Control)
                    FocusedEntry.Selected = !FocusedEntry.Selected;
                else if (e.Modifiers == Keys.Shift)
                    SelectRange(FocusedEntry);
                return;
            }
            else if (e.KeyCode == Keys.Up)
            {
                target = Groups.FirstOrDefault();
                foreach (var entry in VisibleEntries)
                {
                    if (entry == FocusedEntry)
                        break;
                    target = entry;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                bool stop = false;
                foreach (var entry in VisibleEntries)
                {
                    target = entry;
                    if (stop)
                        break;
                    else if (entry == FocusedEntry)
                        stop = true;
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                var focusedGroup = FocusedEntry as TestListGroupEntry;
                if (focusedGroup != null)
                {
                    focusedGroup.IsExpanded = false;
                }
                else
                {
                    foreach (var group in Groups)
                    {
                        foreach (var entry in group.Entries)
                        {
                            if (entry == FocusedEntry)
                            {
                                target = group;
                                break;
                            }
                        }
                        if (target == group)
                            break;
                    }
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                var focusedGroup = FocusedEntry as TestListGroupEntry;
                if (focusedGroup != null)
                {
                    if (focusedGroup.IsExpanded)
                    {
                        if (focusedGroup.Entries.Count > 0)
                            target = focusedGroup.Entries[0];
                    }
                    else
                    {
                        focusedGroup.IsExpanded = true;
                    }
                }
            }

            if (target != null)
            {
                if (e.Modifiers == Keys.None)
                    SelectOnly(target);
                else if (e.Modifiers == Keys.Shift)
                    SelectRange(target);
                else if (e.Modifiers == Keys.Control)
                    FocusedEntry = target;

                ScrollEntryIntoView(target);
            }
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
                        group.IsExpanded = !group.IsExpanded;
                    else if (left || right)
                        SelectOnly(clicked);
                }
                else if (left && ModifierKeys == Keys.Control && clicked != null)
                {
                    clicked.Selected = !clicked.Selected;
                    FocusedEntry = clicked;
                    SelectionStartEntry = clicked;
                }
                else if (left && ModifierKeys == Keys.Shift && clicked != null)
                {
                    SelectRange(clicked);
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

        private void ScrollEntryIntoView(TestListBaseEntry entry)
        {
            int offset = 0;
            foreach (var e in VisibleEntries)
            {
                if (e == entry)
                {
                    int top = VerticalScroll.Value;
                    int bottom = VerticalScroll.Value + ClientSize.Height;
                    int min = VerticalScroll.Minimum;
                    int max = VerticalScroll.Maximum;
                    if (offset < top)
                    {
                        AutoScrollPosition = new Point(0, Math.Max(min, Math.Min(offset, max)));
                    }
                    else if (offset + EntryHeight > bottom)
                    {
                        offset -= ClientSize.Height - EntryHeight;
                        AutoScrollPosition = new Point(0, Math.Max(min, Math.Min(offset, max)));
                    }
                    return;
                }
                offset += EntryHeight;
            }
        }

        private void SelectOnly(TestListBaseEntry target)
        {
            foreach (var entry in AllEntries)
                entry.Selected = false;

            if (target != null)
            {
                target.Selected = true;
                SelectionStartEntry = target;
                FocusedEntry = target;
            }
        }

        private void SelectRange(TestListBaseEntry target)
        {
            bool selected = false;
            foreach (var entry in AllEntries)
            {
                if (entry == target || entry == SelectionStartEntry)
                {
                    entry.Selected = true;
                    selected = target == SelectionStartEntry ? false : !selected;
                }
                else
                {
                    entry.Selected = selected;
                }
            }

            FocusedEntry = target;
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
                OnSelectionChanged(this, EventArgs.Empty);
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (IgnoreSelectionChanged)
            {
                IgnoredSelectionChanges++;
            }
            else
            {
                Invalidate();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnTestResultChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
