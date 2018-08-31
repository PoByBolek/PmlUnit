using System.Collections.Generic;

namespace PmlUnit
{
    interface TestCaseProvider
    {
        ICollection<TestCase> GetTestCases();
    }
}
