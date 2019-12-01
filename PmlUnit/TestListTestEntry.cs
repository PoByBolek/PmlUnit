// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    class TestListTestEntry : TestListEntry
    {
        public event EventHandler GroupChanged;
        public event EventHandler ResultChanged;

        public Test Test { get; }

        private TestListGroupEntry GroupField;

        public TestListTestEntry(Test test)
            : this(test, null)
        {
        }

        public TestListTestEntry(Test test, TestListGroupEntry group)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            Test = test;
            Test.ResultChanged += OnResultChanged;
            GroupField = group;
            if (GroupField != null)
                GroupField.Entries.Add(this);
        }

        public TestListGroupEntry Group
        {
            get { return GroupField; }
            set
            {
                if (value != GroupField)
                {
                    var oldValue = GroupField;
                    if (oldValue != null)
                    {
                        GroupField = null;
                        oldValue.Entries.Remove(this);
                    }

                    GroupField = value;
                    if (value != null)
                        GroupField.Entries.Add(this);

                    GroupChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void OnResultChanged(object sender, EventArgs e)
        {
            ResultChanged?.Invoke(this, EventArgs.Empty);
        }

        public override string ToString()
        {
            return Test.FullName;
        }
    }
}
