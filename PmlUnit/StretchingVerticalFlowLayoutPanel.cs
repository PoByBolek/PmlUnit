// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace PmlUnit
{
    class StretchingVerticalFlowLayoutPanel : Panel
    {
        private LayoutEngine LayoutEngineField;

        public StretchingVerticalFlowLayoutPanel()
        {
        }

        public void Clear()
        {
            var controls = Controls.OfType<Control>().ToList();
            Controls.Clear();
            foreach (var child in controls)
                child.Dispose();
        }

        public override LayoutEngine LayoutEngine
        {
            get
            {
                if (LayoutEngineField == null)
                    LayoutEngineField = new StretchingVerticalFlowLayoutEngine();
                return LayoutEngineField;
            }
        }
    }

    class StretchingVerticalFlowLayoutEngine : LayoutEngine
    {
        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            Control parent = container as Control;

            var displayBounds = parent.DisplayRectangle;
            var nextControlLocation = new Point(parent.Padding.Left, parent.Padding.Top);

            foreach (Control child in parent.Controls)
            {
                if (!child.Visible)
                    continue;

                nextControlLocation.Y += child.Margin.Top;

                var size = child.GetPreferredSize(displayBounds.Size);
                size.Width = displayBounds.Width - parent.Padding.Horizontal;
                child.Size = size;
                child.Location = nextControlLocation;

                nextControlLocation.Y += size.Height + child.Margin.Bottom;
            }

            return false;
        }
    }
}
