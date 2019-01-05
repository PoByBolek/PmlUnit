// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(Instant))]
    public class InstantTest
    {
        [Test]
        public void FromMilliseconds_CalculatesCorrectTicks()
        {
            Assert.AreEqual(0, Instant.FromMilliseconds(0).Ticks);
            Assert.AreEqual(10000, Instant.FromMilliseconds(1).Ticks);
            Assert.AreEqual(20000, Instant.FromMilliseconds(2).Ticks);
            Assert.AreEqual(12345 * 10000, Instant.FromMilliseconds(12345).Ticks);
            Assert.AreEqual(-300000, Instant.FromMilliseconds(-30).Ticks);
        }
        [Test]
        public void FromSeconds_CalculatesCorrectTicks()
        {
            Assert.AreEqual(0, Instant.FromSeconds(0).Ticks);
            Assert.AreEqual(5L * 1000 * 10000, Instant.FromSeconds(5).Ticks);
            Assert.AreEqual(21L * 1000 * 10000, Instant.FromSeconds(21).Ticks);
            Assert.AreEqual(1337L * 1000 * 10000, Instant.FromSeconds(1337).Ticks);
            Assert.AreEqual(-42L * 1000 * 10000, Instant.FromSeconds(-42).Ticks);
        }

        [Test]
        public void FromTicks_UsesSpecifiedValue()
        {
            Assert.AreEqual(0, Instant.FromTicks(0).Ticks);
            Assert.AreEqual(1, Instant.FromTicks(1).Ticks);
            Assert.AreEqual(123456789, Instant.FromTicks(123456789).Ticks);
            Assert.AreEqual(long.MaxValue, Instant.FromTicks(long.MaxValue).Ticks);
            Assert.AreEqual(long.MinValue, Instant.FromTicks(long.MinValue).Ticks);
        }

        [Test]
        public void Operator_Plus()
        {
            Assert.AreEqual(0, (Instant.FromTicks(0) + TimeSpan.FromTicks(0)).Ticks);
            Assert.AreEqual(1234, (Instant.FromTicks(1234) + TimeSpan.FromTicks(0)).Ticks);
            Assert.AreEqual(5678, (Instant.FromTicks(0) + TimeSpan.FromTicks(5678)).Ticks);

            Assert.AreEqual(-500, (Instant.FromTicks(678) + TimeSpan.FromTicks(-1178)).Ticks);
            Assert.AreEqual(1000, (Instant.FromTicks(123) + TimeSpan.FromTicks(877)).Ticks);
            Assert.AreEqual(2000, (Instant.FromTicks(2123) + TimeSpan.FromTicks(-123)).Ticks);

            Assert.AreEqual(long.MinValue, (Instant.FromTicks(long.MaxValue) + TimeSpan.FromTicks(1)).Ticks);
            Assert.AreEqual(long.MinValue, (Instant.FromTicks(1) + TimeSpan.FromTicks(long.MaxValue)).Ticks);
            Assert.AreEqual(long.MaxValue, (Instant.FromTicks(-1) + TimeSpan.FromTicks(long.MinValue)).Ticks);
            Assert.AreEqual(long.MaxValue, (Instant.FromTicks(long.MinValue) + TimeSpan.FromTicks(-1)).Ticks);

            Assert.AreEqual(0, (TimeSpan.FromTicks(0) + Instant.FromTicks(0)).Ticks);
            Assert.AreEqual(1234, (TimeSpan.FromTicks(0) + Instant.FromTicks(1234)).Ticks);
            Assert.AreEqual(5678, (TimeSpan.FromTicks(5678) + Instant.FromTicks(0)).Ticks);

            Assert.AreEqual(-500, (TimeSpan.FromTicks(-1178) + Instant.FromTicks(678)).Ticks);
            Assert.AreEqual(1000, (TimeSpan.FromTicks(877) + Instant.FromTicks(123)).Ticks);
            Assert.AreEqual(2000, (TimeSpan.FromTicks(-123) + Instant.FromTicks(2123)).Ticks);

            Assert.AreEqual(long.MinValue, (TimeSpan.FromTicks(1) + Instant.FromTicks(long.MaxValue)).Ticks);
            Assert.AreEqual(long.MinValue, (TimeSpan.FromTicks(long.MaxValue) + Instant.FromTicks(1)).Ticks);
            Assert.AreEqual(long.MaxValue, (TimeSpan.FromTicks(long.MinValue) + Instant.FromTicks(-1)).Ticks);
            Assert.AreEqual(long.MaxValue, (TimeSpan.FromTicks(-1) + Instant.FromTicks(long.MinValue)).Ticks);
        }

        [Test]
        public void Operator_Minus()
        {
            Assert.AreEqual(0, (Instant.FromTicks(0) - TimeSpan.FromTicks(0)).Ticks);
            Assert.AreEqual(1234, (Instant.FromTicks(1234) - TimeSpan.FromTicks(0)).Ticks);
            Assert.AreEqual(-5678, (Instant.FromTicks(0) - TimeSpan.FromTicks(5678)).Ticks);

            Assert.AreEqual(-500, (Instant.FromTicks(678) - TimeSpan.FromTicks(1178)).Ticks);
            Assert.AreEqual(1000, (Instant.FromTicks(123) - TimeSpan.FromTicks(-877)).Ticks);
            Assert.AreEqual(2000, (Instant.FromTicks(2123) - TimeSpan.FromTicks(123)).Ticks);

            Assert.AreEqual(long.MinValue, (Instant.FromTicks(long.MaxValue) - TimeSpan.FromTicks(-1)).Ticks);
            Assert.AreEqual(long.MinValue, (Instant.FromTicks(-1) - TimeSpan.FromTicks(long.MaxValue)).Ticks);
            Assert.AreEqual(long.MaxValue, (Instant.FromTicks(long.MinValue) - TimeSpan.FromTicks(1)).Ticks);

            Assert.AreEqual(0, (Instant.FromTicks(0) - Instant.FromTicks(0)).Ticks);
            Assert.AreEqual(-1234, (Instant.FromTicks(0) - Instant.FromTicks(1234)).Ticks);
            Assert.AreEqual(5678, (Instant.FromTicks(5678) - Instant.FromTicks(0)).Ticks);

            Assert.AreEqual(-500, (Instant.FromTicks(178) - Instant.FromTicks(678)).Ticks);
            Assert.AreEqual(1000, (Instant.FromTicks(877) - Instant.FromTicks(-123)).Ticks);
            Assert.AreEqual(2000, (Instant.FromTicks(-123) - Instant.FromTicks(-2123)).Ticks);

            Assert.AreEqual(long.MinValue, (Instant.FromTicks(-1) - Instant.FromTicks(long.MaxValue)).Ticks);
            Assert.AreEqual(long.MinValue, (Instant.FromTicks(long.MaxValue) - Instant.FromTicks(-1)).Ticks);
            Assert.AreEqual(long.MaxValue, (Instant.FromTicks(long.MinValue) - Instant.FromTicks(1)).Ticks);
        }

        [Test]
        public void Operator_LessThan()
        {
            Assert.IsTrue(Instant.FromTicks(0) < Instant.FromTicks(1));
            Assert.IsTrue(Instant.FromTicks(1) < Instant.FromTicks(2));
            Assert.IsTrue(Instant.FromTicks(-1) < Instant.FromTicks(0));
            Assert.IsTrue(Instant.FromTicks(-1) < Instant.FromTicks(1));
            Assert.IsTrue(Instant.FromTicks(-1) < Instant.FromTicks(2));
            Assert.IsTrue(Instant.FromTicks(long.MinValue) < Instant.FromTicks(long.MaxValue));
            Assert.IsTrue(Instant.FromTicks(long.MinValue + 2) < Instant.FromTicks(long.MaxValue - 5));

            Assert.IsFalse(Instant.FromTicks(0) < Instant.FromTicks(0));
            Assert.IsFalse(Instant.FromTicks(-42) < Instant.FromTicks(-42));
            Assert.IsFalse(Instant.FromTicks(long.MinValue) < Instant.FromTicks(long.MinValue));
            Assert.IsFalse(Instant.FromTicks(long.MaxValue) < Instant.FromTicks(long.MaxValue));

            Assert.IsFalse(Instant.FromTicks(1) < Instant.FromTicks(0));
            Assert.IsFalse(Instant.FromTicks(4) < Instant.FromTicks(2));
            Assert.IsFalse(Instant.FromTicks(1) < Instant.FromTicks(-1));
            Assert.IsFalse(Instant.FromTicks(long.MaxValue) < Instant.FromTicks(long.MinValue));
            Assert.IsFalse(Instant.FromTicks(long.MaxValue - 42) < Instant.FromTicks(long.MinValue + 17));
        }

        [Test]
        public void Operator_LessThanOrEqual()
        {
            Assert.IsTrue(Instant.FromTicks(0) <= Instant.FromTicks(1));
            Assert.IsTrue(Instant.FromTicks(1) <= Instant.FromTicks(2));
            Assert.IsTrue(Instant.FromTicks(-1) <= Instant.FromTicks(0));
            Assert.IsTrue(Instant.FromTicks(-1) <= Instant.FromTicks(1));
            Assert.IsTrue(Instant.FromTicks(-1) <= Instant.FromTicks(2));
            Assert.IsTrue(Instant.FromTicks(long.MinValue) <= Instant.FromTicks(long.MaxValue));
            Assert.IsTrue(Instant.FromTicks(long.MinValue + 2) <= Instant.FromTicks(long.MaxValue - 5));

            Assert.IsTrue(Instant.FromTicks(0) <= Instant.FromTicks(0));
            Assert.IsTrue(Instant.FromTicks(-42) <= Instant.FromTicks(-42));
            Assert.IsTrue(Instant.FromTicks(long.MinValue) <= Instant.FromTicks(long.MinValue));
            Assert.IsTrue(Instant.FromTicks(long.MaxValue) <= Instant.FromTicks(long.MaxValue));

            Assert.IsFalse(Instant.FromTicks(1) <= Instant.FromTicks(0));
            Assert.IsFalse(Instant.FromTicks(4) <= Instant.FromTicks(2));
            Assert.IsFalse(Instant.FromTicks(1) <= Instant.FromTicks(-1));
            Assert.IsFalse(Instant.FromTicks(long.MaxValue) <= Instant.FromTicks(long.MinValue));
            Assert.IsFalse(Instant.FromTicks(long.MaxValue - 42) <= Instant.FromTicks(long.MinValue + 17));
        }

        [Test]
        public void Operator_GreaterThanOrEqual()
        {
            Assert.IsFalse(Instant.FromTicks(0) >= Instant.FromTicks(1));
            Assert.IsFalse(Instant.FromTicks(1) >= Instant.FromTicks(2));
            Assert.IsFalse(Instant.FromTicks(-1) >= Instant.FromTicks(0));
            Assert.IsFalse(Instant.FromTicks(-1) >= Instant.FromTicks(1));
            Assert.IsFalse(Instant.FromTicks(-1) >= Instant.FromTicks(2));
            Assert.IsFalse(Instant.FromTicks(long.MinValue) >= Instant.FromTicks(long.MaxValue));
            Assert.IsFalse(Instant.FromTicks(long.MinValue + 2) >= Instant.FromTicks(long.MaxValue - 5));

            Assert.IsTrue(Instant.FromTicks(0) >= Instant.FromTicks(0));
            Assert.IsTrue(Instant.FromTicks(-42) >= Instant.FromTicks(-42));
            Assert.IsTrue(Instant.FromTicks(long.MinValue) >= Instant.FromTicks(long.MinValue));
            Assert.IsTrue(Instant.FromTicks(long.MaxValue) >= Instant.FromTicks(long.MaxValue));

            Assert.IsTrue(Instant.FromTicks(1) >= Instant.FromTicks(0));
            Assert.IsTrue(Instant.FromTicks(4) >= Instant.FromTicks(2));
            Assert.IsTrue(Instant.FromTicks(1) >= Instant.FromTicks(-1));
            Assert.IsTrue(Instant.FromTicks(long.MaxValue) >= Instant.FromTicks(long.MinValue));
            Assert.IsTrue(Instant.FromTicks(long.MaxValue - 42) >= Instant.FromTicks(long.MinValue + 17));
        }

        [Test]
        public void Operator_GreaterThan()
        {
            Assert.IsFalse(Instant.FromTicks(0) > Instant.FromTicks(1));
            Assert.IsFalse(Instant.FromTicks(1) > Instant.FromTicks(2));
            Assert.IsFalse(Instant.FromTicks(-1) > Instant.FromTicks(0));
            Assert.IsFalse(Instant.FromTicks(-1) > Instant.FromTicks(1));
            Assert.IsFalse(Instant.FromTicks(-1) > Instant.FromTicks(2));
            Assert.IsFalse(Instant.FromTicks(long.MinValue) > Instant.FromTicks(long.MaxValue));
            Assert.IsFalse(Instant.FromTicks(long.MinValue + 2) > Instant.FromTicks(long.MaxValue - 5));

            Assert.IsFalse(Instant.FromTicks(0) > Instant.FromTicks(0));
            Assert.IsFalse(Instant.FromTicks(-42) > Instant.FromTicks(-42));
            Assert.IsFalse(Instant.FromTicks(long.MinValue) > Instant.FromTicks(long.MinValue));
            Assert.IsFalse(Instant.FromTicks(long.MaxValue) > Instant.FromTicks(long.MaxValue));

            Assert.IsTrue(Instant.FromTicks(1) > Instant.FromTicks(0));
            Assert.IsTrue(Instant.FromTicks(4) > Instant.FromTicks(2));
            Assert.IsTrue(Instant.FromTicks(1) > Instant.FromTicks(-1));
            Assert.IsTrue(Instant.FromTicks(long.MaxValue) > Instant.FromTicks(long.MinValue));
            Assert.IsTrue(Instant.FromTicks(long.MaxValue - 42) > Instant.FromTicks(long.MinValue + 17));
        }

        [Test]
        public void Operator_Equal()
        {
            Assert.IsFalse(Instant.FromTicks(0) == Instant.FromTicks(1));
            Assert.IsFalse(Instant.FromTicks(1) == Instant.FromTicks(2));
            Assert.IsFalse(Instant.FromTicks(-1) == Instant.FromTicks(0));
            Assert.IsFalse(Instant.FromTicks(-1) == Instant.FromTicks(1));
            Assert.IsFalse(Instant.FromTicks(-1) == Instant.FromTicks(2));
            Assert.IsFalse(Instant.FromTicks(long.MinValue) == Instant.FromTicks(long.MaxValue));
            Assert.IsFalse(Instant.FromTicks(long.MinValue + 2) == Instant.FromTicks(long.MaxValue - 5));

            Assert.IsTrue(Instant.FromTicks(0) == Instant.FromTicks(0));
            Assert.IsTrue(Instant.FromTicks(-42) == Instant.FromTicks(-42));
            Assert.IsTrue(Instant.FromTicks(long.MinValue) == Instant.FromTicks(long.MinValue));
            Assert.IsTrue(Instant.FromTicks(long.MaxValue) == Instant.FromTicks(long.MaxValue));

            Assert.IsFalse(Instant.FromTicks(1) == Instant.FromTicks(0));
            Assert.IsFalse(Instant.FromTicks(4) == Instant.FromTicks(2));
            Assert.IsFalse(Instant.FromTicks(1) == Instant.FromTicks(-1));
            Assert.IsFalse(Instant.FromTicks(long.MaxValue) == Instant.FromTicks(long.MinValue));
            Assert.IsFalse(Instant.FromTicks(long.MaxValue - 42) == Instant.FromTicks(long.MinValue + 17));
        }

        [Test]
        public void Operator_NotEqual()
        {
            Assert.IsTrue(Instant.FromTicks(0) != Instant.FromTicks(1));
            Assert.IsTrue(Instant.FromTicks(1) != Instant.FromTicks(2));
            Assert.IsTrue(Instant.FromTicks(-1) != Instant.FromTicks(0));
            Assert.IsTrue(Instant.FromTicks(-1) != Instant.FromTicks(1));
            Assert.IsTrue(Instant.FromTicks(-1) != Instant.FromTicks(2));
            Assert.IsTrue(Instant.FromTicks(long.MinValue) != Instant.FromTicks(long.MaxValue));
            Assert.IsTrue(Instant.FromTicks(long.MinValue + 2) != Instant.FromTicks(long.MaxValue - 5));

            Assert.IsFalse(Instant.FromTicks(0) != Instant.FromTicks(0));
            Assert.IsFalse(Instant.FromTicks(-42) != Instant.FromTicks(-42));
            Assert.IsFalse(Instant.FromTicks(long.MinValue) != Instant.FromTicks(long.MinValue));
            Assert.IsFalse(Instant.FromTicks(long.MaxValue) != Instant.FromTicks(long.MaxValue));

            Assert.IsTrue(Instant.FromTicks(1) != Instant.FromTicks(0));
            Assert.IsTrue(Instant.FromTicks(4) != Instant.FromTicks(2));
            Assert.IsTrue(Instant.FromTicks(1) != Instant.FromTicks(-1));
            Assert.IsTrue(Instant.FromTicks(long.MaxValue) != Instant.FromTicks(long.MinValue));
            Assert.IsTrue(Instant.FromTicks(long.MaxValue - 42) != Instant.FromTicks(long.MinValue + 17));
        }

        [Test]
        public void CompareTo_ComparesTicks()
        {
            Assert.IsTrue(Instant.FromTicks(0).CompareTo(Instant.FromTicks(1)) < 0);
            Assert.IsTrue(Instant.FromTicks(1).CompareTo(Instant.FromTicks(2)) < 0);
            Assert.IsTrue(Instant.FromTicks(-1).CompareTo(Instant.FromTicks(0)) < 0);
            Assert.IsTrue(Instant.FromTicks(-1).CompareTo(Instant.FromTicks(1)) < 0);
            Assert.IsTrue(Instant.FromTicks(-1).CompareTo(Instant.FromTicks(2)) < 0);
            Assert.IsTrue(Instant.FromTicks(long.MinValue).CompareTo(Instant.FromTicks(long.MaxValue)) < 0);
            Assert.IsTrue(Instant.FromTicks(long.MinValue + 2).CompareTo(Instant.FromTicks(long.MaxValue - 5)) < 0);

            Assert.IsTrue(Instant.FromTicks(0).CompareTo(Instant.FromTicks(0)) == 0);
            Assert.IsTrue(Instant.FromTicks(-42).CompareTo(Instant.FromTicks(-42)) == 0);
            Assert.IsTrue(Instant.FromTicks(long.MinValue).CompareTo(Instant.FromTicks(long.MinValue)) == 0);
            Assert.IsTrue(Instant.FromTicks(long.MaxValue).CompareTo(Instant.FromTicks(long.MaxValue)) == 0);

            Assert.IsTrue(Instant.FromTicks(1).CompareTo(Instant.FromTicks(0)) > 0);
            Assert.IsTrue(Instant.FromTicks(4).CompareTo(Instant.FromTicks(2)) > 0);
            Assert.IsTrue(Instant.FromTicks(1).CompareTo(Instant.FromTicks(-1)) > 0);
            Assert.IsTrue(Instant.FromTicks(long.MaxValue).CompareTo(Instant.FromTicks(long.MinValue)) > 0);
            Assert.IsTrue(Instant.FromTicks(long.MaxValue - 42).CompareTo(Instant.FromTicks(long.MinValue + 17)) > 0);
        }

        [Test]
        public void Equals_ComparesTicks()
        {
            Assert.AreNotEqual(Instant.FromTicks(0), Instant.FromTicks(1));
            Assert.AreNotEqual(Instant.FromTicks(1), Instant.FromTicks(2));
            Assert.AreNotEqual(Instant.FromTicks(-1), Instant.FromTicks(0));
            Assert.AreNotEqual(Instant.FromTicks(-1), Instant.FromTicks(1));
            Assert.AreNotEqual(Instant.FromTicks(-1), Instant.FromTicks(2));
            Assert.AreNotEqual(Instant.FromTicks(long.MinValue), Instant.FromTicks(long.MaxValue));
            Assert.AreNotEqual(Instant.FromTicks(long.MinValue + 2), Instant.FromTicks(long.MaxValue - 5));

            Assert.AreEqual(Instant.FromTicks(0), Instant.FromTicks(0));
            Assert.AreEqual(Instant.FromTicks(-42), Instant.FromTicks(-42));
            Assert.AreEqual(Instant.FromTicks(long.MinValue), Instant.FromTicks(long.MinValue));
            Assert.AreEqual(Instant.FromTicks(long.MaxValue), Instant.FromTicks(long.MaxValue));

            Assert.AreNotEqual(Instant.FromTicks(1), Instant.FromTicks(0));
            Assert.AreNotEqual(Instant.FromTicks(4), Instant.FromTicks(2));
            Assert.AreNotEqual(Instant.FromTicks(1), Instant.FromTicks(-1));
            Assert.AreNotEqual(Instant.FromTicks(long.MaxValue), Instant.FromTicks(long.MinValue));
            Assert.AreNotEqual(Instant.FromTicks(long.MaxValue - 42), Instant.FromTicks(long.MinValue + 17));
        }

        [Test]
        public void Equals_ComparesObjects()
        {
            Assert.IsTrue(Instant.FromTicks(0).Equals((object)Instant.FromTicks(0)));
            Assert.IsFalse(Instant.FromTicks(0).Equals((object)Instant.FromTicks(1)));
            Assert.IsFalse(Instant.FromTicks(0).Equals(null));
            Assert.IsFalse(Instant.FromTicks(0).Equals(0));
            Assert.IsFalse(Instant.FromTicks(0).Equals(new object()));
        }

        [Test]
        public void GetHashCode_UsesTicksHashCode()
        {
            Assert.AreEqual((0L).GetHashCode(), Instant.FromTicks(0).GetHashCode());
            Assert.AreEqual((123L).GetHashCode(), Instant.FromTicks(123).GetHashCode());
            Assert.AreEqual((-42L).GetHashCode(), Instant.FromTicks(-42).GetHashCode());
            Assert.AreEqual(long.MinValue.GetHashCode(), Instant.FromTicks(long.MinValue).GetHashCode());
            Assert.AreEqual(long.MaxValue.GetHashCode(), Instant.FromTicks(long.MaxValue).GetHashCode());
        }
    }
}
