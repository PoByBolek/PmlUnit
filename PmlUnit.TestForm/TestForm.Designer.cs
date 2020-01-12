using System.CodeDom.Compiler;

namespace PmlUnit
{
    partial class TestForm
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        [GeneratedCode("Windows Form Designer generated code", "1.0")]
        private void InitializeComponent()
        {
            System.Windows.Forms.Label pathLabel;
            System.Windows.Forms.Button browseButton;
            this.ControlPanel = new System.Windows.Forms.Panel();
            this.PathComboBox = new System.Windows.Forms.ComboBox();
            this.FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            pathLabel = new System.Windows.Forms.Label();
            browseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pathLabel
            // 
            pathLabel.AutoSize = true;
            pathLabel.Location = new System.Drawing.Point(9, 13);
            pathLabel.Name = "pathLabel";
            pathLabel.Size = new System.Drawing.Size(32, 13);
            pathLabel.TabIndex = 0;
            pathLabel.Text = "Path:";
            // 
            // browseButton
            // 
            browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            browseButton.Location = new System.Drawing.Point(400, 8);
            browseButton.Name = "browseButton";
            browseButton.Size = new System.Drawing.Size(75, 23);
            browseButton.TabIndex = 2;
            browseButton.Text = "&Browse...";
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += new System.EventHandler(this.OnBrowseButtonClick);
            // 
            // ControlPanel
            // 
            this.ControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlPanel.Location = new System.Drawing.Point(9, 37);
            this.ControlPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.ControlPanel.Name = "ControlPanel";
            this.ControlPanel.Size = new System.Drawing.Size(466, 415);
            this.ControlPanel.TabIndex = 3;
            // 
            // PathComboBox
            // 
            this.PathComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PathComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.PathComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.PathComboBox.FormattingEnabled = true;
            this.PathComboBox.Location = new System.Drawing.Point(47, 10);
            this.PathComboBox.Name = "PathComboBox";
            this.PathComboBox.Size = new System.Drawing.Size(347, 21);
            this.PathComboBox.TabIndex = 1;
            this.PathComboBox.TextChanged += new System.EventHandler(this.OnPathComboBoxTextChanged);
            // 
            // FolderBrowser
            // 
            this.FolderBrowser.ShowNewFolderButton = false;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 461);
            this.Controls.Add(browseButton);
            this.Controls.Add(this.PathComboBox);
            this.Controls.Add(pathLabel);
            this.Controls.Add(this.ControlPanel);
            this.Name = "TestForm";
            this.Text = "PML Unit Test Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Panel ControlPanel;
        private System.Windows.Forms.ComboBox PathComboBox;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowser;
    }
}

