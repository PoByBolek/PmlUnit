// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PmlUnit
{
    interface TestCaseParser
    {
        TestCase Parse(string fileName);
        TestCase Parse(string fileName, TextReader reader);
    }

    class SimpleTestCaseParser : TestCaseParser
    {
        private static readonly Regex WhitespaceRegex = new Regex(@"\s+");

        public TestCase Parse(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            using (var reader = new StreamReader(fileName, Encoding.UTF8))
            {
                return Parse(fileName, reader);
            }
        }

        public TestCase Parse(string fileName, TextReader reader)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            fileName = Path.GetFullPath(fileName);
            bool inComment = false;
            TestCase result = null;
            int lineNumber = 0;

            foreach (string line in reader.ReadAllLines())
            {
                ++lineNumber;
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
                    result = new TestCase(sanitized.Substring(14), fileName);
                }
                else if (sanitized.StartsWith("define method .", StringComparison.OrdinalIgnoreCase))
                {
                    if (result == null)
                        throw new ParserException();
                    var signature = sanitized.Substring(15);
                    string testCaseName;
                    if (IsTestCaseMethod(signature, out testCaseName))
                        result.Tests.Add(testCaseName, lineNumber);
                    else if (IsSetupMethod(signature))
                        result.HasSetUp = true;
                    else if (IsTearDownMethod(signature))
                        result.HasTearDown = true;
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
    }
}
