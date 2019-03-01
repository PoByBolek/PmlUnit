// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using Aveva.ApplicationFramework.Presentation;
using PmlUnit.Properties;

#if PDMS || E3D_11
using IWindowManager = Aveva.ApplicationFramework.Presentation.WindowManager;
#endif

namespace PmlUnit
{
    class ShowTestRunnerCommand : Command
    {
        private readonly DockedWindow Window;

        public ShowTestRunnerCommand(IWindowManager windowManager, TestRunnerControl control)
        {
            if (windowManager == null)
                throw new ArgumentNullException(nameof(windowManager));
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            Key = "PmlUnit.ShowTestRunnerCommand";

            Window = windowManager.CreateDockedWindow(
                "PmlUnit.TestRunner", "PML Unit", control, DockedPosition.Right
            );
            Window.Image = Resources.TestRunner;
            Window.SaveLayout = true;
            Window.Shown += OnWindowShown;
            Window.Closed += OnWindowClosed;

            windowManager.WindowLayoutLoaded += OnWindowLayoutLoaded;
        }

        private void OnWindowLayoutLoaded(object sender, EventArgs e)
        {
            Checked = Window.Visible;
        }

        private void OnWindowShown(object sender, EventArgs e)
        {
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
