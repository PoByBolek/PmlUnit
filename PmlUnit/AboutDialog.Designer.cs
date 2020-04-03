// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.CodeDom.Compiler;

namespace PmlUnit
{
    partial class AboutDialog
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label licenseLabel;
            System.Windows.Forms.PictureBox iconPicuteBox;
            this.IconLicenseLabel = new PmlUnit.CustomLinkLabel();
            this.StatusIconLicenseLabel = new PmlUnit.CustomLinkLabel();
            this.GithubLabel = new PmlUnit.CustomLinkLabel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.CopyrightLabel = new System.Windows.Forms.Label();
            this.LinkToolTip = new System.Windows.Forms.ToolTip(this.components);
            licenseLabel = new System.Windows.Forms.Label();
            iconPicuteBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(iconPicuteBox)).BeginInit();
            this.SuspendLayout();
            // 
            // licenseLabel
            // 
            licenseLabel.AutoSize = true;
            licenseLabel.Location = new System.Drawing.Point(146, 93);
            licenseLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            licenseLabel.Name = "licenseLabel";
            licenseLabel.Size = new System.Drawing.Size(212, 13);
            licenseLabel.TabIndex = 4;
            licenseLabel.Text = "PML Unit is licensed under the MIT license.\r\n";
            // 
            // iconPicuteBox
            // 
            iconPicuteBox.Image = global::PmlUnit.Properties.Resources.TestRunnerLarge;
            iconPicuteBox.Location = new System.Drawing.Point(12, 12);
            iconPicuteBox.Name = "iconPicuteBox";
            iconPicuteBox.Size = new System.Drawing.Size(128, 128);
            iconPicuteBox.TabIndex = 0;
            iconPicuteBox.TabStop = false;
            // 
            // IconLicenseLabel
            // 
            this.IconLicenseLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IconLicenseLabel.LinkArea = new System.Windows.Forms.LinkArea(33, 11);
            this.IconLicenseLabel.Location = new System.Drawing.Point(146, 111);
            this.IconLicenseLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.IconLicenseLabel.Name = "IconLicenseLabel";
            this.IconLicenseLabel.Size = new System.Drawing.Size(332, 26);
            this.IconLicenseLabel.TabIndex = 5;
            this.IconLicenseLabel.TabStop = true;
            this.IconLicenseLabel.Text = "The PML Unit icon was created by Recep Kütük and is free for personal and commerc" +
    "ial use.";
            this.IconLicenseLabel.UseCompatibleTextRendering = true;
            this.IconLicenseLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkLabelLinkClicked);
            this.IconLicenseLabel.LinkHover += new System.EventHandler<PmlUnit.LinkHoverEventArgs>(this.OnLinkHover);
            this.IconLicenseLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnLinkLabelMouseMove);
            // 
            // StatusIconLicenseLabel
            // 
            this.StatusIconLicenseLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusIconLicenseLabel.LinkArea = new System.Windows.Forms.LinkArea(37, 20);
            this.StatusIconLicenseLabel.Location = new System.Drawing.Point(146, 141);
            this.StatusIconLicenseLabel.Name = "StatusIconLicenseLabel";
            this.StatusIconLicenseLabel.Size = new System.Drawing.Size(332, 26);
            this.StatusIconLicenseLabel.TabIndex = 6;
            this.StatusIconLicenseLabel.TabStop = true;
            this.StatusIconLicenseLabel.Text = "The test status icons are taken from Font-Awesome-SVG-PNG, which is licensed unde" +
    "r the MIT license.";
            this.StatusIconLicenseLabel.UseCompatibleTextRendering = true;
            this.StatusIconLicenseLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkLabelLinkClicked);
            this.StatusIconLicenseLabel.LinkHover += new System.EventHandler<PmlUnit.LinkHoverEventArgs>(this.OnLinkHover);
            this.StatusIconLicenseLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnLinkLabelMouseMove);
            // 
            // GithubLabel
            // 
            this.GithubLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GithubLabel.LinkArea = new System.Windows.Forms.LinkArea(59, 6);
            this.GithubLabel.Location = new System.Drawing.Point(12, 191);
            this.GithubLabel.Name = "GithubLabel";
            this.GithubLabel.Size = new System.Drawing.Size(466, 13);
            this.GithubLabel.TabIndex = 7;
            this.GithubLabel.TabStop = true;
            this.GithubLabel.Text = "PML Unit is free and open source software and is hosted on GitHub.";
            this.GithubLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.GithubLabel.UseCompatibleTextRendering = true;
            this.GithubLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkLabelLinkClicked);
            this.GithubLabel.LinkHover += new System.EventHandler<PmlUnit.LinkHoverEventArgs>(this.OnLinkHover);
            this.GithubLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnLinkLabelMouseMove);
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(143, 12);
            this.TitleLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(108, 25);
            this.TitleLabel.TabIndex = 1;
            this.TitleLabel.Text = "PML Unit";
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.Location = new System.Drawing.Point(146, 40);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(61, 16);
            this.VersionLabel.TabIndex = 2;
            this.VersionLabel.Text = "Version";
            // 
            // CopyrightLabel
            // 
            this.CopyrightLabel.AutoSize = true;
            this.CopyrightLabel.Location = new System.Drawing.Point(146, 60);
            this.CopyrightLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 20);
            this.CopyrightLabel.Name = "CopyrightLabel";
            this.CopyrightLabel.Size = new System.Drawing.Size(51, 13);
            this.CopyrightLabel.TabIndex = 3;
            this.CopyrightLabel.Text = "Copyright";
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 213);
            this.Controls.Add(this.GithubLabel);
            this.Controls.Add(this.StatusIconLicenseLabel);
            this.Controls.Add(this.IconLicenseLabel);
            this.Controls.Add(licenseLabel);
            this.Controls.Add(this.CopyrightLabel);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(iconPicuteBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About PML Unit";
            ((System.ComponentModel.ISupportInitialize)(iconPicuteBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

#endregion
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label CopyrightLabel;
        private PmlUnit.CustomLinkLabel IconLicenseLabel;
        private PmlUnit.CustomLinkLabel StatusIconLicenseLabel;
        private PmlUnit.CustomLinkLabel GithubLabel;
        private System.Windows.Forms.ToolTip LinkToolTip;
    }
}
