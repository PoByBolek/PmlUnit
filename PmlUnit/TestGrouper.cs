// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT

namespace PmlUnit
{
    interface TestGrouper
    {
        TestListGroupEntry GetGroupFor(Test test);
    }
}
