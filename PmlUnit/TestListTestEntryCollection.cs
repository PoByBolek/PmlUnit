// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;

namespace PmlUnit
{
    class TestListTestEntryCollection : ICollection<TestListTestEntry>
    {
        WritableTestListTestEntryCollection Entries;

        public TestListTestEntryCollection(WritableTestListTestEntryCollection entries)
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

    class WritableTestListTestEntryCollection : ICollection<TestListTestEntry>
    {
        private readonly SortedList<Test, TestListTestEntry> Entries;

        public WritableTestListTestEntryCollection()
        {
            Entries = new SortedList<Test, TestListTestEntry>(new TestComparer());
        }

        public int Count => Entries.Count;

        public TestListTestEntry this[Test test]
        {
            get { return Entries[test]; }
        }

        public TestListTestEntryCollection AsReadOnly()
        {
            return new TestListTestEntryCollection(this);
        }

        public TestListTestEntry Add(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            var result = new TestListTestEntry(test);
            Entries.Add(test, result);
            return result;
        }

        public void Add(TestListTestEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Entries.Add(item.Test, item);
        }

        public void Clear()
        {
            Entries.Clear();
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
            return Entries.Remove(test);
        }

        public bool Remove(TestListTestEntry item)
        {
            return Contains(item) && Entries.Remove(item.Test);
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
