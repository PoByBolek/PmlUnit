using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PmlUnit
{
    class IndexFileTestCaseProvider : TestCaseProvider
    {
        private readonly TestCaseParser Parser;
        private readonly HashSet<string> PotentialTestCaseNames;

        public IndexFileTestCaseProvider(string directoryName)
            : this(directoryName, new SimpleTestCaseParser())
        {
        }

        public IndexFileTestCaseProvider(string directoryName, TestCaseParser parser)
            : this(directoryName, CreateIndexFileReader(directoryName), parser, closeReader: true)
        {
        }

        public IndexFileTestCaseProvider(string directoryName, TextReader reader)
            : this(directoryName, reader, new SimpleTestCaseParser(), closeReader: false)
        {
        }

        public IndexFileTestCaseProvider(string directoryName, TextReader reader, TestCaseParser parser)
            : this(directoryName, reader, parser, closeReader: false)
        {
        }

        private IndexFileTestCaseProvider(string directoryName, TextReader reader, TestCaseParser parser, bool closeReader)
        {
            try
            {
                if (string.IsNullOrEmpty(directoryName))
                    throw new ArgumentNullException(nameof(directoryName));
                if (reader == null)
                    throw new ArgumentNullException(nameof(reader));
                if (parser == null)
                    throw new ArgumentNullException(nameof(parser));

                Parser = parser;
                PotentialTestCaseNames = FindPotentialTestCases(directoryName, reader);
            }
            finally
            {
                if (closeReader && reader != null)
                    reader.Close();
            }
        }

        private static TextReader CreateIndexFileReader(string directoryName)
        {
            if (string.IsNullOrEmpty(directoryName))
                throw new ArgumentNullException(nameof(directoryName));

            string path = Path.Combine(directoryName, "pml.index");
            return new StreamReader(path, Encoding.UTF8);
        }

        private static HashSet<string> FindPotentialTestCases(string directoryName, TextReader reader)
        {
            var result = new HashSet<string>();
            string directory = null;

            foreach (string line in reader.ReadAllLines())
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("/"))
                {
                    // TODO: what happens when PDMS encounters "/../../" ?
                    directory = Path.Combine(directoryName, trimmed.Substring(1));
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
