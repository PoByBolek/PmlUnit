// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
namespace PmlUnit
{
    partial class CodeEditorDialog
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
            System.Windows.Forms.Button okButton;
            System.Windows.Forms.Button cancelButton;
            System.Windows.Forms.Button browseButton;
            System.Windows.Forms.Label pathLabel;
            System.Windows.Forms.Label argumentsLabel;
            this.EditorKindComboBox = new System.Windows.Forms.ComboBox();
            this.PathComboBox = new System.Windows.Forms.ComboBox();
            this.ArgumentsTextBox = new System.Windows.Forms.TextBox();
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            editorKindLabel = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            browseButton = new System.Windows.Forms.Button();
            pathLabel = new System.Windows.Forms.Label();
            argumentsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // editorKindLabel
            // 
            editorKindLabel.AutoSize = true;
            editorKindLabel.Location = new System.Drawing.Point(12, 15);
            editorKindLabel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            editorKindLabel.Name = "editorKindLabel";
            editorKindLabel.Size = new System.Drawing.Size(85, 13);
            editorKindLabel.TabIndex = 0;
            editorKindLabel.Text = "Open files using:";
            // 
            // okButton
            // 
            okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Location = new System.Drawing.Point(328, 98);
            okButton.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 7;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(409, 98);
            cancelButton.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 8;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // browseButton
            // 
            browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            browseButton.Location = new System.Drawing.Point(409, 37);
            browseButton.Name = "browseButton";
            browseButton.Size = new System.Drawing.Size(75, 23);
            browseButton.TabIndex = 4;
            browseButton.Text = "&Browse...";
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += new System.EventHandler(this.OnBrowseButtonClick);
            // 
            // EditorKindComboBox
            // 
            this.EditorKindComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EditorKindComboBox.FormattingEnabled = true;
            this.EditorKindComboBox.Location = new System.Drawing.Point(103, 12);
            this.EditorKindComboBox.Name = "EditorKindComboBox";
            this.EditorKindComboBox.Size = new System.Drawing.Size(134, 21);
            this.EditorKindComboBox.TabIndex = 1;
            this.EditorKindComboBox.SelectedIndexChanged += new System.EventHandler(this.OnEditorKindChanged);
            // 
            // pathLabel
            // 
            pathLabel.AutoSize = true;
            pathLabel.Location = new System.Drawing.Point(12, 42);
            pathLabel.Name = "pathLabel";
            pathLabel.Size = new System.Drawing.Size(32, 13);
            pathLabel.TabIndex = 2;
            pathLabel.Text = "Path:";
            // 
            // PathComboBox
            // 
            this.PathComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PathComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.PathComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.PathComboBox.FormattingEnabled = true;
            this.PathComboBox.Location = new System.Drawing.Point(103, 39);
            this.PathComboBox.Name = "PathComboBox";
            this.PathComboBox.Size = new System.Drawing.Size(300, 21);
            this.PathComboBox.TabIndex = 3;
            // 
            // argumentsLabel
            // 
            argumentsLabel.AutoSize = true;
            argumentsLabel.Location = new System.Drawing.Point(12, 69);
            argumentsLabel.Name = "argumentsLabel";
            argumentsLabel.Size = new System.Drawing.Size(60, 13);
            argumentsLabel.TabIndex = 5;
            argumentsLabel.Text = "Arguments:";
            // 
            // ArgumentsTextBox
            // 
            this.ArgumentsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ArgumentsTextBox.Location = new System.Drawing.Point(103, 66);
            this.ArgumentsTextBox.Name = "ArgumentsTextBox";
            this.ArgumentsTextBox.Size = new System.Drawing.Size(381, 20);
            this.ArgumentsTextBox.TabIndex = 6;
            // 
            // FileDialog
            // 
            this.FileDialog.DefaultExt = "exe";
            this.FileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            // 
            // CodeEditorDialog
            // 
            this.AcceptButton = okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancelButton;
            this.ClientSize = new System.Drawing.Size(496, 133);
            this.Controls.Add(cancelButton);
            this.Controls.Add(okButton);
            this.Controls.Add(this.ArgumentsTextBox);
            this.Controls.Add(argumentsLabel);
            this.Controls.Add(browseButton);
            this.Controls.Add(this.PathComboBox);
            this.Controls.Add(pathLabel);
            this.Controls.Add(this.EditorKindComboBox);
            this.Controls.Add(editorKindLabel);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1920, 172);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(346, 172);
            this.Name = "CodeEditorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox PathComboBox;
        private System.Windows.Forms.TextBox ArgumentsTextBox;
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.ComboBox EditorKindComboBox;
    }
}