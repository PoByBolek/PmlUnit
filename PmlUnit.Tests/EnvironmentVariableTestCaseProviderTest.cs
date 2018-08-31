using System;
using System.Collections.Generic;
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
            var path = @"C:\path\to\pmllib";
            Environment.SetEnvironmentVariable("PMLUNIT_TEST_VAR", path);
            var recorder = new DirectoryNameRecorder();
            var provider = new EnvironmentVariableTestCaseProvider("PMLUNIT_TEST_VAR", recorder.Factory);
            // Act
            provider.GetTestCases();
            // Assert
            Assert.Contains(path, recorder.DirectoryNames);
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
