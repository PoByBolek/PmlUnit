// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Drawing;

namespace PmlUnit
{
    class TestListViewEntry : TestListBaseEntry, TestListEntry
    {
        public const string SuccessImageKey = "Success";
        public const string FailureImageKey = "Failure";
        public const string NotExecutedImageKey = "Unknown";

        public event EventHandler SelectionChanged;
        public event EventHandler ResultChanged;

        public Test Test { get; }

        private TestResult ResultField;
        private bool SelectedField;

        public TestListViewEntry(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            Test = test;
        }

        public TestResult Result
        {
            get { return ResultField; }
            set
            {
                if (value != ResultField)
                {
                    ResultField = value;
                    ResultChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool Selected
        {
            get { return SelectedField; }
            set
            {
                if (value != SelectedField)
                {
                    SelectedField = value;
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Paint(Graphics g, Rectangle bounds, TestListPaintOptions options)
        {
            int padding = 2;
            int left = bounds.Left + padding;
            int right = bounds.Right - padding;
            int y = bounds.Top + padding;

            var textBrush = options.NormalTextBrush;
            if (Selected)
            {
                textBrush = options.SelectedTextBrush;
                g.FillRectangle(options.SelectedBackBrush, 0, bounds.Top, bounds.Right, bounds.Height);
            }
            else if (options.FocusedEntry == this)
            {
                g.DrawRectangle(options.FocusRectanglePen, 0, bounds.Top, bounds.Right - 1, bounds.Height);
            }

            g.DrawImage(options.StatusImageList.Images[GetImageKey()], left, y);
            left += 16 + padding;

            if (left < right && Result != null)
            {
                string duration = Result.Duration.Format();
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
            if (Result == null)
                return NotExecutedImageKey;
            else if (Result.Success)
                return SuccessImageKey;
            else
                return FailureImageKey;
        }

        private string FormatDuration()
        {
            if (Result == null)
                return "";
            else
                return Result.Duration.Format();
        }
    }
}
