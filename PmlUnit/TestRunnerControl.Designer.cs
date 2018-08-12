namespace PmlUnit
{
    partial class TestRunnerControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.LinkLabel linkLabel1;
            System.Windows.Forms.LinkLabel linkLabel2;
            System.Windows.Forms.ImageList imageList1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestRunnerControl));
            this.TestView = new System.Windows.Forms.ListView();
            this.TestNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ExecutionTimeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TestResultSplitContainer = new System.Windows.Forms.SplitContainer();
            this.TestResultLabel = new System.Windows.Forms.Label();
            linkLabel1 = new System.Windows.Forms.LinkLabel();
            linkLabel2 = new System.Windows.Forms.LinkLabel();
            imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.TestResultSplitContainer.Panel1.SuspendLayout();
            this.TestResultSplitContainer.Panel2.SuspendLayout();
            this.TestResultSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new System.Drawing.Point(3, 3);
            linkLabel1.Margin = new System.Windows.Forms.Padding(3);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new System.Drawing.Size(40, 13);
            linkLabel1.TabIndex = 0;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Run all";
            linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnRunAllLinkClick);
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new System.Drawing.Point(49, 3);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new System.Drawing.Size(44, 13);
            linkLabel2.TabIndex = 1;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Refresh";
            linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnRefreshLinkClick);
            // 
            // imageList1
            // 
            imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            imageList1.Images.SetKeyName(0, "Unknown");
            imageList1.Images.SetKeyName(1, "Failure");
            imageList1.Images.SetKeyName(2, "Success");
            // 
            // TestView
            // 
            this.TestView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TestView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TestNameColumn,
            this.ExecutionTimeColumn});
            this.TestView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TestView.FullRowSelect = true;
            this.TestView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.TestView.HideSelection = false;
            this.TestView.Location = new System.Drawing.Point(0, 0);
            this.TestView.Name = "TestView";
            this.TestView.Size = new System.Drawing.Size(167, 264);
            this.TestView.SmallImageList = imageList1;
            this.TestView.TabIndex = 0;
            this.TestView.UseCompatibleStateImageBehavior = false;
            this.TestView.View = System.Windows.Forms.View.Details;
            this.TestView.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            this.TestView.SizeChanged += new System.EventHandler(this.OnTestViewSizeChanged);
            // 
            // TestNameColumn
            // 
            this.TestNameColumn.Text = "Name";
            // 
            // ExecutionTimeColumn
            // 
            this.ExecutionTimeColumn.Text = "Time";
            // 
            // TestResultSplitContainer
            // 
            this.TestResultSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TestResultSplitContainer.Location = new System.Drawing.Point(0, 22);
            this.TestResultSplitContainer.Name = "TestResultSplitContainer";
            // 
            // TestResultSplitContainer.Panel1
            // 
            this.TestResultSplitContainer.Panel1.Controls.Add(this.TestView);
            // 
            // TestResultSplitContainer.Panel2
            // 
            this.TestResultSplitContainer.Panel2.Controls.Add(this.TestResultLabel);
            this.TestResultSplitContainer.Size = new System.Drawing.Size(399, 264);
            this.TestResultSplitContainer.SplitterDistance = 167;
            this.TestResultSplitContainer.TabIndex = 2;
            this.TestResultSplitContainer.SizeChanged += new System.EventHandler(this.OnSplitContainerSizeChanged);
            // 
            // TestResultLabel
            // 
            this.TestResultLabel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.TestResultLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TestResultLabel.Location = new System.Drawing.Point(0, 0);
            this.TestResultLabel.Name = "TestResultLabel";
            this.TestResultLabel.Size = new System.Drawing.Size(228, 264);
            this.TestResultLabel.TabIndex = 0;
            // 
            // TestRunnerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.TestResultSplitContainer);
            this.Controls.Add(linkLabel1);
            this.Controls.Add(linkLabel2);
            this.Name = "TestRunnerControl";
            this.Size = new System.Drawing.Size(399, 286);
            this.TestResultSplitContainer.Panel1.ResumeLayout(false);
            this.TestResultSplitContainer.Panel2.ResumeLayout(false);
            this.TestResultSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView TestView;
        private System.Windows.Forms.ColumnHeader TestNameColumn;
        private System.Windows.Forms.ColumnHeader ExecutionTimeColumn;
        private System.Windows.Forms.SplitContainer TestResultSplitContainer;
        private System.Windows.Forms.Label TestResultLabel;
    }
}
