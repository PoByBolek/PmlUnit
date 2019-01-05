// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PmlUnit
{
    class TestCase
    {
        public string Name { get; }

        public bool HasSetUp { get; }
        public bool HasTearDown { get; }

        public TestCollection Tests { get; }

        public TestCase(TestCaseBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Name = builder.Name;
            HasSetUp = builder.HasSetUp;
            HasTearDown = builder.HasTearDown;
            Tests = new TestCollection(this, builder.TestNames);
        }
    }

    class TestCollection : ICollection<Test>
    {
        private readonly List<Test> Tests;

        public TestCollection(TestCase testCase, IEnumerable<string> testNames)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));
            if (testNames == null)
                throw new ArgumentNullException(nameof(testNames));

            Tests = testNames.Select(name => new Test(testCase, name)).ToList();
        }

        public int Count => Tests.Count;

        public Test this[int index] => Tests[index];

        public bool Contains(Test item) => Tests.Contains(item);

        public void CopyTo(Test[] array, int arrayIndex) => Tests.CopyTo(array, arrayIndex);

        public IEnumerator<Test> GetEnumerator() => Tests.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        bool ICollection<Test>.IsReadOnly => true;

        void ICollection<Test>.Add(Test item)
        {
            throw new NotSupportedException();
        }

        void ICollection<Test>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<Test>.Remove(Test item)
        {
            throw new NotSupportedException();
        }
    }
}
