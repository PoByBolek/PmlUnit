// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PmlUnit
{
    class TestListSelectedEntryCollection : ICollection<TestListEntry>
    {
        private readonly ICollection<TestListEntry> Entries;
        private readonly Func<TestListEntry, bool> Predicate;

        public TestListSelectedEntryCollection(ICollection<TestListEntry> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            Entries = entries;
            Predicate = entry => entry.IsSelected;
        }

        public int Count
        {
            get { return Entries.Where(Predicate).Count(); }
        }

        public void Add(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!Entries.Contains(item))
                throw new ArgumentException("Entry does not belong to underlying collection.", nameof(item));
            item.IsSelected = true;
        }

        public void Clear()
        {
            foreach (var entry in Entries)
                entry.IsSelected = false;
        }

        public bool Contains(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            return item.IsSelected && Entries.Contains(item);
        }

        public bool Remove(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!Entries.Contains(item))
                throw new ArgumentException("Entry does not belong to underlying collection.", nameof(item));

            bool result = item.IsSelected;
            item.IsSelected = false;
            return result;
        }

        public void CopyTo(TestListEntry[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "index must be greater than or equal to zero");

            foreach (var entry in this)
            {
                if (index >= array.Length)
                    throw new ArgumentException("array is too small to hold all entries", nameof(array));
                array[index++] = entry;
            }
        }

        bool ICollection<TestListEntry>.IsReadOnly => false;

        public IEnumerator<TestListEntry> GetEnumerator()
        {
            return Entries.Where(Predicate).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
