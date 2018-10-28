using System;
using System.Diagnostics.CodeAnalysis;
using Aveva.ApplicationFramework;
using Aveva.ApplicationFramework.Presentation;

namespace PmlUnit
{
    class ShowTestRunnerCommand : Command
    {
        private readonly DockedWindow Window;

        public ShowTestRunnerCommand(ServiceManager serviceManager)
            : this(
                  serviceManager.GetService<WindowManager>(),
                  serviceManager.GetService<TestCaseProvider>(),
                  serviceManager.GetService<TestRunner>()
            )
        {
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The DockedWindow should dispose the Control when PDMS exits.")]
        public ShowTestRunnerCommand(WindowManager windowManager, TestCaseProvider testCaseProvider, TestRunner testRunner)
        {
            if (windowManager == null)
                throw new ArgumentNullException(nameof(windowManager));
            if (testCaseProvider == null)
                throw new ArgumentNullException(nameof(testCaseProvider));
            if (testRunner == null)
                throw new ArgumentNullException(nameof(testRunner));

            ExecuteOnCheckedChange = false;
            Key = "PmlUnit.ShowTestRunnerCommand";

            var runnerControl = new TestRunnerControl(testCaseProvider, testRunner);
            try
            {

                Window = windowManager.CreateDockedWindow(
                    "PmlUnit.TestRunner", "PML Unit", runnerControl, DockedPosition.Right
                );
                Window.SaveLayout = true;
                Window.Shown += OnWindowShown;
                Window.Closed += OnWindowClosed;
            }
            catch
            {
                runnerControl.Dispose();
                throw;
            }
        }

        public void InitializeCheckedState()
        {
            // Checked is a virtual property of the Command base class. So we
            // shouldn't call this in the constructor. That's why this method
            // exists.
            Checked = Window.Visible;
        }

        private void OnWindowShown(object sender, EventArgs e)
        {
            Checked = true;
            var runnerControl = Window.Control as TestRunnerControl;
            if (runnerControl != null)
                runnerControl.LoadTests();
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
