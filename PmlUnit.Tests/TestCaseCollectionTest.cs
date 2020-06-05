// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(TestCaseCollection))]
    public class TestCaseCollectionTest
    {
        private TestCaseCollection Collection;
        private TestCase First;
        private TestCase Second;

        [SetUp]
        public void Setup()
        {
            Collection = new TestCaseCollection();
            First = new TestCase("First", "first.pmlobj");
            Second = new TestCase("Second", "second.pmlobj");
        }

        [Test]
        public void Add_ChecksForNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => Collection.Add(null));
        }

        [Test]
        public void Add_ChecksForDuplicateTestCaseNames()
        {
            Collection.Add(First);
            Assert.Throws<ArgumentException>(() => Collection.Add(First));
            Assert.Throws<ArgumentException>(() => Collection.Add(new TestCase("first", "first.pmlobj")));
            Assert.Throws<ArgumentException>(() => Collection.Add(new TestCase("FIRST", "first.pmlobj")));
        }

        [Test]
        public void Add_AddsTheTestCaseUnderItsName()
        {
            Collection.Add(First);
            Collection.Add(Second);
            Assert.That(Collection["first"], Is.SameAs(First));
            Assert.That(Collection["First"], Is.SameAs(First));
            Assert.That(Collection["FiRsT"], Is.SameAs(First));
            Assert.That(Collection["FIRST"], Is.SameAs(First));
            Assert.That(Collection["second"], Is.SameAs(Second));
            Assert.That(Collection["Second"], Is.SameAs(Second));
            Assert.That(Collection["SECOND"], Is.SameAs(Second));
        }

        [Test]
        public void Add_IncreasesTheCollectionsCount()
        {

            Assert.That(Collection.Count, Is.EqualTo(0));

            Collection.Add(First);
            Assert.That(Collection.Count, Is.EqualTo(1));

            Collection.Add(Second);
            Assert.That(Collection.Count, Is.EqualTo(2));
        }

        [Test]
        public void Add_RaisesChangedEvent()
        {
            TestCase expected = null;
            int raised = 0;
            Collection.Changed += (sender, e) =>
            {
                ++raised;
                Assert.That(e.AddedTestCases.Count(), Is.EqualTo(1));
                Assert.That(e.AddedTestCases.FirstOrDefault(), Is.SameAs(expected));
                Assert.That(e.RemovedTestCases, Is.Empty);
            };

            expected = First;
            Collection.Add(expected);
            Assert.That(raised, Is.EqualTo(1));

            expected = Second;
            Collection.Add(expected);
            Assert.That(raised, Is.EqualTo(2));
        }

        [Test]
        public void AddRange_ChecksForNull()
        {
            Assert.Throws<ArgumentNullException>(() => Collection.AddRange(null));
            Assert.Throws<ArgumentException>(() => Collection.AddRange(Enumerable.Repeat<TestCase>(null, 1)));
            var list = new List<TestCase>();
            list.Add(First);
            list.Add(null);
            list.Add(Second);
            Assert.Throws<ArgumentException>(() => Collection.AddRange(list));
        }

        [Test]
        public void AddRange_ChecksForDuplicateNames()
        {
            var duplicateList = new List<TestCase>()
            {
                new TestCase("FoO", "foo.pmlobj"),
                new TestCase("bar", "bar.pmlobj"),
                new TestCase("fOo", "foo.pmlobj"),
            };
            Assert.Throws<ArgumentException>(() => Collection.AddRange(duplicateList));

            Collection.Add(new TestCase("bAr", "bar.pmlobj"));
            var duplicateCollection = new List<TestCase>()
            {
                new TestCase("foo", "foo.pmlobj"),
                new TestCase("BaR", "bar.pmlobj"),
            };
            Assert.Throws<ArgumentException>(() => Collection.AddRange(duplicateCollection));
        }

        [Test]
        public void AddRange_TryingToAddDuplicatesDoesNotModifyCollection()
        {
            Collection.Add(First);
            var list = new List<TestCase>()
            {
                Second,
                new TestCase("First", "first.pmlobj"),
                new TestCase("Third", "third.pmlobj"),
            };
            Assert.Throws<ArgumentException>(() => Collection.AddRange(list));
            Assert.That(Collection.Count, Is.EqualTo(1));
            Assert.That(Collection["First"], Is.SameAs(First));

            list = new List<TestCase>()
            {
                new TestCase("Fourth", "fourth.pmlobj"),
                new TestCase("Fifth", "fifth.pmlobj"),
                new TestCase("Fifth", "fifth.pmlobj"),
                new TestCase("Sixth", "sixth.pmlobj"),
            };
            Assert.Throws<ArgumentException>(() => Collection.AddRange(list));
            Assert.That(Collection.Count, Is.EqualTo(1));
            Assert.That(Collection["First"], Is.SameAs(First));
        }

        [Test]
        public void AddRange_AddsTheSpecifiedTestCases()
        {
            var list = new List<TestCase>()
            {
                First, Second, new TestCase("Third", "third.pmlobj"),
            };
            Collection.AddRange(list);

            Assert.That(Collection.Count, Is.EqualTo(3));
            Assert.That(Collection, Contains.Item(list[0]));
            Assert.That(Collection, Contains.Item(list[1]));
            Assert.That(Collection, Contains.Item(list[2]));
        }

        [Test]
        public void AddRange_RaisesChangedEvent()
        {
            var list = new List<TestCase>()
            {
                First, Second, new TestCase("Third", "third.pmlobj"),
            };
            int raised = 0;
            Collection.Changed += (sender, e) =>
            {
                ++raised;
                Assert.That(e.AddedTestCases.Count(), Is.EqualTo(3));
                Assert.That(e.AddedTestCases, Contains.Item(list[0]));
                Assert.That(e.AddedTestCases, Contains.Item(list[1]));
                Assert.That(e.AddedTestCases, Contains.Item(list[2]));
                Assert.That(e.RemovedTestCases, Is.Empty);
            };
            Collection.AddRange(list);
            Assert.That(raised, Is.EqualTo(1));

            Collection.AddRange(Enumerable.Empty<TestCase>());
            Assert.That(raised, Is.EqualTo(1));
        }

        [Test]
        public void AddRange_OnlyEvaluatesCollectionOnce()
        {

            Collection.AddRange(new OneTimeEnumerable());
        }

        [Test]
        public void GetItem_ChecksForNull()
        {
            Assert.Throws<ArgumentNullException>(() => { var x = Collection[null]; });
        }

        [Test]
        public void GetItem_RaisesKeyNotFoundExceptionForMissingKeys()
        {
            Assert.Throws<KeyNotFoundException>(() => { var foo = Collection["foo"]; });

            Collection.Add(new TestCase("foo", "foo.pmlobj"));
            var x = Collection["foo"];

            Assert.Throws<KeyNotFoundException>(() => { var foo = Collection["bar"]; });
        }

        [Test]
        public void SetItem_ChecksForNull()
        {
            Assert.Throws<ArgumentNullException>(() => Collection[null] = null);
            Assert.Throws<ArgumentNullException>(() => Collection["foo"] = null);
            Assert.Throws<ArgumentNullException>(() => Collection[null] = new TestCase("foo", "foo.pmlobj"));
        }

        [Test]
        public void SetItem_ChecksThatKeyAndNameMatch()
        {
            Assert.Throws<ArgumentException>(() => Collection["foo"] = new TestCase("bar", "bar.pmlobj"));
            Collection["foo"] = new TestCase("foo", "foo.pmlobj");
            Collection["FoO"] = new TestCase("Foo", "foo.pmlobj");
            Collection["fOo"] = new TestCase("FoO", "foo.pmlobj");
            Collection["foo"] = new TestCase("FOO", "foo.pmlobj");
        }

        [Test]
        public void SetItem_RaisesChangedEvent()
        {
            TestCase expectedAdd = null;
            TestCase expectedRemove = null;
            int raised = 0;
            Collection.Changed += (sender, e) =>
            {
                raised++;
                Assert.That(e.AddedTestCases.Count(), Is.EqualTo(1));
                Assert.That(e.AddedTestCases.FirstOrDefault(), Is.SameAs(expectedAdd));

                if (expectedRemove == null)
                {
                    Assert.That(e.RemovedTestCases, Is.Empty);
                }
                else
                {
                    Assert.That(e.RemovedTestCases.Count(), Is.EqualTo(1));
                    Assert.That(e.RemovedTestCases.FirstOrDefault(), Is.SameAs(expectedRemove));
                }
            };

            expectedAdd = new TestCase("key", "key.pmlobj");
            Collection["key"] = expectedAdd;
            Assert.That(raised, Is.EqualTo(1));

            expectedRemove = expectedAdd;
            expectedAdd = new TestCase("Key", "key.pmlobj");
            Collection["key"] = expectedAdd;
            Assert.That(raised, Is.EqualTo(2));

            Collection["key"] = expectedAdd;
            Assert.That(raised, Is.EqualTo(2));
        }

        [Test]
        public void Contains_CheckForNull()
        {
            Assert.Throws<ArgumentNullException>(() => Collection.Contains(null));
        }

        [Test]
        public void Contains_WorksWithEmptyCollection()
        {
            Assert.That(Collection.Contains(new TestCase("Foo", "foo.pmlobj")), Is.False);
        }

        [Test]
        public void Contains_ChecksForObjectIdentity()
        {
            Collection.Add(First);
            Collection.Add(Second);

            Assert.That(Collection.Contains(First));
            Assert.That(Collection.Contains(Second));
            Assert.That(Collection.Contains(new TestCase("First", "first.pmlobj")), Is.False);
            Assert.That(Collection.Contains(new TestCase("Second", "second.pmlobj")), Is.False);
        }

        [Test]
        public void Remove_ChecksForNull()
        {
            Assert.Throws<ArgumentNullException>(() => Collection.Remove(null));
        }

        [Test]
        public void Remove_RemovesTheSpecifiedTestFromTheCollection()
        {
            Collection.Add(First);
            Collection.Add(Second);

            Assert.That(Collection.Remove(Second), Is.True);
            Assert.That(Collection.Count, Is.EqualTo(1));
            Assert.That(Collection, Contains.Item(First));

            Assert.That(Collection.Remove(First), Is.True);
            Assert.That(Collection, Is.Empty);
        }

        [Test]
        public void Remove_WorksWithEmptyCollection()
        {
            Assert.That(Collection.Remove(new TestCase("foo", "foo.pmlobj")), Is.False);
        }

        [Test]
        public void Remove_ChecksForObjectIdentity()
        {
            Collection.Add(First);
            Collection.Add(Second);

            Assert.That(Collection.Remove(new TestCase("Second", "second.pmlobj")), Is.False);
            Assert.That(Collection.Count, Is.EqualTo(2));
            Assert.That(Collection, Contains.Item(First));
            Assert.That(Collection, Contains.Item(Second));
        }

        [Test]
        public void Remove_RaisesChangedEvent()
        {
            Collection.Add(First);
            Collection.Add(Second);
            int raised = 0;
            TestCase expected = null;
            Collection.Changed += (sender, e) =>
            {
                ++raised;
                Assert.That(e.AddedTestCases, Is.Empty);
                Assert.That(e.RemovedTestCases.Count(), Is.EqualTo(1));
                Assert.That(e.RemovedTestCases, Contains.Item(expected));
            };

            expected = First;
            Collection.Remove(First);
            Assert.That(raised, Is.EqualTo(1));

            Collection.Remove(First);
            Assert.That(raised, Is.EqualTo(1));

            Collection.Remove(new TestCase("Second", "second.pmlobj"));
            Assert.That(raised, Is.EqualTo(1));

            expected = Second;
            Collection.Remove(Second);
            Assert.That(raised, Is.EqualTo(2));

            Collection.Remove(Second);
            Assert.That(raised, Is.EqualTo(2));
        }

        [Test]
        public void Clear_ClearsTheCollection()
        {
            Collection.Add(new TestCase("First", "first.pmlobj"));
            Collection.Add(new TestCase("Second", "second.pmlobj"));
            Collection.Clear();
            Assert.That(Collection, Is.Empty);
        }

        [Test]
        public void Clear_RaisesChangedEvent()
        {
            var first = new TestCase("First", "first.pmlobj");
            var second = new TestCase("Second", "second.pmlobj");
            int raised = 0;
            Collection.Add(first);
            Collection.Add(second);
            Collection.Changed += (sender, e) =>
            {
                ++raised;
                Assert.That(e.AddedTestCases, Is.Empty);
                Assert.That(e.RemovedTestCases.Count(), Is.EqualTo(2));
                Assert.That(e.RemovedTestCases, Contains.Item(first));
                Assert.That(e.RemovedTestCases, Contains.Item(second));
            };

            Collection.Clear();
            Assert.That(raised, Is.EqualTo(1));

            Collection.Clear();
            Assert.That(raised, Is.EqualTo(1));
        }

        private class OneTimeEnumerable : IEnumerable<TestCase>
        {
            private bool Enumerated = false;

            public IEnumerator<TestCase> GetEnumerator()
            {
                if (Enumerated)
                    Assert.Fail("Should only enumerate once");
                Enumerated = true;
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
