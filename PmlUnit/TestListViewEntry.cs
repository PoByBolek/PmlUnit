// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.ComponentModel;
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

    partial class TestListViewEntry : UserControl, TestListEntry
    {
        public const int ItemHeight = 16;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Test Test { get; }

        private TestResult ResultField;
        private bool IsSelectedField;

        public TestListViewEntry(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            Test = test;

            InitializeComponent();

            NameLabel.Text = test.Name;
        }

        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                NameLabel.Font = value;
                DurationLabel.Font = value;
            }
        }

        [Category("Appearance")]
        public ImageList ImageList
        {
            get { return ImageLabel.ImageList; }
            set
            {
                ImageLabel.ImageList = value;
                if (value != null)
                {
                    ImageLabel.ImageKey = GetImageKey();
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TestResult Result
        {
            get { return ResultField; }
            set
            {
                ResultField = value;
                if (ImageLabel.ImageList != null)
                {
                    ImageLabel.ImageKey = GetImageKey();
                }

                DurationLabel.Text = FormatDuration();
                DurationLabel.Left = Width - DurationLabel.Width;

                NameLabel.Width = Width - ImageLabel.Width - NameLabel.Padding.Horizontal - DurationLabel.Width;

                MinimumSize = new Size(ImageLabel.Width + NameLabel.Margin.Horizontal + DurationLabel.Width, ImageLabel.Height);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Selected
        {
            get { return IsSelectedField; }
            set
            {
                IsSelectedField = value;
                BackColor = value ? SystemColors.Highlight : SystemColors.Control;
                NameLabel.ForeColor = value ? SystemColors.HighlightText : SystemColors.ControlText;
                DurationLabel.ForeColor = value ? SystemColors.HighlightText : SystemColors.ControlText;
            }
        }

        private string GetImageKey()
        {
            if (Result == null)
                return "Unknown";
            else if (Result.Success)
                return "Success";
            else
                return "Failure";
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
