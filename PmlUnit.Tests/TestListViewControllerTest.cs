// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestListView))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TestListViewControllerTest
    {
        private TestListView TestList;
        private TestListViewModel Model;
        private ReadOnlyTestListEntryCollection<TestListEntry> VisibleEntries;
        private TestListSelectedEntryCollection SelectedEntries;
        private TestListViewController Controller;
        private TestCase First;
        private TestCase Second;

        [SetUp]
        public void Setup()
        {
            TestList = new TestListView();
            Model = TestList.GetModel();
            VisibleEntries = Model.VisibleEntries;
            SelectedEntries = Model.SelectedEntries;
            Controller = TestList.GetController();

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

        [TestCase(0, 30, 10, MouseButtons.Left)]
        [TestCase(1, 30, 30, MouseButtons.Left)]
        [TestCase(2, 30, 50, MouseButtons.Left)]
        [TestCase(3, 30, 70, MouseButtons.Left)]
        [TestCase(4, 30, 90, MouseButtons.Left)]
        [TestCase(5, 30, 110, MouseButtons.Left)]
        [TestCase(0, 30, 10, MouseButtons.Right)]
        [TestCase(1, 30, 30, MouseButtons.Right)]
        [TestCase(2, 30, 50, MouseButtons.Right)]
        [TestCase(3, 30, 70, MouseButtons.Right)]
        [TestCase(4, 30, 90, MouseButtons.Right)]
        [TestCase(5, 30, 110, MouseButtons.Right)]
        public void EntryClick_SelectsThatEntry(int index, int x, int y, MouseButtons button)
        {
            PerformMouseClick(x, y, button);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[index]));
        }

        [TestCase(0, 30, 10, MouseButtons.Left)]
        [TestCase(1, 30, 30, MouseButtons.Left)]
        [TestCase(2, 30, 50, MouseButtons.Left)]
        [TestCase(3, 30, 70, MouseButtons.Left)]
        [TestCase(4, 30, 90, MouseButtons.Left)]
        [TestCase(5, 30, 110, MouseButtons.Left)]
        [TestCase(0, 30, 10, MouseButtons.Right)]
        [TestCase(1, 30, 30, MouseButtons.Right)]
        [TestCase(2, 30, 50, MouseButtons.Right)]
        [TestCase(3, 30, 70, MouseButtons.Right)]
        [TestCase(4, 30, 90, MouseButtons.Right)]
        [TestCase(5, 30, 110, MouseButtons.Right)]
        public void EntryClick_SelectsOnlyThatEntry(int index, int x, int y, MouseButtons button)
        {
            foreach (var entry in VisibleEntries)
                entry.IsSelected = true;

            PerformMouseClick(x, y, button);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[index]));
        }

        [TestCase(0, 30, 10, MouseButtons.Left)]
        [TestCase(1, 30, 30, MouseButtons.Left)]
        [TestCase(2, 30, 50, MouseButtons.Left)]
        [TestCase(3, 30, 70, MouseButtons.Left)]
        [TestCase(4, 30, 90, MouseButtons.Left)]
        [TestCase(5, 30, 110, MouseButtons.Left)]
        [TestCase(0, 30, 10, MouseButtons.Right)]
        [TestCase(1, 30, 30, MouseButtons.Right)]
        [TestCase(2, 30, 50, MouseButtons.Right)]
        [TestCase(3, 30, 70, MouseButtons.Right)]
        [TestCase(4, 30, 90, MouseButtons.Right)]
        [TestCase(5, 30, 110, MouseButtons.Right)]
        public void EntryClick_FocusesThatEntry(int index, int x, int y, MouseButtons button)
        {
            PerformMouseClick(x, y, button);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[index]));
        }

        [TestCase(MouseButtons.Left)]
        [TestCase(MouseButtons.Right)]
        public void OutsideClick_SelectsNothing(MouseButtons button)
        {
            foreach (var entry in VisibleEntries)
                entry.IsSelected = true;

            PerformMouseClick(30, 170, button);
            Assert.That(SelectedEntries, Is.Empty);
        }

        [Test]
        public void GroupIconClick_TogglesGroup()
        {
            var group = VisibleEntries[0] as TestListGroupEntry;
            Assert.That(group.IsExpanded);

            PerformMouseClick(10, 10);
            Assert.That(!group.IsExpanded);

            PerformMouseClick(10, 10);
            Assert.That(group.IsExpanded);
        }

        [Test]
        public void GroupDoubleClick_TogglesGroup()
        {
            var group = VisibleEntries[0] as TestListGroupEntry;
            Assert.That(group.IsExpanded);

            PerformDoubleClick(30, 10);
            Assert.That(!group.IsExpanded);

            PerformDoubleClick(30, 10);
            Assert.That(group.IsExpanded);
        }

        [Test]
        public void LeftControlEntryClick_TooglesSelectionOfThatEntry()
        {
            Assert.That(SelectedEntries, Is.Empty);

            PerformMouseClick(30, 110, Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[5]));

            PerformMouseClick(30, 110, Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);

            foreach (var entry in VisibleEntries)
                entry.IsSelected = true;

            PerformMouseClick(30, 110, Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(VisibleEntries.Count - 1));
            Assert.That(SelectedEntries, Does.Not.Contain(VisibleEntries[5]));
        }

        [Test]
        public void RightControlEntryClick_KeepsCurrentSelection()
        {
            Assert.That(SelectedEntries, Is.Empty);

            PerformMouseClick(30, 130, MouseButtons.Right, Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);

            foreach (var entry in VisibleEntries)
                entry.IsSelected = true;
            Assert.That(SelectedEntries.Count, Is.EqualTo(VisibleEntries.Count));

            PerformMouseClick(30, 130, MouseButtons.Right, Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(VisibleEntries.Count));
        }

        [Test]
        public void LeftShiftEntryClick_SelectsVisibleEntriesBetweenLastAndCurrentClick()
        {
            PerformMouseClick(30, 50);

            PerformMouseClick(30, 70, Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[3]));

            PerformMouseClick(30, 30, Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));

            PerformMouseClick(30, 50, Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));

            PerformMouseClick(30, 130, Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(5));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[3]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[4]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[5]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[6]));
        }

        [Test]
        public void LeftShiftEntryClick_StartsAtFirstEntry()
        {
            PerformMouseClick(30, 50, Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(3));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
        }
    

        [Test]
        public void RightShiftEntryClick_KeepsCurrentSelection()
        {
            PerformMouseClick(30, 50);
            PerformMouseClick(30, 70, MouseButtons.Right, Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
        }

        [Test]
        public void UpDownArrow_MovesSelection()
        {
            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));

            PerformKeyPress(Keys.Up);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            PerformKeyPress(Keys.Up);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));

            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[3]));
            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[4]));
        }

        [Test]
        public void UpDownArrow_StartsFromLastClick()
        {
            PerformMouseClick(30, 50);

            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[3]));
        }

        [Test]
        public void CtrlUpDownArrow_MovesFocusButKeepsSelection()
        {
            PerformMouseClick(30, 50);

            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));
            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformKeyPress(Keys.Down | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));
        }

        [Test]
        public void CtrlUpDownArrow_MovesFocusWithoutSelection()
        {
            PerformKeyPress(Keys.Down | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));
            PerformKeyPress(Keys.Down | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[2]));

            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));
            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
        }

        [Test]
        public void ShiftUpDownArrow_ExtendsSelectionFromLastClick()
        {
            PerformMouseClick(30, 30);

            PerformKeyPress(Keys.Down | Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[2]));
            PerformKeyPress(Keys.Down | Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(3));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[3]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[3]));
            PerformKeyPress(Keys.Down | Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(4));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[3]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[4]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[4]));

            PerformKeyPress(Keys.Up | Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(3));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[3]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[3]));
        }

        [Test]
        public void Space_AddsFocusedEntryToSelection()
        {
            PerformKeyPress(Keys.Space);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            PerformKeyPress(Keys.Space);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));

            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);

            PerformKeyPress(Keys.Space);
            Assert.That(SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
        }

        [Test]
        public void CtrlSpace_TogglesSelectionOfFocusedEntry()
        {
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));

            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);

            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
        }

        [Test]
        public void ShiftSpace_SelectsRangeBetweenLastClickAndFocusedEntry()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);

            PerformKeyPress(Keys.Space | Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(4));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[3]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[4]));
        }

        [Test]
        public void Left_SelectsGroupEntry()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformMouseClick(30, 130);
            PerformKeyPress(Keys.Left);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
        }

        [Test]
        public void CtrlLeft_MovesFocusFocusButKeepsSelection()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformMouseClick(30, 130);
            PerformKeyPress(Keys.Left | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[6]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
        }

        [Test]
        public void ShiftLeft_SelectsRangeBetweenLastClickAndGroupEntry()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left | Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformMouseClick(30, 70);
            PerformKeyPress(Keys.Left | Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(4));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[3]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
        }

        [TestCase(Keys.None)]
        [TestCase(Keys.Control)]
        [TestCase(Keys.Shift)]
        public void Left_CollapsesGroupEntry(Keys modifierKeys)
        {
            var group = VisibleEntries[0] as TestListGroupEntry;
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left | modifierKeys);
            Assert.That(group.IsExpanded);
            PerformKeyPress(Keys.Left | modifierKeys);
            Assert.That(!group.IsExpanded);
            PerformKeyPress(Keys.Left | modifierKeys);
            Assert.That(!group.IsExpanded);
        }

        [Test]
        public void Right_SelectsFirstEntryInGroup()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Right);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));
        }

        [Test]
        public void Right_StaysOnCurrentlyFocusedTestEntry()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Down);
            PerformKeyPress(Keys.Down);
            PerformKeyPress(Keys.Right);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[2]));
            PerformKeyPress(Keys.Right);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[2]));
        }

        [Test]
        public void CtrlRight_MovesFocusFocusButKeepsSelection()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Right | Keys.Control);
            Assert.That(SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));
        }

        [Test]
        public void ShiftRight_SelectsRangeBetweenLastClickAndFirstGroupEntry()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Right | Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[0]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));

            PerformMouseClick(30, 70);
            PerformKeyPress(Keys.Up | Keys.Control);
            PerformKeyPress(Keys.Up | Keys.Control);
            PerformKeyPress(Keys.Up | Keys.Control);
            PerformKeyPress(Keys.Right | Keys.Shift);
            Assert.That(SelectedEntries.Count, Is.EqualTo(3));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[1]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[2]));
            Assert.That(SelectedEntries, Contains.Item(VisibleEntries[3]));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));
        }

        [TestCase(Keys.None)]
        [TestCase(Keys.Control)]
        [TestCase(Keys.Shift)]
        public void Right_ExpandsGroupEntry(Keys modifierKeys)
        {
            var group = VisibleEntries[0] as TestListGroupEntry;
            Assert.That(group.IsExpanded);

            PerformMouseClick(30, 10);
            group.IsExpanded = false;

            PerformKeyPress(Keys.Right | modifierKeys);
            Assert.That(group.IsExpanded);
            PerformKeyPress(Keys.Right | modifierKeys);
            Assert.That(group.IsExpanded);
        }


        private void PerformMouseClick(int x, int y)
        {
            PerformMouseClick(x, y, MouseButtons.Left, Keys.None);
        }

        private void PerformMouseClick(int x, int y, Keys modifierKeys)
        {
            PerformMouseClick(x, y, MouseButtons.Left, modifierKeys);
        }

        private void PerformMouseClick(int x, int y, MouseButtons button)
        {
            PerformMouseClick(x, y, button, Keys.None);
        }

        private void PerformMouseClick(int x, int y, MouseButtons button, Keys modifierKeys)
        {
            Controller.HandleMouseClick(new MouseEventArgs(button, 1, x, y, 0), modifierKeys);
        }

        private void PerformDoubleClick(int x, int y)
        {
            Controller.HandleMouseDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, x, y, 0), Keys.None);
        }

        private void PerformKeyPress(Keys keys)
        {
            Controller.HandleKeyDown(new KeyEventArgs(keys));
        }
    }
}
