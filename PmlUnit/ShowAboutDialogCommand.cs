// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Windows.Forms;
using Aveva.ApplicationFramework.Presentation;

#if PDMS || E3D_11
using IWindowManager = Aveva.ApplicationFramework.Presentation.WindowManager;
#endif

namespace PmlUnit
{
    class ShowAboutDialogCommand : Command
    {
        private readonly AboutDialog Dialog;
        private readonly Form MainForm;

        public ShowAboutDialogCommand(IWindowManager windowManager, AboutDialog dialog)
        {
            if (windowManager == null)
                throw new ArgumentNullException(nameof(windowManager));
            if (dialog == null)
                throw new ArgumentNullException(nameof(dialog));

            Key = "PmlUnit.ShowAboutDialogCommand";
            MainForm = windowManager.MainForm;
            Dialog = dialog;
        }

        public override void Execute()
        {
            Dialog.ShowDialog(MainForm);
        }
    }
}
