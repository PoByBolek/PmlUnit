using System;

namespace PmlUnit
{
    interface TestRunner : IDisposable
    {
        void RefreshIndex();
        void Reload(TestCase testCase);
        void Run(TestCase testCase);
        TestResult Run(Test test);
    }
}
