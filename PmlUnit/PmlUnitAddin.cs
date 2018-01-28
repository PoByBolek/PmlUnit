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
            return typeof(PmlUnitAddin).Assembly.GetManifestResourceStream("PmlUnit.PmlUnitAddin.uic");
        }

        public void Stop()
        {
        }
    }
}
