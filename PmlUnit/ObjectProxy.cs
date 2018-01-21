using System;

namespace PmlUnit
{
    interface ObjectProxy : IDisposable
    {
        object Invoke(string method, params object[] arguments);
    }
}
