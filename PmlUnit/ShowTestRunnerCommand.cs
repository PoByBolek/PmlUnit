using System;
using Aveva.ApplicationFramework.Presentation;

namespace PmlUnit
{
    class ShowTestRunnerCommand : Command
    {
        private readonly DockedWindow Window;
        private readonly TestRunnerControl RunnerControl;

        public ShowTestRunnerCommand(WindowManager windowManager)
        {
            if (windowManager == null)
                throw new ArgumentNullException(nameof(windowManager));
            
            ExecuteOnCheckedChange = false;
            Key = "PmlUnit.ShowTestRunnerCommand";
            
            RunnerControl = new TestRunnerControl(new EnvironmentVariableTestCaseProvider());
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
