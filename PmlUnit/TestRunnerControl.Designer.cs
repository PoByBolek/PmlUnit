using System.CodeDom.Compiler;

namespace PmlUnit
{
    partial class TestRunnerControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        [GeneratedCode("Windows Form Designer generated code", "1.0")]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.LinkLabel runAllLinkLabel;
            System.Windows.Forms.LinkLabel refreshLinkLabel;
            System.Windows.Forms.ToolStripMenuItem failedTestsToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem notExecutedTestsToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem succeededTestsToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem selectedTestsToolStripMenuItem;
            this.TestList = new PmlUnit.TestListView();
            this.TestResultSplitContainer = new System.Windows.Forms.SplitContainer();
            this.TestSummary = new PmlUnit.TestSummaryView();
            this.ExecutionProgressBar = new PmlUnit.ColorizedProgressBar();
            this.RunLinkLabel = new System.Windows.Forms.LinkLabel();
            this.RunContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            runAllLinkLabel = new System.Windows.Forms.LinkLabel();
            refreshLinkLabel = new System.Windows.Forms.LinkLabel();
            failedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            notExecutedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            succeededTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            selectedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TestResultSplitContainer.Panel1.SuspendLayout();
            this.TestResultSplitContainer.Panel2.SuspendLayout();
            this.TestResultSplitContainer.SuspendLayout();
            this.RunContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ExecutionProgressBar
            // 
            this.ExecutionProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExecutionProgressBar.BackColor = System.Drawing.Color.Transparent;
            this.ExecutionProgressBar.Location = new System.Drawing.Point(0, 0);
            this.ExecutionProgressBar.Name = "ExecutionProgressBar";
            this.ExecutionProgressBar.Size = new System.Drawing.Size(399, 10);
            this.ExecutionProgressBar.TabIndex = 0;
            // 
            // runAllLinkLabel
            // 
            runAllLinkLabel.AutoSize = true;
            runAllLinkLabel.Location = new System.Drawing.Point(1, 13);
            runAllLinkLabel.Margin = new System.Windows.Forms.Padding(3);
            runAllLinkLabel.Name = "runAllLinkLabel";
            runAllLinkLabel.Size = new System.Drawing.Size(40, 13);
            runAllLinkLabel.TabIndex = 1;
            runAllLinkLabel.TabStop = true;
            runAllLinkLabel.Text = "Run all";
            runAllLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnRunAllLinkClick);
            // 
            // RunLinkLabel
            // 
            this.RunLinkLabel.AutoSize = true;
            this.RunLinkLabel.Location = new System.Drawing.Point(47, 13);
            this.RunLinkLabel.Name = "RunLinkLabel";
            this.RunLinkLabel.Size = new System.Drawing.Size(36, 13);
            this.RunLinkLabel.TabIndex = 2;
            this.RunLinkLabel.TabStop = true;
            this.RunLinkLabel.Text = "Run...";
            this.RunLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnRunLinkClicked);
            // 
            // refreshLinkLabel
            // 
            refreshLinkLabel.AutoSize = true;
            refreshLinkLabel.Location = new System.Drawing.Point(89, 13);
            refreshLinkLabel.Name = "refreshLinkLabel";
            refreshLinkLabel.Size = new System.Drawing.Size(44, 13);
            refreshLinkLabel.TabIndex = 3;
            refreshLinkLabel.TabStop = true;
            refreshLinkLabel.Text = "Refresh";
            refreshLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnRefreshLinkClick);
            // 
            // RunContextMenu
            // 
            this.RunContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            failedTestsToolStripMenuItem,
            notExecutedTestsToolStripMenuItem,
            succeededTestsToolStripMenuItem,
            selectedTestsToolStripMenuItem});
            this.RunContextMenu.Name = "RunContextMenu";
            this.RunContextMenu.Size = new System.Drawing.Size(174, 92);
            // 
            // failedTestsToolStripMenuItem
            // 
            failedTestsToolStripMenuItem.Name = "failedTestsToolStripMenuItem";
            failedTestsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            failedTestsToolStripMenuItem.Text = "Failed Tests";
            failedTestsToolStripMenuItem.Click += new System.EventHandler(this.OnRunFailedTestsMenuItemClick);
            // 
            // notExecutedTestsToolStripMenuItem
            // 
            notExecutedTestsToolStripMenuItem.Name = "notExecutedTestsToolStripMenuItem";
            notExecutedTestsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            notExecutedTestsToolStripMenuItem.Text = "Not Executed Tests";
            notExecutedTestsToolStripMenuItem.Click += new System.EventHandler(this.OnRunNotExecutedTestsMenuItemClick);
            // 
            // succeededTestsToolStripMenuItem
            // 
            succeededTestsToolStripMenuItem.Name = "succeededTestsToolStripMenuItem";
            succeededTestsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            succeededTestsToolStripMenuItem.Text = "Succeeded Tests";
            succeededTestsToolStripMenuItem.Click += new System.EventHandler(this.OnRunSucceededTestsMenuItemClick);
            // 
            // selectedTestsToolStripMenuItem
            // 
            selectedTestsToolStripMenuItem.Name = "selectedTestsToolStripMenuItem";
            selectedTestsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            selectedTestsToolStripMenuItem.Text = "Selected Tests";
            selectedTestsToolStripMenuItem.Click += new System.EventHandler(this.OnRunSelectedTestsMenuItemClick);
            // 
            // TestList
            // 
            this.TestList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TestList.Location = new System.Drawing.Point(0, 0);
            this.TestList.Name = "TestList";
            this.TestList.Size = new System.Drawing.Size(167, 254);
            this.TestList.TabIndex = 0;
            this.TestList.SelectionChanged += new System.EventHandler(this.OnTestListSelectionChanged);
            // 
            // TestResultSplitContainer
            // 
            this.TestResultSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TestResultSplitContainer.Location = new System.Drawing.Point(0, 32);
            this.TestResultSplitContainer.Name = "TestResultSplitContainer";
            // 
            // TestResultSplitContainer.Panel1
            // 
            this.TestResultSplitContainer.Panel1.Controls.Add(this.TestList);
            // 
            // TestResultSplitContainer.Panel2
            // 
            this.TestResultSplitContainer.Panel2.Controls.Add(this.TestDetails);
            this.TestResultSplitContainer.Panel2.Controls.Add(this.TestSummary);
            this.TestResultSplitContainer.Size = new System.Drawing.Size(399, 254);
            this.TestResultSplitContainer.SplitterDistance = 167;
            this.TestResultSplitContainer.TabIndex = 4;
            this.TestResultSplitContainer.SizeChanged += new System.EventHandler(this.OnSplitContainerSizeChanged);
            // 
            // TestSummary
            // 
            this.TestSummary.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.TestSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TestSummary.Location = new System.Drawing.Point(0, 0);
            this.TestSummary.Name = "TestSummary";
            this.TestSummary.Size = new System.Drawing.Size(228, 254);
            this.TestSummary.TabIndex = 0;
            // 
            // TestRunnerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.RunLinkLabel);
            this.Controls.Add(this.ExecutionProgressBar);
            this.Controls.Add(this.TestResultSplitContainer);
            this.Controls.Add(runAllLinkLabel);
            this.Controls.Add(refreshLinkLabel);
            this.MinimumSize = new System.Drawing.Size(150, 50);
            this.Name = "TestRunnerControl";
            this.Size = new System.Drawing.Size(400, 300);
            this.TestResultSplitContainer.Panel1.ResumeLayout(false);
            this.TestResultSplitContainer.Panel2.ResumeLayout(false);
            this.TestResultSplitContainer.ResumeLayout(false);
            this.RunContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TestListView TestList;
        private System.Windows.Forms.SplitContainer TestResultSplitContainer;
        private TestSummaryView TestSummary;
        private ColorizedProgressBar ExecutionProgressBar;
        private System.Windows.Forms.LinkLabel RunLinkLabel;
        private System.Windows.Forms.ContextMenuStrip RunContextMenu;
    }
}
