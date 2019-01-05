// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;

namespace PmlUnit
{
    class PmlTestRunner : TestRunner
    {
        private readonly Clock Clock;
        private ObjectProxy RunnerProxy;

        public PmlTestRunner()
            : this(new SystemClock())
        {
        }

        public PmlTestRunner(Clock clock)
        {
            if (clock == null)
                throw new ArgumentNullException(nameof(clock));

            Clock = clock;
            RunnerProxy = new PmlObjectProxy("PmlTestRunner");
        }

        public PmlTestRunner(ObjectProxy proxy)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            Clock = new SystemClock();
            RunnerProxy = proxy;
        }

        public PmlTestRunner(ObjectProxy proxy, Clock clock)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));
            if (clock == null)
                throw new ArgumentNullException(nameof(clock));

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
            InvokePmlMethod("refreshIndex");
        }

        public void Reload(TestCase testCase)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));

            InvokePmlMethod("reload", testCase.Name);
        }

        public void Run(TestCase testCase)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));
            if (RunnerProxy == null)
                throw new ObjectDisposedException(nameof(PmlTestRunner));

            foreach (var test in testCase.Tests)
                Run(test);
        }

        public TestResult Run(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
            if (RunnerProxy == null)
                throw new ObjectDisposedException(nameof(PmlTestRunner));

            var start = Clock.CurrentInstant;
            try
            {
                var testCase = test.TestCase;
                var result = RunnerProxy.Invoke(
                    "run", testCase.Name, test.Name, testCase.HasSetUp, testCase.HasTearDown
                );
                var elapsed = Clock.CurrentInstant - start;
                return new TestResult(elapsed, UnmarshalException(result));
            }
            catch (PmlException error)
            {
                var elapsed = Clock.CurrentInstant - start;
                return new TestResult(elapsed, error);
            }
        }

        private void InvokePmlMethod(string method, params object[] arguments)
        {
            if (RunnerProxy == null)
                throw new ObjectDisposedException(nameof(PmlTestRunner));

            var result = RunnerProxy.Invoke(method, arguments);
            var exception = UnmarshalException(result);
            if (exception != null)
                throw exception;
        }

        private static PmlException UnmarshalException(object result)
        {
            var stackTrace = result as Hashtable;
            if (stackTrace != null && stackTrace.Count > 0)
                return new PmlException(stackTrace);

            var disposable = result as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            return null;
        }
    }
}
