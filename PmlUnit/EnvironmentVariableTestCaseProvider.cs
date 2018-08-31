using System;
using System.Collections.Generic;
using System.IO;

namespace PmlUnit
{
    class EnvironmentVariableTestCaseProvider : TestCaseProvider
    {
        private readonly Func<string, TestCaseProvider> Factory;
        private readonly string[] Paths;

        public EnvironmentVariableTestCaseProvider()
            : this("PMLLIB")
        {
        }

        public EnvironmentVariableTestCaseProvider(string variableName)
            : this(variableName, CreateTestCaseProvider)
        {
        }

        internal EnvironmentVariableTestCaseProvider(string variableName, Func<string, TestCaseProvider> factory)
        {
            if (string.IsNullOrEmpty(variableName))
                throw new ArgumentNullException(nameof(variableName));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            Factory = factory;
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

        private static TestCaseProvider CreateTestCaseProvider(string directoryName)
        {
            return new IndexFileTestCaseProvider(directoryName);
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
                    provider = Factory(path);
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
