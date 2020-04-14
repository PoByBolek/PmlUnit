// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    public class EntryPoint
    {
        public EntryPointKind Kind { get; }
        public string Name { get; }

        public EntryPoint(EntryPointKind kind, string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Kind = kind;
            Name = name;
        }
    }

    public enum EntryPointKind
    {
        Unknown,
        Macro,
        Function,
        Method,
    }
}
