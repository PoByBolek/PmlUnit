// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.IO;

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

    class IndexFileEntryPointResolver : EntryPointResolver
    {
        private readonly IndexFile Index;

        public IndexFileEntryPointResolver(IndexFile index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            Index = index;
        }

        public EntryPoint Resolve(string value)
        {
            if (value.StartsWith("Macro ", StringComparison.OrdinalIgnoreCase))
            {
                var fileName = Path.GetFullPath(value.Substring(6));
                return new EntryPoint(EntryPointKind.Macro, fileName, fileName);
            }
            else if (value.StartsWith("PML function ", StringComparison.OrdinalIgnoreCase))
            {
                string functionName = value.Substring(13);
                string fileName = null;
                var kind = EntryPointKind.Unknown;

                var dotIndex = functionName.IndexOf(".", StringComparison.Ordinal);
                if (dotIndex >= 0)
                {
                    kind = EntryPointKind.Method;
                    string baseName = functionName.Substring(0, dotIndex);
                    foreach (var extension in new string[] { ".pmlobj", ".pmlfrm", ".pmlcmd" })
                    {
                        if (Index.Files.TryGetValue(baseName + extension, out fileName))
                            break;
                    }
                }
                else
                {
                    kind = EntryPointKind.Function;
                    Index.Files.TryGetValue(functionName + ".pmlfnc", out fileName);
                }
                return new EntryPoint(kind, functionName, fileName);
            }
            else
            {
                return new EntryPoint(EntryPointKind.Unknown, value);
            }
        }
    }
}
