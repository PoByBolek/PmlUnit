// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;

namespace PmlUnit
{
    class TestListGroupEntry : TestListEntry
    {
        public event EventHandler ExpandedChanged;

        public string Name { get; }

        private readonly List<TestListTestEntry> EntriesField;
        private bool ExpandedField;

        public TestListGroupEntry(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            EntriesField = new List<TestListTestEntry>();
            ExpandedField = true;
        }

        public IList<TestListTestEntry> Entries
        {
            get { return EntriesField.AsReadOnly(); }
        }

        public void Add(TestListTestEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            EntriesField.Add(entry);
        }

        public void Remove(TestListTestEntry entry)
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
    }
}
