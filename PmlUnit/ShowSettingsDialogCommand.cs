// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Windows.Forms;
using Aveva.ApplicationFramework.Presentation;

#if PDMS || E3D_11
using IWindowManager = Aveva.ApplicationFramework.Presentation.WindowManager;
#endif

namespace PmlUnit
{
    class ShowSettingsDialogCommand : Command
    {
        private readonly CodeEditorDialog Dialog;
        private readonly SettingsProvider Settings;
        private readonly Form MainForm;

        public ShowSettingsDialogCommand(IWindowManager windowManager, CodeEditorDialog dialog, SettingsProvider settings)
        {
            if (windowManager == null)
                throw new ArgumentNullException(nameof(windowManager));
            if (dialog == null)
                throw new ArgumentNullException(nameof(dialog));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Key = "PmlUnit.ShowSettingsDialogCommand";
            Dialog = dialog;
            Settings = settings;
            MainForm = windowManager.MainForm;
        }

        public override void Execute()
        {
            Dialog.Descriptor = Settings.CodeEditor;
            var result = Dialog.ShowDialog(MainForm);
            if (result == DialogResult.OK)
            {
                Settings.CodeEditor = Dialog.Descriptor;
            }
        }
    }
}