using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PmlUnit
{
    [DefaultProperty("Text")]
    partial class IconLabel : UserControl
    {
        public IconLabel()
        {
            InitializeComponent();
        }

        [Category("Appearance")]
        public Image Image
        {
            get { return ImageLabel.Image; }
            set { ImageLabel.Image = value; }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return TextLabel.Text; }
            set { TextLabel.Text = value; }
        }
        
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set
            {
                base.AutoSize = value;
                TextLabel.AutoSize = value;
            }
        }

        private void OnTextLabelSizeChanged(object sender, EventArgs e)
        {
            if (AutoSize)
            {
                Size = new Size(ImageLabel.Width + TextLabel.Width, Math.Max(ImageLabel.Height, TextLabel.Height));
            }
        }
    }
}
