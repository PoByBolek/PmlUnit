// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using NUnit.Framework;

namespace PmlUnit
{
    [TestFixture]
    public class SmokeTest
    {
        [Test]
        public void TestRunnerControlInstantiation()
        {
            TestRunnerControl control = null;
            TestRunner runner = null;

            try
            {
                runner = new StubTestRunner();
                control = new TestRunnerControl(new EnvironmentVariableTestCaseProvider(), runner);
                runner = null;
            }
            finally
            {
                if (runner != null)
                    runner.Dispose();
                if (control != null)
                    control.Dispose();
            }
        }

        [Test]
        public void TestAboutDialogInstantiation()
        {
            AboutDialog dialog = null;
            try
            {
                dialog = new AboutDialog();
            }
            finally
            {
                if (dialog != null)
                    dialog.Dispose();
            }
        }

        private class StubTestRunner : TestRunner
        {
            public void Dispose()
            {
            }

            public void RefreshIndex()
            {
            }

            public void Reload(TestCase testCase)
            {
            }

            public TestResult Run(Test test)
            {
                return new TestResult(TimeSpan.FromSeconds(1));
            }

            public void Run(TestCase testCase)
            {
            }
        }
    }
}
