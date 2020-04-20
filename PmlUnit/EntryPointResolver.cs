// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PmlUnit
{
    public interface EntryPointResolver
    {
        EntryPoint Resolve(string value, int line);
    }

    class SimpleEntryPointResolver : EntryPointResolver
    {
        public EntryPoint Resolve(string value, int line)
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

    class FileIndexEntryPointResolver : EntryPointResolver
    {
        private readonly FileIndex Index;

        public FileIndexEntryPointResolver()
            : this(new FileIndex())
        {
        }

        public FileIndexEntryPointResolver(FileIndex index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            Index = index;
        }

        public EntryPoint Resolve(string value, int line)
        {
            if (value.StartsWith("Macro ", StringComparison.OrdinalIgnoreCase))
            {
                var fileName = Path.GetFullPath(value.Substring(6));
                return new EntryPoint(EntryPointKind.Macro, Path.GetFileName(fileName), fileName);
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
                        if (Index.TryGetFile(baseName + extension, out fileName))
                        {
                            functionName = TryGetMethodName(fileName, line) ?? functionName;
                            break;
                        }
                    }
                }
                else
                {
                    kind = EntryPointKind.Function;
                    if (Index.TryGetFile(functionName + ".pmlfnc", out fileName))
                    {
                        functionName = TryGetFunctionName(fileName) ?? functionName;
                    }
                }
                return new EntryPoint(kind, functionName, fileName);
            }
            else
            {
                return new EntryPoint(EntryPointKind.Unknown, value);
            }
        }

        private static string TryGetFunctionName(string fileName)
        {
            foreach (string line in TryReadFile(fileName))
            {
                if (line.StartsWith("define function !!", StringComparison.OrdinalIgnoreCase))
                    return ParseSignature(line.Substring(16));
            }
            return null;
        }

        private static string TryGetMethodName(string fileName, int lineNumber)
        {
            if (lineNumber <= 0)
                return null;
            
            int currentLine = 0;
            string baseName = null;
            string methodName = null;
            foreach (string line in TryReadFile(fileName))
            {
                ++currentLine;
                if (currentLine > lineNumber)
                {
                    break;
                }
                else if (line.StartsWith("define object ", StringComparison.OrdinalIgnoreCase))
                {
                    if (baseName != null)
                        return null;
                    baseName = line.Substring(14);
                }
                else if (line.StartsWith("setup form !!", StringComparison.OrdinalIgnoreCase))
                {
                    if (baseName != null)
                        return null;
                    baseName = line.Substring(11);
                }
                else if (line.StartsWith("setup command !!", StringComparison.OrdinalIgnoreCase))
                {
                    if (baseName != null)
                        return null;
                    baseName = line.Substring(14);
                }
                else if (line.StartsWith("define method .", StringComparison.OrdinalIgnoreCase))
                {
                    if (methodName != null)
                        return null;
                    methodName = ParseSignature(line.Substring(15));
                    if (methodName == null)
                        return null;
                }
                else if (line.StartsWith("endmethod", StringComparison.OrdinalIgnoreCase))
                {
                    methodName = null;
                }
            }

            if (baseName == null || methodName == null)
                return null;
            else
                return baseName + "." + methodName;
        }

        private static string ParseSignature(string signature)
        {
            int startIndex = signature.IndexOf('(');
            int endIndex = signature.IndexOf(')');
            if (startIndex < 0 || endIndex < 0 || startIndex > endIndex)
                return null;

            StringBuilder result = new StringBuilder(signature.Substring(0, startIndex));
            string[] arguments;
            var argumentString = signature.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
            if (string.IsNullOrEmpty(argumentString))
                arguments = new string[0];
            else
                arguments = argumentString.Split(',');

            result.Append('(');
            for (int i = 0; i < arguments.Length; i++)
            {
                if (i > 0)
                    result.Append(", ");

                string argument = arguments[i];
                int isIndex = argument.IndexOf(" is ", StringComparison.OrdinalIgnoreCase);
                if (isIndex >= 0)
                    result.Append(argument.Substring(isIndex + 4).Trim());
                else
                    return null;
            }
            result.Append(')');
            return result.ToString();
        }

        private static IEnumerable<string> TryReadFile(string fileName)
        {
            StreamReader reader;
            try
            {
                reader = new StreamReader(fileName, Encoding.UTF8);
            }
            catch (IOException)
            {
                yield break;
            }

            using (reader)
            {
                var whiteSpaceRegex = new Regex(@"\s+");
                bool inComment = false;
                while (true)
                {
                    string line;
                    try
                    {
                        line = reader.ReadLine();
                    }
                    catch (IOException)
                    {
                        yield break;
                    }

                    if (line == null)
                        yield break;

                    line = whiteSpaceRegex.Replace(line, " ").Trim();
                    if (inComment)
                    {
                        if (line.IndexOf("$)", StringComparison.Ordinal) >= 0)
                            inComment = false;
                        continue;
                    }
                    else if (line.IndexOf("$(", StringComparison.Ordinal) >= 0)
                    {
                        inComment = true;
                        continue;
                    }
                    else
                    {
                        yield return line;
                    }
                }
            }
        }
    }
}
