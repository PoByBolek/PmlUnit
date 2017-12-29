using System.Collections.Generic;

namespace PmlUnit
{
    class TestSuite
    {
        public string Name { get; }

        public bool HasSetUpMethod { get; set; }
        public bool HasTearDownMethod { get; set; }

        public List<TestCase> TestCases { get; }

        public TestSuite(string name)
        {
            Name = name;
            TestCases = new List<TestCase>();
            HasSetUpMethod = false;
            HasTearDownMethod = false;
        }
    }
}
