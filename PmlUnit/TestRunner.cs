// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    interface TestRunner : IDisposable
    {
        void RefreshIndex();
        void Reload(TestCase testCase);

        /// <summary>
        /// Runs all tests in the specified test case. Assigns the outcome of
        /// each test to its <see cref="Test.Result"/> property.
        /// </summary>
        /// <param name="testCase">The test case that should be executed.</param>
        void Run(TestCase testCase);

        /// <summary>
        /// Runs the specified test. Assigns the outcome of the test to its
        /// <see cref="Test.Result"/> property and also returns it.
        /// </summary>
        /// <param name="test">The test that should be executed.</param>
        /// <returns>The outcome of the test.</returns>
        TestResult Run(Test test);
    }
}
