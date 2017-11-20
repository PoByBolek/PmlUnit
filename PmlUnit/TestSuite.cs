using System.Collections.Generic;

namespace PmlUnit
{
    class TestSuite
    {
        public string Name { get; }

        public List<TestCase> TestCases { get; }

        public TestSuite(string name)
        {
            Name = name;
            TestCases = new List<TestCase>();
        }
    }
}
