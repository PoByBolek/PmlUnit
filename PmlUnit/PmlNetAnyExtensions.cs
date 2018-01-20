using System;

using Aveva.PDMS.PMLNet;

namespace PmlUnit
{
    static class PmlNetAnyExtensions
    {
        public static object Invoke(this PMLNetAny obj, string method, params object[] arguments)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException(nameof(method));

            arguments = arguments ?? new object[0];
            object result = null;
            obj.invokeMethod(method, arguments, arguments.Length, ref result);
            return result;
        }
    }
}
