using System.Collections.Generic;

namespace PmlUnit
{
    class TestCase
    {
        public string Name { get; }

        public bool HasSetUp { get; set; }
        public bool HasTearDown { get; set; }

        public List<Test> Tests { get; }

        public TestCase(string name)
        {
            Name = name;
            Tests = new List<Test>();
            HasSetUp = false;
            HasTearDown = false;
        }
    }
}
