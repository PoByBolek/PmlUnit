// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PmlUnit;

namespace TestForm
{
    public partial class TestForm : Form
    {
        private readonly TestRunnerControl RunnerControl;
        private readonly MutablePathTestCaseProvider Provider;

        public TestForm()
        {
            Provider = new MutablePathTestCaseProvider(Path.GetFullPath("..\\..\\..\\..\\pmllib-tests"));
            RunnerControl = new TestRunnerControl(Provider, new DummyTestRunner());
            RunnerControl.Dock = DockStyle.Fill;

            InitializeComponent();

            PathComboBox.Text = Provider.Path;
            FolderBrowser.SelectedPath = Provider.Path;

            ControlPanel.Controls.Add(RunnerControl);
        }

        private void OnBrowseButtonClick(object sender, EventArgs e)
        {
            var result = FolderBrowser.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                Provider.Path = FolderBrowser.SelectedPath;
                PathComboBox.Text = FolderBrowser.SelectedPath;
                RunnerControl.LoadTests();
            }
        }

        private void OnPathComboBoxTextChanged(object sender, EventArgs e)
        {
            Provider.Path = PathComboBox.Text;
            FolderBrowser.SelectedPath = PathComboBox.Text;
            RunnerControl.LoadTests();
        }

        private class MutablePathTestCaseProvider : TestCaseProvider
        {
            public string Path { get; set; }

            private TestCaseProvider Provider;

            public MutablePathTestCaseProvider(string path)
            {
                Path = path;
                Provider = null;
            }

            public ICollection<TestCase> GetTestCases()
            {
                if (string.IsNullOrEmpty(Path) || !File.Exists(System.IO.Path.Combine(Path, "pml.index")))
                    return new List<TestCase>();
                if (Provider == null)
                    Provider = new IndexFileTestCaseProvider(Path);
                return Provider.GetTestCases();
            }
        }

        private class DummyTestRunner : TestRunner
        {
            private readonly Random RNG;

            public DummyTestRunner()
            {
                RNG = new Random();
            }

            public void Dispose()
            {
            }

            public void RefreshIndex()
            {
            }

            public void Reload(TestCase testCase)
            {
            }

            public TestResult Run(Test test)
            {
                var duration = TimeSpan.FromMilliseconds(RNG.Next(2000));

                if (RNG.NextDouble() <= 0.8)
                    return new TestResult(duration);
                else
                    return new TestResult(duration, new Exception());
            }

            public void Run(TestCase testCase)
            {
                foreach (var test in testCase.Tests)
                    Run(test);
            }
        }
    }
}
