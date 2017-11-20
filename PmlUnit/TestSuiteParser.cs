using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PmlUnit
{
    class TestSuiteParser
    {
        private static readonly Regex WhitespaceRegex = new Regex(@"\s+");

        public TestSuite Parse(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            bool inComment = false;
            TestSuite result = null;

            foreach (string line in ReadAllLines(reader))
            {
                string sanitized = WhitespaceRegex.Replace(line, " ").Trim();
                if (inComment)
                {
                    if (line.IndexOf("$)", StringComparison.Ordinal) >= 0)
                        inComment = false;
                    continue;
                }
                else if (sanitized.StartsWith("$(", StringComparison.Ordinal))
                {
                    inComment = true;
                    continue;
                }
                else if (sanitized.StartsWith("define object ", StringComparison.OrdinalIgnoreCase))
                {
                    if (result != null)
                        throw new ParserException();
                    result = new TestSuite(sanitized.Substring(14));
                }
                else if (sanitized.StartsWith("define method .", StringComparison.OrdinalIgnoreCase))
                {
                    if (result == null)
                        throw new ParserException();
                    var signature = sanitized.Substring(15);
                    string testCaseName;
                    if (IsTestCaseMethod(signature, out testCaseName))
                        result.TestCases.Add(new TestCase(testCaseName));
                }
            }

            if (result == null)
                throw new ParserException();
            else
                return result;
        }

        private static bool IsTestCaseMethod(string signature, out string testCaseName)
        {
            var startIndex = signature.IndexOf('(');
            var endIndex = signature.IndexOf(')');
            if (startIndex < 0 || endIndex < 0 || startIndex > endIndex)
                throw new ParserException();

            testCaseName = signature.Substring(0, startIndex);
            if (!testCaseName.StartsWith("test", StringComparison.OrdinalIgnoreCase))
                return false;

            var parameterString = signature
                .Substring(startIndex + 1, endIndex - startIndex - 1)
                .Trim();
            if (string.IsNullOrEmpty(parameterString))
                return false;

            var parameters = parameterString.Split(',');
            if (parameters.Length != 1)
                return false;

            var isIndex = parameters[0].IndexOf(" is ", StringComparison.OrdinalIgnoreCase);
            if (isIndex < 0)
                throw new ParserException();
            var parameterType = parameters[0].Substring(isIndex + 4).Trim();
            return parameterType.Equals("PmlAssert", StringComparison.OrdinalIgnoreCase);
        }

        private static IEnumerable<string> ReadAllLines(TextReader reader)
        {
            string line = reader.ReadLine();
            while (line != null)
            {
                yield return line;
                line = reader.ReadLine();
            }
        }
    }
}
