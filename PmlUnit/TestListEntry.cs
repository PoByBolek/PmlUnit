// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    abstract class TestListEntry
    {
        public event EventHandler SelectionChanged;

        private bool IsSelectedField;

        public bool IsSelected
        {
            get { return IsSelectedField; }
            set
            {
                if (value != IsSelectedField)
                {
                    IsSelectedField = value;
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
