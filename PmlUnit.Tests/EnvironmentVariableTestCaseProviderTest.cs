// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Moq;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(EnvironmentVariableTestCaseProvider))]
    class EnvironmentVariableTestCaseProviderTest
    {
        [Test]
        public void GetTestCases_UsesSpecifiedEnvironmentVariable()
        {
            // Arrange
            var env = "PMLUNIT_TEST_VAR_1";
            var path = @"C:\path\to\pmllib";
            Environment.SetEnvironmentVariable(env, path);
            var recorder = new DirectoryNameRecorder();
            var provider = new EnvironmentVariableTestCaseProvider(env, recorder.Factory);
            // Act
            provider.GetTestCases();
            // Assert
            Assert.Contains(path, recorder.DirectoryNames);
        }

        [Test]
        public void GetTestCases_SplitsEnvironmentVariableBySpace()
        {
            // Arrange
            var env = "PMLUNIT_TEST_VAR_2";
            var paths = new string[] {
                @"C:\path\to\pmllib", @"C:\path\to\another\pmllib",
                @"D:\yet\another\path", @"D:\even\more\paths",
            };
            Environment.SetEnvironmentVariable(env, string.Join(" ", paths));
            var recorder = new DirectoryNameRecorder();
            var provider = new EnvironmentVariableTestCaseProvider(env, recorder.Factory);
            // Act
            provider.GetTestCases();
            // Assert
            Assert.AreEqual(paths.Length, recorder.DirectoryNames.Count);
            for (var i = 0; i < paths.Length; i++)
                Assert.AreEqual(paths[i], recorder.DirectoryNames[i]);
        }

        [Test]
        public void GetTestCases_SplitsEnvironmentVariableByPathSeparator()
        {
            // Arrange
            var env = "PMLUNIT_TEST_VAR_2";
            var paths = new string[] {
                @"C:\path\to\pmllib", @"C:\path\to\another\pmllib",
                @"D:\path with\spaces in it", @"D:\a path with even\more    spaces",
            };
            string sep = Path.PathSeparator.ToString();
            Environment.SetEnvironmentVariable(env, string.Join(sep, paths));
            var recorder = new DirectoryNameRecorder();
            var provider = new EnvironmentVariableTestCaseProvider(env, recorder.Factory);
            // Act
            provider.GetTestCases();
            // Assert
            Assert.AreEqual(paths.Length, recorder.DirectoryNames.Count);
            for (var i = 0; i < paths.Length; i++)
                Assert.AreEqual(paths[i], recorder.DirectoryNames[i]);
        }

        [Test]
        public void GetTestCases_OnlyUsesAbsolutePaths()
        {
            // Arrange
            var env = "PMLUNIT_TEST_VAR_3";
            var absolute = @"C:\an\absolute\path";
            var paths = new string[] {
                @"relative", @"another\relative\path",
                absolute, @"..\and\more\relative\paths"
            };
            Environment.SetEnvironmentVariable(env, string.Join(" ", paths));
            var recorder = new DirectoryNameRecorder();
            var provider = new EnvironmentVariableTestCaseProvider(env, recorder.Factory);
            // Act
            provider.GetTestCases();
            // Assert
            Assert.AreEqual(1, recorder.DirectoryNames.Count);
            Assert.Contains(absolute, recorder.DirectoryNames);
        }

        [Test]
        public void GetTestCases_SupportsMissingEnvironmentVariables()
        {
            // Arrange
            var env = "PMLUNIT_TEST_VAR_4";
            Environment.SetEnvironmentVariable(env, null);
            var recorder = new DirectoryNameRecorder();
            var provider = new EnvironmentVariableTestCaseProvider(env, recorder.Factory);
            // Act
            provider.GetTestCases();
            // Assert
            Assert.IsEmpty(recorder.DirectoryNames);
        }

        [Test]
        public void GetTestCases_IgnoresFileNotFoundExceptionsFromFactory()
        {
            // Arrange
            var env = "PMLUNIT_TEST_VAR_4";
            Environment.SetEnvironmentVariable(env, @"C:\");
            var provider = new EnvironmentVariableTestCaseProvider(
                env, delegate (string path) { throw new FileNotFoundException(); }
            );
            // Act
            provider.GetTestCases();
        }

        [Test]
        public void GetTestCases_DoesNotIgnoreOtherExceptions()
        {
            // Arrange
            var env = "PMLUNIT_TEST_VAR_5";
            Environment.SetEnvironmentVariable(env, @"C:\path\to\pmllib");
            var provider = new EnvironmentVariableTestCaseProvider(
                env, delegate (string path) { throw new ArgumentNullException(); }
            );
            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => provider.GetTestCases());
        }

        [Test]
        public void GetTestCases_DoesNotIgnoreFileNotFoundExceptionsFromOtherProviders()
        {
            // Arrange
            var env = "PMLUNIT_TEST_VAR_6";
            Environment.SetEnvironmentVariable(env, @"C:\path\to\pmllib");
            var mock = new Mock<TestCaseProvider>();
            mock.Setup(p => p.GetTestCases()).Throws(new FileNotFoundException());
            var provider = new EnvironmentVariableTestCaseProvider(env, path => mock.Object);
            // Act, Assert
            Assert.Throws<FileNotFoundException>(() => provider.GetTestCases());
        }

        [Test]
        public void GetTestCases_ReturnsTestCasesFromChildProviders()
        {
            // Arrange
            var env = "PMLUNIT_TEST_VAR_7";
            Environment.SetEnvironmentVariable(env, @"C:\path\to\pmllib");
            var expected = new List<TestCase>();
            expected.Add(new TestCaseBuilder("first").Build());
            expected.Add(new TestCaseBuilder("second").Build());
            var mock = new Mock<TestCaseProvider>();
            mock.Setup(p => p.GetTestCases()).Returns(expected);
            var provider = new EnvironmentVariableTestCaseProvider(env, path => mock.Object);
            // Act
            var result = provider.GetTestCases();
            // Assert
            Assert.AreEqual(expected.Count, result.Count);
            foreach (var testCase in expected)
                Assert.Contains(testCase, (ICollection)result);
        }

        private class DirectoryNameRecorder
        {
            public List<string> DirectoryNames { get; }

            public DirectoryNameRecorder()
            {
                DirectoryNames = new List<string>();
            }

            public TestCaseProvider Factory(string directoryName)
            {
                DirectoryNames.Add(directoryName);
                var mock = new Mock<TestCaseProvider>();
                mock.Setup(provider => provider.GetTestCases())
                    .Returns(new List<TestCase>());
                return mock.Object;
            }
        }
    }
}
