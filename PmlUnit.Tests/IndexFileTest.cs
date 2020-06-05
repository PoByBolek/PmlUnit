// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(IndexFile))]
    public class IndexFileTest
    {
        [Test]
        public void ValidatesNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new IndexFile(null));
            Assert.Throws<ArgumentNullException>(() => new IndexFile(""));

            Assert.Throws<ArgumentNullException>(() => new IndexFile(null, null));
            Assert.Throws<ArgumentNullException>(() => new IndexFile("", null));
            Assert.Throws<ArgumentNullException>(() => new IndexFile(null, TextReader.Null));
            Assert.Throws<ArgumentNullException>(() => new IndexFile("", TextReader.Null));
        }

        [Test]
        public void ParseSingleDirectory()
        {
            var contents = @"
/path/to/tests/
random.pmlobj
stuff.pmlfrm
foo.bar";
            IndexFile index;
            using (var reader = new StringReader(contents))
            {
                index = new IndexFile(@"C:\testing\pml.index", reader);
            }

            Assert.That(index.Files, Is.EquivalentTo(new List<string>
            {
                @"C:\testing\path\to\tests\random.pmlobj",
                @"C:\testing\path\to\tests\stuff.pmlfrm",
                @"C:\testing\path\to\tests\foo.bar",
            }));
        }

        [Test]
        public void ParseMultipleDirectories()
        {
            var contents = @"
ignored.pmlfnc

/first/
simple.pmlmac

/first/nested/
one.pmlcmd
two.pmlfnc

/second/
three.png
override.pmlobj

/second/nested/
OVERRIDE.pmlobj";
            IndexFile index;
            using (var reader = new StringReader(contents))
            {
                index = new IndexFile(@"C:\testing\pml.index", reader);
            }

            Assert.That(index.Files, Is.EquivalentTo(new List<string>
            {
                @"C:\testing\first\simple.pmlmac",
                @"C:\testing\first\nested\one.pmlcmd",
                @"C:\testing\first\nested\two.pmlfnc",
                @"C:\testing\second\three.png",
                @"C:\testing\second\nested\OVERRIDE.pmlobj",
            }));
        }
    }
}
