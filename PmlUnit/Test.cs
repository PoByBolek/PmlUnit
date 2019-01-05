// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    class Test
    {
        public TestCase TestCase { get; }
        public string Name { get; }

        public Test(TestCase testCase, string name)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            TestCase = testCase;
            Name = name;
        }
    }
}
