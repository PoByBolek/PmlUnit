// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Drawing;

namespace PmlUnit
{
    abstract class TestListEntry
    {
        public event EventHandler SelectionChanged;

        private bool SelectedField;

        public bool Selected
        {
            get { return SelectedField; }
            set
            {
                if (value != SelectedField)
                {
                    SelectedField = value;
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public abstract void Paint(Graphics g, Rectangle bounds, TestListPaintOptions options);
    }
}
