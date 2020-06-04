// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(CodeEditorDescriptor))]
    public class CodeEditorDescriptorTest
    {
        public void ValidatesNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new CodeEditorDescriptor(CodeEditorKind.Other, null, null));
            Assert.Throws<ArgumentNullException>(() => new CodeEditorDescriptor(CodeEditorKind.Other, null, ""));
            Assert.Throws<ArgumentNullException>(() => new CodeEditorDescriptor(CodeEditorKind.Other, "", ""));
            Assert.Throws<ArgumentNullException>(() => new CodeEditorDescriptor(CodeEditorKind.Other, "", null));
            Assert.Throws<ArgumentNullException>(() => new CodeEditorDescriptor(CodeEditorKind.Other, "foo", null));

            var editor = new CodeEditorDescriptor(CodeEditorKind.Other, "foo", "");
            Assert.Throws<ArgumentNullException>(() => editor.FileName = null);
            Assert.Throws<ArgumentNullException>(() => editor.FileName = "");
            Assert.Throws<ArgumentNullException>(() => editor.FixedArguments = null);
            editor.FixedArguments = "";
        }
        
        [TestCase(CodeEditorKind.Atom, @"C:\Program Files\Atom\atom.exe", "random stuff", typeof(AtomCodeEditor), "random", "stuff")]
        [TestCase(CodeEditorKind.Atom, @"C:\Users\Current\AppData\Local\Atom\atom.exe", "\"other stuff\"", typeof(AtomCodeEditor), "other stuff")]
        [TestCase(CodeEditorKind.NotepadPlusPlus, @"C:\Program Files\Notepad++\notepad++.exe", "foobar", typeof(NotepadPlusPlusCodeEditor), "foobar")]
        [TestCase(CodeEditorKind.NotepadPlusPlus, @"C:\Program Files (x86)\Notepad++\notepad++.exe", "foobar", typeof(NotepadPlusPlusCodeEditor), "foobar")]
        [TestCase(CodeEditorKind.SublimeText, @"C:\Program Files\Sublime Text 3\subl.exe", "", typeof(SublimeTextCodeEditor))]
        [TestCase(CodeEditorKind.SublimeText, @"C:\Program Files\Sublime Text 3\subl.exe", "empty", typeof(SublimeTextCodeEditor), "empty")]
        [TestCase(CodeEditorKind.UltraEdit, @"C:\Program Files\UltraEdit\uedit64.exe", "--t\"e\"s\"t", typeof(UltraEditCodeEditor), "--test")]
        [TestCase(CodeEditorKind.UltraEdit, @"C:\Program Files (x86)\UltraEdit\uedit32.exe", "--\\clear\\\"", typeof(UltraEditCodeEditor), "--\\clear\"")]
        [TestCase(CodeEditorKind.VisualStudioCode, @"C:\Users\Current\AppData\Local\Programs\Microsoft VS Code\Code.exe", "escaped \\\\\\\" stuff", typeof(VisualStudioCodeCodeEditor), "escaped", "\\\"", "stuff")]
        [TestCase(CodeEditorKind.VisualStudioCode, @"C:\Program Files\Microsoft VS Code\Code.exe", "random", typeof(VisualStudioCodeCodeEditor), "random")]
        [TestCase(CodeEditorKind.Other, @"C:\Program Files\Editor\Editor.exe", "--be-awesome", typeof(OtherCodeEditor), "--be-awesome")]
        [TestCase(CodeEditorKind.Other, @"C:\Program Files\Something Else\foo.exe", "--bar", typeof(OtherCodeEditor), "--bar")]
        public void CreatesCodeEditorInstance(int kind, string fileName, string fixedArguments, Type expectedType, params string[] expectedArguments)
        {
            var descriptor = new CodeEditorDescriptor((CodeEditorKind)kind, fileName, fixedArguments);
            var editor = descriptor.ToEditor();
            Assert.That(editor, Is.InstanceOf(expectedType));
            var fileNameField = expectedType.GetField("FileName", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(fileNameField.GetValue(editor), Is.EqualTo(fileName));
            var fixedArgumentsField = expectedType.GetField("FixedArguments", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(fixedArgumentsField.GetValue(editor), Is.EquivalentTo(expectedArguments));
        }
    }

    [TestFixture]
    [TestOf(typeof(BaseCodeEditor))]
    public class CodeEditorTest
    {
        [TestCase(@"C:\path\to\file.pmlobj:25", "", @"C:\path\to\file.pmlobj:25")]
        [TestCase("\"C:\\path with spaces\\to\\file.pmlfrm:26\"", "", @"C:\path with spaces\to\file.pmlfrm:26")]
        [TestCase("multiple arguments \"that may have\" spaces", "", "multiple", "arguments", "that may have", "spaces")]
        [TestCase("\"quotes\\\"in\\\"arguments\\\"must\\\"be\\\"escaped\"", "", "quotes\"in\"arguments\"must\"be\"escaped")]
        [TestCase("backslashes\\must only be escaped \"before\\\\\\\"quotes\"", "", "backslashes\\must", "only", "be", "escaped", "before\\\"quotes")]
        [TestCase("fixed \"arguments sh\\\"ould\" be parsed \"dynamic arguments should come after the fixed ones\"", "fixed arg\"uments sh\\\"ould\" be parsed", "dynamic arguments should come after the fixed ones")]
        public void EscapesCommandLineArguments(string expectedArguments, string fixedArguments, params string[] extraArguments)
        {
            var editor = new StubCodeEditor(@"C:\Program Files\Editor\editor.exe", fixedArguments, extraArguments);
            Assert.That(editor.Arguments, Is.EqualTo(expectedArguments));
        }

        [TestCase(typeof(AtomCodeEditor), "", @"C:\path\to\file.pmlobj", 12345, @"C:\path\to\file.pmlobj:12345")]
        [TestCase(typeof(AtomCodeEditor), "", @"C:\path\to\file.pmlobj", 0, @"C:\path\to\file.pmlobj")]
        [TestCase(typeof(AtomCodeEditor), "", @"C:\path to\file.pmlobj", 1234, "\"C:\\path to\\file.pmlobj:1234\"")]
        [TestCase(typeof(AtomCodeEditor), "", @"C:\path to\file.pmlobj", 0, "\"C:\\path to\\file.pmlobj\"")]
        [TestCase(typeof(NotepadPlusPlusCodeEditor), "", @"C:\path\to\file.pmlobj", 12345, @"-n12345 C:\path\to\file.pmlobj")]
        [TestCase(typeof(NotepadPlusPlusCodeEditor), "", @"C:\path\to\file.pmlobj", 0, @"C:\path\to\file.pmlobj")]
        [TestCase(typeof(NotepadPlusPlusCodeEditor), "", @"C:\path to\file.pmlobj", 1234, "-n1234 \"C:\\path to\\file.pmlobj\"")]
        [TestCase(typeof(NotepadPlusPlusCodeEditor), "", @"C:\path to\file.pmlobj", 0, "\"C:\\path to\\file.pmlobj\"")]
        [TestCase(typeof(SublimeTextCodeEditor), "", @"C:\path\to\file.pmlobj", 12345, @"C:\path\to\file.pmlobj:12345")]
        [TestCase(typeof(SublimeTextCodeEditor), "", @"C:\path\to\file.pmlobj", 0, @"C:\path\to\file.pmlobj")]
        [TestCase(typeof(SublimeTextCodeEditor), "", @"C:\path to\file.pmlobj", 1234, "\"C:\\path to\\file.pmlobj:1234\"")]
        [TestCase(typeof(SublimeTextCodeEditor), "", @"C:\path to\file.pmlobj", 0, "\"C:\\path to\\file.pmlobj\"")]
        [TestCase(typeof(UltraEditCodeEditor), "", @"C:\path\to\file.pmlobj", 12345, @"C:\path\to\file.pmlobj -l12345")]
        [TestCase(typeof(UltraEditCodeEditor), "", @"C:\path\to\file.pmlobj", 0, @"C:\path\to\file.pmlobj")]
        [TestCase(typeof(UltraEditCodeEditor), "", @"C:\path to\file.pmlobj", 1234, "\"C:\\path to\\file.pmlobj\" -l1234")]
        [TestCase(typeof(UltraEditCodeEditor), "", @"C:\path to\file.pmlobj", 0, "\"C:\\path to\\file.pmlobj\"")]
        [TestCase(typeof(VisualStudioCodeCodeEditor), "", @"C:\path\to\file.pmlobj", 12345, @"--goto C:\path\to\file.pmlobj:12345")]
        [TestCase(typeof(VisualStudioCodeCodeEditor), "", @"C:\path\to\file.pmlobj", 0, @"C:\path\to\file.pmlobj")]
        [TestCase(typeof(VisualStudioCodeCodeEditor), "", @"C:\path to\file.pmlobj", 1234, "--goto \"C:\\path to\\file.pmlobj:1234\"")]
        [TestCase(typeof(VisualStudioCodeCodeEditor), "", @"C:\path to\file.pmlobj", 0, "\"C:\\path to\\file.pmlobj\"")]
        [TestCase(typeof(OtherCodeEditor), "!fileName !lineNumber", @"C:\path\to\file.pmlobj", 12345, @"C:\path\to\file.pmlobj 12345")]
        [TestCase(typeof(OtherCodeEditor), "!fileName !lineNumber", @"C:\path\to\file.pmlobj", 0, @"C:\path\to\file.pmlobj 0")]
        [TestCase(typeof(OtherCodeEditor), "!fileName !lineNumber", @"C:\path to\file.pmlobj", 1234, "\"C:\\path to\\file.pmlobj\" 1234")]
        [TestCase(typeof(OtherCodeEditor), "!fileName !lineNumber", @"C:\path to\file.pmlobj", 0, "\"C:\\path to\\file.pmlobj\" 0")]
        [TestCase(typeof(OtherCodeEditor), "\"!fileName\" !lineNumber", @"C:\path\to\file.pmlobj", 12345, @"C:\path\to\file.pmlobj 12345")]
        [TestCase(typeof(OtherCodeEditor), "\"!fileName\" !lineNumber", @"C:\path\to\file.pmlobj", 0, @"C:\path\to\file.pmlobj 0")]
        [TestCase(typeof(OtherCodeEditor), "\"!fileName\" !lineNumber", @"C:\path to\file.pmlobj", 1234, "\"C:\\path to\\file.pmlobj\" 1234")]
        [TestCase(typeof(OtherCodeEditor), "\"!fileName\" !lineNumber", @"C:\path to\file.pmlobj", 0, "\"C:\\path to\\file.pmlobj\" 0")]
        public void BuildsProcessStartInfoCorrectly(Type editorType, string fixedArguments, string fileName, int lineNumber, string expectedArguments)
        {
            var constructor = editorType.GetConstructor(new Type[] { typeof(string), typeof(string) });
            var editor = constructor.Invoke(new object[] { @"C:\Program Files\Editor\Editor.exe", fixedArguments });
            var method = editorType.GetMethod("BuildStartInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            var startInfo = method.Invoke(editor, new object[] { fileName, lineNumber }) as ProcessStartInfo;
            Assert.That(startInfo.FileName, Is.EqualTo(@"C:\Program Files\Editor\Editor.exe"));
            Assert.That(startInfo.Arguments, Is.EqualTo(expectedArguments));
            Assert.That(startInfo.UseShellExecute, Is.False);
        }
    }

    class StubCodeEditor : BaseCodeEditor
    {
        private readonly IEnumerable<string> ExtraArguments;

        public StubCodeEditor(string fileName, string fixedArguments, IEnumerable<string> extraArguments)
            : base(fileName, fixedArguments)
        {
            ExtraArguments = extraArguments.ToList();
        }

        public string Arguments
        {
            get { return BuildStartInfo("foo", 1).Arguments; }
        }

        protected override IEnumerable<string> GetExtraArguments(string fileName, int lineNumber)
        {
            return ExtraArguments;
        }
    }
}
