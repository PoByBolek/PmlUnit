// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.IO;

namespace PmlUnit
{
    class FileIndexTestCaseProvider : TestCaseProvider
    {
        private readonly FileIndex Index;
        private readonly TestCaseParser Parser;

        public FileIndexTestCaseProvider()
            : this(new FileIndex())
        {
        }
        
        public FileIndexTestCaseProvider(FileIndex index)
            : this(index, new SimpleTestCaseParser())
        {
        }

        public FileIndexTestCaseProvider(FileIndex index, TestCaseParser parser)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            Index = index;
            Parser = parser;
        }

        public ICollection<TestCase> GetTestCases()
        {
            var result = new List<TestCase>();
            foreach (string path in Index)
            {
                if (path.EndsWith("test.pmlobj", StringComparison.OrdinalIgnoreCase))
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
            }
            return result;
        }
    }
}
