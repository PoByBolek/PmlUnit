// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using Microsoft.Win32;

namespace PmlUnit
{
    interface CodeEditorProvider
    {
        CodeEditorDescriptor LoadDescriptor();
        void SaveDescriptor(CodeEditorDescriptor descriptor);
    }

    class RegistryCodeEditorProvider : CodeEditorProvider
    {
        private const string KeyPath = "Software\\PML Unit";
        private const string KindValueName = "EditorKind";
        private const string PathValueName = "EditorPath";
        private const string ArgumentsValueName = "EditorArguments";

        public CodeEditorDescriptor LoadDescriptor()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(KeyPath))
            {
                if (key == null)
                    return null;

                CodeEditorKind kind;
                if (!TryGetKind(key, KindValueName, out kind))
                    return null;

                string path;
                if (!TryGetString(key, PathValueName, out path) || string.IsNullOrEmpty(path))
                    return null;

                string arguments;
                if (!TryGetString(key, ArgumentsValueName, out arguments))
                    return null;

                return new CodeEditorDescriptor(kind, path, arguments);
            }
        }

        private bool TryGetKind(RegistryKey key, string name, out CodeEditorKind result)
        {
            var value = key.GetValue(name);
            if (value is int && Enum.IsDefined(typeof(CodeEditorKind), value))
            {
                result = (CodeEditorKind)value;
                return true;
            }
            else
            {
                result = CodeEditorKind.Other;
                return false;
            }
        }

        private bool TryGetString(RegistryKey key, string name, out string result)
        {
            result = key.GetValue(name) as string;
            return result != null;
        }

        public void SaveDescriptor(CodeEditorDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            using (var key = Registry.CurrentUser.CreateSubKey(KeyPath))
            {
                if (key == null)
                    return;

                key.SetValue(KindValueName, descriptor.Kind, RegistryValueKind.DWord);
                key.SetValue(PathValueName, descriptor.FileName, RegistryValueKind.String);
                key.SetValue(ArgumentsValueName, descriptor.FixedArguments, RegistryValueKind.String);
            }
        }
    }

}
