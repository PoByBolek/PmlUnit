// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using Microsoft.Win32;

namespace PmlUnit
{
    static class RegistryExtensions
    {
        public static bool TryGetEnum<T>(this RegistryKey key, string name, out T result)
        {
            result = default(T);
            if (key == null)
                return false;

            var value = key.GetValue(name);
            if (value is int && Enum.IsDefined(typeof(T), value))
            {
                result = (T)value;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }

        public static bool TryGetString(this RegistryKey key, string name, out string result)
        {
            result = null;
            if (key == null)
                return false;

            result = key.GetValue(name) as string;
            return result != null;
        }
    }
}
