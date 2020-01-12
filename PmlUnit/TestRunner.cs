// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;

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
        /// Runs the specified collection of tests. Assigns the outcome of each
        /// test to its <see cref="Test.Result"/>  property.
        /// </summary>
        /// <param name="tests">The tests that should be executed</param>
        void Run(IEnumerable<Test> tests);

        /// <summary>
        /// Runs the specified test. Assigns the outcome of the test to its
        /// <see cref="Test.Result"/> property and also returns it.
        /// </summary>
        /// <param name="test">The test that should be executed.</param>
        /// <returns>The outcome of the test.</returns>
        TestResult Run(Test test);
    }

    interface AsyncTestRunner : TestRunner
    {
        event EventHandler<TestCompletedEventArgs> TestCompleted;
        event EventHandler<TestRunCompletedEventArgs> RunCompleted;

        bool IsBusy { get; }
        bool CancellationPending { get; }

        void RunAsync(TestCase testCase);

        void RunAsync(IEnumerable<Test> tests);

        void CancelAsync();
    }

    class TestCompletedEventArgs : EventArgs
    {
        public Test Test { get; }

        public TestCompletedEventArgs(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
            Test = test;
        }
    }

    class TestRunCompletedEventArgs : EventArgs
    {
        public IEnumerable<Test> Tests { get; }
        public Exception Error { get; }

        public TestRunCompletedEventArgs(IEnumerable<Test> tests, Exception error)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests));

            Tests = tests;
            Error = error;
        }
    }
}
