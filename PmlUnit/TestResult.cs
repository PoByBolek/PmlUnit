using System;

namespace PmlUnit
{
    class TestResult
    {
        public Exception Error { get; }
        public TimeSpan Duration { get; }

        public TestResult(TimeSpan duration)
            : this(duration, null)
        {
        }

        public TestResult(TimeSpan duration, Exception error)
        {
            Error = error;
            Duration = duration;
        }

        public bool Success
        {
            get { return Error == null; }
        }
    }
}
