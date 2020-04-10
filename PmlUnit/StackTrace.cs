// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PmlUnit
{

    class StackTrace : ICollection<StackFrame>
    {
        private readonly List<StackFrame> Frames;

        public int Count => Frames.Count;

        public StackFrame this[int index] => Frames[index];

        public StackTrace()
        {
            Frames = new List<StackFrame>();
        }

        public StackTrace(IEnumerable<StackFrame> frames)
        {
            if (frames == null)
                throw new ArgumentNullException(nameof(frames));

            Frames = frames.ToList();
        }

        public bool Contains(StackFrame item)
        {
            return Frames.Contains(item);
        }

        public void CopyTo(StackFrame[] array, int arrayIndex)
        {
            Frames.CopyTo(array, arrayIndex);
        }

        public IEnumerator<StackFrame> GetEnumerator()
        {
            return Frames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<StackFrame>.IsReadOnly => true;

        void ICollection<StackFrame>.Add(StackFrame item)
        {
            throw new NotSupportedException();
        }

        void ICollection<StackFrame>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<StackFrame>.Remove(StackFrame item)
        {
            throw new NotSupportedException();
        }
    }

    class StackFrame
    {
        public static bool operator ==(StackFrame left, StackFrame right)
        {
            if (ReferenceEquals(left, right))
                return true;
            else if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(StackFrame left, StackFrame right)
        {
            return !(left == right);
        }

        public int LineNumber { get; }
        public int ColumnNumber { get; }
        public string CallSite { get; }

        public StackFrame(string lineInformation, string callSite)
        {
            if (string.IsNullOrEmpty(lineInformation))
                throw new ArgumentNullException(nameof(lineInformation));
            if (string.IsNullOrEmpty(callSite))
                throw new ArgumentNullException(nameof(callSite));

            var match = Regex.Match(lineInformation, @"^(?:In|Called from) line (\d+) of .+$");
            if (!match.Success)
                throw new FormatException("Line information has an invalid format");

            LineNumber = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            int column = callSite.IndexOf("^^", StringComparison.Ordinal);
            if (column >= 0)
            {
                ColumnNumber = column;
                CallSite = callSite.Remove(column, 2);
            }
            else
            {
                ColumnNumber = 0;
                CallSite = callSite;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StackFrame);
        }

        public bool Equals(StackFrame other)
        {
            if (ReferenceEquals(this, other))
                return true;
            else if (ReferenceEquals(other, null))
                return false;

            return LineNumber == other.LineNumber
                && ColumnNumber == other.ColumnNumber
                && CallSite == other.CallSite;
        }

        public override int GetHashCode()
        {
            return LineNumber.GetHashCode()
                ^ ColumnNumber.GetHashCode()
                ^ CallSite.GetHashCode();
        }
    }
}
