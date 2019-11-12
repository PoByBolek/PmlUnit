// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    class TestListGroupEntry : TestListEntry
    {
        public event EventHandler ExpandedChanged;

        public string Name { get; }
        public TestListTestEntryCollection Entries { get; }

        private bool ExpandedField;

        public TestListGroupEntry(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Entries = new TestListTestEntryCollection();
            Entries.Changed += OnEntriesChanged;
            ExpandedField = true;
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

        private void OnEntriesChanged(object sender, TestListEntriesChangedEventArgs e)
        {
            foreach (var entry in e.RemovedEntries)
                entry.Group = null;
            foreach (var entry in e.AddedEntries)
                entry.Group = this;
        }
    }
}
