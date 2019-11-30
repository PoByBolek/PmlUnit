// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;

namespace PmlUnit
{

    class TestListEntryCollection<T> : ICollection<T> where T : TestListEntry
    {
        private readonly SortedList<T, T> Entries;

        public TestListEntryCollection(IComparer<T> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            Entries = new SortedList<T, T>(comparer);
        }

        public int Count => Entries.Count;

        public T this[int index] => Entries.Values[index];

        public ReadOnlyTestListEntryCollection<T> AsReadOnly()
        {
            return new ReadOnlyTestListEntryCollection<T>(this);
        }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!Entries.ContainsKey(item))
                Entries.Add(item, item);
        }

        public bool Contains(T item)
        {
            return Entries.ContainsKey(item);
        }

        public int IndexOf(T item)
        {
            return Entries.IndexOfKey(item);
        }

        public bool Remove(T item)
        {
            return Entries.Remove(item);
        }

        public void Clear()
        {
            Entries.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Entries.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Entries.Values.GetEnumerator();
        }

        bool ICollection<T>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    class ReadOnlyTestListEntryCollection<T> : ICollection<T> where T : TestListEntry
    {
        private readonly TestListEntryCollection<T> Entries;

        public ReadOnlyTestListEntryCollection(TestListEntryCollection<T> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));
            Entries = entries;
        }

        public int Count => Entries.Count;

        public T this[int index] => Entries[index];

        public bool Contains(T item)
        {
            return Entries.Contains(item);
        }

        public int IndexOf(T item)
        {
            return Entries.IndexOf(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Entries.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        bool ICollection<T>.IsReadOnly => true;

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
