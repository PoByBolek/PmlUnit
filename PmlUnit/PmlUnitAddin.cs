// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using Aveva.ApplicationFramework;

#if PDMS || E3D_11
using IAddin = Aveva.ApplicationFramework.IAddin;
using ICommandManager = Aveva.ApplicationFramework.Presentation.CommandManager;
using IWindowManager = Aveva.ApplicationFramework.Presentation.WindowManager;
#else
using IAddin = Aveva.ApplicationFramework.IAddinInjected;
using ICommandManager = Aveva.ApplicationFramework.Presentation.ICommandManager;
using IWindowManager = Aveva.ApplicationFramework.Presentation.IWindowManager;
#endif

namespace PmlUnit
{
    public class PmlUnitAddin : IAddin, IDisposable
    {
        private readonly TestRunner TestRunner;
        private readonly TestCaseProvider TestCaseProvider;
        private readonly TestRunnerControl RunnerControl;

        public PmlUnitAddin()
        {
            try
            {
                TestRunner = new PmlTestRunner();
                TestCaseProvider = new EnvironmentVariableTestCaseProvider();
                RunnerControl = new TestRunnerControl(TestCaseProvider, TestRunner);
            }
            catch
            {
                if (TestRunner != null)
                    TestRunner.Dispose();
                if (RunnerControl != null)
                    RunnerControl.Dispose();
                throw;
            }
        }

        internal PmlUnitAddin(TestCaseProvider provider, TestRunner runner)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));

            TestCaseProvider = provider;
            TestRunner = runner;
            RunnerControl = new TestRunnerControl(TestCaseProvider, TestRunner);
        }

        ~PmlUnitAddin()
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
            if (disposing)
            {
                TestRunner.Dispose();
                RunnerControl.Dispose();
            }
        }

        public string Description
        {
            get { return "PML Unit Test Runner Addin"; }
        }

        public string Name
        {
            get { return "PML Unit"; }
        }

        [CLSCompliant(false)]
        public void Start(ServiceManager serviceManager)
        {
            Start(new ProxyServiceProvider(serviceManager));
        }

#if E3D_21
        [CLSCompliant(false)]
        public void Start(IDependencyResolver resolver)
        {
            Start(new DependencyResolverServiceProvider(resolver));
        }
#endif

        internal void Start(ServiceProvider provider)
        {
            try
            {
                RunnerControl.LoadTests();

                var windowManager = provider.GetService<IWindowManager>();
                var commandManager = provider.GetService<ICommandManager>();
                if (windowManager != null && commandManager != null)
                    commandManager.Commands.Add(new ShowTestRunnerCommand(windowManager, RunnerControl));
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Stop()
        {
            // PDMS crashes if we also dispose the TestRunnerControl here
            TestRunner.Dispose();
        }
    }
}
