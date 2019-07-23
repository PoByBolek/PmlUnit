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

        public string Name { get; }

        private readonly List<TestListViewEntry> EntriesField;
        private bool SelectedField;

        public TestListGroupEntry(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            EntriesField = new List<TestListViewEntry>();
        }

        public IList<TestListViewEntry> Entries
        {
            get { return EntriesField.AsReadOnly(); }
        }

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

        public void Expand()
        {
        }

        public void Collapse()
        {
        }

        public bool IsExpanded
        {
            get { return true; }
        }

        public void Paint(Graphics g, Rectangle bounds, TestListPaintOptions options)
        {
            int padding = 2;
            int x = bounds.Left + padding;
            int y = bounds.Top + padding;

            var textBrush = options.NormalTextBrush;
            if (Selected)
            {
                textBrush = options.SelectedTextBrush;
                g.FillRectangle(options.SelectedBackBrush, bounds);
            }

            g.DrawImage(options.ExpanderImageList.Images[IsExpanded ? ExpandedImageKey : CollapsedImageKey], x, y);
            x += 16 + padding;

            int nameWidth = (int)Math.Ceiling(g.MeasureString(Name, options.HeaderFont).Width);
            g.DrawString(Name, options.HeaderFont, textBrush, x, y);
            x += nameWidth;

            var count = " (" + Entries.Count + ")";
            g.DrawString(count, options.EntryFont, textBrush, x, y);
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
