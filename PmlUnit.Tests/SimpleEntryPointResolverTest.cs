// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(SimpleEntryPointResolver))]
    public class SimpleEntryPointResolverTest
    {
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
        public void ResolvesBasicEntryPoints(string entryPoint, EntryPointKind expectedKind, string expectedName)
        {
            var resolver = new SimpleEntryPointResolver();
            var result = resolver.Resolve(entryPoint, 0);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Kind, Is.EqualTo(expectedKind));
            Assert.That(result.Name, Is.EqualTo(expectedName));
        }
    }
}
