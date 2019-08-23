// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Drawing;

namespace PmlUnit
{
    class TestListTestEntry : TestListEntry
    {
        public const string SuccessImageKey = "Success";
        public const string FailureImageKey = "Failure";
        public const string NotExecutedImageKey = "Unknown";

        public event EventHandler ResultChanged
        {
            add { Test.ResultChanged += value; }
            remove { Test.ResultChanged -= value; }
        }

        public Test Test { get; }

        public TestListTestEntry(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
            Test = test;
        }

        public override void Paint(Graphics g, Rectangle bounds, TestListPaintOptions options)
        {
            int padding = 2;
            int left = bounds.Left + padding + 20;
            int right = bounds.Right - padding;
            int y = bounds.Top + padding;

            var textBrush = options.NormalTextBrush;
            if (Selected)
            {
                textBrush = options.SelectedTextBrush;
                g.FillRectangle(options.SelectedBackBrush, bounds);
            }
            else if (options.FocusedEntry == this)
            {
                var copy = bounds;
                copy.Width -= 1;
                copy.Height -= 1;
                g.DrawRectangle(options.FocusRectanglePen, copy);
            }

            g.DrawImage(options.StatusImageList.Images[GetImageKey()], left, y);
            left += 16 + padding;

            if (left < right && Test.Result != null)
            {
                string duration = Test.Result.Duration.Format();
                int durationWidth = (int)Math.Ceiling(g.MeasureString(duration, options.EntryFont).Width);
                int durationX = Math.Max(left, right - durationWidth);
                g.DrawString(duration, options.EntryFont, textBrush, durationX, y);
                right = durationX - padding;
            }

            if (left < right)
            {
                var nameBounds = new RectangleF(left, y, right - left, 16);
                g.DrawString(Test.Name, options.EntryFont, textBrush, nameBounds, options.EntryFormat);
            }
        }

        private string GetImageKey()
        {
            if (Test.Status == TestStatus.NotExecuted)
                return NotExecutedImageKey;
            else if (Test.Status == TestStatus.Successful)
                return SuccessImageKey;
            else
                return FailureImageKey;
        }
    }
}
