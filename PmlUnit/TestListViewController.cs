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
        private TestListGroupEntry HighlightedIconEntryField;

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
                    ScrollEntryIntoView(value);
                    View.Invalidate();
                }
            }
        }

        public TestListGroupEntry HighlightedIconEntry
        {
            get { return HighlightedIconEntryField; }
            private set
            {
                if (value != HighlightedIconEntryField)
                {
                    HighlightedIconEntryField = value;
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
            if (VisibleEntries.Count == 0)
                return;
            
            if (e.KeyCode == Keys.Space)
                ToggleSelectionOfFocusedEntry(e.Modifiers);
            else if (e.KeyCode == Keys.Up)
                MoveFocus(-1, e.Modifiers);
            else if (e.KeyCode == Keys.Down)
                MoveFocus(+1, e.Modifiers);
            else if (e.KeyCode == Keys.Left)
                CollapseFocusedGroup(e.Modifiers);
            else if (e.KeyCode == Keys.Right)
                ExpandFocusedGroup(e.Modifiers);
        }

        private void ToggleSelectionOfFocusedEntry(Keys modifierKeys)
        {
            if (modifierKeys == Keys.None)
            {
                FocusedEntry = GetEntryRelativeToFocus(0);
                FocusedEntry.Selected = true;
            }
            else if (modifierKeys == Keys.Control)
            {
                FocusedEntry = GetEntryRelativeToFocus(0);
                FocusedEntry.Selected = !FocusedEntry.Selected;
            }
            else if (modifierKeys == Keys.Shift)
            {
                FocusedEntry = GetEntryRelativeToFocus(0);
                SelectRange(FocusedEntry);
            }
        }

        private void MoveFocus(int offset, Keys modifierKeys)
        {
            MoveFocus(GetEntryRelativeToFocus(offset), modifierKeys);
        }

        private void CollapseFocusedGroup(Keys modifierKeys)
        {
            var focus = GetEntryRelativeToFocus(0);
            var group = focus as TestListGroupEntry;
            var entry = focus as TestListTestEntry;
            if (entry != null)
                group = entry.Group;

            if (MoveFocus(group, modifierKeys) && entry == null)
                group.IsExpanded = false;
        }

        private void ExpandFocusedGroup(Keys modifierKeys)
        {
            var focus = GetEntryRelativeToFocus(0);
            var entry = focus as TestListTestEntry;
            var group = focus as TestListGroupEntry;
            if (group != null)
                entry = group.Entries.FirstOrDefault();

            if (group == null || group.IsExpanded)
                MoveFocus(entry, modifierKeys);
            else if (MoveFocus(group, modifierKeys))
                group.IsExpanded = true;
        }

        private bool MoveFocus(TestListEntry target, Keys modifierKeys)
        {
            if (modifierKeys == Keys.None)
            {
                FocusedEntry = target;
                SelectOnly(target);
                return true;
            }
            else if (modifierKeys == Keys.Control)
            {
                FocusedEntry = target;
                return true;
            }
            else if (modifierKeys == Keys.Shift)
            {
                FocusedEntry = target;
                SelectRange(target);
                return true;
            }
            else
            {
                return false;
            }
        }

        private TestListEntry GetEntryRelativeToFocus(int offset)
        {
            int index = 0;
            if (FocusedEntry != null)
                index = VisibleEntries.IndexOf(FocusedEntry);
            index = Math.Max(0, Math.Min(index + offset, VisibleEntries.Count - 1));

            if (index < VisibleEntries.Count)
                return VisibleEntries[index];
            else
                return null;

        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            var group = FindEntry(e.Location) as TestListGroupEntry;
            if (group != null && IsWithinIconBounds(e.Location))
                HighlightedIconEntry = group;
            else
                HighlightedIconEntry = null;
        }

        public void HandleMouseLeave(EventArgs e)
        {
            HighlightedIconEntry = null;
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
                    if (left && group != null && IsWithinIconBounds(e.Location))
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

        private bool IsWithinIconBounds(Point location)
        {
            int x = location.X;
            int y = (location.Y + View.VerticalScroll.Value) % View.EntryHeight;
            int width = View.ImageSize.Width + 2 * View.EntryPadding;
            int height = View.ImageSize.Height + 2 * View.EntryPadding;
            return x >= 0 && x < width && y >= 0 && y < height;
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
            if (SelectionStartEntry == null)
                SelectionStartEntry = AllEntries.FirstOrDefault();

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
