// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    interface ObjectProxy : IDisposable
    {
        object Invoke(string method, params object[] arguments);
    }
}
