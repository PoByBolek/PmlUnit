// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using PmlUnit.Properties;

namespace PmlUnit
{
    partial class TestDetailsView : UserControl
    {
        [Category("Behavior")]
        public event EventHandler<FileEventArgs> FileActivate;

        private Test TestField;
        private List<LinkLabel> StackTraceLabels;

        public TestDetailsView()
        {
            StackTraceLabels = new List<LinkLabel>();

            InitializeComponent();

            TestNameLabel.Font = new Font(Font.FontFamily, Font.Size * 1.5f, FontStyle.Bold);
            ErrorMessageLabel.Font = new Font(Font, FontStyle.Bold);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Test Test
        {
            get { return TestField; }
            set
            {
                if (value == TestField)
                    return;
                if (TestField != null)
                    TestField.ResultChanged -= OnTestResultChanged;
                if (value != null)
                    value.ResultChanged += OnTestResultChanged;

                TestField = value;
                if (TestField == null)
                {
                    TestNameLabel.Text = "";
                    TestResultIconLabel.Image = Resources.NotExecuted;
                    TestResultIconLabel.Text = "Not executed";
                    ElapsedTimeLabel.Text = "";
                    SetError(null);
                }
                else
                {
                    TestNameLabel.Text = TestField.Name;
                    OnTestResultChanged(TestField, EventArgs.Empty);
                }
            }
        }

        private void OnTestResultChanged(object sender, EventArgs e)
        {
            var status = TestField.Status;
            if (status == TestStatus.NotExecuted)
            {
                TestResultIconLabel.Image = Resources.NotExecuted;
                TestResultIconLabel.Text = "Not executed";
                ElapsedTimeLabel.Text = "";
                SetError(null);
            }
            else if (status == TestStatus.Passed)
            {
                TestResultIconLabel.Image = Resources.Passed;
                TestResultIconLabel.Text = "Passed";
                ElapsedTimeLabel.Text = "Elapsed time: " + TestField.Result.Duration.Format();
                SetError(null);
            }
            else
            {
                TestResultIconLabel.Image = Resources.Failed;
                TestResultIconLabel.Text = "Failed";
                ElapsedTimeLabel.Text = "Elapsed time: " + TestField.Result.Duration.Format();
                SetError(TestField.Result.Error);
            }
        }

        private void SetError(PmlError error)
        {
            LayoutPanel.SuspendLayout();
            try
            {
                RemoveStackTraceLabels();

                if (error == null)
                {
                    ErrorMessageLabel.Text = "";
                }
                else
                {
                    ErrorMessageLabel.Text = error.Message;
                    AddStackTraceLabels(error.StackTrace);
                }
            }
            finally
            {
                LayoutPanel.ResumeLayout(performLayout: true);
            }
        }

        private void RemoveStackTraceLabels()
        {
            foreach (var label in StackTraceLabels)
                label.Dispose();
            StackTraceLabels.Clear();
            LinkToolTip.RemoveAll();
        }

        private void AddStackTraceLabels(StackTrace stackTrace)
        {
            var nextLocation = ErrorMessageLabel.Location;
            nextLocation.Y = ErrorMessageLabel.Bottom + ErrorMessageLabel.Margin.Bottom;
            foreach (var frame in stackTrace)
            {
                LinkLabel label = null;
                try
                {
                    label = BuildStackFrameLabel(frame, nextLocation);
                    nextLocation.Y += label.Height + label.Margin.Vertical;

                    LayoutPanel.Controls.Add(label);
                    StackTraceLabels.Add(label);
                    label = null;
                }
                finally
                {
                    if (label != null)
                        label.Dispose();
                }
            }
        }

        private LinkLabel BuildStackFrameLabel(StackFrame frame, Point location)
        {
            var label = new LinkLabel();
            try
            {
                label.Location = location + new Size(0, 3);
                label.Margin = new Padding(left: 3, top: 3, right: 3, bottom: 0);
                label.Size = ErrorMessageLabel.Size;
                label.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

                label.Text = frame.EntryPoint.Name;
                if (frame.LineNumber > 0)
                    label.Text += string.Format(CultureInfo.CurrentCulture, " line {0}", frame.LineNumber);
                label.AutoEllipsis = true;

                if (string.IsNullOrEmpty(frame.EntryPoint.FileName))
                {
                    label.Links.Clear();
                    LinkToolTip.SetToolTip(label, "");
                }
                else
                {
                    label.Links[0].LinkData = frame;
                    LinkToolTip.SetToolTip(label, frame.EntryPoint.FileName);
                }
                label.LinkBehavior = LinkBehavior.HoverUnderline;

                label.LinkClicked += OnStackTraceLabelLinkClicked;

                var result = label;
                label = null;
                return result;
            }
            finally
            {
                if (label != null)
                    label.Dispose();
            }
        }

        private void OnStackTraceLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var frame = e.Link.LinkData as StackFrame;
            if (frame != null && !string.IsNullOrEmpty(frame.EntryPoint.FileName))
            {
                FileActivate?.Invoke(this, new FileEventArgs(frame.EntryPoint.FileName, frame.LineNumber));
            }
        }
    }

    class FileEventArgs : EventArgs
    {
        public string FileName { get; }
        public int LineNumber { get; }

        public FileEventArgs(string fileName, int lineNumber)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (lineNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(lineNumber));

            FileName = fileName;
            LineNumber = lineNumber;
        }
    }
}
