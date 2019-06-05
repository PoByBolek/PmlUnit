// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class TestListGroupEntry : UserControl
    {
        public const string ExpandedImageKey = "Expanded";
        public const string ExpandedHighlightImageKey = "ExpandedHighlight";
        public const string CollapsedImageKey = "Collapsed";
        public const string CollapsedHighlightImageKey = "CollapsedHighlight";

        [Category("Behavior")]
        public event EventHandler<EntryClickEventArgs> EntryClick;
        [Category("Behavior")]
        public event EventHandler SelectionChanged;

        private ImageList StatusImageListField;

        public TestListGroupEntry()
        {
            InitializeComponent();

            Height = ImageLabel.Height;
            ImageLabel.ImageKey = ExpandedImageKey;
        }

        public TestListGroupEntry(string name)
            : this()
        {
            Text = name;
        }

        [Category("Appearance")]
        public ImageList ExpanderImageList
        {
            get { return ImageLabel.ImageList; }
            set { ImageLabel.ImageList = value; }
        }

        [Category("Appearance")]
        public ImageList StatusImageList
        {
            get { return StatusImageListField; }
            set
            {
                StatusImageListField = value;
                foreach (Control child in EntryPanel.Controls)
                {
                    var entry = child as TestListViewEntry;
                    if (entry != null)
                        entry.ImageList = value;
                }
            }
        }

        public override string Text
        {
            get { return NameLabel.Text; }
            set
            {
                NameLabel.Text = value;
                CountLabel.Left = NameLabel.Right;
                CountLabel.Width = Width - CountLabel.Left;
            }
        }

        public IEnumerable<TestListEntry> Entries
        {
            get { return EntryPanel.Controls.OfType<TestListEntry>(); }
        }

        public TestListViewEntry Add(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
            
            var entry = new TestListViewEntry(test);
            try
            {
                entry.ImageList = StatusImageListField;
                entry.Click += OnEntryClick;
                entry.SelectionChanged += OnSelectionChanged;
                EntryPanel.Controls.Add(entry);
                OnEntriesChanged();

                return entry;
            }
            catch
            {
                entry.Dispose();
                throw;
            }
        }

        public void Remove(TestListViewEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            EntryPanel.Controls.Remove(entry);
            OnEntriesChanged();
        }

        public void Expand()
        {
            EntryPanel.Visible = true;
            ImageLabel.ImageKey = ExpandedImageKey;
            Height = ExpandedHeight;
        }

        public void Collapse()
        {
            EntryPanel.Visible = false;
            ImageLabel.ImageKey = CollapsedImageKey;
            Height = CollapsedHeight;
        }

        public bool IsExpanded
        {
            get { return EntryPanel.Visible; }
        }

        private int ExpandedHeight
        {
            get { return ImageLabel.Height + TestListViewEntry.ItemHeight * EntryPanel.Controls.Count; }
        }

        private int CollapsedHeight
        {
            get { return ImageLabel.Height; }
        }

        private void OnToggleExpanded(object sender, EventArgs e)
        {
            if (IsExpanded)
                Collapse();
            else
                Expand();
        }

        private void OnEntryClick(object sender, EventArgs e)
        {
            var entry = sender as TestListViewEntry;
            if (entry != null)
                EntryClick?.Invoke(this, new EntryClickEventArgs(entry));
        }

        private void OnEntriesChanged()
        {
            if (IsExpanded)
                Height = ExpandedHeight;
            CountLabel.Text = string.Format(CultureInfo.CurrentCulture, "({0})", EntryPanel.Controls.Count);
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        private void OnImageLabelMouseEnter(object sender, EventArgs e)
        {
            ImageLabel.ImageKey = IsExpanded ? ExpandedHighlightImageKey : CollapsedHighlightImageKey;
        }

        private void OnImageLabelClick(object sender, EventArgs e)
        {
            OnToggleExpanded(sender, e);
            OnImageLabelMouseEnter(sender, e);
        }

        private void OnImageLabelMouseLeave(object sender, EventArgs e)
        {
            ImageLabel.ImageKey = IsExpanded ? ExpandedImageKey : CollapsedImageKey;
        }

        private void OnGroupHeaderClick(object sender, EventArgs e)
        {
            OnClick(e);
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
