// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PmlUnit
{
    class TestListPaintOptions : IDisposable
    {
        public Rectangle ClipRectangle { get; }
        public TestListBaseEntry FocusedEntry { get; }
        public Pen FocusRectanglePen { get; }
        public Brush NormalTextBrush { get; }
        public Brush SelectedTextBrush { get; }
        public Brush SelectedBackBrush { get; }
        public ImageList StatusImageList { get; }
        public ImageList ExpanderImageList { get; }
        public Font EntryFont { get; }
        public Font HeaderFont { get; }
        public StringFormat EntryFormat { get; }

        public TestListPaintOptions(TestListView view, Rectangle clipRectangle, TestListBaseEntry focusedEntry, ImageList statusImageList, ImageList expanderImageList)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (statusImageList == null)
                throw new ArgumentNullException(nameof(statusImageList));
            if (expanderImageList == null)
                throw new ArgumentNullException(nameof(expanderImageList));

            try
            {
                ClipRectangle = clipRectangle;
                FocusedEntry = view.Focused ? focusedEntry : null;
                FocusRectanglePen = view.Focused ? SystemPens.Highlight.Clone() as Pen : SystemPens.Control.Clone() as Pen;
                StatusImageList = statusImageList;
                ExpanderImageList = expanderImageList;
                EntryFont = view.Font;
                NormalTextBrush = new SolidBrush(view.ForeColor);
                SelectedTextBrush = view.Focused ? SystemBrushes.HighlightText.Clone() as Brush : new SolidBrush(view.ForeColor);
                SelectedBackBrush = view.Focused ? SystemBrushes.Highlight.Clone() as Brush : SystemBrushes.Control.Clone() as Brush;
                HeaderFont = new Font(view.Font, FontStyle.Bold);
                EntryFormat = new StringFormat(StringFormatFlags.NoWrap);
                EntryFormat.Trimming = StringTrimming.EllipsisCharacter;
            }
            catch
            {
                if (FocusRectanglePen != null)
                    FocusRectanglePen.Dispose();
                if (NormalTextBrush != null)
                    NormalTextBrush.Dispose();
                if (SelectedTextBrush != null)
                    SelectedTextBrush.Dispose();
                if (SelectedBackBrush != null)
                    SelectedBackBrush.Dispose();
                if (HeaderFont != null)
                    HeaderFont.Dispose();
                if (EntryFormat != null)
                    EntryFormat.Dispose();
                throw;
            }
        }

        ~TestListPaintOptions()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                FocusRectanglePen.Dispose();
                NormalTextBrush.Dispose();
                SelectedTextBrush.Dispose();
                SelectedBackBrush.Dispose();
                HeaderFont.Dispose();
                EntryFormat.Dispose();
            }
        }
    }
}
