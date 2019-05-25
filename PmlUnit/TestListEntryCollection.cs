// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PmlUnit
{

    class TestListEntryCollection : ICollection<TestListEntry>
    {
        public event EventHandler Changed;

        private readonly List<TestListEntry> Entries;

        public TestListEntryCollection()
        {
            Entries = new List<TestListEntry>();
        }

        public int Count => Entries.Count;

        public bool IsReadOnly => false;

        public TestListEntry this[int index]
        {
            get { return Entries[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                Entries[index] = value;
                RaiseChanged();
            }
        }

        public void Add(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Entries.Add(item);
            RaiseChanged();
        }

        public void AddRange(IEnumerable<TestListEntry> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var list = items.ToList(); // only enumerate items once
            if (list.Contains(null))
                throw new ArgumentException("Cannot add null entries", nameof(items));

            Entries.AddRange(list);
            if (list.Count > 0)
                RaiseChanged();
        }

        public bool Remove(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            bool result = Entries.Remove(item);
            if (result)
                RaiseChanged();
            return result;
        }

        public void Clear()
        {
            if (Entries.Count > 0)
            {
                Entries.Clear();
                RaiseChanged();
            }
        }

        public bool Contains(TestListEntry item) => Entries.Contains(item);

        public void CopyTo(TestListEntry[] array, int arrayIndex) => Entries.CopyTo(array, arrayIndex);

        public IEnumerator<TestListEntry> GetEnumerator() => Entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
