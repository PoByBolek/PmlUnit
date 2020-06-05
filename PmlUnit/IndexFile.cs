// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PmlUnit
{
    class IndexFile
    {
        public const string DefaultName = "pml.index";

        public static IndexFile FromEnvironmentVariable()
        {
            return FromEnvironmentVariable("PMLLIB");
        }

        public static IndexFile FromEnvironmentVariable(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                throw new ArgumentNullException(nameof(variableName));

            var result = new IndexFile();
            var pmllib = Environment.GetEnvironmentVariable(variableName);
            if (string.IsNullOrEmpty(pmllib))
                return result;

            string[] paths;
            if (pmllib.IndexOf(Path.PathSeparator) >= 0)
                paths = pmllib.Split(Path.PathSeparator);
            else if (Directory.Exists(pmllib))
                paths = new string[] { pmllib };
            else
                paths = pmllib.Split(' ');

            foreach (var path in paths)
            {
                if (!Path.IsPathRooted(path))
                    continue;

                IndexFile index;
                try
                {
                    index = FromDirectory(path);
                }
                catch (FileNotFoundException)
                {
                    continue;
                }

                foreach (var fileName in index.Files)
                    result.Files[Path.GetFileName(fileName)] = fileName;
            }

            return result;
        }

        public static IndexFile FromDirectory(string directoryName)
        {
            if (string.IsNullOrEmpty(directoryName))
                throw new ArgumentNullException(nameof(directoryName));

            return new IndexFile(Path.Combine(directoryName, DefaultName));
        }

        public IndexedFileCollection Files { get; }

        public IndexFile()
        {
            Files = new IndexedFileCollection();
        }

        public IndexFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            fileName = Path.GetFullPath(fileName);
            using (var reader = new StreamReader(fileName, Encoding.UTF8))
            {
                Files = ParseFile(fileName, reader);
            }
        }

        public IndexFile(string fileName, TextReader reader)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            fileName = Path.GetFullPath(fileName);
            Files = ParseFile(fileName, reader);
        }

        private static IndexedFileCollection ParseFile(string fileName, TextReader reader)
        {
            var result = new IndexedFileCollection();
            string baseDirectory = Path.GetDirectoryName(Path.GetFullPath(fileName));
            string directory = null;
            
            foreach (string line in reader.ReadAllLines().Select(line => line.Trim()))
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                else if (line.StartsWith("/", StringComparison.Ordinal))
                {
                    // TODO: what happens when PDMS encounters "/../../" ?
                    directory = Path.Combine(baseDirectory, line.Substring(1));
                    directory = directory.Replace('/', Path.DirectorySeparatorChar);
                }
                else if (directory != null)
                {
                    result[line] = Path.Combine(directory, line);
                }
            }

            return result;
        }
    }

    class IndexedFileCollection : ICollection<string>, IDictionary<string, string>
    {
        public int Count => Files.Count;

        public bool IsReadOnly => false;

        private readonly Dictionary<string, string> Files;

        public IndexedFileCollection()
        {
            Files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));
                return Files[key];
            }

            set
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(value));
                Files[key] = value;
            }
        }

        ICollection<string> IDictionary<string, string>.Keys => Files.Keys;

        ICollection<string> IDictionary<string, string>.Values => Files.Values;

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            if (string.IsNullOrEmpty(item.Key))
                throw new ArgumentException("Key must not be null or empty", nameof(item));
            if (string.IsNullOrEmpty(item.Value))
                throw new ArgumentException("Value must not be null or empty", nameof(item));
            Files.Add(item.Key, item.Value);
        }

        public void Add(string item)
        {
            if (string.IsNullOrEmpty(item))
                throw new ArgumentNullException(nameof(item));

            Files.Add(Path.GetFileName(item), item);
        }

        public void Add(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            Files.Add(key, value);
        }

        public void Clear()
        {
            Files.Clear();
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            return ContainsPair(item.Key, item.Value);
        }

        public bool Contains(string item)
        {
            if (string.IsNullOrEmpty(item))
                return false;

            string key = Path.GetFileName(item);
            if (key == item)
                return Files.ContainsKey(key);
            else
                return ContainsPair(key, item);
        }

        bool IDictionary<string, string>.ContainsKey(string key)
        {
            return Files.ContainsKey(key);
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index + Count >= array.Length)
                throw new ArgumentException("Array cannot hold all elements in the collection.");

            foreach (var pair in Files)
                array[index++] = pair;
        }

        public void CopyTo(string[] array, int index)
        {
            Files.Values.CopyTo(array, index);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Files.Values.GetEnumerator();
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            if (ContainsPair(item.Key, item.Value))
                return Files.Remove(item.Key);
            else
                return false;
        }

        public bool Remove(string item)
        {
            if (string.IsNullOrEmpty(item))
                return false;

            string key = Path.GetFileName(item);
            if (key == item)
                return Files.Remove(key);
            else if (ContainsPair(key, item))
                return Files.Remove(key);
            else
                return false;
        }

        private bool ContainsPair(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return false;

            string storedValue;
            return Files.TryGetValue(key, out storedValue)
                && value.Equals(storedValue, StringComparison.OrdinalIgnoreCase);
        }

        public bool TryGetValue(string key, out string value)
        {
            bool result = Files.TryGetValue(key, out value);
            if (!result)
                value = "";
            return result;
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return Files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
