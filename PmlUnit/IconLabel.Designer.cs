namespace PmlUnit
{
    partial class IconLabel
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
        private void InitializeComponent()
        {
            this.ImageLabel = new System.Windows.Forms.Label();
            this.TextLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ImageLabel
            // 
            this.ImageLabel.Location = new System.Drawing.Point(0, 0);
            this.ImageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.ImageLabel.Name = "ImageLabel";
            this.ImageLabel.Size = new System.Drawing.Size(16, 16);
            this.ImageLabel.TabIndex = 0;
            // 
            // TextLabel
            // 
            this.TextLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextLabel.Location = new System.Drawing.Point(16, 0);
            this.TextLabel.Margin = new System.Windows.Forms.Padding(0);
            this.TextLabel.Name = "TextLabel";
            this.TextLabel.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.TextLabel.Size = new System.Drawing.Size(170, 16);
            this.TextLabel.TabIndex = 1;
            this.TextLabel.Text = "label1";
            this.TextLabel.SizeChanged += new System.EventHandler(this.OnTextLabelSizeChanged);
            // 
            // IconLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TextLabel);
            this.Controls.Add(this.ImageLabel);
            this.MinimumSize = new System.Drawing.Size(16, 16);
            this.Name = "IconLabel";
            this.Size = new System.Drawing.Size(186, 16);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label ImageLabel;
        private System.Windows.Forms.Label TextLabel;
    }
}
