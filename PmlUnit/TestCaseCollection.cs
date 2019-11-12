// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PmlUnit
{
    class TestCaseCollection : ICollection<TestCase>
    {
        public event EventHandler<TestCasesChangedEventArgs> Changed;

        private readonly Dictionary<string, TestCase> TestCases;

        public TestCaseCollection()
        {
            TestCases = new Dictionary<string, TestCase>(StringComparer.OrdinalIgnoreCase);
        }

        public int Count => TestCases.Count;

        public TestCase this[string name]
        {
            get { return TestCases[name]; }
            set
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException(nameof(name));
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (!TestCases.Comparer.Equals(name, value.Name))
                    throw new ArgumentException("name does not match value.Name");

                TestCase old;
                bool exists = TestCases.TryGetValue(name, out old);
                if (value != old)
                {
                    TestCases[name] = value;
                    OnChanged(value, old);
                }
            }
        }

        public void Add(TestCase item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            TestCases.Add(item.Name, item);
            OnChanged(item, null);
        }

        public void AddRange(IEnumerable<TestCase> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var copy = collection.ToList();
            if (copy.Count == 0)
                return;

            var names = new HashSet<string>(TestCases.Comparer);
            foreach (var testCase in copy)
            {
                if (testCase == null)
                    throw new ArgumentException("Collection contains null.", nameof(collection));
                if (!names.Add(testCase.Name))
                    throw new ArgumentException(string.Format("Collection contains duplicate test case name \"{0}\".", testCase.Name), nameof(collection));
                if (TestCases.ContainsKey(testCase.Name))
                    throw new ArgumentException(string.Format("There already is a test case named \"{0}\" in this collection.", testCase.Name), nameof(collection));
            }

            foreach (var testCase in copy)
                TestCases.Add(testCase.Name, testCase);

            OnChanged(copy, null);
        }

        public void Clear()
        {
            if (Count > 0)
            {
                var copy = TestCases.Values.ToList();
                TestCases.Clear();
                OnChanged(null, copy);
            }
        }

        public bool Contains(TestCase item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            TestCase result;
            return TestCases.TryGetValue(item.Name, out result) && item == result;
        }

        public bool Remove(TestCase item)
        {
            bool result = Contains(item) && TestCases.Remove(item.Name);
            if (result)
                OnChanged(null, item);
            return result;
        }

        public void CopyTo(TestCase[] array, int index)
        {
            TestCases.Values.CopyTo(array, index);
        }

        public IEnumerator<TestCase> GetEnumerator()
        {
            return TestCases.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<TestCase>.IsReadOnly => false;

        private void OnChanged(TestCase added, TestCase removed)
        {
            Changed?.Invoke(this, new TestCasesChangedEventArgs(added, removed));
        }

        private void OnChanged(IEnumerable<TestCase> added, IEnumerable<TestCase> removed)
        {
            Changed?.Invoke(this, new TestCasesChangedEventArgs(added, removed));
        }
    }

    class TestCasesChangedEventArgs : EventArgs
    {
        public IEnumerable<TestCase> AddedTestCases { get; }
        public IEnumerable<TestCase> RemovedTestCases { get; }

        public TestCasesChangedEventArgs(TestCase added, TestCase removed)
        {
            AddedTestCases = added == null ? Enumerable.Empty<TestCase>() : Enumerable.Repeat(added, 1);
            RemovedTestCases = removed == null ? Enumerable.Empty<TestCase>() : Enumerable.Repeat(removed, 1);
        }

        public TestCasesChangedEventArgs(IEnumerable<TestCase> added, IEnumerable<TestCase> removed)
        {
            AddedTestCases = added == null ? Enumerable.Empty<TestCase>() : added.ToList();
            RemovedTestCases = removed == null ? Enumerable.Empty<TestCase>() : removed.ToList();
        }
    }
}
