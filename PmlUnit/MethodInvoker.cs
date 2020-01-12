// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    interface MethodInvoker
    {
        void BeginInvoke(Delegate method, params object[] arguments);
    }
}
