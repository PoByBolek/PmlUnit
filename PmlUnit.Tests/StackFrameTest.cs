// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Globalization;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(StackFrame))]
    public class StackFrameTest
    {
        [Test]
        public void ValidatesNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new StackFrame(null, null));
            Assert.Throws<ArgumentNullException>(() => new StackFrame(null, ""));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("", null));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("", ""));

            Assert.Throws<ArgumentNullException>(() => new StackFrame(null, "foo"));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("", "foo"));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("foo", null));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("foo", ""));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(45)]
        [TestCase(4123)]
        [TestCase(int.MaxValue)]
        public void ParsesLineNumber(int lineNumber)
        {
            var inLine = string.Format(CultureInfo.InvariantCulture, "In line {0} of PML function foo", lineNumber);
            var frame = new StackFrame(inLine, "!!foo()");
            Assert.That(frame.LineNumber, Is.EqualTo(lineNumber));

            var calledFrom = string.Format(CultureInfo.InvariantCulture, "Called from line {0} of PML function foo", lineNumber);
            frame = new StackFrame(calledFrom, "!!foo()");
            Assert.That(frame.LineNumber, Is.EqualTo(lineNumber));
        }

        [TestCase("!!foo()", 0)]
        [TestCase("!!foo(^)", 0)]
        [TestCase("!!foo(^ ^)", 0)]
        [TestCase("!!foo(^^)", 6)]
        [TestCase("!!foo(^^^^)", 6)]
        [TestCase("!!^^foo()", 2)]
        public void ParsesColumnNumber(string callSite, int expectedColumn)
        {
            var frame = new StackFrame("Called from line 1 of PML function foo", callSite);
            Assert.That(frame.ColumnNumber, Is.EqualTo(expectedColumn));
        }

        [TestCase("!!foo()", "!!foo()")]
        [TestCase("!!foo(^)", "!!foo(^)")]
        [TestCase("!!foo(^ ^)", "!!foo(^ ^)")]
        [TestCase("!!foo(^^)", "!!foo()")]
        [TestCase("!!foo(^^^^)", "!!foo(^^)")]
        [TestCase("!!^^foo()", "!!foo()")]
        [TestCase("  !!bar()  ", "  !!bar()  ")]
        [TestCase(" random stu^^ff ", " random stuff ")]
        public void RemovesCaretFromCallSite(string callSiteArg, string expectedCallSite)
        {
            var frame = new StackFrame("Called from line 1 of PML function foo", callSiteArg);
            Assert.That(frame.CallSite, Is.EqualTo(expectedCallSite));
        }

        [TestCase("PML function foo", EntryPointKind.Function, "foo")]
        [TestCase("pml function foo", EntryPointKind.Function, "foo")]
        [TestCase("PML FUNCTION foo", EntryPointKind.Function, "foo")]
        [TestCase("pml FUNCTION foo", EntryPointKind.Function, "foo")]
        [TestCase("PML function foo.BAR", EntryPointKind.Method, "foo.BAR")]
        [TestCase("pml function foo.BAR", EntryPointKind.Method, "foo.BAR")]
        [TestCase("pml FUNCTION foo.BAR", EntryPointKind.Method, "foo.BAR")]
        [TestCase("PML FUNCTION foo.BAR", EntryPointKind.Method, "foo.BAR")]
        [TestCase("Macro C:\\foo\\bar\\macro.pmlmac", EntryPointKind.Macro, "C:\\foo\\bar\\macro.pmlmac")]
        [TestCase("macro C:\\foo\\bar\\macro.pmlmac", EntryPointKind.Macro, "C:\\foo\\bar\\macro.pmlmac")]
        [TestCase("MACRO C:\\foo\\bar\\macro.pmlmac", EntryPointKind.Macro, "C:\\foo\\bar\\macro.pmlmac")]
        [TestCase("MaCrO C:\\foo\\bar\\macro.pmlmac", EntryPointKind.Macro, "C:\\foo\\bar\\macro.pmlmac")]
        [TestCase("PML functionality test", EntryPointKind.Unknown, "PML functionality test")]
        [TestCase("Macroscopic command", EntryPointKind.Unknown, "Macroscopic command")]
        [TestCase("something else", EntryPointKind.Unknown, "something else")]
        public void DeterminesEntryPoint(string entryPoint, EntryPointKind expectedKind, string expectedName)
        {
            var frame = new StackFrame("In line 123 of " + entryPoint, "!!foo()");
            Assert.That(frame.EntryPoint.Kind, Is.EqualTo(expectedKind));
            Assert.That(frame.EntryPoint.Name, Is.EqualTo(expectedName));

            frame = new StackFrame("Called from line 123 of " + entryPoint, "!!foo()");
            Assert.That(frame.EntryPoint.Kind, Is.EqualTo(expectedKind));
            Assert.That(frame.EntryPoint.Name, Is.EqualTo(expectedName));
        }
    }
}
