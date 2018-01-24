using System;

namespace PmlUnit
{
    interface Clock
    {
        Instant CurrentInstant { get; }
    }

    class SystemClock : Clock
    {
        public Instant CurrentInstant
        {
            get { return Instant.FromTicks(DateTime.UtcNow.Ticks); }
        }
    }

    struct Instant : IComparable<Instant>, IEquatable<Instant>
    {
        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = 1000 * TicksPerMillisecond;

        public static Instant FromSeconds(int seconds) => new Instant(seconds * TicksPerSecond);
        public static Instant FromMilliseconds(long milliseconds) => new Instant(milliseconds * TicksPerMillisecond);
        public static Instant FromTicks(long ticks) => new Instant(ticks);

        public static Instant operator +(Instant left, TimeSpan right) => new Instant(left.Ticks + right.Ticks);
        public static Instant operator +(TimeSpan left, Instant right) => new Instant(left.Ticks + right.Ticks);
        public static Instant operator -(Instant left, TimeSpan right) => new Instant(left.Ticks - right.Ticks);
        public static TimeSpan operator -(Instant left, Instant right) => TimeSpan.FromTicks(left.Ticks - right.Ticks);

        public static bool operator <(Instant left, Instant right) => left.Ticks < right.Ticks;
        public static bool operator <=(Instant left, Instant right) => left.Ticks <= right.Ticks;
        public static bool operator >=(Instant left, Instant right) => left.Ticks >= right.Ticks;
        public static bool operator >(Instant left, Instant right) => left.Ticks > right.Ticks;

        public static bool operator ==(Instant left, Instant right) => left.Ticks == right.Ticks;
        public static bool operator !=(Instant left, Instant right) => left.Ticks != right.Ticks;

        public long Ticks { get; }

        private Instant(long ticks)
        {
            Ticks = ticks;
        }

        public int CompareTo(Instant other) => Ticks.CompareTo(other.Ticks);

        public override bool Equals(object obj)
        {
            if (obj is Instant)
                return Equals((Instant)obj);
            else
                return false;
        }

        public bool Equals(Instant other) => Ticks == other.Ticks;

        public override int GetHashCode() => Ticks.GetHashCode();

    }
}
