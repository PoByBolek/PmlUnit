// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    public interface EntryPointResolver
    {
        EntryPoint Resolve(string value);
    }

    class SimpleEntryPointResolver : EntryPointResolver
    {
        public EntryPoint Resolve(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(value);

            if (value.StartsWith("Macro ", StringComparison.OrdinalIgnoreCase))
            {
                return new EntryPoint(EntryPointKind.Macro, value.Substring(6));
            }
            else if (value.StartsWith("PML function ", StringComparison.OrdinalIgnoreCase))
            {
                var kind = EntryPointKind.Unknown;
                if (value.IndexOf(".", StringComparison.Ordinal) >= 0)
                    kind = EntryPointKind.Method;
                else
                    kind = EntryPointKind.Function;
                return new EntryPoint(kind, value.Substring(13));
            }
            else
            {
                return new EntryPoint(EntryPointKind.Unknown, value);
            }
        }
    }
}
