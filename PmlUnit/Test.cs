// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    class Test
    {
        public event EventHandler ResultChanged;

        public TestCase TestCase { get; }
        public string Name { get; }

        private TestResult ResultField;

        public Test(TestCase testCase, string name)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!TestCase.NameRegex.IsMatch(name))
                throw new ArgumentException("Test names must start with a letter and only contain letters and digits", nameof(name));

            TestCase = testCase;
            Name = name;
        }

        public TestResult Result
        {
            get { return ResultField; }
            set
            {
                if (value != ResultField)
                {
                    ResultField = value;
                    ResultChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public TestStatus Status
        {
            get
            {
                if (ResultField == null)
                    return TestStatus.NotExecuted;
                else if (ResultField.Success)
                    return TestStatus.Successful;
                else
                    return TestStatus.Failed;
            }
        }

        public string FullName
        {
            get { return TestCase.Name + "." + Name; }
        }

        public override string ToString()
        {
            return FullName;
        }
    }

    enum TestStatus
    {
        NotExecuted,
        Failed,
        Successful,
    }
}
