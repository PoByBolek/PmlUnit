// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using Microsoft.Win32;

namespace PmlUnit
{
    interface SettingsProvider
    {
        TestListGrouping TestGrouping { get; set; }
        CodeEditorDescriptor CodeEditor { get; set; }
    }

    class RegistrySettingsProvider : SettingsProvider
    {
        private const string KeyPath = "Software\\PML Unit";

        private const string TestGroupingValueName = "TestGrouping";

        private const string KindValueName = "EditorKind";
        private const string PathValueName = "EditorPath";
        private const string ArgumentsValueName = "EditorArguments";

        public TestListGrouping TestGrouping
        {
            get
            {
                using (var key = OpenRead())
                {
                    TestListGrouping result;
                    if (key == null || !key.TryGetEnum(TestGroupingValueName, out result))
                        result = TestListGrouping.Result;
                    return result;
                }
            }
            set
            {
                using (var key = OpenWrite())
                {
                    if (key == null)
                        return;

                    key.SetValue(TestGroupingValueName, value, RegistryValueKind.DWord);
                }
            }
        }

        public CodeEditorDescriptor CodeEditor
        {
            get
            {
                using (var key = OpenRead())
                {
                    if (key == null)
                        return null;

                    CodeEditorKind kind;
                    if (!key.TryGetEnum(KindValueName, out kind))
                        return null;

                    string path;
                    if (!key.TryGetString(PathValueName, out path) || string.IsNullOrEmpty(path))
                        return null;

                    string arguments;
                    if (!key.TryGetString(ArgumentsValueName, out arguments))
                        return null;

                    return new CodeEditorDescriptor(kind, path, arguments);
                }
            }
            set
            {
                using (var key = OpenWrite())
                {
                    if (key == null)
                        return;

                    if (value == null)
                    {
                        key.DeleteValue(KindValueName, throwOnMissingValue: false);
                        key.DeleteValue(PathValueName, throwOnMissingValue: false);
                        key.DeleteValue(ArgumentsValueName, throwOnMissingValue: false);
                    }
                    else
                    {
                        key.SetValue(KindValueName, value.Kind, RegistryValueKind.DWord);
                        key.SetValue(PathValueName, value.FileName, RegistryValueKind.String);
                        key.SetValue(ArgumentsValueName, value.FixedArguments, RegistryValueKind.String);
                    }
                }
            }
        }

        private static RegistryKey OpenRead()
        {
            return Registry.CurrentUser.OpenSubKey(KeyPath);
        }

        private static RegistryKey OpenWrite()
        {
            return Registry.CurrentUser.CreateSubKey(KeyPath);
        }
    }

}
