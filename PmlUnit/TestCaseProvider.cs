﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PmlUnit
{
    interface TestCaseProvider
    {
        ICollection<TestCase> GetTestCases();
    }

    class EnvironmentVariableTestCaseProvider : TestCaseProvider
    {
        private readonly string[] Paths;

        public EnvironmentVariableTestCaseProvider()
            : this("PMLLIB")
        {
        }

        public EnvironmentVariableTestCaseProvider(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                throw new ArgumentNullException(nameof(variableName));

            var pmllib = Environment.GetEnvironmentVariable(variableName);
            if (string.IsNullOrEmpty(pmllib))
                Paths = new string[0];
            else if (pmllib.IndexOf(Path.PathSeparator) >= 0)
                Paths = pmllib.Split(Path.PathSeparator);
            else if (Directory.Exists(pmllib))
                Paths = new string[] { pmllib };
            else
                Paths = pmllib.Split(' ');
        }

        public ICollection<TestCase> GetTestCases()
        {
            var result = new List<TestCase>();

            foreach (string path in Paths)
            {
                if (!Path.IsPathRooted(path))
                    continue;
                
                TestCaseProvider provider;
                try
                {
                    provider = new IndexFileTestCaseProvider(Path.Combine(path, "pml.index"));
                }
                catch (FileNotFoundException)
                {
                    continue;
                }

                result.AddRange(provider.GetTestCases());
            }

            return result;
        }
    }

    class IndexFileTestCaseProvider : TestCaseProvider
    {
        private readonly TestCaseParser Parser;
        private readonly HashSet<string> PotentialTestCaseNames;

        public IndexFileTestCaseProvider(string indexFilePath)
            : this(indexFilePath, new SimpleTestCaseParser())
        {
        }

        public IndexFileTestCaseProvider(string indexFilePath, TestCaseParser parser)
        {
            if (string.IsNullOrEmpty(indexFilePath))
                throw new ArgumentNullException(nameof(indexFilePath));
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            string basePath = Path.GetDirectoryName(indexFilePath);

            using (var reader = new StreamReader(indexFilePath, Encoding.UTF8))
            {
                Parser = parser;
                PotentialTestCaseNames = FindPotentialTestCases(reader, basePath);
            }
        }

        public IndexFileTestCaseProvider(TextReader reader, string basePath)
            : this(reader, basePath, new SimpleTestCaseParser())
        {
        }

        public IndexFileTestCaseProvider(TextReader reader, string basePath, TestCaseParser parser)
        {
            if (string.IsNullOrEmpty(basePath))
                throw new ArgumentNullException(nameof(basePath));
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            
            Parser = parser;
            PotentialTestCaseNames = FindPotentialTestCases(reader, basePath);
        }

        private static HashSet<string> FindPotentialTestCases(TextReader reader, string basePath)
        {
            var result = new HashSet<string>();
            string directory = null;

            foreach (string line in reader.ReadAllLines())
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("/"))
                {
                    // TODO: what happens when PDMS encounters "/../../" ?
                    directory = Path.Combine(basePath, trimmed.Substring(1));
                    directory = directory.Replace('/', Path.DirectorySeparatorChar);
                }
                else if (directory != null && trimmed.EndsWith("test.pmlobj", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(Path.Combine(directory, trimmed));
                }
            }

            return result;
        }

        public ICollection<TestCase> GetTestCases()
        {
            var result = new List<TestCase>();
            foreach (string path in PotentialTestCaseNames)
            {
                try
                {
                    result.Add(Parser.Parse(path));
                }
                catch (ParserException)
                {
                }
                catch (FileNotFoundException)
                {
                }
            }
            return result;
        }
    }
}
