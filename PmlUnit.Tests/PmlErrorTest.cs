// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(PmlError))]
    public class PmlErrorTest
    {
        [Test]
        public void EmptyHashTableReturnsNoError()
        {
            Assert.That(PmlError.FromHashTable(null), Is.Null);
            Assert.That(PmlError.FromHashTable(new Hashtable()), Is.Null);
        }

        [Test]
        public void EmptyListReturnsNoError()
        {
            Assert.That(PmlError.FromList(null), Is.Null);
            Assert.That(PmlError.FromList(new List<string>()), Is.Null);
        }

        [Test]
        public void FromHashTable_ReconstructsStackFrames()
        {
            var error = Parse(
                "(44,33)   FNF:File not found",
                @"In line 44 of Macro C:\Aveva\Plant\E3D21~1.0\PMLLIB\common\functions\runsynonym.pmlmac",
                "^^$M \"/%PMLUI%/CLIB/FILES/UELEMSEL\" =1/1",
                "Called from line 34 of PML function runsynonym",
                "  $m \"$!<macro>\" $<$!<action>$>",
                "Called from line 62 of PML function pmlasserttest.TESTEQUALWITHUNEQUALVALUES",
                "    !!runSynonym('CALLIB UELEMSEL =1/1')",
                "Called from line 53 of PML function pmltestrunner.RUNINTERNAL",
                "    !testCase.$!<testName>(object PmlAssert())",
                "Called from line 37 of PML function pmltestrunner.RUN",
                "    !this.runInternal(!testCaseName, !testName, !hasSetup, !hasTearDown)"
            );
            Assert.That(error.Message, Is.EqualTo("(44,33)   FNF:File not found"));
            Assert.That(error.StackTrace, Is.EquivalentTo(new List<StackFrame>()
            {
                new StackFrame(
                    @"In line 44 of Macro C:\Aveva\Plant\E3D21~1.0\PMLLIB\common\functions\runsynonym.pmlmac",
                    "^^$M \"/%PMLUI%/CLIB/FILES/UELEMSEL\" =1/1"
                ),
                new StackFrame(
                    "Called from line 34 of PML function runsynonym",
                    "  $m \"$!<macro>\" $<$!<action>$>"
                ),
                new StackFrame(
                    "Called from line 62 of PML function pmlasserttest.TESTEQUALWITHUNEQUALVALUES",
                    "    !!runSynonym('CALLIB UELEMSEL =1/1')"
                ),
                new StackFrame(
                    "Called from line 53 of PML function pmltestrunner.RUNINTERNAL",
                    "    !testCase.$!<testName>(object PmlAssert())"
                ),
                new StackFrame(
                    "Called from line 37 of PML function pmltestrunner.RUN",
                    "    !this.runInternal(!testCaseName, !testName, !hasSetup, !hasTearDown)"
                )
            }));
        }

        [Test]
        public void FromHashTable_IgnoresMissingStackTrace()
        {
            var error = Parse(
                "(61,123)   FM: Form FOOBAR not found",
                " *** Error Line not available",
                " *** Error Command not available"
            );
            Assert.That(error.Message, Is.EqualTo("(61,123)   FM: Form FOOBAR not found"));
            Assert.That(error.StackTrace, Is.Empty);
        }

        private static PmlError Parse(params string[] lines)
        {
            var result = new Hashtable(lines.Length);
            for (int i = 0; i < lines.Length; i++)
                result[(double)(i + 1)] = lines[i];
            return PmlError.FromHashTable(result);
        }
    }
}
