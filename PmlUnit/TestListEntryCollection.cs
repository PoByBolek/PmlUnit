// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;

namespace PmlUnit
{
    class TestListEntryCollection : ICollection<TestListEntry>
    {
        WritableTestListEntryCollection Entries;

        public TestListEntryCollection(WritableTestListEntryCollection entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));
            Entries = entries;
        }

        public int Count => Entries.Count;

        public TestListEntry this[Test test] => Entries[test];

        public bool Contains(TestListEntry item)
        {
            return Entries.Contains(item);
        }

        public void CopyTo(TestListEntry[] array, int arrayIndex)
        {
            Entries.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TestListEntry> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
    }

    class WritableTestListEntryCollection : ICollection<TestListEntry>
    {
        private readonly SortedList<Test, TestListEntry> Entries;

        public WritableTestListEntryCollection()
        {
            Entries = new SortedList<Test, TestListEntry>(new TestComparer());
        }

        public int Count => Entries.Count;

        public TestListEntry this[Test test]
        {
            get { return Entries[test]; }
        }

        public TestListEntryCollection AsReadOnly()
        {
            return new TestListEntryCollection(this);
        }

        public TestListEntry Add(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            var result = new TestListViewEntry(test);
            Entries.Add(test, result);
            return result;
        }

        public void Add(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Entries.Add(item.Test, item);
        }

        public void Clear()
        {
            Entries.Clear();
        }

        public bool Contains(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            TestListEntry entry;
            return Entries.TryGetValue(item.Test, out entry) && entry == item;
        }

        public bool Remove(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
            return Entries.Remove(test);
        }

        public bool Remove(TestListEntry item)
        {
            return Contains(item) && Entries.Remove(item.Test);
        }

        public void CopyTo(TestListEntry[] array, int arrayIndex)
        {
            Entries.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TestListEntry> GetEnumerator()
        {
            return Entries.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<TestListEntry>.IsReadOnly => false;

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
}
