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

        [Category("Behavior")]
        public event EventHandler SelectionChanged;

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
                ImageLabel.ImageKey = GetImageKey();
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
                ImageLabel.ImageKey = GetImageKey();

                DurationLabel.Text = FormatDuration();
                DurationLabel.Left = Width - DurationLabel.Width;

                NameLabel.Width = Width - ImageLabel.Width - NameLabel.Padding.Horizontal - DurationLabel.Width;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Selected
        {
            get { return IsSelectedField; }
            set
            {
                bool oldValue = IsSelectedField;
                IsSelectedField = value;
                BackColor = value ? SystemColors.Highlight : SystemColors.Window;
                NameLabel.ForeColor = value ? SystemColors.HighlightText : SystemColors.ControlText;
                DurationLabel.ForeColor = value ? SystemColors.HighlightText : SystemColors.ControlText;

                if (value != oldValue)
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
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

        private void OnLabelClick(object sender, EventArgs e)
        {
            OnClick(e);
        }
    }
}
