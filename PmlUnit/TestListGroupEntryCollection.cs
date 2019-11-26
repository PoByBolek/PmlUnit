// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;

namespace PmlUnit
{
    class TestListGroupEntryCollection : ICollection<TestListGroupEntry>
    {
        private readonly SortedList<string, TestListGroupEntry> Entries;

        public TestListGroupEntryCollection()
        {
            Entries = new SortedList<string, TestListGroupEntry>(StringComparer.OrdinalIgnoreCase);
        }

        public int Count => Entries.Count;

        public TestListGroupEntry this[int index] => Entries.Values[index];
        public TestListGroupEntry this[string key] => Entries[key];

        public ReadOnlyTestListGroupEntryCollection AsReadOnly()
        {
            return new ReadOnlyTestListGroupEntryCollection(this);
        }

        public void Add(TestListGroupEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Entries.Add(item.Key, item);
        }

        public void Clear()
        {
            Entries.Clear();
        }

        public bool ContainsKey(string key)
        {
            return Entries.ContainsKey(key);
        }

        public bool Contains(TestListGroupEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            TestListGroupEntry entry;
            return Entries.TryGetValue(item.Key, out entry) && entry == item;
        }

        public void CopyTo(TestListGroupEntry[] array, int arrayIndex)
        {
            Entries.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return Entries.Remove(name);
        }

        public bool Remove(TestListGroupEntry item)
        {
            return Contains(item) && Entries.Remove(item.Name);
        }

        public IEnumerator<TestListGroupEntry> GetEnumerator()
        {
            return Entries.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<TestListGroupEntry>.IsReadOnly => false;
    }

    class ReadOnlyTestListGroupEntryCollection : ICollection<TestListGroupEntry>
    {
        private TestListGroupEntryCollection Entries;

        public ReadOnlyTestListGroupEntryCollection(TestListGroupEntryCollection entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));
            Entries = entries;
        }

        public int Count => Entries.Count;

        public TestListGroupEntry this[int index] => Entries[index];
        public TestListGroupEntry this[string key] => Entries[key];

        public bool Contains(TestListGroupEntry item)
        {
            return Entries.Contains(item);
        }

        public void CopyTo(TestListGroupEntry[] array, int arrayIndex)
        {
            Entries.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TestListGroupEntry> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<TestListGroupEntry>.IsReadOnly => true;

        bool ICollection<TestListGroupEntry>.Remove(TestListGroupEntry item)
        {
            throw new NotSupportedException();
        }
        void ICollection<TestListGroupEntry>.Add(TestListGroupEntry item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TestListGroupEntry>.Clear()
        {
            throw new NotSupportedException();
        }
    }
}
