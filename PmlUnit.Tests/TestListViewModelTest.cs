// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestListViewModel))]
    public class TestListViewModelTest
    {
        private TestListViewModel Model;
        private TestCase TestCase;

        [SetUp]
        public void Setup()
        {
            var testCase = new TestCase("Test");
            testCase.Tests.Add("first");
            testCase.Tests.Add("second");
            Model = new TestListViewModel();
            Model.TestCases.Add(testCase);

            TestCase = new TestCase("TestCase");
            TestCase.Tests.Add("third");
            TestCase.Tests.Add("fourth");
        }

        [Test]
        public void EntriesAreEmptyByDefault()
        {
            // Arrange
            var model = new TestListViewModel();
            // Asser
            Assert.That(model.Entries, Is.Empty);
        }

        [Test]
        public void AddingTestCaseAddsEntries()
        {
            // Arrange
            var testCase = new TestCase("Test");
            var firstTest = testCase.Tests.Add("first");
            var secondTest = testCase.Tests.Add("second");
            var model = new TestListViewModel();
            // Act
            model.TestCases.Add(testCase);
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(model.Entries.Count, Is.EqualTo(3));
                var group = model.Entries[0] as TestListGroupEntry;
                var first = model.Entries[1] as TestListTestEntry;
                var second = model.Entries[2] as TestListTestEntry;
                Assert.That(group.Entries, Is.EquivalentTo(new List<TestListTestEntry>() { first, second }));
                Assert.That(first.Test, Is.SameAs(firstTest));
                Assert.That(first.Group, Is.SameAs(group));
                Assert.That(second.Test, Is.SameAs(secondTest));
                Assert.That(second.Group, Is.SameAs(group));
            });
        }

        [Test]
        public void AddingTestCaseRaisesChangedEvent()
        {
            // Arrange
            bool raised = false;
            Model.Changed += (sender, args) => raised = true;
            // Act
            Model.TestCases.Add(TestCase);
            // Assert
            Assert.That(raised, "should have raised the Changed event");
        }

        [Test]
        public void VisibileEntriesAreEmptyByDefault()
        {
            // Arrange
            var model = new TestListViewModel();
            // Assert
            Assert.That(model.VisibleEntries, Is.Empty);
        }

        [Test]
        public void AddingTestCaseAddsVisibleEntries()
        {
            // Act
            Model.TestCases.Add(TestCase);
            // Assert
            Assert.That(Model.VisibleEntries, Is.EquivalentTo(Model.Entries));
        }

        [Test]
        public void AddingTestCaseRaisesVisibleEntriesChangedEvent()
        {
            // Arrange
            bool raised = false;
            Model.VisibleEntriesChanged += (sender, args) => raised = true;
            // Act
            Model.TestCases.Add(TestCase);
            // Assert
            Assert.That(raised, "should have raised the VisibleEntriesChanged event");
        }

        [Test]
        public void CollapsingGroupHidesChildEntries()
        {
            // Arrange
            var group = Model.Entries[0] as TestListGroupEntry;
            // Act
            group.IsExpanded = false;
            // Assert
            Assert.That(Model.VisibleEntries, Is.EquivalentTo(new List<TestListEntry>() { group }));
        }

        [Test]
        public void ExpandingGroupShowsChildEntries()
        {
            // Arrange
            var group = Model.Entries[0] as TestListGroupEntry;
            group.IsExpanded = false;
            // Act
            group.IsExpanded = true;
            // Assert
            Assert.That(Model.VisibleEntries, Is.EquivalentTo(Model.Entries));
        }

        [Test]
        public void CollapsingGroupDoesNotModifyEntries()
        {
            // Arrange
            Model.TestCases.Add(TestCase);
            var entries = Model.Entries.ToList();
            var group = Model.Entries[0] as TestListGroupEntry;
            // Act & Assert
            group.IsExpanded = false;
            Assert.That(Model.Entries, Is.EquivalentTo(entries));
            group.IsExpanded = true;
            Assert.That(Model.Entries, Is.EquivalentTo(entries));
        }

        [Test]
        public void CollapsingGroupRaisesVisibleChangedEvent()
        {
            // Arrange
            var group = Model.Entries[0] as TestListGroupEntry;
            bool raised = false;
            Model.VisibleEntriesChanged += (sender, args) => raised = true;
            // Act & Assert
            group.IsExpanded = false;
            Assert.That(raised, "should have raised VisibleEntriesChanged event");

            raised = false;
            group.IsExpanded = true;
            Assert.That(raised, "should have raised VisibleEntriesChanged event");
        }

        [Test]
        public void SelectingAnEntryRaisesSelectionChangedEvent()
        {
            // Arrange
            bool raised = false;
            Model.SelectionChanged += (sender, args) => raised = true;
            // Act & Assert
            Model.Entries[1].IsSelected = true;
            Assert.That(raised, "should have raised SelectionChanged event");

            raised = false;
            Model.Entries[1].IsSelected = false;
            Assert.That(raised, "should have raised SelectionChanged event");
        }

        [Test]
        public void SelectingAnEntryWithoutEventHandlerDoesntCrash()
        {
            Model.Entries[0].IsSelected = true;
        }

        [Test]
        public void ChangingEntrysTestResultRegroupsThatEntry()
        {
            // Arrange
            var entry = Model.Entries[1] as TestListTestEntry;
            var oldGroup = entry.Group;
            // Act
            entry.Test.Result = new TestResult(TimeSpan.FromSeconds(1));
            // Assert
            Assert.That(entry.Group, Is.Not.SameAs(oldGroup));
        }

        [Test]
        public void ChangingEntrysTestResultChangesEntries()
        {
            // Arrange
            var entry = Model.Entries[1] as TestListTestEntry;
            var oldGroup = entry.Group;
            bool raised = false;
            Model.Changed += (sender, args) => raised = true;
            // Act
            entry.Test.Result = new TestResult(TimeSpan.FromSeconds(1));
            // Assert
            Assert.That(raised, "should have raised Changed event");
            Assert.That(Model.Entries, Contains.Item(entry));
            Assert.That(Model.Entries, Contains.Item(entry.Group));
            Assert.That(Model.Entries, Contains.Item(oldGroup));
        }

        [Test]
        public void ChangingEntryTestResultChangesVisibleEntries()
        {
            // Arrange
            var entry = Model.Entries[1] as TestListTestEntry;
            var oldGroup = entry.Group;
            bool raised = false;
            Model.VisibleEntriesChanged += (sender, args) => raised = true;
            // Act
            entry.Test.Result = new TestResult(TimeSpan.FromSeconds(1));
            // Assert
            Assert.That(raised, "should have raised VisibleEntriesChanged event");
            Assert.That(Model.VisibleEntries, Contains.Item(entry));
            Assert.That(Model.VisibleEntries, Contains.Item(entry.Group));
            Assert.That(Model.VisibleEntries, Contains.Item(oldGroup));
        }

        [Test]
        public void RegroupingAllEntriesRemovesInitialGroupEntry()
        {
            // Arrange
            var group = Model.Entries[0] as TestListGroupEntry;
            var first = Model.Entries[1] as TestListTestEntry;
            var second = Model.Entries[2] as TestListTestEntry;
            // Act
            first.Test.Result = new TestResult(TimeSpan.FromSeconds(1));
            second.Test.Result = new TestResult(TimeSpan.FromSeconds(2));
            // Assert
            Assert.That(Model.Entries, Does.Not.Contain(group));
            Assert.That(Model.Entries, Contains.Item(first.Group));
            Assert.That(Model.Entries, Contains.Item(second.Group));

            Assert.That(Model.VisibleEntries, Does.Not.Contain(group));
            Assert.That(Model.VisibleEntries, Contains.Item(first.Group));
            Assert.That(Model.VisibleEntries, Contains.Item(second.Group));
        }

        [Test]
        public void RemovingTestCaseRemovesEntries()
        {
            // Act
            Model.TestCases.Clear();
            // Assert
            Assert.That(Model.Entries, Is.Empty);
            Assert.That(Model.VisibleEntries, Is.Empty);
        }

        [Test]
        public void RemovingTestCaseRaisesChangedEvent()
        {
            // Arrange
            bool raised = false;
            Model.Changed += (sender, args) => raised = true;
            // Act
            Model.TestCases.Clear();
            // Assert
            Assert.That(raised, "should have raised the Changed event");
        }

        [Test]
        public void RemovingTestCaseRaisesVisibileEntriesChangedEvent()
        {
            // Arrange
            bool raised = false;
            Model.VisibleEntriesChanged += (sender, args) => raised = true;
            // Act
            Model.TestCases.Clear();
            // Assert
            Assert.That(raised, "should have raised the VisibleEntriesChanged event");
        }

        [Test]
        public void RemovingTestCaseOnlyRemovesRelevantEntries()
        {
            // Arrange
            var entries = Model.Entries.ToList();
            var visibleEntries = Model.VisibleEntries.ToList();
            var testCase = new TestCase("OtherTest");
            testCase.Tests.Add("foo");
            testCase.Tests.Add("bar");
            Model.TestCases.Add(testCase);
            // Act
            Model.TestCases.Remove(testCase);
            // Assert
            Assert.That(Model.Entries, Is.EquivalentTo(entries));
            Assert.That(Model.VisibleEntries, Is.EquivalentTo(visibleEntries));
        }

        [Test]
        public void SettingGrouperRegroupsAllEntries()
        {
            // Arrange
            var oldGroup = Model.Entries[0] as TestListGroupEntry;
            var first = Model.Entries[1] as TestListTestEntry;
            var second = Model.Entries[2] as TestListTestEntry;
            // Act
            Model.Grouper = new TestCaseGrouper();
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(Model.Entries.Count, Is.EqualTo(3));
                Assert.That(Model.Entries, Contains.Item(first));
                Assert.That(Model.Entries, Contains.Item(second));
                var group = Model.Entries[0] as TestListGroupEntry;
                Assert.That(group, Is.Not.SameAs(oldGroup));
                Assert.That(group.Entries, Is.EquivalentTo(new List<TestListTestEntry>() { first, second }));
                Assert.That(first.Group, Is.SameAs(group));
                Assert.That(second.Group, Is.SameAs(group));
            });
        }
    }
}
