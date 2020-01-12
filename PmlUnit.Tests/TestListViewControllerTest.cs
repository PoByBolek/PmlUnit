// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestListViewController))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TestListViewControllerTest
    {
        private TestListView TestList;
        private TestListViewModel Model;
        private ReadOnlyTestListEntryCollection VisibleEntries;
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
            // Act
            PerformMouseClick(x, y, button);
            // Assert
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(index)));
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
            // Arrange
            foreach (var entry in VisibleEntries)
                entry.IsSelected = true;
            // Act
            PerformMouseClick(x, y, button);
            // Assert
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(index)));
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
            // Act
            PerformMouseClick(x, y, button);
            // Assert
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[index]));
        }

        [TestCase(MouseButtons.Left)]
        [TestCase(MouseButtons.Right)]
        public void OutsideClick_SelectsNothing(MouseButtons button)
        {
            // Arrange
            foreach (var entry in VisibleEntries)
                entry.IsSelected = true;
            // Act
            PerformMouseClick(30, 170, button);
            // Assert
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
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(5)));

            PerformMouseClick(30, 110, Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);

            foreach (var entry in VisibleEntries)
                entry.IsSelected = true;

            PerformMouseClick(30, 110, Keys.Control);
            var expected = VisibleEntries.Where((entry, index) => index != 5);
            Assert.That(SelectedEntries, Is.EquivalentTo(expected));
        }

        [Test]
        public void RightControlEntryClick_KeepsCurrentSelection()
        {
            Assert.That(SelectedEntries, Is.Empty);

            PerformMouseClick(30, 130, MouseButtons.Right, Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);

            foreach (var entry in VisibleEntries)
                entry.IsSelected = true;
            Assert.That(SelectedEntries, Is.EquivalentTo(VisibleEntries));

            PerformMouseClick(30, 130, MouseButtons.Right, Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(VisibleEntries));
        }

        [Test]
        public void LeftShiftEntryClick_SelectsVisibleEntriesBetweenLastAndCurrentClick()
        {
            PerformMouseClick(30, 50);

            PerformMouseClick(30, 70, Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2, 3)));

            PerformMouseClick(30, 30, Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1, 2)));

            PerformMouseClick(30, 50, Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));

            PerformMouseClick(30, 130, Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2, 6)));
        }

        [Test]
        public void LeftShiftEntryClick_StartsAtFirstEntry()
        {
            PerformMouseClick(30, 50, Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0, 2)));
        }
    

        [Test]
        public void RightShiftEntryClick_KeepsCurrentSelection()
        {
            PerformMouseClick(30, 50);
            PerformMouseClick(30, 70, MouseButtons.Right, Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
        }

        [Test]
        public void UpDownArrow_MovesSelection()
        {
            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1)));

            PerformKeyPress(Keys.Up);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));
            PerformKeyPress(Keys.Up);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));

            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1)));
            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(3)));
            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(4)));
        }

        [Test]
        public void UpDownArrow_StartsFromLastClick()
        {
            PerformMouseClick(30, 50);

            PerformKeyPress(Keys.Down);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(3)));
        }

        [Test]
        public void CtrlUpDownArrow_MovesFocusButKeepsSelection()
        {
            PerformMouseClick(30, 50);

            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));
            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
            PerformKeyPress(Keys.Up | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformKeyPress(Keys.Down | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
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
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1, 2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[2]));
            PerformKeyPress(Keys.Down | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1, 3)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[3]));
            PerformKeyPress(Keys.Down | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1, 4)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[4]));

            PerformKeyPress(Keys.Up | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1, 3)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[3]));
        }

        [Test]
        public void PageUpPageDown_MovesSelection()
        {
            TestList.Size = new Size(300, 50);

            PerformKeyPress(Keys.PageDown);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            PerformKeyPress(Keys.PageDown);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(4)));
            PerformKeyPress(Keys.PageDown);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(6)));

            PerformKeyPress(Keys.PageUp);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(4)));
            PerformKeyPress(Keys.PageUp);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            PerformKeyPress(Keys.PageUp);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));
        }

        [Test]
        public void CtrlPageUpPageDown_MovesFocusButKeepsSelection()
        {
            TestList.Size = new Size(300, 50);

            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.PageUp | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
            PerformKeyPress(Keys.PageUp | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.PageDown | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[3]));
            PerformKeyPress(Keys.PageDown | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[5]));
        }

        [Test]
        public void CtrlPageUpPageDown_MovesFocusWithoutSelection()
        {
            TestList.Size = new Size(300, 50);

            PerformKeyPress(Keys.PageDown | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[2]));
            PerformKeyPress(Keys.PageDown | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[4]));

            PerformKeyPress(Keys.PageUp | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[2]));
            PerformKeyPress(Keys.PageUp | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
        }

        [Test]
        public void ShiftPageUpPageDown_ExtendsSelectionFromLastClick()
        {
            TestList.Size = new Size(300, 50);

            PerformMouseClick(30, 30);

            PerformKeyPress(Keys.PageUp | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0, 1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformKeyPress(Keys.PageDown | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1, 2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[2]));
            PerformKeyPress(Keys.Down | Keys.Shift);
            PerformKeyPress(Keys.PageDown | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1, 5)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[5]));
        }

        [Test]
        public void HomeEnd_MovesSelection()
        {
            PerformKeyPress(Keys.Home);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));
            PerformKeyPress(Keys.Home);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));

            PerformKeyPress(Keys.Down);
            PerformKeyPress(Keys.Down);
            PerformKeyPress(Keys.Down);
            PerformKeyPress(Keys.Home);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));

            PerformKeyPress(Keys.End);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(-1)));
            PerformKeyPress(Keys.End);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(-1)));

            PerformKeyPress(Keys.Up);
            PerformKeyPress(Keys.Up);
            PerformKeyPress(Keys.Up);
            PerformKeyPress(Keys.End);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(-1)));
        }

        [Test]
        public void CtrlHomeEnd_MovesFocusButKeepsSelection()
        {
            PerformMouseClick(30, 50);

            PerformKeyPress(Keys.Home | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
            PerformKeyPress(Keys.Home | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformKeyPress(Keys.End | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[VisibleEntries.Count - 1]));
            PerformKeyPress(Keys.End | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[VisibleEntries.Count - 1]));
        }

        [Test]
        public void CtrlHomeEnd_MovesFocusWithoutSelection()
        {
            PerformKeyPress(Keys.End | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[VisibleEntries.Count - 1]));
            PerformKeyPress(Keys.End | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[VisibleEntries.Count - 1]));
            
            PerformKeyPress(Keys.Home | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
            PerformKeyPress(Keys.Home | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
        }

        [Test]
        public void ShiftHomeEnd_ExtendsSelectionFromLastClick()
        {
            PerformMouseClick(30, 50);

            PerformKeyPress(Keys.Home | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0, 2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformKeyPress(Keys.End | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2, VisibleEntries.Count - 1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[VisibleEntries.Count - 1]));
            PerformKeyPress(Keys.End | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2, VisibleEntries.Count - 1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[VisibleEntries.Count - 1]));
        }

        [Test]
        public void Space_AddsFocusedEntryToSelection()
        {
            PerformKeyPress(Keys.Space);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));
            PerformKeyPress(Keys.Space);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));

            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);

            PerformKeyPress(Keys.Space);
            var expected = Slice(0).Concat(Slice(2));
            Assert.That(SelectedEntries, Is.EquivalentTo(expected));
        }

        [Test]
        public void CtrlSpace_TogglesSelectionOfFocusedEntry()
        {
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(SelectedEntries, Is.Empty);
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));

            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);

            PerformKeyPress(Keys.Space | Keys.Control);
            var expected = Slice(0).Concat(Slice(2));
            Assert.That(SelectedEntries, Is.EquivalentTo(expected));
            PerformKeyPress(Keys.Space | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));
        }

        [Test]
        public void ShiftSpace_SelectsRangeBetweenLastClickAndFocusedEntry()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);
            PerformKeyPress(Keys.Down | Keys.Control);

            PerformKeyPress(Keys.Space | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1, 4)));
        }

        [Test]
        public void Left_SelectsGroupEntry()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformMouseClick(30, 130);
            PerformKeyPress(Keys.Left);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
        }

        [Test]
        public void CtrlLeft_MovesFocusFocusButKeepsSelection()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformMouseClick(30, 130);
            PerformKeyPress(Keys.Left | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(6)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));
        }

        [Test]
        public void ShiftLeft_SelectsRangeBetweenLastClickAndGroupEntry()
        {
            PerformMouseClick(30, 30);
            PerformKeyPress(Keys.Left | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0, 1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[0]));

            PerformMouseClick(30, 70);
            PerformKeyPress(Keys.Left | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0, 3)));
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
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));
        }

        [Test]
        public void Right_StaysOnCurrentlyFocusedTestEntry()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Down);
            PerformKeyPress(Keys.Down);
            PerformKeyPress(Keys.Right);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[2]));
            PerformKeyPress(Keys.Right);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(2)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[2]));
        }

        [Test]
        public void CtrlRight_MovesFocusFocusButKeepsSelection()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Right | Keys.Control);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));
        }

        [Test]
        public void ShiftRight_SelectsRangeBetweenLastClickAndFirstGroupEntry()
        {
            PerformMouseClick(30, 10);
            PerformKeyPress(Keys.Right | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(0, 1)));
            Assert.That(Model.FocusedEntry, Is.SameAs(VisibleEntries[1]));

            PerformMouseClick(30, 70);
            PerformKeyPress(Keys.Up | Keys.Control);
            PerformKeyPress(Keys.Up | Keys.Control);
            PerformKeyPress(Keys.Up | Keys.Control);
            PerformKeyPress(Keys.Right | Keys.Shift);
            Assert.That(SelectedEntries, Is.EquivalentTo(Slice(1, 3)));
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

        private IEnumerable<TestListEntry> Slice(int index)
        {
            return Slice(index, index);
        }

        private IEnumerable<TestListEntry> Slice(int start, int end)
        {
            if (start < 0)
                start += VisibleEntries.Count;
            if (end < 0)
                end += VisibleEntries.Count;

            var result = new List<TestListEntry>();
            for (int i = start; i <= end; i++)
                result.Add(VisibleEntries[i]);
            return result;
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
