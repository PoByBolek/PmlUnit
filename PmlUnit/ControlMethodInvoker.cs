// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Windows.Forms;

namespace PmlUnit
{
    class ControlMethodInvoker : MethodInvoker
    {
        private readonly Func<Control> Factory;
        private Control Control;

        public ControlMethodInvoker(Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            Control = control;
        }

        public ControlMethodInvoker(Func<Control> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            Factory = factory;
        }

        public void BeginInvoke(Delegate method, params object[] arguments)
        {
            if (Control == null)
            {
                Control = Factory();
                if (Control == null)
                    throw new InvalidOperationException("Factory did not return a control");
            }

            Control.BeginInvoke(method, arguments);
        }
    }
}
