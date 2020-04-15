using System.CodeDom.Compiler;

namespace PmlUnit
{
    partial class TestDetailsView
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
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
        [GeneratedCode("Windows Form Designer generated code", "1.0")]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TestNameLabel = new System.Windows.Forms.Label();
            this.LayoutPanel = new System.Windows.Forms.Panel();
            this.TestResultIconLabel = new PmlUnit.IconLabel();
            this.ElapsedTimeLabel = new System.Windows.Forms.Label();
            this.ErrorMessageLabel = new System.Windows.Forms.Label();
            this.LinkToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.LayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TestNameLabel
            // 
            this.TestNameLabel.AutoSize = true;
            this.TestNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TestNameLabel.Location = new System.Drawing.Point(3, 3);
            this.TestNameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.TestNameLabel.Name = "TestNameLabel";
            this.TestNameLabel.Size = new System.Drawing.Size(95, 20);
            this.TestNameLabel.TabIndex = 0;
            this.TestNameLabel.Text = "Test Name";
            // 
            // LayoutPanel
            // 
            this.LayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LayoutPanel.AutoScroll = true;
            this.LayoutPanel.Controls.Add(this.TestResultIconLabel);
            this.LayoutPanel.Controls.Add(this.ElapsedTimeLabel);
            this.LayoutPanel.Controls.Add(this.ErrorMessageLabel);
            this.LayoutPanel.Location = new System.Drawing.Point(0, 29);
            this.LayoutPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.LayoutPanel.Name = "LayoutPanel";
            this.LayoutPanel.Size = new System.Drawing.Size(272, 159);
            this.LayoutPanel.TabIndex = 1;
            // 
            // TestResultIconLabel
            // 
            this.TestResultIconLabel.AutoScroll = true;
            this.TestResultIconLabel.Image = global::PmlUnit.Properties.Resources.NotExecuted;
            this.TestResultIconLabel.Location = new System.Drawing.Point(3, 3);
            this.TestResultIconLabel.MinimumSize = new System.Drawing.Size(16, 16);
            this.TestResultIconLabel.Name = "TestResultIconLabel";
            this.TestResultIconLabel.Size = new System.Drawing.Size(95, 17);
            this.TestResultIconLabel.TabIndex = 0;
            this.TestResultIconLabel.Text = "Not executed";
            // 
            // ElapsedTimeLabel
            // 
            this.ElapsedTimeLabel.AutoSize = true;
            this.ElapsedTimeLabel.Location = new System.Drawing.Point(3, 23);
            this.ElapsedTimeLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ElapsedTimeLabel.Name = "ElapsedTimeLabel";
            this.ElapsedTimeLabel.Size = new System.Drawing.Size(123, 13);
            this.ElapsedTimeLabel.TabIndex = 1;
            this.ElapsedTimeLabel.Text = "Elapsed Time: Unknown";
            // 
            // ErrorMessageLabel
            // 
            this.ErrorMessageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorMessageLabel.AutoEllipsis = true;
            this.ErrorMessageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorMessageLabel.Location = new System.Drawing.Point(3, 42);
            this.ErrorMessageLabel.Margin = new System.Windows.Forms.Padding(3);
            this.ErrorMessageLabel.Name = "ErrorMessageLabel";
            this.ErrorMessageLabel.Size = new System.Drawing.Size(266, 13);
            this.ErrorMessageLabel.TabIndex = 2;
            // 
            // TestDetailsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LayoutPanel);
            this.Controls.Add(this.TestNameLabel);
            this.Name = "TestDetailsView";
            this.Size = new System.Drawing.Size(272, 188);
            this.LayoutPanel.ResumeLayout(false);
            this.LayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TestNameLabel;
        private System.Windows.Forms.Panel LayoutPanel;
        private IconLabel TestResultIconLabel;
        private System.Windows.Forms.Label ElapsedTimeLabel;
        private System.Windows.Forms.Label ErrorMessageLabel;
        private System.Windows.Forms.ToolTip LinkToolTip;
    }
}
