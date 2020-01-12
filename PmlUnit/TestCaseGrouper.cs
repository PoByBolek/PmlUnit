// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;

namespace PmlUnit
{
    class TestCaseGrouper : TestGrouper
    {
        private readonly Dictionary<TestCase, TestListGroupEntry> Entries;

        public TestCaseGrouper()
        {
            Entries = new Dictionary<TestCase, TestListGroupEntry>();
        }

        public TestListGroupEntry GetGroupFor(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            var testCase = test.TestCase;
            TestListGroupEntry result;
            if (Entries.TryGetValue(testCase, out result))
                return result;

            result = new TestListGroupEntry(testCase.Name, testCase.Name);
            Entries[testCase] = result;
            return result;
        }
    }
}
