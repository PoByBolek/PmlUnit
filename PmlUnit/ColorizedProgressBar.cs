// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class ColorizedProgressBar : UserControl
    {
        private int ValueField;
        private int MaximumField;

        public ColorizedProgressBar()
        {
            InitializeComponent();

            ValueField = 0;
            MaximumField = 100;
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Green")]
        public Color Color
        {
            get { return Progress.BackColor; }
            set { Progress.BackColor = value; }
        }

        [Category("Behavior")]
        [DefaultValue(0)]
        public int Value
        {
            get { return ValueField; }
            set
            {
                ValueField = value;
                OnSizeChanged(this, EventArgs.Empty);
            }
        }

        [Category("Behavior")]
        [DefaultValue(100)]
        public int Maximum
        {
            get { return MaximumField; }
            set
            {
                MaximumField = value;
                OnSizeChanged(this, EventArgs.Empty);
            }
        }

        public void Increment(int value)
        {
            Value = ValueField + value;
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            if (Width == 0 || MaximumField == 0)
            {
                Progress.Width = 0;
            }
            else
            {
                var ratio = Math.Max(0, Math.Min((double)ValueField / (double)MaximumField, 1));
                Progress.Width = (int)(Width * ratio);
            }
        }
    }
}
