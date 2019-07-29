// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PmlUnit
{
    class TestListGroupEntry : TestListBaseEntry
    {
        public const string ExpandedImageKey = "Expanded";
        public const string ExpandedHighlightImageKey = "ExpandedHighlight";
        public const string CollapsedImageKey = "Collapsed";
        public const string CollapsedHighlightImageKey = "CollapsedHighlight";

        public event EventHandler SelectionChanged;
        public event EventHandler ExpandedChanged;

        public string Name { get; }

        private readonly List<TestListViewEntry> EntriesField;
        private bool SelectedField;
        private bool ExpandedField;

        public TestListGroupEntry(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            EntriesField = new List<TestListViewEntry>();
            SelectedField = false;
            ExpandedField = true;
        }

        public IList<TestListViewEntry> Entries
        {
            get { return EntriesField.AsReadOnly(); }
        }

        public Rectangle IconBounds => new Rectangle(0, 0, 20, TestListView.EntryHeight);

        public bool Selected
        {
            get { return SelectedField; }
            set
            {
                if (value != SelectedField)
                {
                    SelectedField = value;
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int Height
        {
            get
            {
                int result = TestListView.EntryHeight;
                if (IsExpanded)
                    result += TestListView.EntryHeight * EntriesField.Count;
                return result;
            }
        }

        public void Add(TestListViewEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            EntriesField.Add(entry);
        }

        public void Remove(TestListViewEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            EntriesField.Remove(entry);
        }

        public bool IsExpanded
        {
            get { return ExpandedField; }
            set
            {
                if (value != ExpandedField)
                {
                    ExpandedField = value;
                    ExpandedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Paint(Graphics g, Rectangle bounds, TestListPaintOptions options)
        {
            int minY = options.ClipRectangle.Top;
            int maxY = options.ClipRectangle.Bottom;
            int y = bounds.Y;

            if (y > maxY)
                return;

            if (y + TestListView.EntryHeight >= minY)
            {
                var headerBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, TestListView.EntryHeight);
                PaintHeader(g, headerBounds, options);
            }
            y += TestListView.EntryHeight;

            if (IsExpanded)
            {
                int totalHeight = EntriesField.Count * TestListView.EntryHeight;
                if (y + totalHeight >= minY)
                {
                    var entryBounds = new Rectangle(bounds.X + 20, y, bounds.Width - 20, totalHeight);
                    PaintEntries(g, entryBounds, options);
                }
            }
        }

        private void PaintHeader(Graphics g, Rectangle bounds, TestListPaintOptions options)
        {
            int padding = 2;
            int x = bounds.Left + padding;
            int y = bounds.Top + padding;

            var textBrush = options.NormalTextBrush;
            if (Selected)
            {
                textBrush = options.SelectedTextBrush;
                g.FillRectangle(options.SelectedBackBrush, 0, bounds.Top, bounds.Right, bounds.Height);
            }
            else if (options.FocusedEntry == this)
            {
                g.DrawRectangle(options.FocusRectanglePen, 0, bounds.Top, bounds.Right - 1, bounds.Height);
            }

            g.DrawImage(options.ExpanderImageList.Images[IsExpanded ? ExpandedImageKey : CollapsedImageKey], x, y);
            x += 16 + padding;

            int nameWidth = (int)Math.Ceiling(g.MeasureString(Name, options.HeaderFont).Width);
            g.DrawString(Name, options.HeaderFont, textBrush, x, y);
            x += nameWidth;

            var count = " (" + Entries.Count + ")";
            g.DrawString(count, options.EntryFont, textBrush, x, y);
        }

        private void PaintEntries(Graphics g, Rectangle bounds, TestListPaintOptions options)
        {
            int minY = options.ClipRectangle.Top;
            int maxY = options.ClipRectangle.Bottom;
            int y = bounds.Top;

            foreach (var entry in EntriesField)
            {
                if (y > maxY)
                    return;

                if (y + TestListView.EntryHeight >= minY)
                {
                    var entryBounds = new Rectangle(bounds.X, y, bounds.Width, TestListView.EntryHeight);
                    entry.Paint(g, entryBounds, options);
                }
                y += TestListView.EntryHeight;
            }
        }
    }

    class EntryClickEventArgs : EventArgs
    {
        public TestListViewEntry Entry { get; }

        public EntryClickEventArgs(TestListViewEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            Entry = entry;
        }
    }
}
