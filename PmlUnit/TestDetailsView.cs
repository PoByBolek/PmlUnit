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
        private TestResult ResultField;

        public TestDetailsView()
        {
            InitializeComponent();

            var builder = new TestCaseBuilder("Test");
            builder.AddTest("Test");
            Test = builder.Build().Tests[0];
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Test Test
        {
            get { return TestField; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                TestField = value;
                TestNameLabel.Text = value.Name;
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
                if (value == null)
                {
                    TestResultIconLabel.Image = Resources.Unknown;
                    TestResultIconLabel.Text = "Not executed";
                    StackTraceLabel.Text = "";
                    ElapsedTimeLabel.Text = "";
                }
                else
                {
                    if (value.Error == null)
                    {
                        TestResultIconLabel.Image = Resources.Success;
                        TestResultIconLabel.Text = "Successful";
                        StackTraceLabel.Text = "";
                    }
                    else
                    {
                        TestResultIconLabel.Image = Resources.Failure;
                        TestResultIconLabel.Text = "Failed";
                        StackTraceLabel.Text = value.Error.Message;
                    }
                    ElapsedTimeLabel.Text = "Elapsed time: " + value.Duration.Format();
                }
            }
        }
    }
}
