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
            this.ElapsedTimeLabel = new System.Windows.Forms.Label();
            this.TestNameLabel = new System.Windows.Forms.Label();
            this.TestResultIconLabel = new PmlUnit.IconLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.StackTraceLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            // TestResultIconLabel
            // 
            this.TestResultIconLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TestResultIconLabel.AutoSize = true;
            this.TestResultIconLabel.Image = global::PmlUnit.Properties.Resources.NotExecuted;
            this.TestResultIconLabel.Location = new System.Drawing.Point(3, 3);
            this.TestResultIconLabel.MinimumSize = new System.Drawing.Size(16, 16);
            this.TestResultIconLabel.Name = "TestResultIconLabel";
            this.TestResultIconLabel.Size = new System.Drawing.Size(87, 17);
            this.TestResultIconLabel.TabIndex = 0;
            this.TestResultIconLabel.Text = "Not executed";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.StackTraceLabel);
            this.panel1.Controls.Add(this.TestResultIconLabel);
            this.panel1.Controls.Add(this.ElapsedTimeLabel);
            this.panel1.Location = new System.Drawing.Point(0, 29);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(295, 121);
            this.panel1.TabIndex = 1;
            // 
            // StackTraceLabel
            // 
            this.StackTraceLabel.AutoSize = true;
            this.StackTraceLabel.Location = new System.Drawing.Point(3, 42);
            this.StackTraceLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.StackTraceLabel.Name = "StackTraceLabel";
            this.StackTraceLabel.Size = new System.Drawing.Size(0, 13);
            this.StackTraceLabel.TabIndex = 2;
            // 
            // TestDetailsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.TestNameLabel);
            this.Name = "TestDetailsView";
            this.Size = new System.Drawing.Size(295, 150);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TestNameLabel;
        private IconLabel TestResultIconLabel;
        private System.Windows.Forms.Label ElapsedTimeLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label StackTraceLabel;
    }
}
