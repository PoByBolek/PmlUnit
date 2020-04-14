﻿// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.Collections;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(PmlError))]
    public class PmlErrorTest
    {
        private List<string> Lines;
        private List<string> MissingStackTrace;
        private List<StackFrame> ExpectedStackTrace;

        [SetUp]
        public void Setup()
        {
            Lines = new List<string>()
            {
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
            };
            MissingStackTrace = new List<string>()
            {
                "(61,123)   FM: Form FOOBAR not found",
                " *** Error Line not available",
                " *** Error Command not available"
            };
            ExpectedStackTrace = new List<StackFrame>()
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
            };
        }

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
        public void ParseFromHashTable()
        {
            var message = Lines[0];
            var error = PmlError.FromHashTable(ToHashTable(Lines));
            Assert.That(error.Message, Is.EqualTo(message));
            Assert.That(error.StackTrace, Is.EquivalentTo(ExpectedStackTrace));

            message = MissingStackTrace[0];
            error = PmlError.FromHashTable(ToHashTable(MissingStackTrace));
            Assert.That(error.Message, Is.EqualTo(message));
            Assert.That(error.StackTrace, Is.Empty);
        }

        [Test]
        public void ParseFromList()
        {
            var message = Lines[0];
            var error = PmlError.FromList(Lines);
            Assert.That(error.Message, Is.EqualTo(message));
            Assert.That(error.StackTrace, Is.EquivalentTo(ExpectedStackTrace));

            message = MissingStackTrace[0];
            error = PmlError.FromList(MissingStackTrace);
            Assert.That(error.Message, Is.EqualTo(message));
            Assert.That(error.StackTrace, Is.Empty);
        }

        [Test]
        public void ParseFromString()
        {
            var message = Lines[0];
            var error = PmlError.FromString(ToString(Lines));
            Assert.That(error.Message, Is.EqualTo(message));
            Assert.That(error.StackTrace, Is.EquivalentTo(ExpectedStackTrace));

            message = MissingStackTrace[0];
            error = PmlError.FromString(ToString(MissingStackTrace));
            Assert.That(error.Message, Is.EqualTo(message));
            Assert.That(error.StackTrace, Is.Empty);
        }

        private static Hashtable ToHashTable(IEnumerable<string> lines)
        {
            var result = new Hashtable();
            double key = 1.0;
            foreach (var line in lines)
                result[key++] = line;
            return result;
        }

        private static string ToString(IEnumerable<string> lines)
        {
            var builder = new StringBuilder();
            foreach (var line in lines)
                builder.AppendLine(line);
            return builder.ToString();
        }
    }
}
