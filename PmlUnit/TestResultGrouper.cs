// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    class TestResultGrouper : TestGrouper
    {
        private readonly TestListGroupEntry FailedGroup;
        private readonly TestListGroupEntry PassedGroup;
        private readonly TestListGroupEntry NotExecutedGroup;

        public TestResultGrouper()
        {
            FailedGroup = new TestListGroupEntry(1, "Failed Tests");
            PassedGroup = new TestListGroupEntry(2, "Passed Tests");
            NotExecutedGroup = new TestListGroupEntry(3, "Not executed Tests");
        }

        public TestListGroupEntry GetGroupFor(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            switch (test.Status)
            {
                case TestStatus.Failed:
                    return FailedGroup;
                case TestStatus.NotExecuted:
                    return NotExecutedGroup;
                case TestStatus.Passed:
                    return PassedGroup;
                default:
                    return null;
            }
        }
    }
}
