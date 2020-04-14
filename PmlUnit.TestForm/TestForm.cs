// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class TestForm : Form
    {
        private readonly TestRunnerControl RunnerControl;
        private readonly MutablePathTestCaseProvider Provider;

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification="proxy, runner, and control are all disposed if an Exception occurs")]
        public TestForm()
        {
            ObjectProxy proxy = null;
            AsyncTestRunner runner = null;
            TestRunnerControl control = null;
            try
            {
                Provider = new MutablePathTestCaseProvider(Path.GetFullPath("..\\..\\..\\..\\pmllib-tests"));
                proxy = new StubObjectProxy();
                runner = new PmlTestRunner(proxy, new ControlMethodInvoker(this), new StubClock());
                proxy = null;
                control = new TestRunnerControl(Provider, runner);
                RunnerControl = control;
                runner = null;
                control.Dock = DockStyle.Fill;

                InitializeComponent();

                PathComboBox.Text = Provider.Path;
                FolderBrowser.SelectedPath = Provider.Path;

                ControlPanel.Controls.Add(control);
                
                control = null;
            }
            finally
            {
                if (control != null)
                    control.Dispose();
                if (runner != null)
                    runner.Dispose();
                if (proxy != null)
                    proxy.Dispose();
            }
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

        private class StubObjectProxy : ObjectProxy
        {
            private readonly Random RNG;

            public StubObjectProxy()
            {
                RNG = new Random();
            }

            public void Dispose()
            {
            }

            public object Invoke(string method, params object[] arguments)
            {
                var result = new Hashtable();
                if (RNG.NextDouble() > 0.8)
                {
                    result[1.0] = "Fail";
                }
                return result;
            }
        }

        private class StubClock : Clock
        {
            private readonly Random RNG;
            private long Ticks;

            public StubClock()
            {
                RNG = new Random();
                Ticks = DateTime.Now.Ticks;
            }

            public Instant CurrentInstant
            {
                get
                {
                    Ticks += TimeSpan.FromMilliseconds(RNG.Next(2000)).Ticks;
                    return Instant.FromTicks(Ticks);
                }
            }
        }
    }
}
