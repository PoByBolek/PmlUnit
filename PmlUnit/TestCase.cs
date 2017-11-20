using System;

namespace PmlUnit
{
    class TestCase
    {
        public string Name { get; }

        public TestCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }
    }
}
