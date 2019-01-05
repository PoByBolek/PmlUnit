// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
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
