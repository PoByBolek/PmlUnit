// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using Aveva.ApplicationFramework;
using Aveva.ApplicationFramework.Presentation;

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

        internal PmlUnitAddin(TestRunner testRunner)
        {
            if (testRunner == null)
                throw new ArgumentNullException(nameof(testRunner));

            TestRunner = testRunner;
            TestCaseProvider = new EnvironmentVariableTestCaseProvider();
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
            try
            {
                StartInternal(serviceManager);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private void StartInternal(ServiceManager serviceManager)
        {
            serviceManager.AddService(TestRunner);
            serviceManager.AddService(TestCaseProvider);

            var windowManager = serviceManager.GetService<WindowManager>();
            var commandManager = serviceManager.GetService<CommandManager>();
            if (windowManager != null && commandManager != null)
                commandManager.Commands.Add(new ShowTestRunnerCommand(windowManager, RunnerControl));
        }

        public void Stop()
        {
            // PDMS crashes if we also dispose the TestRunnerControl here
            TestRunner.Dispose();
        }
    }
}
