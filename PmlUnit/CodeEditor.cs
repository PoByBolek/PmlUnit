// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
            if (fileName == null)
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
                if (value == null)
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
            if (string.IsNullOrEmpty(FileNameField))
                throw new InvalidOperationException("FileName must not be empty.");

            switch (Kind)
            {
                case CodeEditorKind.Atom:
                    return new AtomCodeEditor(FileNameField, FixedArgumentsField);
                case CodeEditorKind.NotepadPlusPlus:
                    return new NotepadPlusPlusCodeEditor(FileNameField, FixedArgumentsField);
                case CodeEditorKind.PmlStudio:
                    return new PmlStudioCodeEditor(FileNameField, FixedArgumentsField);
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
        PmlStudio = 0x504d4c53, // PMLS
        SublimeText = 0x5355424c, // SUBL
        UltraEdit = 0x55454454, // UEDT
        VisualStudioCode = 0x56534344, // VSCD
    }

    abstract class BaseCodeEditor : CodeEditor
    {
        protected readonly string FileName;
        protected readonly IEnumerable<string> FixedArguments;

        protected BaseCodeEditor(string fileName, string fixedArguments)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            FileName = fileName;
            FixedArguments = SplitArguments(fixedArguments);
        }

        public void OpenFile(string fileName, int lineNumber)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (lineNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(lineNumber));

            using (var process = new Process())
            {
                process.StartInfo = BuildStartInfo(fileName, lineNumber);
                process.Start();
            }
        }

        protected ProcessStartInfo BuildStartInfo(string fileName, int lineNumber)
        {
            var result = new ProcessStartInfo(FileName);
            result.UseShellExecute = false;

            var arguments = new StringBuilder();
            foreach (string argument in GetArguments(fileName, lineNumber))
                arguments.Append(' ').Append(Escape(argument));
            result.Arguments = arguments.ToString().Trim();

            return result;
        }

        protected virtual IEnumerable<string> GetArguments(string fileName, int lineNumber)
        {
            return FixedArguments.Concat(GetExtraArguments(fileName, lineNumber));
        }

        protected abstract IEnumerable<string> GetExtraArguments(string fileName, int lineNumber);

        private static List<string> SplitArguments(string arguments)
        {
            var result = new List<string>();

            if (arguments == null)
                return result;
            arguments = arguments.Trim();
            if (string.IsNullOrEmpty(arguments))
                return result;

            // see https://docs.microsoft.com/en-us/previous-versions/17w5ykft(v=vs.85) << note the closing parenthesis
            var argument = new StringBuilder();
            bool backslash = false;
            bool quote = false;
            foreach (char c in arguments)
            {
                if (c == '\\')
                {
                    if (backslash)
                        argument.Append('\\');
                    backslash = !backslash;
                    continue;
                }
                else if (c == '"')
                {
                    if (backslash)
                    {
                        argument.Append('"');
                        backslash = false;
                    }
                    else
                    {
                        quote = !quote;
                    }
                    continue;
                }

                if (backslash)
                {
                    argument.Append('\\');
                    backslash = false;
                }

                if (c == ' ' || c == '\t')
                {
                    if (quote)
                    {
                        argument.Append(c);
                    }
                    else if (argument.Length > 0)
                    {
                        result.Add(argument.ToString());
                        argument.Remove(0, argument.Length);
                    }
                }
                else
                {
                    argument.Append(c);
                }
            }

            if (backslash)
                argument.Append('\\');
            if (argument.Length > 0)
                result.Add(argument.ToString());

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

    class PmlStudioCodeEditor : BaseCodeEditor
    {
        public PmlStudioCodeEditor(string fileName, string arguments)
            : base(fileName, arguments)
        {
        }

        protected override IEnumerable<string> GetExtraArguments(string fileName, int lineNumber)
        {
            // see https://docs.microsoft.com/en-us/visualstudio/ide/reference/go-to-command?view=vs-2019
            if (lineNumber > 0)
                return new string[] { fileName, "/command", "edit.goto " + lineNumber.ToString(CultureInfo.InvariantCulture) };
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
        public const string FileNameVariable = "!fileName";
        public const string LineNumberVariable = "!lineNumber";

        public OtherCodeEditor(string fileName, string arguments)
            : base(fileName, arguments)
        {
        }

        protected override IEnumerable<string> GetArguments(string fileName, int lineNumber)
        {
            return GetExtraArguments(fileName, lineNumber);
        }

        protected override IEnumerable<string> GetExtraArguments(string fileName, int lineNumber)
        {
            foreach (string argument in FixedArguments)
            {
                yield return argument
                    .Replace(LineNumberVariable, lineNumber.ToString(CultureInfo.InvariantCulture))
                    .Replace(FileNameVariable, fileName);
            }
        }
    }
}
