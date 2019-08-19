// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Forms;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestListView))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TestListViewMouseTest
    {
        private TestListView TestList;
        private MethodInfo OnMouseClick;
        private TestCase First;
        private TestCase Second;

        [SetUp]
        public void Setup()
        {
            TestList = new TestListView();
            OnMouseClick = TestList.GetType().GetMethod(
                "OnMouseClick", BindingFlags.Instance | BindingFlags.NonPublic,
                null, new Type[] { typeof(MouseEventArgs), typeof(Keys) }, null
            );
            Assert.NotNull(OnMouseClick);

            First = new TestCase("First");
            First.Tests.Add("a1");
            First.Tests.Add("a2");
            First.Tests.Add("a3");
            TestList.TestCases.Add(First);
            Second = new TestCase("Second");
            Second.Tests.Add("b1");
            Second.Tests.Add("b2");
            Second.Tests.Add("b3");
            TestList.TestCases.Add(Second);
        }

        [TearDown]
        public void TearDown()
        {
            TestList.Dispose();
        }

        [Test]
        public void EntryClick_SelectsThatEntry()
        {
            var selected = PerformMouseClick(30, 30);
            Assert.AreEqual(1, selected.Count);
            Assert.Contains(First.Tests["a1"], selected);
        }

        [Test]
        public void EntryClick_SelectsOnlyThatEntry()
        {
            foreach (var entry in TestList.AllTestEntries)
                entry.Selected = true;

            var selected = PerformMouseClick(30, 50);
            Assert.AreEqual(1, selected.Count);
            Assert.Contains(First.Tests["a2"], selected);
        }

        [Test]
        public void RightEntryClick_SelectsThatEntry()
        {
            var selected = PerformMouseClick(30, 70, MouseButtons.Right);
            Assert.AreEqual(1, selected.Count);
            Assert.Contains(First.Tests["a3"], selected);
        }

        [Test]
        public void RightEntryClick_SelectsOnlyThatEntry()
        {
            foreach (var entry in TestList.AllTestEntries)
                entry.Selected = true;

            var selected = PerformMouseClick(30, 70, MouseButtons.Right);
            Assert.AreEqual(1, selected.Count);
            Assert.Contains(First.Tests["a3"], selected);
        }

        [Test]
        public void ControlEntryClick_TooglesSelectionOfThatEntry()
        {
            var clicked = Second.Tests["b1"];
            Assert.AreEqual(0, TestList.SelectedTests.Count);

            var selected = PerformMouseClick(30, 110, Keys.Control);
            Assert.AreEqual(1, selected.Count);
            Assert.Contains(clicked, selected);

            selected = PerformMouseClick(30, 110, Keys.Control);
            Assert.AreEqual(0, selected.Count);

            foreach (var entry in TestList.AllTestEntries)
                entry.Selected = true;

            selected = PerformMouseClick(30, 110, Keys.Control);
            Assert.AreEqual(5, selected.Count);
            foreach (var test in selected)
                Assert.AreNotSame(clicked, test);
        }

        [Test]
        public void RightControlEntryClick_KeepsCurrentSelection()
        {
            var clicked = Second.Tests["b2"];
            Assert.AreEqual(0, TestList.SelectedTests.Count);

            var selected = PerformMouseClick(30, 130, MouseButtons.Right, Keys.Control);
            Assert.AreEqual(0, selected.Count);

            foreach (var entry in TestList.AllTestEntries)
                entry.Selected = true;
            Assert.AreEqual(6, TestList.SelectedTests.Count);

            selected = PerformMouseClick(30, 130, MouseButtons.Right, Keys.Control);
            Assert.AreEqual(6, selected.Count);
        }

        [Test]
        public void ShiftEntryClick_SelectsAllEntriesBetweenLastAndCurrentClick()
        {
            PerformMouseClick(30, 50);

            var selected = PerformMouseClick(30, 70, Keys.Shift);
            Assert.AreEqual(2, selected.Count);
            Assert.Contains(First.Tests["a2"], selected);
            Assert.Contains(First.Tests["a3"], selected);

            selected = PerformMouseClick(30, 30, Keys.Shift);
            Assert.AreEqual(2, selected.Count);
            Assert.Contains(First.Tests["a1"], selected);
            Assert.Contains(First.Tests["a2"], selected);

            selected = PerformMouseClick(30, 50, Keys.Shift);
            Assert.AreEqual(1, selected.Count);
            Assert.Contains(First.Tests["a2"], selected);

            PerformMouseClick(30, 130, Keys.Shift);
            PerformMouseClick(30, 90, Keys.Control); // deselect the group entry, because SelectedTests will return all child entries if the group is selected
            selected = TestList.SelectedTests;
            Assert.AreEqual(4, selected.Count);
            Assert.Contains(First.Tests["a2"], selected);
            Assert.Contains(First.Tests["a3"], selected);
            Assert.Contains(Second.Tests["b1"], selected);
            Assert.Contains(Second.Tests["b2"], selected);
        }

        [Test]
        public void RightShiftEntryClick_KeepsCurrentSelection()
        {
            PerformMouseClick(30, 50);
            var selected = PerformMouseClick(30, 70, MouseButtons.Right, Keys.Shift);
            Assert.AreEqual(1, selected.Count);
            Assert.Contains(First.Tests["a2"], selected);
        }

        private List<Test> PerformMouseClick(int x, int y)
        {
            return PerformMouseClick(x, y, MouseButtons.Left, Keys.None);
        }

        private List<Test> PerformMouseClick(int x, int y, Keys modifierKeys)
        {
            return PerformMouseClick(x, y, MouseButtons.Left, modifierKeys);
        }

        private List<Test> PerformMouseClick(int x, int y, MouseButtons button)
        {
            return PerformMouseClick(x, y, button, Keys.None);
        }

        private List<Test> PerformMouseClick(int x, int y, MouseButtons button, Keys modifierKeys)
        {
            var args = new MouseEventArgs(button, 1, x, y, 0);
            OnMouseClick.Invoke(TestList, new object[] { args, modifierKeys });
            return TestList.SelectedTests;
        }
    }
}
