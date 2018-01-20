﻿using System;
using Aveva.PDMS.PMLNet;

namespace PmlUnit
{
    class PmlProxy : IDisposable
    {
        private PMLNetAny Object;

        public PmlProxy(string objectName, params object[] arguments)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException(nameof(objectName));

            arguments = arguments ?? new object[0];
            Object = PMLNetAny.createInstance(objectName, arguments, arguments.Length);
        }

        private PmlProxy(PMLNetAny obj)
        {
            Object = obj;
        }

        ~PmlProxy()
        {
            GC.SuppressFinalize(this);
            Dispose(false);
        }

        public object Invoke(string method, params object[] arguments)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException(nameof(method));
            if (Object == null)
                throw new ObjectDisposedException(nameof(PmlProxy));

            arguments = arguments ?? new object[0];
            object result = null;
            Object.invokeMethod(method, arguments, arguments.Length, ref result);

            var any = result as PMLNetAny;
            if (any == null)
                return result;
            else
                return new PmlProxy(any);
        }

        public void Dispose()
        {
            Dispose(true);
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
