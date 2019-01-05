// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.IO;

namespace PmlUnit
{
    static class TextReaderExtensions
    {
        public static IEnumerable<string> ReadAllLines(this TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                yield return line;
        }
    }
}
