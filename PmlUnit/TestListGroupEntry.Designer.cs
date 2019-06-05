namespace PmlUnit
{
    partial class TestListGroupEntry
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.ImageLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.CountLabel = new System.Windows.Forms.Label();
            this.EntryPanel = new PmlUnit.StretchingVerticalFlowLayoutPanel();
            this.SuspendLayout();
            // 
            // ImageLabel
            // 
            this.ImageLabel.Location = new System.Drawing.Point(0, 0);
            this.ImageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.ImageLabel.Name = "ImageLabel";
            this.ImageLabel.Size = new System.Drawing.Size(16, 16);
            this.ImageLabel.TabIndex = 0;
            this.ImageLabel.Click += new System.EventHandler(this.OnImageLabelClick);
            this.ImageLabel.MouseEnter += new System.EventHandler(this.OnImageLabelMouseEnter);
            this.ImageLabel.MouseLeave += new System.EventHandler(this.OnImageLabelMouseLeave);
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(16, 2);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(77, 13);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "Group Name";
            this.NameLabel.Click += new System.EventHandler(this.OnGroupHeaderClick);
            this.NameLabel.DoubleClick += new System.EventHandler(this.OnToggleExpanded);
            // 
            // CountLabel
            // 
            this.CountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CountLabel.Location = new System.Drawing.Point(96, 2);
            this.CountLabel.Margin = new System.Windows.Forms.Padding(0);
            this.CountLabel.Name = "CountLabel";
            this.CountLabel.Size = new System.Drawing.Size(54, 16);
            this.CountLabel.TabIndex = 2;
            this.CountLabel.Text = "(0)";
            this.CountLabel.Click += new System.EventHandler(this.OnGroupHeaderClick);
            this.CountLabel.DoubleClick += new System.EventHandler(this.OnToggleExpanded);
            // 
            // EntryPanel
            // 
            this.EntryPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EntryPanel.Location = new System.Drawing.Point(0, 16);
            this.EntryPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.EntryPanel.Name = "EntryPanel";
            this.EntryPanel.Padding = new System.Windows.Forms.Padding(19, 0, 0, 0);
            this.EntryPanel.Size = new System.Drawing.Size(150, 144);
            this.EntryPanel.TabIndex = 3;
            // 
            // TestListGroupEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.EntryPanel);
            this.Controls.Add(this.CountLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.ImageLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(19, 16);
            this.Name = "TestListGroupEntry";
            this.Size = new System.Drawing.Size(150, 160);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ImageLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label CountLabel;
        private StretchingVerticalFlowLayoutPanel EntryPanel;
    }
}
