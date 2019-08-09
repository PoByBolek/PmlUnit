// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Reflection;

namespace PmlUnit
{
    static class AssemblyExtensions
    {
        public static T[] GetCustomAttributes<T>(this Assembly assembly, bool inherit) where T : Attribute
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            return assembly.GetCustomAttributes(typeof(T), inherit) as T[];
        }
    }
}
