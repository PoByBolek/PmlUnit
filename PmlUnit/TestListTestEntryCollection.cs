// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PmlUnit
{
    class ReadOnlyTestListTestEntryCollection : ICollection<TestListTestEntry>
    {
        TestListTestEntryCollection Entries;

        public ReadOnlyTestListTestEntryCollection(TestListTestEntryCollection entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));
            Entries = entries;
        }

        public int Count => Entries.Count;

        public TestListTestEntry this[Test test] => Entries[test];

        public bool Contains(TestListTestEntry item)
        {
            return Entries.Contains(item);
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

        bool ICollection<TestListTestEntry>.IsReadOnly => true;

        void ICollection<TestListTestEntry>.Add(TestListTestEntry item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TestListTestEntry>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<TestListTestEntry>.Remove(TestListTestEntry item)
        {
            throw new NotSupportedException();
        }
    }

    class TestListTestEntryCollection : ICollection<TestListTestEntry>
    {
        public event EventHandler<TestListEntriesChangedEventArgs> Changed;

        private readonly SortedList<Test, TestListTestEntry> Entries;

        public TestListTestEntryCollection()
        {
            Entries = new SortedList<Test, TestListTestEntry>(new TestComparer());
        }

        public int Count => Entries.Count;

        public TestListTestEntry this[Test test]
        {
            get { return Entries[test]; }
        }

        public ReadOnlyTestListTestEntryCollection AsReadOnly()
        {
            return new ReadOnlyTestListTestEntryCollection(this);
        }

        public TestListTestEntry Add(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            var result = new TestListTestEntry(test);
            Entries.Add(test, result);
            OnChanged(result, null);

            return result;
        }

        public void Add(TestListTestEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Entries.Add(item.Test, item);
            OnChanged(item, null);
        }

        public void Clear()
        {
            if (Entries.Count > 0)
            {
                var copy = Entries.Values.ToList();
                Entries.Clear();
                OnChanged(null, copy);
            }
        }

        public bool Contains(TestListTestEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            TestListTestEntry entry;
            return Entries.TryGetValue(item.Test, out entry) && entry == item;
        }

        public bool Remove(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            TestListTestEntry entry;
            bool result = Entries.TryGetValue(test, out entry);
            if (result)
            {
                Entries.Remove(test);
                OnChanged(null, entry);
            }
            return result;
        }

        public bool Remove(TestListTestEntry item)
        {
            bool result = Contains(item) && Entries.Remove(item.Test);
            if (result)
                OnChanged(null, item);
            return result;
        }

        public void CopyTo(TestListTestEntry[] array, int arrayIndex)
        {
            Entries.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TestListTestEntry> GetEnumerator()
        {
            return Entries.Values.GetEnumerator();
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

        private class TestComparer : IComparer<Test>
        {
            public int Compare(Test left, Test right)
            {
                if (left == null)
                    throw new ArgumentNullException(nameof(left));
                if (right == null)
                    throw new ArgumentNullException(nameof(right));

                int result = string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);
                if (result == 0)
                    result = string.Compare(left.TestCase.Name, right.TestCase.Name, StringComparison.OrdinalIgnoreCase);
                return result;
            }
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
