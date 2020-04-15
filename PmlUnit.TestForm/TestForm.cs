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
        private readonly MutablePathIndex Index;

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification="proxy, runner, and control are all disposed if an Exception occurs")]
        public TestForm()
        {
            ObjectProxy proxy = null;
            AsyncTestRunner runner = null;
            TestRunnerControl control = null;
            try
            {
                Index = new MutablePathIndex(Path.GetFullPath("..\\..\\..\\..\\pmllib-tests"));
                proxy = new StubObjectProxy();
                runner = new PmlTestRunner(proxy, new ControlMethodInvoker(this), new StubClock(), Index);
                proxy = null;
                control = new TestRunnerControl(Index, runner);
                RunnerControl = control;
                runner = null;
                control.Dock = DockStyle.Fill;

                InitializeComponent();

                PathComboBox.Text = Index.Path;
                FolderBrowser.SelectedPath = Index.Path;

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
                Index.Path = FolderBrowser.SelectedPath;
                PathComboBox.Text = FolderBrowser.SelectedPath;
                RunnerControl.LoadTests();
            }
        }

        private void OnPathComboBoxTextChanged(object sender, EventArgs e)
        {
            Index.Path = PathComboBox.Text;
            FolderBrowser.SelectedPath = PathComboBox.Text;
            RunnerControl.LoadTests();
        }

        private class MutablePathIndex : TestCaseProvider, EntryPointResolver
        {
            private TestCaseProvider Provider;
            private EntryPointResolver Resolver;
            private string PathField;

            public MutablePathIndex(string path)
            {
                Path = path;
            }

            public string Path
            {
                get { return PathField; }
                set
                {
                    if (value != PathField)
                    {
                        PathField = value;
                        var index = GetIndexFile(value);
                        Provider = new IndexFileTestCaseProvider(index);
                        Resolver = new IndexFileEntryPointResolver(index);
                    }
                }
            }

            private static IndexFile GetIndexFile(string directory)
            {
                if (string.IsNullOrEmpty(directory))
                    return new IndexFile();

                string fileName = System.IO.Path.Combine(directory, "pml.index");
                if (File.Exists(fileName))
                    return new IndexFile(fileName);
                else
                    return new IndexFile();
            }

            public ICollection<TestCase> GetTestCases()
            {
                return Provider.GetTestCases();
            }

            public EntryPoint Resolve(string value)
            {
                return Resolver.Resolve(value);
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
                    result[1.0] = "(44,33)   FNF:File not found";
                    result[2.0] = @"In line 44 of Macro C:\Aveva\Plant\E3D21~1.0\PMLLIB\common\functions\runsynonym.pmlmac";
                    result[3.0] = "^^$M \"/%PMLUI%/CLIB/FILES/UELEMSEL\" =1/1";
                    result[4.0] = "Called from line 34 of PML function runsynonym";
                    result[5.0] = "  $m \"$!<macro>\" $<$!<action>$>";
                    result[6.0] = "Called from line 62 of PML function pmlasserttest.TESTEQUALWITHUNEQUALVALUES";
                    result[7.0] = "    !!runSynonym('CALLIB UELEMSEL =1/1')";
                    result[8.0] = "Called from line 53 of PML function pmltestrunner.RUNINTERNAL";
                    result[9.0] = "    !testCase.$!<testName>(object PmlAssert())";
                    result[10.0] = "Called from line 37 of PML function pmltestrunner.RUN";
                    result[11.0] = "    !this.runInternal(!testCaseName, !testName, !hasSetup, !hasTearDown)";
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
