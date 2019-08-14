using System;
using System.Collections;
using System.Collections.Generic;

namespace PmlUnit
{
    class TestListEntryCollection : ICollection<TestListEntry>
    {
        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly => false;

        public void Add(TestListEntry item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(TestListEntry item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TestListEntry[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TestListEntry> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(TestListEntry item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
