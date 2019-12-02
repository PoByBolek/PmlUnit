// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;

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
            AsyncTestRunner runner = null;

            try
            {
                runner = new PmlTestRunner(new StubObjectProxy(), new StubMethodInvoker());
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

        private class StubObjectProxy : ObjectProxy
        {
            public void Dispose()
            {
            }

            public object Invoke(string method, params object[] arguments)
            {
                return new Hashtable();
            }
        }

        private class StubMethodInvoker : MethodInvoker
        {
            public void BeginInvoke(Delegate method, params object[] arguments)
            {
            }
        }
    }
}
