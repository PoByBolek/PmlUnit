// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class TestListGroupEntry : UserControl
    {
        private ImageList ImageListField;

        public TestListGroupEntry()
        {
            InitializeComponent();

            Height = ImageLabel.Height;
        }

        public TestListGroupEntry(string name)
            : this()
        {
            Text = name;
        }

        public ImageList ImageList
        {
            get { return ImageListField; }
            set
            {
                ImageListField = value;
                foreach (Control child in Controls)
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
                entry.ImageList = ImageListField;
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
            Height = ExpandedHeight;
        }

        public void Collapse()
        {
            EntryPanel.Visible = false;
            Height = CollapsedHeight;
        }

        public bool IsExpanded
        {
            get { return EntryPanel.Visible; }
        }

        private void OnEntriesChanged()
        {
            if (IsExpanded)
                Height = ExpandedHeight;
            CountLabel.Text = string.Format(CultureInfo.CurrentCulture, "({0})", EntryPanel.Controls.Count);
        }

        private void OnToggleExpanded(object sender, EventArgs e)
        {
            if (IsExpanded)
                Collapse();
            else
                Expand();
        }

        private int ExpandedHeight
        {
            get { return ImageLabel.Height + TestListViewEntry.ItemHeight * EntryPanel.Controls.Count; }
        }

        private int CollapsedHeight
        {
            get { return ImageLabel.Height; }
        }
    }
}
