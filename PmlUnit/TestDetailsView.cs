// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.ComponentModel;
using System.Windows.Forms;
using PmlUnit.Properties;

namespace PmlUnit
{
    partial class TestDetailsView : UserControl
    {
        private Test TestField;

        public TestDetailsView()
        {
            InitializeComponent();
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
                    StackTraceLabel.Text = "";
                    ElapsedTimeLabel.Text = "";
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
                StackTraceLabel.Text = "";
                ElapsedTimeLabel.Text = "";
            }
            else if (status == TestStatus.Passed)
            {
                TestResultIconLabel.Image = Resources.Passed;
                TestResultIconLabel.Text = "Passed";
                StackTraceLabel.Text = "";
                ElapsedTimeLabel.Text = "Elapsed time: " + TestField.Result.Duration.Format();
            }
            else
            {
                TestResultIconLabel.Image = Resources.Failed;
                TestResultIconLabel.Text = "Failed";
                StackTraceLabel.Text = TestField.Result.Error.Message;
                ElapsedTimeLabel.Text = "Elapsed time: " + TestField.Result.Duration.Format();
            }
        }
    }
}
