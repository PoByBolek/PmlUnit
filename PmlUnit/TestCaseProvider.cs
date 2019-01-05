// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.Collections.Generic;

namespace PmlUnit
{
    interface TestCaseProvider
    {
        ICollection<TestCase> GetTestCases();
    }
}
