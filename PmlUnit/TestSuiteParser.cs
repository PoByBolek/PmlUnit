using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    else if (IsSetupMethod(signature))
                        result.HasSetUpMethod = true;
                    else if (IsTearDownMethod(signature))
                        result.HasTearDownMethod = true;
                }
            }

            if (result == null)
                throw new ParserException();
            else
                return result;
        }

        private static bool IsTestCaseMethod(string signature, out string testCaseName)
        {
            string[] argumentTypes;
            testCaseName = ParseSignature(signature, out argumentTypes);
            if (!testCaseName.StartsWith("test", StringComparison.OrdinalIgnoreCase))
                return false;
            if (argumentTypes.Length != 1)
                return false;

            return argumentTypes[0].Equals("PmlAssert", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSetupMethod(string signature)
        {
            string[] argumentTypes;
            var methodName = ParseSignature(signature, out argumentTypes);
            return methodName.Equals("setUp", StringComparison.OrdinalIgnoreCase)
                && argumentTypes.Length == 0;
        }

        private static bool IsTearDownMethod(string signature)
        {
            string[] argumentTypes;
            var methodName = ParseSignature(signature, out argumentTypes);
            return methodName.Equals("tearDown", StringComparison.OrdinalIgnoreCase)
                && argumentTypes.Length == 0;
        }

        private static string ParseSignature(string signature, out string[] argumentTypes)
        {
            var startIndex = signature.IndexOf('(');
            var endIndex = signature.IndexOf(')');
            if (startIndex < 0 || endIndex < 0 || startIndex > endIndex)
                throw new ParserException();

            var methodName = signature.Substring(0, startIndex).Trim();
            var argumentString = signature
                .Substring(startIndex + 1, endIndex - startIndex - 1)
                .Trim();
            if (string.IsNullOrEmpty(argumentString))
            {
                argumentTypes = new string[0];
                return methodName;
            }

            argumentTypes = argumentString.Split(',').Select(
                arg => ParseArgumentType(arg)
            ).ToArray();
            return methodName;
        }

        private static string ParseArgumentType(string argument)
        {
            var isIndex = argument.IndexOf(" is ", StringComparison.OrdinalIgnoreCase);
            if (isIndex < 0)
                throw new ParserException();
            return argument.Substring(isIndex + 4).Trim();
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
