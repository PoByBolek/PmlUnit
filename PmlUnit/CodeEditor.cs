// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace PmlUnit
{
    interface CodeEditor
    {
        void OpenFile(string fileName, int lineNumber);
    }

    sealed class CodeEditorDescriptor
    {
        public CodeEditorKind Kind { get; set; }

        private string FileNameField;
        private string FixedArgumentsField;

        public CodeEditorDescriptor(CodeEditorKind kind, string fileName, string arguments)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            Kind = kind;
            FileNameField = fileName;
            FixedArgumentsField = arguments;
        }

        public string FileName
        {
            get { return FileNameField; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(value));
                FileNameField = value;
            }
        }

        public string FixedArguments
        {
            get { return FixedArgumentsField; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                FixedArgumentsField = value;
            }
        }

        public CodeEditor ToEditor()
        {
            switch (Kind)
            {
                case CodeEditorKind.Atom:
                    return new AtomCodeEditor(FileNameField, FixedArgumentsField);
                case CodeEditorKind.NotepadPlusPlus:
                    return new NotepadPlusPlusCodeEditor(FileNameField, FixedArgumentsField);
                case CodeEditorKind.SublimeText:
                    return new SublimeTextCodeEditor(FileNameField, FixedArgumentsField);
                case CodeEditorKind.UltraEdit:
                    return new UltraEditCodeEditor(FileNameField, FixedArgumentsField);
                case CodeEditorKind.VisualStudioCode:
                    return new VisualStudioCodeCodeEditor(FileNameField, FixedArgumentsField);
                default:
                    return new OtherCodeEditor(FileNameField, FixedArgumentsField);
            }
        }
    }

    enum CodeEditorKind
    {
        Other = 0,
        Atom = 0x41544f4d, // ATOM
        NotepadPlusPlus = 0x4e502b2b, // NP++
        SublimeText = 0x5355424c, // SUBL
        UltraEdit = 0x55454454, // UEDT
        VisualStudioCode = 0x56534344, // VSCD
    }

    abstract class BaseCodeEditor : CodeEditor
    {
        protected readonly string FileName;
        protected readonly string FixedArguments;

        protected BaseCodeEditor(string fileName, string fixedArguments)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            FileName = fileName;
            FixedArguments = fixedArguments ?? "";
        }

        public void OpenFile(string fileName, int lineNumber)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (lineNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(lineNumber));

            Process.Start(BuildStartInfo(fileName, lineNumber));
        }

        protected ProcessStartInfo BuildStartInfo(string fileName, int lineNumber)
        {
            var result = new ProcessStartInfo(FileName);
            result.UseShellExecute = false;

            var arguments = new StringBuilder(FixedArguments);
            foreach (string argument in GetExtraArguments(fileName, lineNumber))
                arguments.Append(' ').Append(Escape(argument));
            result.Arguments = arguments.ToString().Trim();

            return result;
        }

        private static string Escape(string argument)
        {
            // see https://docs.microsoft.com/en-us/previous-versions/17w5ykft(v=vs.85) << note the closing parenthesis
            int index = argument.IndexOfAny(new char[] { ' ', '\t', '"' });
            if (index < 0)
                return argument;

            var result = new StringBuilder();
            int backslashes = 0;
            result.Append('"');
            foreach (char c in argument)
            {
                if (c == '\\')
                {
                    backslashes++;
                }
                else if (c == '"')
                {
                    // escape all backslashes before this quote and the quote itself
                    result.Append('\\', 2 * backslashes + 1).Append('"');
                    backslashes = 0;
                }
                else
                {
                    // backslashes only have a special meaning before quotes
                    result.Append('\\', backslashes).Append(c);
                    backslashes = 0;
                }
            }
            // escape all backslashes before the final quote so that it does not get escaped
            result.Append('\\', 2 * backslashes).Append('"');
            return result.ToString();
        }

        protected abstract IEnumerable<string> GetExtraArguments(string fileName, int lineNumber);
    }

    class AtomCodeEditor : BaseCodeEditor
    {
        public AtomCodeEditor(string fileName, string arguments)
            : base(fileName, arguments)
        {
        }

        protected override IEnumerable<string> GetExtraArguments(string fileName, int lineNumber)
        {
            // see https://flight-manual.atom.io/getting-started/sections/atom-basics/
            if (lineNumber > 0)
                return new string[] { fileName + ":" + lineNumber.ToString(CultureInfo.InvariantCulture) };
            else
                return new string[] { fileName };
        }
    }

    class NotepadPlusPlusCodeEditor : BaseCodeEditor
    {
        public NotepadPlusPlusCodeEditor(string fileName, string arguments)
            : base(fileName, arguments)
        {
        }

        protected override IEnumerable<string> GetExtraArguments(string fileName, int lineNumber)
        {
            // see https://npp-user-manual.org/docs/command-prompt/
            if (lineNumber > 0)
                return new string[] { "-n" + lineNumber.ToString(CultureInfo.InvariantCulture), fileName };
            else
                return new string[] { fileName };
        }
    }

    class SublimeTextCodeEditor : BaseCodeEditor
    {
        public SublimeTextCodeEditor(string fileName, string arguments)
            : base(fileName, arguments)
        {
        }

        protected override IEnumerable<string> GetExtraArguments(string fileName, int lineNumber)
        {
            // see https://sublime-text-unofficial-documentation.readthedocs.io/en/latest/command_line/command_line.html
            if (lineNumber > 0)
                return new string[] { fileName + ":" + lineNumber.ToString(CultureInfo.InvariantCulture) };
            else
                return new string[] { fileName };
        }
    }

    class UltraEditCodeEditor : BaseCodeEditor
    {
        public UltraEditCodeEditor(string fileName, string arguments)
            : base(fileName, arguments)
        {
        }

        protected override IEnumerable<string> GetExtraArguments(string fileName, int lineNumber)
        {
            // see https://www.ultraedit.com/wiki/Command_line_parameters
            if (lineNumber > 0)
                return new string[] { fileName, "-l" + lineNumber.ToString(CultureInfo.InvariantCulture) };
            else
                return new string[] { fileName };
        }
    }

    class VisualStudioCodeCodeEditor : BaseCodeEditor
    {
        public VisualStudioCodeCodeEditor(string fileName, string arguments)
            : base(fileName, arguments)
        {
        }

        protected override IEnumerable<string> GetExtraArguments(string fileName, int lineNumber)
        {
            // see https://code.visualstudio.com/docs/editor/command-line
            if (lineNumber > 0)
                return new string[] { "--goto", fileName + ":" + lineNumber.ToString(CultureInfo.InvariantCulture) };
            else
                return new string[] { fileName };
        }
    }

    class OtherCodeEditor : BaseCodeEditor
    {
        public OtherCodeEditor(string fileName, string arguments)
            : base(fileName, arguments)
        {
        }

        protected override IEnumerable<string> GetExtraArguments(string fileName, int lineNumber)
        {
            throw new NotImplementedException();
        }
    }
}
