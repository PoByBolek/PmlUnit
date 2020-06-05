// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PmlUnit
{
    class TestCase
    {
        internal static readonly Regex NameRegex = new Regex("^[a-z][a-z0-9]*$", RegexOptions.IgnoreCase);

        public string FileName { get; }
        public string Name { get; }
        public TestCollection Tests { get; }

        public bool HasSetUp { get; set; }
        public bool HasTearDown { get; set; }

        public TestCase(string name, string fileName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!NameRegex.IsMatch(name))
                throw new ArgumentException("Test case names must start with a letter and only contain letters and digits.", nameof(name));
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            Name = name;
            FileName = Path.GetFullPath(fileName);
            Tests = new TestCollection(this);
            HasSetUp = false;
            HasTearDown = false;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class TestCollection : ICollection<Test>
    {
        private readonly TestCase TestCase;
        private readonly Dictionary<string, Test> Tests;

        public TestCollection(TestCase testCase)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));

            TestCase = testCase;
            Tests = new Dictionary<string, Test>(StringComparer.OrdinalIgnoreCase);
        }

        public int Count => Tests.Count;

        public Test this[string name] => Tests[name];

        public Test Add(string name)
        {
            return Add(name, 0);
        }

        public Test Add(string name, int lineNumber)
        {
            var result = new Test(TestCase, name, lineNumber);
            Add(result);
            return result;
        }

        public void Add(Test item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.TestCase != TestCase)
                throw new ArgumentException("Test does not belong to test case", nameof(item));
            Tests.Add(item.Name, item);
        }

        public bool Contains(Test item)
        {
            if (item == null || item.TestCase != TestCase)
                return false;

            Test value;
            if (Tests.TryGetValue(item.Name, out value))
                return item == value;
            else
                return false;
        }

        public bool Contains(string name)
        {
            return name != null && Tests.ContainsKey(name);
        }

        public void Clear()
        {
            Tests.Clear();
        }

        public bool Remove(Test item)
        {
            if (item == null || item.TestCase != TestCase)
                return false;
            Test value;
            if (Tests.TryGetValue(item.Name, out value) && item == value)
                return Tests.Remove(item.Name);
            else
                return false;
        }

        public void CopyTo(Test[] array, int arrayIndex) => Tests.Values.CopyTo(array, arrayIndex);

        public IEnumerator<Test> GetEnumerator() => Tests.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        bool ICollection<Test>.IsReadOnly => false;
    }
}
