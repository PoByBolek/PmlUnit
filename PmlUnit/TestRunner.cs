using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PmlUnit
{
    class TestRunner : IDisposable
    {
        private PmlProxy PmlTestRunner;

        public TestRunner()
        {
            PmlTestRunner = new PmlProxy("PmlTestRunner");
        }

        ~TestRunner()
        {
            GC.SuppressFinalize(this);
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (PmlTestRunner == null)
                return;

            if (disposing)
                PmlTestRunner.Dispose();

            PmlTestRunner = null;
        }

        public void Run(TestCase testCase)
        {
            if (PmlTestRunner == null)
                throw new ObjectDisposedException(nameof(TestRunner));

            foreach (var test in testCase.Tests)
                Run(test);
        }

        public TestResult Run(Test test)
        {
            if (PmlTestRunner == null)
                throw new ObjectDisposedException(nameof(TestRunner));

            var watch = Stopwatch.StartNew();
            try
            {
                var testCase = test.TestCase;
                var result = PmlTestRunner.Invoke(
                    "run", testCase.Name, test.Name, testCase.HasSetUp, testCase.HasTearDown
                );
                watch.Stop();
                return new TestResult(watch.Elapsed, UnmarshalResult(result));
            }
            catch (Exception error)
            {
                watch.Stop();
                return new TestResult(watch.Elapsed, error);
            }
        }

        private static Exception UnmarshalResult(object result)
        {
            var stackTrace = result as Hashtable;
            if (stackTrace != null && stackTrace.Count > 0)
            {
                var message = new StringBuilder();
                foreach (var key in stackTrace.Keys.OfType<double>().OrderBy(x => x))
                {
                    message.Append(stackTrace[key]);
                    message.Append('\n');
                }
                return new Exception(message.ToString());
            }

            var disposable = result as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

            return null;
        }
    }
}
