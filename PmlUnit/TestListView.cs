// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PmlUnit.Properties;

namespace PmlUnit
{
    partial class TestListView : UserControl
    {
        [Category("Behavior")]
        public event EventHandler SelectionChanged;

        public TestListView()
        {
            InitializeComponent();

            TestStatusImageList.Images.Add("Unknown", Resources.Unknown);
            TestStatusImageList.Images.Add("Failure", Resources.Failure);
            TestStatusImageList.Images.Add("Success", Resources.Success);
        }

        public void SetTests(IEnumerable<Test> tests)
        {
            TestList.Nodes.Clear();

            foreach (var testGroup in tests.GroupBy(test => test.TestCase))
            {
                var groupName = string.Format(CultureInfo.CurrentCulture, "{0} ({1})", testGroup.Key.Name, testGroup.Count());
                var group = TestList.Nodes.Add(testGroup.Key.Name, groupName);
                foreach (var test in testGroup)
                {
                    var node = group.Nodes.Add(test.Name);
                    node.Tag = new Entry(test, node);
                }
            }

            TestList.ExpandAll();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> AllTests => FilterTests(entry => true);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SucceededTests => FilterTests(entry => entry.Succeeded);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> FailedTests => FilterTests(entry => entry.Failed);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> NotExecutedTests => FilterTests(entry => !entry.HasRun);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TestListEntry> SelectedTests => FilterTests(entry => entry.Selected);

        private List<TestListEntry> FilterTests(Func<Entry, bool> predicate)
        {
            var result = new List<TestListEntry>();
            foreach (TreeNode groupNode in TestList.Nodes)
            {
                foreach (TreeNode node in groupNode.Nodes)
                {
                    var entry = node.Tag as Entry;
                    if (entry != null && predicate(entry))
                        result.Add(entry);
                }
            }
            return result;
        }

        private void OnAfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        private class Entry : TestListEntry
        {
            public Test Test { get; }

            private readonly TreeNode Node;
            private TestResult ResultField;

            public Entry(Test test, TreeNode node)
            {
                if (test == null)
                    throw new ArgumentNullException(nameof(test));
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                Test = test;
                Node = node;
                Node.ImageKey = GetImageKey();
                Node.SelectedImageKey = Node.ImageKey;
            }

            public TestResult Result
            {
                get { return ResultField; }
                set
                {
                    ResultField = value;
                    Node.ImageKey = GetImageKey();
                    Node.SelectedImageKey = Node.ImageKey;
                }
            }

            public bool Succeeded => Result != null && Result.Success;

            public bool Failed => Result != null && !Result.Success;

            public bool HasRun => Result != null;

            public bool Selected
            {
                get { return Node.IsSelected; }
                set { Node.TreeView.SelectedNode = value ? Node : null; }
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

    interface TestListEntry
    {
        Test Test { get; }
        TestResult Result { get; set; }
        bool Selected { get; set; }
    }
}
