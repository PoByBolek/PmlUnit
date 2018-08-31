using System;
using System.Collections.Generic;
using System.IO;

namespace PmlUnit
{
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
}
