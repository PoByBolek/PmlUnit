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
            System.Windows.Forms.LinkLabel linkLabel1;
            System.Windows.Forms.LinkLabel linkLabel2;
            this.TestView = new System.Windows.Forms.ListView();
            this.TestNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ExecutionTimeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            linkLabel1 = new System.Windows.Forms.LinkLabel();
            linkLabel2 = new System.Windows.Forms.LinkLabel();
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
            // TestView
            // 
            this.TestView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TestView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TestView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TestNameColumn,
            this.ExecutionTimeColumn});
            this.TestView.FullRowSelect = true;
            this.TestView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.TestView.HideSelection = false;
            this.TestView.Location = new System.Drawing.Point(0, 22);
            this.TestView.Name = "TestView";
            this.TestView.Size = new System.Drawing.Size(150, 128);
            this.TestView.TabIndex = 2;
            this.TestView.UseCompatibleStateImageBehavior = false;
            this.TestView.View = System.Windows.Forms.View.Details;
            // 
            // TestNameColumn
            // 
            this.TestNameColumn.Text = "Name";
            // 
            // ExecutionTimeColumn
            // 
            this.ExecutionTimeColumn.Text = "Time";
            // 
            // TestRunnerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(linkLabel1);
            this.Controls.Add(linkLabel2);
            this.Controls.Add(this.TestView);
            this.Name = "TestRunnerControl";
            this.SizeChanged += new System.EventHandler(this.OnSizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView TestView;
        private System.Windows.Forms.ColumnHeader TestNameColumn;
        private System.Windows.Forms.ColumnHeader ExecutionTimeColumn;
    }
}
