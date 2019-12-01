// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PmlUnit
{
    class TestListTestEntryCollection : ICollection<TestListTestEntry>
    {
        public event EventHandler<TestListEntriesChangedEventArgs> Changed;

        private readonly HashSet<TestListTestEntry> Entries;

        public TestListTestEntryCollection()
        {
            Entries = new HashSet<TestListTestEntry>();
        }

        public int Count => Entries.Count;

        public void Add(TestListTestEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (Entries.Add(item))
                OnChanged(item, null);
        }

        public void Clear()
        {
            if (Entries.Count > 0)
            {
                var copy = new HashSet<TestListTestEntry>(Entries);
                Entries.Clear();
                OnChanged(null, copy);
            }
        }

        public bool Contains(TestListTestEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return Entries.Contains(item);
        }

        public bool Remove(TestListTestEntry item)
        {
            bool result = Entries.Remove(item);
            if (result)
                OnChanged(null, item);
            return result;
        }

        public void CopyTo(TestListTestEntry[] array, int arrayIndex)
        {
            Entries.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TestListTestEntry> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<TestListTestEntry>.IsReadOnly => false;

        private void OnChanged(TestListTestEntry added, TestListTestEntry removed)
        {
            Changed?.Invoke(this, new TestListEntriesChangedEventArgs(added, removed));
        }

        private void OnChanged(IEnumerable<TestListTestEntry> added, IEnumerable<TestListTestEntry> removed)
        {
            Changed?.Invoke(this, new TestListEntriesChangedEventArgs(added, removed));
        }
    }

    class TestListEntriesChangedEventArgs : EventArgs
    {
        public IEnumerable<TestListTestEntry> AddedEntries { get; }
        public IEnumerable<TestListTestEntry> RemovedEntries { get; }

        public TestListEntriesChangedEventArgs(TestListTestEntry added, TestListTestEntry removed)
        {
            AddedEntries = added == null ? Enumerable.Empty<TestListTestEntry>() : Enumerable.Repeat(added, 1);
            RemovedEntries = removed == null ? Enumerable.Empty<TestListTestEntry>() : Enumerable.Repeat(removed, 1);
        }

        public TestListEntriesChangedEventArgs(IEnumerable<TestListTestEntry> added, IEnumerable<TestListTestEntry> removed)
        {
            AddedEntries = added == null ? Enumerable.Empty<TestListTestEntry>() : added.ToList();
            RemovedEntries = removed == null ? Enumerable.Empty<TestListTestEntry>() : removed.ToList();
        }
    }
}
