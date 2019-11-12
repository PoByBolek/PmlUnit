// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
        private TestListViewController Controller;
        private TestCase First;
        private TestCase Second;

        [SetUp]
        public void Setup()
        {
            TestList = new TestListView();
            Controller = TestList.GetType()
                .GetField("Controller", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(TestList) as TestListViewController;
            Assert.NotNull(Controller);

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

        [TestCase(0, 30, 30, MouseButtons.Left)]
        [TestCase(1, 30, 50, MouseButtons.Left)]
        [TestCase(2, 30, 70, MouseButtons.Left)]
        [TestCase(3, 30, 110, MouseButtons.Left)]
        [TestCase(4, 30, 130, MouseButtons.Left)]
        [TestCase(5, 30, 150, MouseButtons.Left)]
        [TestCase(0, 30, 30, MouseButtons.Right)]
        [TestCase(1, 30, 50, MouseButtons.Right)]
        [TestCase(2, 30, 70, MouseButtons.Right)]
        [TestCase(3, 30, 110, MouseButtons.Right)]
        [TestCase(4, 30, 130, MouseButtons.Right)]
        [TestCase(5, 30, 150, MouseButtons.Right)]
        public void EntryClick_SelectsThatEntry(int index, int x, int y, MouseButtons button)
        {
            PerformMouseClick(x, y, button);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[index]));
        }

        [TestCase(0, 30, 30, MouseButtons.Left)]
        [TestCase(1, 30, 50, MouseButtons.Left)]
        [TestCase(2, 30, 70, MouseButtons.Left)]
        [TestCase(3, 30, 110, MouseButtons.Left)]
        [TestCase(4, 30, 130, MouseButtons.Left)]
        [TestCase(5, 30, 150, MouseButtons.Left)]
        [TestCase(0, 30, 30, MouseButtons.Right)]
        [TestCase(1, 30, 50, MouseButtons.Right)]
        [TestCase(2, 30, 70, MouseButtons.Right)]
        [TestCase(3, 30, 110, MouseButtons.Right)]
        [TestCase(4, 30, 130, MouseButtons.Right)]
        [TestCase(5, 30, 150, MouseButtons.Right)]
        public void EntryClick_SelectsOnlyThatEntry(int index, int x, int y, MouseButtons button)
        {
            foreach (var entry in TestList.Entries)
                entry.Selected = true;

            PerformMouseClick(x, y, button);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[index]));
        }

        [TestCase(0, 30, 30, MouseButtons.Left)]
        [TestCase(1, 30, 50, MouseButtons.Left)]
        [TestCase(2, 30, 70, MouseButtons.Left)]
        [TestCase(3, 30, 110, MouseButtons.Left)]
        [TestCase(4, 30, 130, MouseButtons.Left)]
        [TestCase(5, 30, 150, MouseButtons.Left)]
        [TestCase(0, 30, 30, MouseButtons.Right)]
        [TestCase(1, 30, 50, MouseButtons.Right)]
        [TestCase(2, 30, 70, MouseButtons.Right)]
        [TestCase(3, 30, 110, MouseButtons.Right)]
        [TestCase(4, 30, 130, MouseButtons.Right)]
        [TestCase(5, 30, 150, MouseButtons.Right)]
        public void EntryClick_FocusesThatEntry(int index, int x, int y, MouseButtons button)
        {
            PerformMouseClick(x, y, button);
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[index]));
        }

        [TestCase(MouseButtons.Left)]
        [TestCase(MouseButtons.Right)]
        public void OutsideClick_SelectsNothing(MouseButtons button)
        {
            foreach (var entry in TestList.Entries)
                entry.Selected = true;

            PerformMouseClick(30, 170, button);
            Assert.That(TestList.SelectedEntries, Is.Empty);
        }

        [Test]
        public void LeftControlEntryClick_TooglesSelectionOfThatEntry()
        {
            Assert.That(TestList.SelectedEntries, Is.Empty);

            PerformMouseClick(30, 110, Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[3]));

            PerformMouseClick(30, 110, Keys.Control);
            Assert.That(TestList.SelectedEntries, Is.Empty);

            foreach (var entry in TestList.Entries)
                entry.Selected = true;

            PerformMouseClick(30, 110, Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(5));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[2]));
            Assert.That(TestList.SelectedEntries, Does.Not.Contain(TestList.Entries[3]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[4]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[5]));
        }

        [Test]
        public void RightControlEntryClick_KeepsCurrentSelection()
        {
            Assert.That(TestList.SelectedEntries, Is.Empty);

            PerformMouseClick(30, 130, MouseButtons.Right, Keys.Control);
            Assert.That(TestList.SelectedEntries, Is.Empty);

            foreach (var entry in TestList.Entries)
                entry.Selected = true;
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(6));

            PerformMouseClick(30, 130, MouseButtons.Right, Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(6));
        }

        [Test]
        public void LeftShiftEntryClick_SelectsAllEntriesBetweenLastAndCurrentClick()
        {
            PerformMouseClick(30, 50);

            PerformMouseClick(30, 70, Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[2]));

            PerformMouseClick(30, 30, Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));

            PerformMouseClick(30, 50, Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));

            PerformMouseClick(30, 130, Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(5));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[2]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[1]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[3]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[4]));
        }

        [Test]
        public void LeftShiftEntryClick_StartsAtFirstEntry()
        {
            PerformMouseClick(30, 50, Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(3));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
        }
    

        [Test]
        public void RightShiftEntryClick_KeepsCurrentSelection()
        {
            PerformMouseClick(30, 50);
            PerformMouseClick(30, 70, MouseButtons.Right, Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
        }

        [Test]
        public void UpDownArrow_MovesSelection()
        {
            PerformKeyPress(Keys.Down);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));

            PerformKeyPress(Keys.Up);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
            PerformKeyPress(Keys.Up);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));

            PerformKeyPress(Keys.Down);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            PerformKeyPress(Keys.Down);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            PerformKeyPress(Keys.Down);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[2]));
            PerformKeyPress(Keys.Down);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[1]));
        }

        [Test]
        public void UpDownArrow_StartsFromLastClick()
        {
            PerformMouseClick(30, 50);

            PerformKeyPress(Keys.Down);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[2]));
        }

        [Test]
        public void CtrlUpDownArrow_MovesFocusButKeepsSelection()
        {
            PerformMouseClick(30, 50);

            PerformKeyPress(Keys.Down | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[2]));
            PerformKeyPress(Keys.Down | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Groups[1]));

            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[2]));
        }

        [Test]
        public void CtrlUpDownArrow_MovesFocusWithoutSelection()
        {
            PerformKeyPress(Keys.Down | Keys.Control);
            Assert.That(TestList.SelectedEntries, Is.Empty);
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[0]));
            PerformKeyPress(Keys.Down | Keys.Control);
            Assert.That(TestList.SelectedEntries, Is.Empty);
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[1]));

            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(TestList.SelectedEntries, Is.Empty);
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[0]));
            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(TestList.SelectedEntries, Is.Empty);
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Groups[0]));
            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(TestList.SelectedEntries, Is.Empty);
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Groups[0]));
        }

        [Test]
        public void ShiftUpDownArrow_ExtendsSelectionFromLastClick()
        {
            PerformMouseClick(30, 30);

            PerformKeyPress(Keys.Down | Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[1]));
            PerformKeyPress(Keys.Down | Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(3));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[2]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[2]));
            PerformKeyPress(Keys.Down | Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(4));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[2]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[1]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Groups[1]));

            PerformKeyPress(Keys.Up | Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(3));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[2]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[2]));
        }

        [Test]
        public void Space_AddsFocusedEntryToSelection()
        {
            PerformKeyPress(Keys.Space);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
            PerformKeyPress(Keys.Space);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));

            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);

            PerformKeyPress(Keys.Space);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
        }

        [Test]
        public void CtrlSpace_TogglesSelectionOfFocusedEntry()
        {
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(TestList.SelectedEntries, Is.Empty);
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));

            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);

            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
        }

        [Test]
        public void ShiftSpace_SelectsRangeBetweenLastClickAndFocusedEntry()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);

            PerformKeyPress(Keys.Space | Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(4));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[2]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[1]));
        }

        [Test]
        public void Left_SelectsGroupEntry()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Groups[0]));

            PerformMouseClick(30, 130);
            PerformKeyPress(Keys.Left);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[1]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Groups[1]));
        }

        [Test]
        public void CtrlLeft_MovesFocusFocusButKeepsSelection()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Groups[0]));

            PerformMouseClick(30, 130);
            PerformKeyPress(Keys.Left | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[4]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Groups[1]));
        }

        [Test]
        public void ShiftLeft_SelectsRangeBetweenLastClickAndGroupEntry()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left | Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Groups[0]));

            PerformMouseClick(30, 130);
            PerformKeyPress(Keys.Left | Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(3));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[1]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[3]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[4]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Groups[1]));
        }

        [TestCase(Keys.None)]
        [TestCase(Keys.Control)]
        [TestCase(Keys.Shift)]
        public void Left_CollapsesGroupEntry(Keys modifierKeys)
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left | modifierKeys);
            Assert.That(TestList.Groups[0].IsExpanded);
            PerformKeyPress(Keys.Left | modifierKeys);
            Assert.That(!TestList.Groups[0].IsExpanded);
            PerformKeyPress(Keys.Left | modifierKeys);
            Assert.That(!TestList.Groups[0].IsExpanded);
        }

        [Test]
        public void Right_SelectsFirstEntryInGroup()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Right);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[0]));

            PerformMouseClick(30, 90);
            PerformKeyPress(Keys.Right);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[3]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[3]));
        }

        [Test]
        public void Right_StaysOnCurrentlyFocusedTestEntry()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Down);
            PerformKeyPress(Keys.Down);
            PerformKeyPress(Keys.Right);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[1]));
            PerformKeyPress(Keys.Right);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[1]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[1]));
        }

        [Test]
        public void CtrlRight_MovesFocusFocusButKeepsSelection()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Right | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[0]));

            PerformMouseClick(30, 90);
            PerformKeyPress(Keys.Right | Keys.Control);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(1));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[1]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[3]));
        }

        [Test]
        public void ShiftRight_SelectsRangeBetweenLastClickAndFirstGroupEntry()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Right | Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[0]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[0]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[0]));

            PerformMouseClick(30, 90);
            PerformKeyPress(Keys.Right | Keys.Shift);
            Assert.That(TestList.SelectedEntries.Count, Is.EqualTo(2));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Groups[1]));
            Assert.That(TestList.SelectedEntries, Contains.Item(TestList.Entries[3]));
            Assert.That(Controller.FocusedEntry, Is.SameAs(TestList.Entries[3]));
        }

        [TestCase(Keys.None)]
        [TestCase(Keys.Control)]
        [TestCase(Keys.Shift)]
        public void Right_ExpandsGroupEntry(Keys modifierKeys)
        {
            PerformMouseClick(30, 10);
            TestList.Groups[0].IsExpanded = false;

            PerformKeyPress(Keys.Right | modifierKeys);
            Assert.That(TestList.Groups[0].IsExpanded);
            PerformKeyPress(Keys.Right | modifierKeys);
            Assert.That(TestList.Groups[0].IsExpanded);
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

        private void PerformKeyPress(Keys keys)
        {
            Controller.HandleKeyDown(new KeyEventArgs(keys));
        }
    }
}
