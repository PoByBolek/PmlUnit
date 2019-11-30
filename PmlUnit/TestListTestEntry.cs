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
        private bool GroupIsChanging;

        public TestListTestEntry(Test test, TestListGroupEntry group)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            Test = test;
            Test.ResultChanged += OnResultChanged;
            GroupField = group;
            GroupField.Entries.Add(this);
        }

        public TestListGroupEntry Group
        {
            get { return GroupField; }
            set
            {
                if (GroupIsChanging)
                    return;
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value != GroupField)
                {
                    GroupIsChanging = true;
                    try
                    {
                        GroupField.Entries.Remove(this);
                        GroupField = value;
                        GroupField.Entries.Add(this);
                    }
                    finally
                    {
                        GroupIsChanging = false;
                    }

                    GroupChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string Key => Test.Name + ":" + Test.TestCase.Name;

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
