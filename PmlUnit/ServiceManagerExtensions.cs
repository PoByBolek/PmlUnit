using System;
using Aveva.ApplicationFramework;

namespace PmlUnit
{
    public static class ServiceManagerExtensions
    {
        public static T GetService<T>(this ServiceManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));
            return (T)manager.GetService(typeof(T));
        }
    }
}
