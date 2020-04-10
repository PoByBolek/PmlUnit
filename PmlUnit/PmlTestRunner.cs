// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PmlUnit
{
    class PmlTestRunner : AsyncTestRunner
    {
        public event EventHandler<TestCompletedEventArgs> TestCompleted;
        public event EventHandler<TestRunCompletedEventArgs> RunCompleted;

        public bool IsBusy { get; private set; }
        public bool CancellationPending { get; private set; }

        private delegate void RunDelegate(IList<Test> tests, int index);

        private readonly MethodInvoker Invoker;
        private readonly Clock Clock;
        private ObjectProxy RunnerProxy;

        public PmlTestRunner(MethodInvoker invoker)
            : this(new SystemClock(), invoker)
        {
        }

        public PmlTestRunner(Clock clock, MethodInvoker invoker)
        {
            if (clock == null)
                throw new ArgumentNullException(nameof(clock));
            if (invoker == null)
                throw new ArgumentNullException(nameof(invoker));

            Invoker = invoker;
            Clock = clock;
            RunnerProxy = new PmlObjectProxy("PmlTestRunner");
        }

        public PmlTestRunner(ObjectProxy proxy, MethodInvoker invoker)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));
            if (invoker == null)
                throw new ArgumentNullException(nameof(invoker));

            Invoker = invoker;
            Clock = new SystemClock();
            RunnerProxy = proxy;
        }

        public PmlTestRunner(ObjectProxy proxy, Clock clock, MethodInvoker invoker)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));
            if (clock == null)
                throw new ArgumentNullException(nameof(clock));
            if (invoker == null)
                throw new ArgumentNullException(nameof(invoker));

            Invoker = invoker;
            Clock = clock;
            RunnerProxy = proxy;
        }

        ~PmlTestRunner()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (RunnerProxy == null)
                return;

            if (disposing)
                RunnerProxy.Dispose();

            RunnerProxy = null;
        }

        public void RefreshIndex()
        {
            var error = InvokePmlMethod("refreshIndex");
            if (error != null)
                throw new PmlException("Failed to refresh index", error);
        }

        public void Reload(TestCase testCase)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));

            var error = InvokePmlMethod("reload", testCase.Name);
            if (error != null)
                throw new PmlException("Failed to reload test case " + testCase.Name, error);
        }

        public void Run(TestCase testCase)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));

            Run(testCase.Tests);
        }

        public TestResult Run(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            Run(Enumerable.Repeat(test, 1));
            return test.Result;
        }

        public void Run(IEnumerable<Test> tests)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests));
            ValidateState();

            IsBusy = true;
            try
            {
                foreach (var test in tests)
                {
                    if (CancellationPending)
                        break;
                    if (test == null)
                        throw new ArgumentException("tests must not be null", nameof(tests));

                    RunInternal(test);
                }
            }
            finally
            {
                IsBusy = false;
            }

            OnTestRunCompleted(tests, null);
        }

        public void RunAsync(TestCase testCase)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));

            RunAsync(testCase.Tests);
        }

        public void RunAsync(IEnumerable<Test> tests)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests));
            ValidateState();

            IsBusy = true;
            try
            {
                Invoker.BeginInvoke(new RunDelegate(RunAsync), tests.ToList(), 0);
            }
            catch
            {
                IsBusy = false;
                throw;
            }
        }

        public void CancelAsync()
        {
            if (IsBusy)
                CancellationPending = true;
        }

        private void RunAsync(IList<Test> tests, int index)
        {
            if (CancellationPending)
            {
                OnTestRunCompleted(tests, null);
                return;
            }
            else if (RunnerProxy == null)
            {
                OnTestRunCompleted(tests, new ObjectDisposedException(nameof(PmlTestRunner)));
                return;
            }
            else if (index < 0 || index >= tests.Count)
            {
                OnTestRunCompleted(tests, null);
                return;
            }

            var test = tests[index];
            if (test == null)
            {
                OnTestRunCompleted(tests, new ArgumentException("tests must not be null", nameof(tests)));
                return;
            }

            try
            {
                RunInternal(test);
                Invoker.BeginInvoke(new RunDelegate(RunAsync), tests, index + 1);
            }
            catch (Exception error)
            {
                OnTestRunCompleted(tests, error);
            }
        }

        private void OnTestRunCompleted(IEnumerable<Test> tests, Exception error)
        {
            IsBusy = false;
            RunCompleted?.Invoke(this, new TestRunCompletedEventArgs(tests, error));
        }

        private void ValidateState()
        {
            if (RunnerProxy == null)
                throw new ObjectDisposedException(nameof(PmlTestRunner));
            if (IsBusy)
                throw new InvalidOperationException("The test runner is currently busy");
        }

        private void RunInternal(Test test)
        {
            var start = Clock.CurrentInstant;
            var testCase = test.TestCase;
            var error = InvokePmlMethod(
                "run", testCase.Name, test.Name, testCase.HasSetUp, testCase.HasTearDown
            );
            var elapsed = Clock.CurrentInstant - start;
            test.Result = new TestResult(elapsed, error);

            TestCompleted?.Invoke(this, new TestCompletedEventArgs(test));
        }

        private PmlError InvokePmlMethod(string method, params object[] arguments)
        {
            if (RunnerProxy == null)
                throw new ObjectDisposedException(nameof(PmlTestRunner));

            var result = RunnerProxy.Invoke(method, arguments);
            var stackTrace = result as Hashtable;
            if (stackTrace != null)
                return PmlError.FromHashTable(stackTrace);

            var disposable = result as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            return null;
        }
    }
}
