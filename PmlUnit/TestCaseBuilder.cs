// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PmlUnit
{
    class TestCaseBuilder
    {
        internal static readonly Regex NameRegex = new Regex("^[a-z][a-z0-9]*$", RegexOptions.IgnoreCase);

        public TestNameCollection TestNames { get; }
        public bool HasSetUp { get; set; }
        public bool HasTearDown { get; set; }

        private string NameField;

        public TestCaseBuilder(string name)
        {
            Name = name;
            TestNames = new TestNameCollection();
        }

        public string Name
        {
            get { return NameField; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(value));
                if (!NameRegex.IsMatch(value))
                    throw new ArgumentException("Test case names must start with a letter and only contain letters and digits", nameof(value));

                NameField = value;
            }
        }

        public TestCaseBuilder AddTest(string name)
        {
            TestNames.Add(name);
            return this;
        }

        public TestCase Build()
        {
            return new TestCase(this);
        }
    }

    class TestNameCollection : ICollection<string>
    {
        private readonly HashSet<string> Names;

        public TestNameCollection()
        {
            Names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public int Count => Names.Count;

        public bool IsReadOnly => false;

        public void Add(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!TestCaseBuilder.NameRegex.IsMatch(name))
                throw new ArgumentException("Test names must start with a letter and only contain letters and digits", nameof(name));

            Names.Add(name);
        }

        public void Clear() => Names.Clear();

        public bool Contains(string name) => Names.Contains(name);

        public void CopyTo(string[] array, int arrayIndex) => Names.CopyTo(array, arrayIndex);

        public bool Remove(string name) => Names.Remove(name);

        public IEnumerator<string> GetEnumerator() => Names.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
