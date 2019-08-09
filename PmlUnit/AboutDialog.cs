// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
            
            TitleLabel.Text = GetAssemblyTitle();
            VersionLabel.Text = "Version " + GetAssemblyVersion();
            CopyrightLabel.Text = GetAssemblyCopyright();

            IconLicenseLabel.Links[0].LinkData = "http://www.recepkutuk.com/bitsies/"; // https seems to be broken
            StatusIconLicenseLabel.Links[0].LinkData = "https://github.com/encharm/Font-Awesome-SVG-PNG";
            GithubLabel.Links[0].LinkData = "https://github.com/PoByBolek/PmlUnit";
        }

        private string GetAssemblyTitle()
        {
            foreach (var attribute in Assembly.GetCustomAttributes<AssemblyTitleAttribute>(inherit: false))
            {
                if (!string.IsNullOrEmpty(attribute.Title))
                    return attribute.Title;
            }
            return "PML Unit";
        }

        private string GetAssemblyVersion()
        {
            return Assembly.GetName().Version.ToString();
        }

        private string GetAssemblyCopyright()
        {
            foreach (var attribute in Assembly.GetCustomAttributes<AssemblyCopyrightAttribute>(inherit: false))
            {
                if (!string.IsNullOrEmpty(attribute.Copyright))
                    return attribute.Copyright;
            }
            return "Copyright © 2019 Florian Zimmermann";
        }

        private Assembly Assembly => typeof(PmlUnitAddin).Assembly;

        private void OnLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var url = e.Link.LinkData as string;
            if (string.IsNullOrEmpty(url))
                return;
            else if (url.StartsWith("http://") || url.StartsWith("https://"))
                Process.Start(url);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
