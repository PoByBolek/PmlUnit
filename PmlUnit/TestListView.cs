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
        private const int EntryHeight = 20;
        private const int EntryPadding = 2;

        private const string ExpandedImageKey = "Expanded";
        private const string ExpandedHighlightImageKey = "ExpandedHighlight";
        private const string CollapsedImageKey = "Collapsed";
        private const string CollapsedHighlightImageKey = "CollapsedHighlight";

        private const string SuccessImageKey = "Success";
        private const string FailureImageKey = "Failure";
        private const string NotExecutedImageKey = "Unknown";

        [Category("Behavior")]
        public event EventHandler SelectionChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TestCaseCollection TestCases { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ReadOnlyTestListTestEntryCollection Entries { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ReadOnlyTestListGroupEntryCollection Groups { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TestListSelectedEntryCollection SelectedEntries { get; }

        private readonly TestListTestEntryCollection EntriesField;
        private readonly TestListGroupEntryCollection GroupsField;
        private readonly SortedList<string, TestListEntry> AllEntries;
        private readonly SortedList<string, TestListEntry> VisibleEntries;

        private bool IgnoreSelectionChanged;
        private int IgnoredSelectionChanges;
        private TestListEntry SelectionStartEntry;
        private TestListEntry FocusedEntryField;

        public TestListView()
        {
            TestCases = new TestCaseCollection();
            TestCases.Changed += OnTestCasesChanged;
            EntriesField = new TestListTestEntryCollection();
            Entries = EntriesField.AsReadOnly();
            GroupsField = new TestListGroupEntryCollection();
            Groups = GroupsField.AsReadOnly();

            AllEntries = new SortedList<string, TestListEntry>(StringComparer.OrdinalIgnoreCase);
            VisibleEntries = new SortedList<string, TestListEntry>(StringComparer.OrdinalIgnoreCase);
            SelectedEntries = new TestListSelectedEntryCollection(AllEntries.Values);

            InitializeComponent();

            DoubleBuffered = true;
            IgnoreSelectionChanged = false;
            IgnoredSelectionChanges = 0;

            EntryImages.Images.Add(ExpandedImageKey, Resources.Expanded);
            EntryImages.Images.Add(ExpandedHighlightImageKey, Resources.ExpandedHighlight);
            EntryImages.Images.Add(CollapsedImageKey, Resources.Collapsed);
            EntryImages.Images.Add(CollapsedHighlightImageKey, Resources.CollapsedHighlight);

            EntryImages.Images.Add(NotExecutedImageKey, Resources.Unknown);
            EntryImages.Images.Add(FailureImageKey, Resources.Failure);
            EntryImages.Images.Add(SuccessImageKey, Resources.Success);
        }

        private void OnTestCasesChanged(object sender, TestCasesChangedEventArgs e)
        {
            foreach (var testCase in e.RemovedTestCases)
            {
                GroupsField.Remove(testCase.Name);
                AllEntries.Remove(testCase.Name);
                VisibleEntries.Remove(testCase.Name);
                foreach (var test in testCase.Tests)
                {
                    EntriesField.Remove(test);
                    AllEntries.Remove(test.FullName);
                    VisibleEntries.Remove(test.FullName);
                }
            }
            foreach (var testCase in e.AddedTestCases)
            {
                var group = new TestListGroupEntry(testCase.Name);
                group.SelectionChanged += OnSelectionChanged;
                group.ExpandedChanged += OnGroupExpandedChanged;
                foreach (var test in testCase.Tests)
                {
                    var entry = EntriesField.Add(test);
                    entry.SelectionChanged += OnSelectionChanged;
                    entry.ResultChanged += OnTestResultChanged;
                    group.Entries.Add(entry);
                    AllEntries.Add(test.FullName, entry);
                    VisibleEntries.Add(test.FullName, entry);
                }
                GroupsField.Add(group);
                AllEntries.Add(testCase.Name, group);
                VisibleEntries.Add(testCase.Name, group);
            }

            FocusedEntry = null;
            SelectionStartEntry = GroupsField.FirstOrDefault();
            AutoScrollMinSize = new Size(0, VisibleEntries.Count * EntryHeight);

            Invalidate();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Test> AllTests => AllTestsInternal.ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Test> SucceededTests => AllTestsInternal.Where(test => test.Status == TestStatus.Successful).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Test> FailedTests => AllTestsInternal.Where(test => test.Status == TestStatus.Failed).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Test> NotExecutedTests => AllTestsInternal.Where(test => test.Status == TestStatus.NotExecuted).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Test> SelectedTests
        {
            get
            {
                var result = new HashSet<Test>();
                foreach (var group in GroupsField)
                {
                    if (group.Selected)
                    {
                        foreach (var entry in group.Entries)
                            result.Add(entry.Test);
                    }
                    else
                    {
                        foreach (var entry in group.Entries)
                        {
                            if (entry.Selected)
                                result.Add(entry.Test);
                        }
                    }
                }
                return result.ToList();
            }
        }

        private IEnumerable<Test> AllTestsInternal => Entries.Select(entry => entry.Test);

        private TestListEntry FocusedEntry
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

            using (var options = new TestListPaintOptions(this, e.ClipRectangle, FocusedEntryField))
            {
                int width = ClientSize.Width;
                int offset = VerticalScroll.Value;
                int startIndex = (e.ClipRectangle.Top + offset) / EntryHeight;
                startIndex = Math.Max(0, startIndex);
                int endIndex = (e.ClipRectangle.Bottom + offset) / EntryHeight;
                endIndex = Math.Min(endIndex, VisibleEntries.Count - 1);

                for (int i = startIndex; i <= endIndex; i++)
                {
                    var bounds = new Rectangle(0, i * EntryHeight - offset, width, EntryHeight);
                    var entry = VisibleEntries.Values[i];
                    PaintEntryBackground(g, entry, bounds, options);

                    var testEntry = entry as TestListTestEntry;
                    if (testEntry != null)
                    {
                        PaintTestEntry(g, testEntry, bounds, options);
                        continue;
                    }

                    var groupEntry = entry as TestListGroupEntry;
                    if (groupEntry != null)
                    {
                        PaintGroupEntry(g, groupEntry, bounds, options);
                        continue;
                    }
                }
            }
        }

        private void PaintTestEntry(Graphics g, TestListTestEntry entry, Rectangle bounds, TestListPaintOptions options)
        {
            int left = bounds.Left + EntryPadding + 20;
            int right = bounds.Right - EntryPadding;
            int y = bounds.Top + EntryPadding;

            g.DrawImage(GetTestEntryImage(entry.Test.Status), left, y);
            left += 16 + EntryPadding;

            var textBrush = options.GetTextBrush(entry);
            if (left < right && entry.Test.Result != null)
            {
                string duration = entry.Test.Result.Duration.Format();
                int durationWidth = (int)Math.Ceiling(g.MeasureString(duration, options.EntryFont).Width);
                int durationX = Math.Max(left, right - durationWidth);
                g.DrawString(duration, options.EntryFont, textBrush, durationX, y);
                right = durationX - EntryPadding;
            }

            if (left < right)
            {
                var nameBounds = new RectangleF(left, y, right - left, 16);
                g.DrawString(entry.Test.Name, options.EntryFont, textBrush, nameBounds, options.EntryFormat);
            }
        }

        private void PaintGroupEntry(Graphics g, TestListGroupEntry group, Rectangle bounds, TestListPaintOptions options)
        {
            int x = bounds.Left + EntryPadding;
            int y = bounds.Top + EntryPadding;

            g.DrawImage(GetGroupEntryImage(group.IsExpanded), x, y);
            x += 16 + EntryPadding;

            var textBrush = options.GetTextBrush(group);
            int nameWidth = (int)Math.Ceiling(g.MeasureString(group.Name, options.HeaderFont).Width);
            g.DrawString(group.Name, options.HeaderFont, textBrush, x, y);
            x += nameWidth;

            var count = " (" + group.Entries.Count + ")";
            g.DrawString(count, options.EntryFont, textBrush, x, y);
        }

        private void PaintEntryBackground(Graphics g, TestListEntry entry, Rectangle bounds, TestListPaintOptions options)
        {
            if (entry.Selected)
            {
                g.FillRectangle(options.SelectedBackBrush, bounds);
            }
            else if (entry == FocusedEntryField)
            {
                bounds.Width -= 1;
                bounds.Height -= 1;
                g.DrawRectangle(options.FocusRectanglePen, bounds);
            }
        }

        private Image GetTestEntryImage(TestStatus status)
        {
            if (status == TestStatus.NotExecuted)
                return EntryImages.Images[NotExecutedImageKey];
            else if (status == TestStatus.Successful)
                return EntryImages.Images[SuccessImageKey];
            else
                return EntryImages.Images[FailureImageKey];
        }

        private Image GetGroupEntryImage(bool expanded)
        {
            if (expanded)
                return EntryImages.Images[ExpandedImageKey];
            else
                return EntryImages.Images[CollapsedImageKey];
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

            TestListEntry target = null;

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
                int index = VisibleEntries.IndexOfValue(FocusedEntry);
                if (index > 0)
                    target = VisibleEntries.Values[index - 1];
                else
                    target = VisibleEntries.Values.FirstOrDefault();
            }
            else if (e.KeyCode == Keys.Down)
            {
                int index = VisibleEntries.IndexOfValue(FocusedEntry);
                if (index < VisibleEntries.Count - 1)
                    target = VisibleEntries.Values[index + 1];
                else
                    target = VisibleEntries.Values.LastOrDefault();
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
                    foreach (var group in GroupsField)
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
                        target = focusedGroup.Entries.FirstOrDefault();
                    else
                        focusedGroup.IsExpanded = true;
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
            OnMouseClick(e, ModifierKeys);
        }

        private void OnMouseClick(MouseEventArgs e, Keys modifierKeys)
        {
            var clicked = FindEntry(e.Location);
            bool left = e.Button == MouseButtons.Left;
            bool right = e.Button == MouseButtons.Right;

            BeginUpdate();
            try
            {
                if (modifierKeys == Keys.None)
                {
                    var group = clicked as TestListGroupEntry;
                    var relativeClickLocation = new Point(e.X, (e.Y + VerticalScroll.Value) % EntryHeight);
                    var iconBounds = new Rectangle(Point.Empty, EntryImages.ImageSize);
                    iconBounds.Inflate(2 * EntryPadding, 2 * EntryPadding);
                    if (left && group != null && iconBounds.Contains(relativeClickLocation))
                        group.IsExpanded = !group.IsExpanded;
                    else if (left || right)
                        SelectOnly(clicked);
                }
                else if (left && modifierKeys == Keys.Control && clicked != null)
                {
                    clicked.Selected = !clicked.Selected;
                    FocusedEntry = clicked;
                    SelectionStartEntry = clicked;
                }
                else if (left && modifierKeys == Keys.Shift && clicked != null)
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

        private TestListEntry FindEntry(Point location)
        {
            int clientY = location.Y + VerticalScroll.Value;
            int index = clientY / EntryHeight;
            if (index < 0 || index >= VisibleEntries.Count)
                return null;
            else
                return VisibleEntries.Values[index];
        }

        private void ScrollEntryIntoView(TestListEntry entry)
        {
            int index = VisibleEntries.IndexOfValue(entry);
            if (index >= 0)
            {
                int offset = index * EntryHeight;
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
            }
        }

        private void SelectOnly(TestListEntry target)
        {
            foreach (var entry in AllEntries.Values)
                entry.Selected = false;

            if (target != null)
            {
                target.Selected = true;
                SelectionStartEntry = target;
                FocusedEntry = target;
            }
        }

        private void SelectRange(TestListEntry target)
        {
            bool selected = false;
            foreach (var entry in AllEntries.Values)
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
            var group = sender as TestListGroupEntry;
            if (group.IsExpanded)
            {
                foreach (var entry in group.Entries)
                    VisibleEntries.Add(entry.Test.FullName, entry);
            }
            else
            {
                foreach (var entry in group.Entries)
                    VisibleEntries.Remove(entry.Test.FullName);
            }

            AutoScrollMinSize = new Size(0, VisibleEntries.Count * EntryHeight);
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
