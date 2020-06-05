// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PmlUnit
{
    class CustomLinkLabel : LinkLabel
    {
        [Category("Mouse")]
        public event EventHandler<LinkHoverEventArgs> LinkHover;

        private Link HoverLink;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            HoverLink = PointInLink(e.X, e.Y);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            if (HoverLink != null)
                OnLinkHover(new LinkHoverEventArgs(HoverLink));
        }

        protected virtual void OnLinkHover(LinkHoverEventArgs e)
        {
            LinkHover?.Invoke(this, e);
        }
    }

    class LinkHoverEventArgs : EventArgs
    {
        public LinkLabel.Link Link { get; }

        public LinkHoverEventArgs(LinkLabel.Link link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));
            Link = link;
        }
    }
}
