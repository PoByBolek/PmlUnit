using System;
using System.Diagnostics.CodeAnalysis;

#if E3D_21
using Aveva.Core.PMLNet;
#else
using Aveva.PDMS.PMLNet;
#endif

namespace PmlUnit
{
    class PmlObjectProxy : ObjectProxy, IDisposable
    {
        private PMLNetAny Object;

        public PmlObjectProxy(string objectName, params object[] arguments)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException(nameof(objectName));

            arguments = arguments ?? new object[0];
            Object = PMLNetAny.createInstance(objectName, arguments, arguments.Length);
        }

        private PmlObjectProxy(PMLNetAny obj)
        {
            Object = obj;
        }

        ~PmlObjectProxy()
        {
            Dispose(false);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Callers are required to dispose the return value of this method.")]
        public object Invoke(string method, params object[] arguments)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException(nameof(method));
            if (Object == null)
                throw new ObjectDisposedException(nameof(PmlObjectProxy));

            arguments = arguments ?? new object[0];
            object result = null;
            Object.invokeMethod(method, arguments, arguments.Length, ref result);

            var any = result as PMLNetAny;
            if (any == null)
                return result;
            else
                return new PmlObjectProxy(any);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Object == null)
                return;

            if (disposing)
                Object.Dispose();

            Object = null;
        }
    }
}
