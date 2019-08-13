// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Drawing;

namespace PmlUnit
{
    interface TestListEntry
    {
        Test Test { get; }
        bool Selected { get; set; }
    }

    interface TestListBaseEntry
    {
        event EventHandler SelectionChanged;

        bool Selected { get; set; }
        void Paint(Graphics g, Rectangle bounds, TestListPaintOptions options);
    }
}
