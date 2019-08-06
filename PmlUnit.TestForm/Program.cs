// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Windows.Forms;

namespace PmlUnit
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm());
        }
    }
}
