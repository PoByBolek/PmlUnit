// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(FileIndex))]
    public class FileIndexTest
    {
        [Test]
        public void ValidatesNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new FileIndex((string)null));
            Assert.Throws<ArgumentNullException>(() => new FileIndex(""));

            Assert.Throws<ArgumentNullException>(() => new FileIndex((IndexFile)null));
            Assert.Throws<ArgumentNullException>(() => new FileIndex((IEnumerable<IndexFile>)null));
            Assert.Throws<ArgumentException>(() => new FileIndex(new List<IndexFile>() { null }));
        }

        [Test]
        public void EnumeratesFilesFromOneIndex()
        {
            var indexFile = new IndexFile();
            indexFile.Files.Add(@"C:\pmllib\foo.pmlobj");
            indexFile.Files.Add(@"C:\pmllib\bar.pmlfrm");
            var index = new FileIndex(indexFile);

            Assert.That(index, Is.EquivalentTo(indexFile.Files));
        }

        [Test]
        public void EnumeratesFilesFromMultipleIndices()
        {
            var first = new IndexFile();
            first.Files.Add(@"C:\pmllib\first\duplicate.pmlobj");
            first.Files.Add(@"C:\pmllib\first\unique.pmlfnc");
            var second = new IndexFile();
            second.Files.Add(@"C:\pmllib\second\duplicate.pmlobj");
            second.Files.Add(@"C:\pmllib\second\stuff.txt");
            var index = new FileIndex(new List<IndexFile>() { first, second });

            Assert.That(index, Is.EquivalentTo(new List<string>()
            {
                @"C:\pmllib\first\duplicate.pmlobj",
                @"C:\pmllib\first\unique.pmlfnc",
                @"C:\pmllib\second\stuff.txt"
            }));
        }

        [Test]
        public void QueriesIndicesInOrder()
        {
            var first = new IndexFile();
            first.Files.Add(@"C:\pmllib\first\duplicate.pmlobj");
            first.Files.Add(@"C:\pmllib\first\unique.pmlfnc");
            var second = new IndexFile();
            second.Files.Add(@"C:\pmllib\second\duplicate.pmlobj");
            second.Files.Add(@"C:\pmllib\second\stuff.txt");
            var index = new FileIndex(new List<IndexFile>() { first, second });

            string result;
            Assert.That(index.TryGetFile("duplicate.pmlobj", out result));
            Assert.That(result, Is.EqualTo(@"C:\pmllib\first\duplicate.pmlobj"));
            Assert.That(index.TryGetFile("DUPLICATE.PMLOBJ", out result));
            Assert.That(result, Is.EqualTo(@"C:\pmllib\first\duplicate.pmlobj"));
            Assert.That(index.TryGetFile("DuPlIcAtE.PmLoBj", out result));
            Assert.That(result, Is.EqualTo(@"C:\pmllib\first\duplicate.pmlobj"));

            Assert.That(index.TryGetFile("stuff.txt", out result));
            Assert.That(result, Is.EqualTo(@"C:\pmllib\second\stuff.txt"));

            Assert.That(!index.TryGetFile("non-existent.pmlobj", out result));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void IgnoresMissingDirectories()
        {
            var guid = Guid.NewGuid();
            Environment.SetEnvironmentVariable("PML_UNIT_FILE_INDEX_TEST_1", "C:\\some\\path\\that\\does\\not\\exist\\" + guid);
            try
            {
                new FileIndex("PML_UNIT_FILE_INDEX_TEST_1");
            }
            catch (DirectoryNotFoundException)
            {
                Assert.Fail("FileIndex should handle DirectoryNotFoundExceptions");
            }
        }

        [Test]
        public void IgnoresMissingFiles()
        {
            var guid = Guid.NewGuid();
            Environment.SetEnvironmentVariable("PML_UNIT_FILE_INDEX_TEST_2", "C:\\Windows\\System32");
            try
            {
                new FileIndex("PML_UNIT_FILE_INDEX_TEST_2");
            }
            catch (FileNotFoundException)
            {
                Assert.Fail("FileIndex should handle FileNotFoundException");
            }
        }
    }
}
