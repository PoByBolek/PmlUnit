// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    public class EntryPoint
    {
        public EntryPointKind Kind { get; }
        public string Name { get; }
        public string FileName { get; }

        public EntryPoint(EntryPointKind kind, string name)
            : this(kind, name, "")
        {
        }

        public EntryPoint(EntryPointKind kind, string name, string fileName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Kind = kind;
            Name = name;
            FileName = fileName ?? "";
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
