using System;
using System.IO;
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

            var commandBarManager = serviceManager.GetService<CommandBarManager>();
            if (commandBarManager != null)
            {
                using (var stream = GetCustomizationFileStream())
                {
                    if (stream != null)
                        commandBarManager.AddUICustomizationFromStream(stream, "PmlUnit");
                }
            }
        }

        private static Stream GetCustomizationFileStream()
        {
            var assembly = typeof(PmlUnitAddin).Assembly;
            // Visual Studio (or MSBuild?) seems to give the resource different names from time to time...
#if PDMS
            var resourceNames = new string[] { "PmlUnit.PmlUnitAddin.pdms.uic", "PmlUnit.PmlUnitAddin.pdms" };
#else
            var resourceNames = new string[] { "PmlUnit.PmlUnitAddin.e3d.uic", "PmlUnit.PmlUnitAddin.e3d" };
#endif

            foreach (var resource in resourceNames)
            {
                var result = assembly.GetManifestResourceStream(resource);
                if (result != null)
                    return result;
            }

            return null;
        }

        public void Stop()
        {
            // PDMS crashes if we also dispose the TestRunnerControl here
            TestRunner.Dispose();
        }
    }
}
