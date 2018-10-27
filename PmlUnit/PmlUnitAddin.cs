using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Aveva.ApplicationFramework;
using Aveva.ApplicationFramework.Presentation;

namespace PmlUnit
{
    public class PmlUnitAddin : IAddin
    {
        public string Description
        {
            get { return "PML Unit Test Runner Addin"; }
        }

        public string Name
        {
            get { return "PML Unit"; }
        }

        [CLSCompliant(false)]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Once the addin has successfully, started the TestRunnerControl will dispose the TestRunner.")]
        public void Start(ServiceManager serviceManager)
        {
            var testRunner = new PmlTestRunner();
            try
            {
                Start(serviceManager, testRunner);
            }
            catch
            {
                testRunner.Dispose();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "For consistency with the public Start() method of the IAddin interface, this is also an instance method.")]
        internal void Start(ServiceManager serviceManager, TestRunner testRunner)
        {
            serviceManager.AddService(testRunner);
            serviceManager.AddService<TestCaseProvider>(
                new EnvironmentVariableTestCaseProvider()
            );

            var commandManager = serviceManager.GetService<CommandManager>();
            if (commandManager != null)
                commandManager.Commands.Add(new ShowTestRunnerCommand(serviceManager));

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
            var resourceNames = new string[] { "PmlUnit.PmlUnitAddin.uic", "PmlUnit.PmlUnitAddin" };

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
        }
    }
}
