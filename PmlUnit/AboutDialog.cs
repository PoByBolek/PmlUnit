// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class AboutDialog : Form
    {
        private DateTime TooltipShown;

        public AboutDialog()
        {
            InitializeComponent();
            SetDerivedFonts();

            TooltipShown = DateTime.MinValue;

            TitleLabel.Text = GetAssemblyTitle();
            VersionLabel.Text = GetAssemblyVersion();
            CopyrightLabel.Text = GetAssemblyCopyright();

            IconLicenseLabel.Links[0].LinkData = "http://www.recepkutuk.com/bitsies/"; // https seems to be broken
            StatusIconLicenseLabel.Links[0].LinkData = "https://github.com/encharm/Font-Awesome-SVG-PNG";
            GithubLabel.Links[0].LinkData = "https://github.com/PoByBolek/PmlUnit";
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            SetDerivedFonts();
        }

        private void SetDerivedFonts()
        {
            TitleLabel.Font = new Font(Font.FontFamily, Font.Size * 1.9f, FontStyle.Bold);
            VersionLabel.Font = new Font(Font.FontFamily, Font.Size * 1.2f, FontStyle.Bold);
        }

        private static string GetAssemblyTitle()
        {
            foreach (var attribute in Assembly.GetCustomAttributes<AssemblyTitleAttribute>(inherit: false))
            {
                if (!string.IsNullOrEmpty(attribute.Title))
                    return attribute.Title;
            }
            return "PML Unit";
        }

        private static string GetAssemblyVersion()
        {
            return "Version " + Assembly.GetName().Version.ToString();
        }

        private static string GetAssemblyCopyright()
        {
            foreach (var attribute in Assembly.GetCustomAttributes<AssemblyCopyrightAttribute>(inherit: false))
            {
                if (!string.IsNullOrEmpty(attribute.Copyright))
                    return attribute.Copyright;
            }
            return "Copyright © 2019 Florian Zimmermann";
        }

        private static Assembly Assembly => typeof(PmlUnitAddin).Assembly;

        private void OnLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var url = e.Link.LinkData as string;
            if (string.IsNullOrEmpty(url))
                return;
            else if (url.StartsWith("http://", StringComparison.Ordinal) || url.StartsWith("https://", StringComparison.Ordinal))
                Process.Start(url);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e != null && e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
        

        private void OnLinkHover(object sender, LinkHoverEventArgs e)
        {
            var url = e.Link.LinkData as string;
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            else if (url.StartsWith("http://", StringComparison.Ordinal) || url.StartsWith("https://", StringComparison.Ordinal))
            {
                LinkToolTip.Show(url, this, PointToClient(MousePosition));
                TooltipShown = DateTime.Now;
            }
        }

        private void OnLinkLabelMouseMove(object sender, MouseEventArgs e)
        {
            var delay = TimeSpan.FromMilliseconds(LinkToolTip.InitialDelay);
            if (DateTime.Now > TooltipShown + delay)
                LinkToolTip.Hide(this);
        }

        private void OnLinkLabelMouseLeave(object sender, EventArgs e)
        {
            LinkToolTip.Hide(this);
        }
    }
}
