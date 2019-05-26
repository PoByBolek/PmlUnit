// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.ComponentModel;
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

        public StretchingVerticalFlowLayoutPanel(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            container.Add(this);
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

        protected override void OnSizeChanged(EventArgs e)
        {
            PerformLayout();
            base.OnSizeChanged(e);
        }
    }

    class StretchingVerticalFlowLayoutEngine : LayoutEngine
    {
        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            Control parent = container as Control;

            var displayBounds = parent.DisplayRectangle;
            var nextControlLocation = new Point();

            foreach (Control child in parent.Controls)
            {
                if (!child.Visible)
                    continue;

                nextControlLocation.Y += child.Margin.Top;

                var size = child.GetPreferredSize(displayBounds.Size);
                size.Width = displayBounds.Width;
                child.Size = size;
                child.Location = nextControlLocation;

                nextControlLocation.Y += size.Height + child.Margin.Bottom;
            }

            parent.Height = nextControlLocation.Y;

            return false;
        }
    }
}
