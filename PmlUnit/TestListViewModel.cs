// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;

namespace PmlUnit
{
    class TestListViewModel
    {
        public event EventHandler Changed;
        public event EventHandler VisibleEntriesChanged;
        public event EventHandler SelectionChanged;
        public event EventHandler FocusedEntryChanged;

        public TestCaseCollection TestCases { get; }

        public ReadOnlyTestListEntryCollection<TestListEntry> AllEntries { get; }
        public TestListSelectedEntryCollection SelectedEntries { get; }
        public ReadOnlyTestListEntryCollection<TestListEntry> VisibleEntries { get; }
        public ReadOnlyTestListEntryCollection<TestListTestEntry> TestEntries { get; }

        private readonly TestListEntryCollection<TestListEntry> AllEntriesField;
        private readonly TestListEntryCollection<TestListEntry> VisibleEntriesField;
        private readonly TestListEntryCollection<TestListTestEntry> TestEntriesField;

        private readonly TestListGroupEntry FailedGroup;
        private readonly TestListGroupEntry NotExecutedGroup;
        private readonly TestListGroupEntry PassedGroup;

        private TestListGroupEntry HighlightedIconEntryField;
        private TestListEntry FocusedEntryField;

        public TestListViewModel()
        {
            TestCases = new TestCaseCollection();
            TestCases.Changed += OnTestCasesChanged;

            var groupComparer = new TestListGroupEntryComparer();
            var testComparer = new TestListTestEntryComparer(groupComparer);
            var entryComparer = new TestListEntryComparer(groupComparer, testComparer);

            AllEntriesField = new TestListEntryCollection<TestListEntry>(entryComparer);
            AllEntries = AllEntriesField.AsReadOnly();
            SelectedEntries = new TestListSelectedEntryCollection(AllEntries);
            VisibleEntriesField = new TestListEntryCollection<TestListEntry>(entryComparer);
            VisibleEntries = VisibleEntriesField.AsReadOnly();
            TestEntriesField = new TestListEntryCollection<TestListTestEntry>(testComparer);
            TestEntries = TestEntriesField.AsReadOnly();

            FailedGroup = new TestListGroupEntry("1_failed", "Failed Tests");
            SetupGroupEntry(FailedGroup);
            NotExecutedGroup = new TestListGroupEntry("2_not_executed", "Not executed Tests");
            SetupGroupEntry(NotExecutedGroup);
            PassedGroup = new TestListGroupEntry("3_passed", "Passed Tests");
            SetupGroupEntry(PassedGroup);
        }

        public TestListEntry FocusedEntry
        {
            get { return FocusedEntryField; }
            set
            {
                if (value != FocusedEntryField)
                {
                    FocusedEntryField = value;
                    FocusedEntryChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public TestListGroupEntry HighlightedIconEntry
        {
            get { return HighlightedIconEntryField; }
            set
            {
                if (value != HighlightedIconEntryField)
                {
                    HighlightedIconEntryField = value;
                    Changed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void SetupGroupEntry(TestListGroupEntry group)
        {
            group.SelectionChanged += OnSelectionChanged;
            group.EntriesChanged += OnGroupEntriesChanged;
            group.ExpandedChanged += OnGroupExpandedChanged;
            AllEntriesField.Add(group);
        }

        private void OnTestCasesChanged(object sender, TestCasesChangedEventArgs e)
        {
            foreach (var testCase in e.RemovedTestCases)
                Remove(testCase);
            foreach (var testCase in e.AddedTestCases)
                Add(testCase);
        }

        private void Add(TestCase testCase)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));

            foreach (var test in testCase.Tests)
                Add(test);
        }

        private void Add(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            var entry = new TestListTestEntry(test, GetGroupFor(test));
            AllEntriesField.Add(entry);
            TestEntriesField.Add(entry);
            if (entry.Group.IsExpanded)
            {
                VisibleEntriesField.Add(entry);
                VisibleEntriesChanged?.Invoke(this, EventArgs.Empty);
            }

            entry.SelectionChanged += OnSelectionChanged;
            entry.ResultChanged += OnTestResultChanged;
            entry.GroupChanged += OnGroupChanged;
        }

        public void Remove(TestCase testCase)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));

            foreach (var test in testCase.Tests)
                Remove(test);
        }

        public void Remove(Test test)
        {
            throw new NotImplementedException();
        }

        private TestListGroupEntry GetGroupFor(Test test)
        {
            switch (test.Status)
            {
                case TestStatus.Failed:
                    return FailedGroup;
                case TestStatus.NotExecuted:
                    return NotExecutedGroup;
                case TestStatus.Passed:
                    return PassedGroup;
                default:
                    return null;
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnTestResultChanged(object sender, EventArgs e)
        {
            var entry = sender as TestListTestEntry;
            if (entry == null)
                return;

            entry.Group = GetGroupFor(entry.Test);
        }

        private void OnGroupChanged(object sender, EventArgs e)
        {
            var entry = sender as TestListTestEntry;
            if (entry == null)
                return;

            if (entry.Group.IsExpanded)
                VisibleEntriesField.Add(entry);
            else
                VisibleEntriesField.Remove(entry);
        }

        private void OnGroupEntriesChanged(object sender, TestListEntriesChangedEventArgs e)
        {
            var group = sender as TestListGroupEntry;
            if (group == null)
                return;

            if (group.Entries.Count > 0)
                VisibleEntriesField.Add(group);
            else
                VisibleEntriesField.Remove(group);

            VisibleEntriesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnGroupExpandedChanged(object sender, EventArgs e)
        {
            var group = sender as TestListGroupEntry;
            if (group == null)
                return;

            if (group.IsExpanded)
            {
                foreach (var entry in group.Entries)
                    VisibleEntriesField.Add(entry);
            }
            else
            {
                foreach (var entry in group.Entries)
                    VisibleEntriesField.Remove(entry);
            }

            VisibleEntriesChanged?.Invoke(this, EventArgs.Empty);
        }

        private class TestListEntryComparer : IComparer<TestListEntry>
        {
            private readonly IComparer<TestListGroupEntry> GroupComparer;
            private readonly IComparer<TestListTestEntry> TestComparer;

            public TestListEntryComparer(IComparer<TestListGroupEntry> groupComparer, IComparer<TestListTestEntry> testComparer)
            {
                if (groupComparer == null)
                    throw new ArgumentNullException(nameof(groupComparer));
                if (testComparer == null)
                    throw new ArgumentNullException(nameof(testComparer));

                GroupComparer = groupComparer;
                TestComparer = testComparer;
            }

            public int Compare(TestListEntry left, TestListEntry right)
            {
                if (left == right)
                    return 0;
                else if (left == null)
                    return 1;
                else if (right == null)
                    return -1;

                var leftGroup = left as TestListGroupEntry;
                var leftTest = left as TestListTestEntry;
                var rightGroup = right as TestListGroupEntry;
                var rightTest = right as TestListTestEntry;

                if (leftGroup != null && rightGroup != null)
                    return GroupComparer.Compare(leftGroup, rightGroup);
                else if (leftGroup != null && rightTest != null)
                    return -CompareTestInGroup(rightTest, leftGroup);
                else if (leftTest != null && rightGroup != null)
                    return CompareTestInGroup(leftTest, rightGroup);
                else if (leftTest != null && rightTest != null)
                    return TestComparer.Compare(leftTest, rightTest);
                else
                    throw new ArgumentException(string.Format(
                        "Expected two {} or {} instances but got {} and {} instead.",
                        typeof(TestListTestEntry).FullName, typeof(TestListGroupEntry).FullName,
                        left.GetType().FullName, right.GetType().FullName
                    ));
            }

            private int CompareTestInGroup(TestListTestEntry left, TestListGroupEntry right)
            {
                if (left.Group == null)
                    return -1;
                else if (left.Group == right)
                    return 1;
                else
                    return GroupComparer.Compare(left.Group, right);
            }
        }

        private class TestListGroupEntryComparer : IComparer<TestListGroupEntry>
        {
            public int Compare(TestListGroupEntry left, TestListGroupEntry right)
            {
                if (left == right)
                    return 0;
                else if (left == null)
                    return -1;
                else if (right == null)
                    return 1;
                else
                    return string.Compare(left.Key, right.Key, StringComparison.Ordinal);
            }
        }

        private class TestListTestEntryComparer : IComparer<TestListTestEntry>
        {
            private readonly IComparer<TestListGroupEntry> GroupComparer;

            public TestListTestEntryComparer(IComparer<TestListGroupEntry> groupComparer)
            {
                if (groupComparer == null)
                    throw new ArgumentNullException(nameof(groupComparer));

                GroupComparer = groupComparer;
            }

            public int Compare(TestListTestEntry left, TestListTestEntry right)
            {
                int result = GroupComparer.Compare(left.Group, right.Group);
                if (result == 0)
                    result = string.Compare(left.Key, right.Key, StringComparison.OrdinalIgnoreCase);
                return result;
            }
        }
    }
}
