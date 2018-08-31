using System;
using Aveva.ApplicationFramework;
using Aveva.ApplicationFramework.Presentation;

namespace PmlUnit
{
    class ShowTestRunnerCommand : Command
    {
        private readonly DockedWindow Window;
        private readonly TestRunnerControl RunnerControl;

        public ShowTestRunnerCommand(ServiceManager serviceManager)
            : this(serviceManager.GetService<WindowManager>(), serviceManager.GetService<TestCaseProvider>())
        {
        }

        public ShowTestRunnerCommand(WindowManager windowManager, TestCaseProvider testCaseProvider)
        {
            if (windowManager == null)
                throw new ArgumentNullException(nameof(windowManager));
            if (testCaseProvider == null)
                throw new ArgumentNullException(nameof(testCaseProvider));

            ExecuteOnCheckedChange = false;
            Key = "PmlUnit.ShowTestRunnerCommand";

            RunnerControl = new TestRunnerControl(testCaseProvider);
            try
            {

                Window = windowManager.CreateDockedWindow(
                    "PmlUnit.TestRunner", "PML Unit", RunnerControl, DockedPosition.Right
                );
                Window.SaveLayout = true;
                Window.Shown += OnWindowShown;
                Window.Closed += OnWindowClosed;

                Checked = Window.Visible;
            }
            catch
            {
                RunnerControl.Dispose();
                throw;
            }
        }

        private void OnWindowShown(object sender, EventArgs e)
        {
            Checked = true;
            RunnerControl.LoadTests();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            Checked = false;
        }

        public override void Execute()
        {
            if (Checked)
                Window.Show();
            else
                Window.Hide();
        }
    }
}
