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

        public void Start(ServiceManager serviceManager)
        {
            var windowManager = serviceManager.GetService<WindowManager>();
            var commandManager = serviceManager.GetService<CommandManager>();
            commandManager.Commands.Add(new ShowTestRunnerCommand(windowManager));

            var commandBarManager = serviceManager.GetService<CommandBarManager>();
            using (var stream = GetCustomizationFileStream())
            {
                if (stream != null)
                    commandBarManager.AddUICustomizationFromStream(stream, "PmlUnit");
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
