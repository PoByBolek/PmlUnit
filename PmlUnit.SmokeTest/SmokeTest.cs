// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using NUnit.Framework;

namespace PmlUnit
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public static class SmokeTest
    {
        [Test]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "proxy, runner, and control are all disposed if an Exception occurs")]
        public static void TestRunnerControlInstantiation()
        {
            TestRunnerControl control = null;
            AsyncTestRunner runner = null;
            ObjectProxy proxy = null;

            try
            {
                proxy = new StubObjectProxy();
                runner = new PmlTestRunner(proxy, new StubMethodInvoker());
                proxy = null;
                var provider = new FileIndexTestCaseProvider();
                control = new TestRunnerControl(provider, runner, new RegistrySettingsProvider());
                runner = null;
            }
            finally
            {
                if (proxy != null)
                    proxy.Dispose();
                if (runner != null)
                    runner.Dispose();
                if (control != null)
                    control.Dispose();
            }
        }

        [Test]
        public static void TestAboutDialogInstantiation()
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
