using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PmlUnit
{
    class TestListViewController
    {
        public event EventHandler SelectionChanged;

        private readonly TestListView View;
        private readonly IList<TestListEntry> AllEntries;
        private readonly IList<TestListEntry> VisibleEntries;

        private bool IgnoreSelectionChanged;
        private int IgnoredSelectionChanges;
        private TestListEntry SelectionStartEntry;
        private TestListEntry FocusedEntryField;

        public TestListViewController(TestListView view, IList<TestListEntry> allEntries, IList<TestListEntry> visibleEntries)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (allEntries == null)
                throw new ArgumentNullException(nameof(allEntries));
            if (visibleEntries == null)
                throw new ArgumentNullException(nameof(visibleEntries));

            View = view;
            AllEntries = allEntries;
            VisibleEntries = visibleEntries;

            IgnoreSelectionChanged = false;
            IgnoredSelectionChanges = 0;
        }

        public TestListEntry FocusedEntry
        {
            get { return FocusedEntryField; }
            private set
            {
                if (value != FocusedEntryField)
                {
                    FocusedEntryField = value;
                    View.Invalidate();
                }
            }
        }

        public void HandleSelectionChanged(object sender, EventArgs e)
        {
            if (IgnoreSelectionChanged)
                IgnoredSelectionChanges++;
            else
                SelectionChanged?.Invoke(this, e);
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
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
                int index = VisibleEntries.IndexOf(FocusedEntry);
                if (index > 0)
                    target = VisibleEntries[index - 1];
                else
                    target = VisibleEntries.FirstOrDefault();
            }
            else if (e.KeyCode == Keys.Down)
            {

                int index = VisibleEntries.IndexOf(FocusedEntry);
                if (index < VisibleEntries.Count - 1)
                    target = VisibleEntries[index + 1];
                else
                    target = VisibleEntries.LastOrDefault();
            }
            else if (e.KeyCode == Keys.Left)
            {
                var entry = FocusedEntry as TestListTestEntry;
                if (entry != null)
                {
                    target = entry.Group;
                }
                else
                {
                    var group = FocusedEntry as TestListGroupEntry;
                    if (group != null)
                    {
                        group.IsExpanded = false;
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

        public void HandleMouseClick(MouseEventArgs e, Keys modifierKeys)
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
                    var relativeClickLocation = new Point(e.X, (e.Y + View.VerticalScroll.Value) % View.EntryHeight);
                    var iconBounds = new Rectangle(Point.Empty, View.ImageSize);
                    iconBounds.Inflate(2 * View.EntryPadding, 2 * View.EntryPadding);
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

        public void HandleMouseDoubleClick(MouseEventArgs e, Keys modifierKeys)
        {
            if (modifierKeys == Keys.None && e.Button == MouseButtons.Left)
            {
                var group = FindEntry(e.Location) as TestListGroupEntry;
                if (group != null)
                    group.IsExpanded = !group.IsExpanded;
            }
        }

        private TestListEntry FindEntry(Point location)
        {
            int clientY = location.Y + View.VerticalScroll.Value;
            int index = clientY / View.EntryHeight;
            if (index < 0 || index >= VisibleEntries.Count)
                return null;
            else
                return VisibleEntries[index];
        }

        private void ScrollEntryIntoView(TestListEntry entry)
        {
            int index = VisibleEntries.IndexOf(entry);
            if (index >= 0)
            {
                int offset = index * View.EntryHeight;
                int top = View.VerticalScroll.Value;
                int bottom = View.VerticalScroll.Value + View.ClientSize.Height;
                int min = View.VerticalScroll.Minimum;
                int max = View.VerticalScroll.Maximum;
                if (offset < top)
                {
                    View.AutoScrollPosition = new Point(0, Math.Max(min, Math.Min(offset, max)));
                }
                else if (offset + View.EntryHeight > bottom)
                {
                    offset -= View.ClientSize.Height - View.EntryHeight;
                    View.AutoScrollPosition = new Point(0, Math.Max(min, Math.Min(offset, max)));
                }
            }
        }

        private void SelectOnly(TestListEntry target)
        {
            View.SelectedEntries.Clear();

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
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
