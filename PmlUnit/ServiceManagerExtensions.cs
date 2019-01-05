// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using Aveva.ApplicationFramework;

namespace PmlUnit
{
    [CLSCompliant(false)]
    public static class ServiceManagerExtensions
    {
        public static T GetService<T>(this ServiceManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));
            return (T)manager.GetService(typeof(T));
        }

        public static void AddService<T>(this ServiceManager manager, T service)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            manager.AddService(typeof(T), service);
        }
    }
}
