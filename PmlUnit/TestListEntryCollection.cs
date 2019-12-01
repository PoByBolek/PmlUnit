// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;

namespace PmlUnit
{
    class TestListEntryCollection : ICollection<TestListEntry>
    {
        private readonly List<TestListEntry> Entries;
        private readonly IComparer<TestListEntry> Comparer;

        public TestListEntryCollection(IComparer<TestListEntry> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            
            Entries = new List<TestListEntry>();
            Comparer = comparer;
        }

        public int Count => Entries.Count;

        public TestListEntry this[int index] => Entries[index];

        public ReadOnlyTestListEntryCollection AsReadOnly()
        {
            return new ReadOnlyTestListEntryCollection(this);
        }

        public bool Add(TestListEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (Entries.Contains(item))
            {
                return false;
            }
            else
            {
                Register(item);
                Entries.Add(item);
                Entries.Sort(Comparer);
                return true;
            }
        }

        public bool TryGetTestEntry(Test test, out TestListTestEntry entry)
        {
            int index = IndexOf(test);
            if (index >= 0)
            {
                entry = Entries[index] as TestListTestEntry;
                return entry != null;
            }
            else
            {
                entry = null;
                return false;
            }
        }

        public bool Contains(Test test)
        {
            return IndexOf(test) >= 0;
        }

        public bool Contains(TestListEntry item)
        {
            return Entries.Contains(item);
        }

        public int IndexOf(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            for (int i = 0; i < Entries.Count; i++)
            {
                var entry = Entries[i] as TestListTestEntry;
                if (entry != null && entry.Test == test)
                    return i;
            }

            return -1;
        }

        public int IndexOf(TestListEntry item)
        {
            return Entries.IndexOf(item);
        }

        public bool Remove(Test test)
        {
            int index = IndexOf(test);
            if (index >= 0)
            {
                Deregister(Entries[index]);
                Entries.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(TestListEntry item)
        {
            bool result = Entries.Remove(item);
            if (result)
                Deregister(item);
            return result;
        }

        public void Clear()
        {
            foreach (var entry in Entries)
                Deregister(entry);
            Entries.Clear();
        }

        public void CopyTo(TestListEntry[] array, int arrayIndex)
        {
            Entries.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TestListEntry> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        bool ICollection<TestListEntry>.IsReadOnly => false;

        void ICollection<TestListEntry>.Add(TestListEntry item)
        {
            Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Register(TestListEntry item)
        {
            var entry = item as TestListTestEntry;
            if (entry != null)
            {
                entry.ResultChanged += OnTestEntryChanged;
                entry.GroupChanged += OnTestEntryChanged;
            }
        }

        private void Deregister(TestListEntry item)
        {
            var entry = item as TestListTestEntry;
            if (entry != null)
            {
                entry.GroupChanged -= OnTestEntryChanged;
                entry.ResultChanged -= OnTestEntryChanged;
            }
        }

        private void OnTestEntryChanged(object sender, EventArgs e)
        {
            Entries.Sort(Comparer);
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
