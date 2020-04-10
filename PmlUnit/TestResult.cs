// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    class TestResult
    {
        public PmlError Error { get; }
        public TimeSpan Duration { get; }

        public TestResult(TimeSpan duration)
            : this(duration, null)
        {
        }

        public TestResult(TimeSpan duration, PmlError error)
        {
            Error = error;
            Duration = duration;
        }

        public bool Passed
        {
            get { return Error == null; }
        }
    }
}
