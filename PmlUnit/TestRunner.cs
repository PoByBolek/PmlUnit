using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace PmlUnit
{
    class TestRunner : IDisposable
    {
        private readonly Clock Clock;
        private ObjectProxy RunnerProxy;

        public TestRunner()
            : this(new SystemClock())
        {
        }

        public TestRunner(Clock clock)
        {
            if (clock == null)
                throw new ArgumentNullException(nameof(clock));

            Clock = clock;
            RunnerProxy = new PmlObjectProxy("PmlTestRunner");
        }

        public TestRunner(ObjectProxy proxy)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            Clock = new SystemClock();
            RunnerProxy = proxy;
        }

        public TestRunner(ObjectProxy proxy, Clock clock)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));
            if (clock == null)
                throw new ArgumentNullException(nameof(clock));

            Clock = clock;
            RunnerProxy = proxy;
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
            if (RunnerProxy == null)
                return;

            if (disposing)
                RunnerProxy.Dispose();

            RunnerProxy = null;
        }

        public void Reload(TestCase testCase)
        {
            if (RunnerProxy == null)
                throw new ObjectDisposedException(nameof(TestRunner));

            var result = RunnerProxy.Invoke("reload", testCase.Name);
            var exception = UnmarshalException(result);
            if (exception != null)
                throw exception;
        }

        public void Run(TestCase testCase)
        {
            if (RunnerProxy == null)
                throw new ObjectDisposedException(nameof(TestRunner));

            foreach (var test in testCase.Tests)
                Run(test);
        }

        public TestResult Run(Test test)
        {
            if (RunnerProxy == null)
                throw new ObjectDisposedException(nameof(TestRunner));

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
            catch (Exception error)
            {
                var elapsed = Clock.CurrentInstant - start;
                return new TestResult(elapsed, error);
            }
        }

        private static Exception UnmarshalException(object result)
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
                disposable.Dispose();

            return null;
        }
    }
}
