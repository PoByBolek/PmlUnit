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

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TestCaseCollection TestCases { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TestListEntryCollection Entries { get; }

        private readonly SortedList<string, TestListGroupEntry> Groups;
        private readonly WritableTestListEntryCollection EntriesField;
        private readonly SortedList<string, TestListBaseEntry> AllEntries;
        private readonly SortedList<string, TestListBaseEntry> VisibleEntries;

        private bool IgnoreSelectionChanged;
        private int IgnoredSelectionChanges;
        private TestListBaseEntry SelectionStartEntry;
        private TestListBaseEntry FocusedEntryField;

        public TestListView()
        {
            TestCases = new TestCaseCollection();
            TestCases.Changed += OnTestCasesChanged;
            Groups = new SortedList<string, TestListGroupEntry>(StringComparer.OrdinalIgnoreCase);
            EntriesField = new WritableTestListEntryCollection();
            Entries = EntriesField.AsReadOnly();
            AllEntries = new SortedList<string, TestListBaseEntry>(StringComparer.OrdinalIgnoreCase);
            VisibleEntries = new SortedList<string, TestListBaseEntry>(StringComparer.OrdinalIgnoreCase);

            InitializeComponent();

            DoubleBuffered = true;
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

        private void OnTestCasesChanged(object sender, TestCasesChangedEventArgs e)
        {
            foreach (var testCase in e.RemovedTestCases)
            {
                Groups.Remove(testCase.Name);
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
                    var entry = EntriesField.Add(test) as TestListViewEntry;
                    entry.SelectionChanged += OnSelectionChanged;
                    entry.ResultChanged += OnTestResultChanged;
                    group.Add(entry);
                    AllEntries.Add(test.FullName, entry);
                    VisibleEntries.Add(test.FullName, entry);
                }
                Groups.Add(testCase.Name, group);
                AllEntries.Add(testCase.Name, group);
                VisibleEntries.Add(testCase.Name, group);
            }

            FocusedEntry = null;
            SelectionStartEntry = Groups.Values.FirstOrDefault();
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
                foreach (var group in Groups.Values)
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
                int offset = VerticalScroll.Value;
                int startIndex = (e.ClipRectangle.Top + offset) / EntryHeight;
                startIndex = Math.Max(0, startIndex);
                int endIndex = (e.ClipRectangle.Bottom + offset) / EntryHeight;
                endIndex = Math.Min(endIndex, VisibleEntries.Count - 1);

                for (int i = startIndex; i <= endIndex; i++)
                {
                    var entry = VisibleEntries.Values[i];
                    var bounds = new Rectangle(0, i * EntryHeight - offset, width, EntryHeight);
                    entry.Paint(g, bounds, options);
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
                    foreach (var group in Groups.Values)
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
                    if (left && group != null && group.IconBounds.Contains(relativeClickLocation))
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

        private TestListBaseEntry FindEntry(Point location)
        {
            int clientY = location.Y + VerticalScroll.Value;
            int index = clientY / EntryHeight;
            if (index < 0 || index >= VisibleEntries.Count)
                return null;
            else
                return VisibleEntries.Values[index];
        }

        private void ScrollEntryIntoView(TestListBaseEntry entry)
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

        private void SelectOnly(TestListBaseEntry target)
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

        private void SelectRange(TestListBaseEntry target)
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
