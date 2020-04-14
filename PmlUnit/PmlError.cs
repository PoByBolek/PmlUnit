// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PmlUnit
{
    public sealed class PmlError
    {
        public static PmlError FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var lines = value.Trim().Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.TrimEnd('\r'))
                .ToList();
            return FromList(lines);
        }

        public static PmlError FromHashTable(Hashtable stackTrace)
        {
            if (stackTrace == null || stackTrace.Count == 0)
                return null;

            var stack = new List<string>(stackTrace.Count);
            foreach (var key in stackTrace.Keys.OfType<double>().OrderBy(k => k))
            {
                var line = stackTrace[key] as string;
                if (line == null)
                    throw new ArgumentException("stack trace contains values other than strings", nameof(stackTrace));
                else if (string.IsNullOrEmpty(line))
                    throw new ArgumentException("stack trace contains empty string", nameof(stackTrace));
                stack.Add(line);
            }

            return FromList(stack);
        }

        public static PmlError FromList(IList<string> stackTrace)
        {
            if (stackTrace == null || stackTrace.Count == 0)
                return null;
            if (stackTrace.Count % 2 == 0)
                throw new ArgumentException("number of stack lines should be odd", nameof(stackTrace));

            string message = stackTrace[0];
            if (StackInformationIsMissing(stackTrace))
                return new PmlError(message);

            var frames = new List<StackFrame>((stackTrace.Count - 1 / 2));
            for (int i = 1; i < stackTrace.Count; i += 2)
                frames.Add(new StackFrame(stackTrace[i], stackTrace[i + 1]));

            return new PmlError(message, new StackTrace(frames));
        }

        private static bool StackInformationIsMissing(IList<string> stackTrace)
        {
            return stackTrace.Count == 3
                && stackTrace[1].StartsWith(" *** ", StringComparison.Ordinal)
                && stackTrace[2].StartsWith(" *** ", StringComparison.Ordinal);
        }

        public string Message { get; }
        public StackTrace StackTrace { get; }

        public PmlError(string message)
            : this(message, null)
        {
        }

        public PmlError(string message, StackTrace stackTrace)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            Message = message;
            StackTrace = stackTrace ?? new StackTrace();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Message);
            foreach (var frame in StackTrace)
                builder.AppendLine(frame.ToString());
            return builder.ToString();
        }
    }
}
