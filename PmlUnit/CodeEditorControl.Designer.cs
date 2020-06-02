// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
namespace PmlUnit
{
    partial class CodeEditorControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label editorKindLabel;
            System.Windows.Forms.Button browseButton;
            System.Windows.Forms.Label pathLabel;
            this.ArgumentsLabel = new System.Windows.Forms.Label();
            this.EditorKindComboBox = new System.Windows.Forms.ComboBox();
            this.PathComboBox = new System.Windows.Forms.ComboBox();
            this.ArgumentsTextBox = new System.Windows.Forms.TextBox();
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ArgumentsHintLabel = new System.Windows.Forms.Label();
            editorKindLabel = new System.Windows.Forms.Label();
            browseButton = new System.Windows.Forms.Button();
            pathLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // editorKindLabel
            // 
            editorKindLabel.AutoSize = true;
            editorKindLabel.Location = new System.Drawing.Point(0, 3);
            editorKindLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            editorKindLabel.Name = "editorKindLabel";
            editorKindLabel.Size = new System.Drawing.Size(85, 13);
            editorKindLabel.TabIndex = 0;
            editorKindLabel.Text = "Open files using:";
            // 
            // browseButton
            // 
            browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            browseButton.Location = new System.Drawing.Point(375, 23);
            browseButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            browseButton.Name = "browseButton";
            browseButton.Size = new System.Drawing.Size(75, 23);
            browseButton.TabIndex = 4;
            browseButton.Text = "&Browse...";
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += new System.EventHandler(this.OnBrowseButtonClick);
            // 
            // pathLabel
            // 
            pathLabel.AutoSize = true;
            pathLabel.Location = new System.Drawing.Point(0, 28);
            pathLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            pathLabel.Name = "pathLabel";
            pathLabel.Size = new System.Drawing.Size(32, 13);
            pathLabel.TabIndex = 2;
            pathLabel.Text = "Path:";
            // 
            // ArgumentsLabel
            // 
            this.ArgumentsLabel.AutoSize = true;
            this.ArgumentsLabel.Location = new System.Drawing.Point(0, 55);
            this.ArgumentsLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 4);
            this.ArgumentsLabel.Name = "ArgumentsLabel";
            this.ArgumentsLabel.Size = new System.Drawing.Size(87, 13);
            this.ArgumentsLabel.TabIndex = 5;
            this.ArgumentsLabel.Text = "Extra Arguments:";
            // 
            // EditorKindComboBox
            // 
            this.EditorKindComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EditorKindComboBox.FormattingEnabled = true;
            this.EditorKindComboBox.Location = new System.Drawing.Point(93, 0);
            this.EditorKindComboBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.EditorKindComboBox.Name = "EditorKindComboBox";
            this.EditorKindComboBox.Size = new System.Drawing.Size(144, 21);
            this.EditorKindComboBox.TabIndex = 1;
            this.EditorKindComboBox.SelectedIndexChanged += new System.EventHandler(this.OnEditorKindChanged);
            // 
            // PathComboBox
            // 
            this.PathComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PathComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.PathComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.PathComboBox.FormattingEnabled = true;
            this.PathComboBox.Location = new System.Drawing.Point(93, 25);
            this.PathComboBox.Name = "PathComboBox";
            this.PathComboBox.Size = new System.Drawing.Size(276, 21);
            this.PathComboBox.TabIndex = 3;
            // 
            // ArgumentsTextBox
            // 
            this.ArgumentsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ArgumentsTextBox.Location = new System.Drawing.Point(93, 52);
            this.ArgumentsTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.ArgumentsTextBox.Name = "ArgumentsTextBox";
            this.ArgumentsTextBox.Size = new System.Drawing.Size(357, 20);
            this.ArgumentsTextBox.TabIndex = 6;
            // 
            // FileDialog
            // 
            this.FileDialog.DefaultExt = "exe";
            this.FileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            // 
            // ArgumentsHintLabel
            // 
            this.ArgumentsHintLabel.AutoSize = true;
            this.ArgumentsHintLabel.Location = new System.Drawing.Point(90, 76);
            this.ArgumentsHintLabel.Name = "ArgumentsHintLabel";
            this.ArgumentsHintLabel.Size = new System.Drawing.Size(249, 13);
            this.ArgumentsHintLabel.TabIndex = 7;
            this.ArgumentsHintLabel.Text = "Use the $fileName and $lineNumber variables here.";
            // 
            // CodeEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ArgumentsHintLabel);
            this.Controls.Add(this.ArgumentsTextBox);
            this.Controls.Add(this.ArgumentsLabel);
            this.Controls.Add(browseButton);
            this.Controls.Add(this.PathComboBox);
            this.Controls.Add(pathLabel);
            this.Controls.Add(this.EditorKindComboBox);
            this.Controls.Add(editorKindLabel);
            this.MinimumSize = new System.Drawing.Size(340, 89);
            this.Name = "CodeEditorControl";
            this.Size = new System.Drawing.Size(450, 89);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox PathComboBox;
        private System.Windows.Forms.TextBox ArgumentsTextBox;
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.ComboBox EditorKindComboBox;
        private System.Windows.Forms.Label ArgumentsLabel;
        private System.Windows.Forms.Label ArgumentsHintLabel;
    }
}