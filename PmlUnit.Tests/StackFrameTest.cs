// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Globalization;

using Moq;

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
            Assert.Throws<ArgumentNullException>(() => new StackFrame(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new StackFrame(null, "", null));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("", null, null));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("", "", null));

            Assert.Throws<ArgumentNullException>(() => new StackFrame(null, null, Mock.Of<EntryPointResolver>()));
            Assert.Throws<ArgumentNullException>(() => new StackFrame(null, "", Mock.Of<EntryPointResolver>()));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("", null, Mock.Of<EntryPointResolver>()));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("", "", Mock.Of<EntryPointResolver>()));

            Assert.Throws<ArgumentNullException>(() => new StackFrame(null, "foo", null));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("", "foo", null));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("foo", null, null));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("foo", "", null));

            Assert.Throws<ArgumentNullException>(() => new StackFrame(null, "foo", Mock.Of<EntryPointResolver>()));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("", "foo", Mock.Of<EntryPointResolver>()));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("foo", null, Mock.Of<EntryPointResolver>()));
            Assert.Throws<ArgumentNullException>(() => new StackFrame("foo", "", Mock.Of<EntryPointResolver>()));

            Assert.Throws<ArgumentNullException>(() => new StackFrame("foo", "bar", null));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(45)]
        [TestCase(4123)]
        [TestCase(int.MaxValue)]
        public void ParsesLineNumber(int lineNumber)
        {
            var inLine = string.Format(CultureInfo.InvariantCulture, "In line {0} of PML function foo", lineNumber);
            var frame = new StackFrame(inLine, "!!foo()", Mock.Of<EntryPointResolver>());
            Assert.That(frame.LineNumber, Is.EqualTo(lineNumber));

            var calledFrom = string.Format(CultureInfo.InvariantCulture, "Called from line {0} of PML function foo", lineNumber);
            frame = new StackFrame(calledFrom, "!!foo()", Mock.Of<EntryPointResolver>());
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
            var frame = new StackFrame("Called from line 1 of PML function foo", callSite, Mock.Of<EntryPointResolver>());
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
            var frame = new StackFrame("Called from line 1 of PML function foo", callSiteArg, Mock.Of<EntryPointResolver>());
            Assert.That(frame.CallSite, Is.EqualTo(expectedCallSite));
        }

        [TestCase("PML function foo")]
        [TestCase("PML FUNCTION bar")]
        [TestCase("pml function foo.BAR")]
        [TestCase("PmL FuNcTiOn FoO.bAr")]
        [TestCase("Macro C:\\foo\\bar\\macro.pmlmac")]
        [TestCase("Something else")]
        public void PassesEntryPointToResolver(string entryPoint)
        {
            var result = new EntryPoint(EntryPointKind.Unknown, "foo");
            var mock = new Mock<EntryPointResolver>();
            mock.Setup(resolver => resolver.Resolve(entryPoint, 123)).Returns(result);
            var frame = new StackFrame("In line 123 of " + entryPoint, "!!foo()", mock.Object);
            mock.Verify();
            Assert.That(frame.EntryPoint, Is.SameAs(result));

            result = new EntryPoint(EntryPointKind.Unknown, "bar");
            mock = new Mock<EntryPointResolver>();
            mock.Setup(resolver => resolver.Resolve(entryPoint, 123)).Returns(result);
            frame = new StackFrame("Called from line 123 of " + entryPoint, "!!foo()", mock.Object);
            mock.Verify();
            Assert.That(frame.EntryPoint, Is.SameAs(result));
        }
    }
}
