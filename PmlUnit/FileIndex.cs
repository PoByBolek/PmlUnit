// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PmlUnit
{
    class FileIndex : IEnumerable<string>
    {
        private readonly List<IndexFile> Indices;

        public FileIndex()
            : this("PMLLIB")
        {
        }

        public FileIndex(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                throw new ArgumentNullException(nameof(variableName));

            Indices = new List<IndexFile>();

            foreach (var path in GetSearchPath(Environment.GetEnvironmentVariable(variableName)))
            {
                if (!Path.IsPathRooted(path))
                    continue;
                
                try
                {
                    Indices.Add(IndexFile.FromDirectory(path));
                }
                catch (IOException)
                {
                    continue;
                }
                catch (NotSupportedException)
                {
                    continue;
                }
            }
        }

        public FileIndex(IndexFile index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            Indices = new List<IndexFile>() { index };
        }

        public FileIndex(IEnumerable<IndexFile> indices)
        {
            if (indices == null)
                throw new ArgumentNullException(nameof(indices));

            Indices = indices.ToList();
            foreach (var index in Indices)
            {
                if (index == null)
                    throw new ArgumentException("Collection must not contain null values.", nameof(indices));
            }
        }

        private static string[] GetSearchPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return new string[0];
            
            if (path.IndexOf(Path.PathSeparator) >= 0)
                return path.Split(Path.PathSeparator);
            else if (Directory.Exists(path))
                return new string[] { path };
            else
                return path.Split(' ');
        }

        public bool TryGetFile(string fileName, out string fullFileName)
        {
            foreach (var index in Indices)
            {
                if (index.Files.TryGetValue(fileName, out fullFileName))
                    return true;
            }

            fullFileName = null;
            return false;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new Enumerator(Indices);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<string>
        {
            private readonly IEnumerator<IndexFile> IndexEnumerator;
            private readonly HashSet<string> SeenFiles;
            private IEnumerator<string> FileEnumerator;

            public Enumerator(IEnumerable<IndexFile> indices)
            {
                if (indices == null)
                    throw new ArgumentNullException(nameof(indices));

                IndexEnumerator = indices.GetEnumerator();
                SeenFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                FileEnumerator = null;
            }

            ~Enumerator()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    IndexEnumerator.Dispose();
                    if (FileEnumerator != null)
                    {
                        FileEnumerator.Dispose();
                        FileEnumerator = null;
                    }
                }
            }

            public string Current
            {
                get
                {
                    if (FileEnumerator == null)
                        return null;
                    else
                        return FileEnumerator.Current;
                }
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                while (true)
                {
                    if (FileEnumerator == null)
                    {
                        if (IndexEnumerator.MoveNext())
                            FileEnumerator = IndexEnumerator.Current.Files.GetEnumerator();
                        else
                            return false;
                    }

                    while (FileEnumerator.MoveNext())
                    {
                        string fileName = Path.GetFileName(FileEnumerator.Current);
                        if (SeenFiles.Add(fileName))
                            return true;
                    }
                    FileEnumerator.Dispose();
                    FileEnumerator = null;
                }
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
    }
}
