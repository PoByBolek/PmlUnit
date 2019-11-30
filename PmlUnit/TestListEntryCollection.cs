// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;

namespace PmlUnit
{
    class TestListEntryCollection : ICollection<TestListEntry>
    {
        private readonly SortedList<TestListEntry, TestListEntry> Entries;

        public TestListEntryCollection(IComparer<TestListEntry> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            Entries = new SortedList<TestListEntry, TestListEntry>(comparer);
        }

        public int Count => Entries.Count;

        public TestListEntry this[int index] => Entries.Values[index];

        public ReadOnlyTestListEntryCollection AsReadOnly()
        {
            return new ReadOnlyTestListEntryCollection(this);
        }

        public void Add(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!Entries.ContainsKey(item))
                Entries.Add(item, item);
        }

        public bool Contains(TestListEntry item)
        {
            return Entries.ContainsKey(item);
        }

        public int IndexOf(TestListEntry item)
        {
            return Entries.IndexOfKey(item);
        }

        public bool Remove(TestListEntry item)
        {
            return Entries.Remove(item);
        }

        public void Clear()
        {
            Entries.Clear();
        }

        public void CopyTo(TestListEntry[] array, int arrayIndex)
        {
            Entries.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TestListEntry> GetEnumerator()
        {
            return Entries.Values.GetEnumerator();
        }

        bool ICollection<TestListEntry>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class ReadOnlyTestListEntryCollection : ICollection<TestListEntry>
    {
        private readonly TestListEntryCollection Entries;

        public ReadOnlyTestListEntryCollection(TestListEntryCollection entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));
            Entries = entries;
        }

        public int Count => Entries.Count;

        public TestListEntry this[int index] => Entries[index];

        public bool Contains(TestListEntry item)
        {
            return Entries.Contains(item);
        }

        public int IndexOf(TestListEntry item)
        {
            return Entries.IndexOf(item);
        }

        public void CopyTo(TestListEntry[] array, int arrayIndex)
        {
            Entries.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TestListEntry> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        bool ICollection<TestListEntry>.IsReadOnly => true;

        void ICollection<TestListEntry>.Add(TestListEntry item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TestListEntry>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<TestListEntry>.Remove(TestListEntry item)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
