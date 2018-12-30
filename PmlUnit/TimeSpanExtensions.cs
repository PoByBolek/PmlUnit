using System;
using System.Globalization;

namespace PmlUnit
{
    static class TimeSpanExtensions
    {
        public static string Format(this TimeSpan value)
        {
            var millis = value.TotalMilliseconds;
            if (millis < 1)
                return "< 1 ms";
            else if (millis < 1000)
                return string.Format(CultureInfo.CurrentCulture, "{0} ms", (int)millis);
            else if (millis < 10000)
                return string.Format(CultureInfo.CurrentCulture, "{0:N1} s", ((int)millis / 100) / 10.0);
            else
                return string.Format(CultureInfo.CurrentCulture, "{0:N0} s", millis / 1000);
        }
    }
}
