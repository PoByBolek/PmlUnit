using System;

namespace PmlUnit
{
    class Test
    {
        public string Name { get; }

        public Test(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }
    }
}
