// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    class TestListTestEntry : TestListEntry
    {
        public event EventHandler ResultChanged
        {
            add { Test.ResultChanged += value; }
            remove { Test.ResultChanged -= value; }
        }

        public Test Test { get; }

        public TestListTestEntry(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
            Test = test;
        }
    }
}
