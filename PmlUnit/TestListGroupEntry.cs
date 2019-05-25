// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class TestListGroupEntry : UserControl
    {
        private readonly Size MinimumCollapsedSize;
        private Size MinimumExpandedSize;
        private ImageList ImageListField;

        public TestListGroupEntry()
        {
            InitializeComponent();

            MinimumCollapsedSize = new Size(ImageLabel.Width + NameLabel.Margin.Horizontal, ImageLabel.Height);
            MinimumExpandedSize = MinimumCollapsedSize;
        }

        public TestListGroupEntry(string name)
            : this()
        {
            Text = name;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                DisposeChildControls();
            }
            base.Dispose(disposing);
        }

        private void DisposeChildControls()
        {
            var children = Controls.OfType<Control>().ToList();
            Controls.Clear();
            foreach (var child in children)
                child.Dispose();
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
                entry.Top = EntryPanel.Controls.Count * entry.Height;
                entry.Width = EntryPanel.Width;
                entry.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                entry.ImageList = ImageListField;

                EntryPanel.Controls.Add(entry);
                
                MinimumExpandedSize.Width = Math.Max(MinimumExpandedSize.Width, EntryPanel.Margin.Left + entry.MinimumSize.Width);
                MinimumExpandedSize.Height += entry.Height;
                if (IsExpanded)
                {
                    MinimumSize = MinimumExpandedSize;
                    Height = MinimumSize.Height;
                }

                CountLabel.Text = string.Format(CultureInfo.CurrentCulture, "({0})", EntryPanel.Controls.Count);

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

            int index = EntryPanel.Controls.Count;
            for (int i = 0; i < EntryPanel.Controls.Count; i++)
            {
                if (EntryPanel.Controls[i] == entry)
                {
                    MinimumExpandedSize.Height -= entry.Height;
                    if (IsExpanded)
                    {
                        MinimumSize = MinimumExpandedSize;
                        Height = MinimumSize.Height;
                    }

                    EntryPanel.Controls.RemoveAt(i);
                    index = i;
                    break;
                }
            }

            for (int i = index; i < EntryPanel.Controls.Count; i++)
            {
                var other = EntryPanel.Controls[i];
                other.Location = new Point(0, i * entry.Height);
            }

            CountLabel.Text = string.Format(CultureInfo.CurrentCulture, "({0})", EntryPanel.Controls.Count);
        }

        public void Expand()
        {
            EntryPanel.Visible = true;
            MinimumSize = MinimumExpandedSize;
            Height = MinimumExpandedSize.Height;
        }

        public void Collapse()
        {
            EntryPanel.Visible = false;
            MinimumSize = MinimumCollapsedSize;
            Height = MinimumCollapsedSize.Height;
        }

        public bool IsExpanded
        {
            get { return EntryPanel.Visible; }
        }

        private void OnToggleExpanded(object sender, EventArgs e)
        {
            if (IsExpanded)
                Collapse();
            else
                Expand();
        }
    }
}
