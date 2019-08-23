// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;

namespace PmlUnit
{
    class TestListTestEntry : TestListEntry
    {
        public event EventHandler GroupChanged;
        public event EventHandler ResultChanged
        {
            add { Test.ResultChanged += value; }
            remove { Test.ResultChanged -= value; }
        }

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
            GroupField = group;
            if (group != null)
                group.Entries.Add(this);
        }

        public TestListGroupEntry Group
        {
            get { return GroupField; }
            set
            {
                if (value != GroupField)
                {
                    var oldGroup = GroupField;
                    var newGroup = value;
                    if (oldGroup != null)
                    {
                        GroupField = null;
                        oldGroup.Entries.Remove(this);
                    }
                    GroupField = newGroup;
                    if (newGroup != null && !newGroup.Entries.Contains(this))
                    {
                        newGroup.Entries.Add(this);
                    }

                    GroupChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
