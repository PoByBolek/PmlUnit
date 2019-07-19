// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PmlUnit
{
    interface TestListEntry
    {
        Test Test { get; }
        TestResult Result { get; set; }
        bool Selected { get; set; }
    }

    class TestListViewEntry : TestListEntry
    {
        public const string SuccessImageKey = "Success";
        public const string FailureImageKey = "Failure";
        public const string NotExecutedImageKey = "Unknown";

        public event EventHandler SelectionChanged;
        public event EventHandler ResultChanged;

        public Test Test { get; }

        private readonly TestListView View;

        private TestResult ResultField;
        private bool SelectedField;

        public TestListViewEntry(Test test, TestListView view)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            Test = test;
            View = view;
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

        public void Paint(Graphics g, Rectangle bounds, ImageList icons, Brush brush, StringFormat format)
        {
            int padding = 2;
            var y = bounds.Top + padding;

            g.DrawImage(icons.Images[GetImageKey()], padding, y);

            int durationWidth = 0;
            if (bounds.Width > 20 && Result != null)
            {
                var duration = Result.Duration.Format();
                durationWidth = (int)Math.Ceiling(g.MeasureString(duration, View.Font).Width) + padding;
                int x = Math.Max(20, bounds.Width - durationWidth);
                g.DrawString(duration, View.Font, brush, x, y);
            }

            g.DrawString(Test.Name, View.Font, brush, new RectangleF(20, y, bounds.Width - durationWidth - 20 - padding, 16), format);
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
