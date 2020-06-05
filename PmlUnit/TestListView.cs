// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using PmlUnit.Properties;

namespace PmlUnit
{
    partial class TestListView : ScrollableControl
    {
        private const string ExpandedImageKey = "Expanded";
        private const string ExpandedHighlightImageKey = "ExpandedHighlight";
        private const string CollapsedImageKey = "Collapsed";
        private const string CollapsedHighlightImageKey = "CollapsedHighlight";

        private const string PassedImageKey = "Passed";
        private const string FailedImageKey = "Failed";
        private const string NotExecutedImageKey = "NotExecuted";

        [Category("Behavior")]
        public event EventHandler SelectionChanged;

        [Category("Behavior")]
        public event EventHandler GroupingChanged;

        [Category("Behavior")]
        public event EventHandler<TestEventArgs> TestActivate;

        public int EntryHeight => 20;

        public int EntryPadding => 2;

        public Size ImageSize => EntryImages.ImageSize;

        private readonly TestListViewModel Model;
        private readonly TestListViewController Controller;
        private TestListGrouping GroupingField;

        public TestListView()
        {
            GroupingField = TestListGrouping.Result;

            Model = new TestListViewModel();
            Model.Grouper = new TestResultGrouper();
            Model.Changed += OnModelChanged;
            Model.FocusedEntryChanged += OnModelChanged;
            Model.VisibleEntriesChanged += OnVisibleEntriesChanged;
            Model.SelectionChanged += OnModelSelectionChanged;

            Controller = new TestListViewController(Model, this);
            Controller.SelectionChanged += OnControllerSelectionChanged;
            Controller.TestActivate += OnControllerTestActivate;

            InitializeComponent();

            DoubleBuffered = true;

            EntryImages.Images.Add(ExpandedImageKey, Resources.Expanded);
            EntryImages.Images.Add(ExpandedHighlightImageKey, Resources.ExpandedHighlight);
            EntryImages.Images.Add(CollapsedImageKey, Resources.Collapsed);
            EntryImages.Images.Add(CollapsedHighlightImageKey, Resources.CollapsedHighlight);

            EntryImages.Images.Add(NotExecutedImageKey, Resources.NotExecuted);
            EntryImages.Images.Add(FailedImageKey, Resources.Failed);
            EntryImages.Images.Add(PassedImageKey, Resources.Passed);
        }

        [Category("Behavior")]
        [DefaultValue(TestListGrouping.Result)]
        public TestListGrouping Grouping
        {
            get { return GroupingField; }
            set
            {
                if (value != GroupingField)
                {
                    GroupingField = value;

                    if (value == TestListGrouping.Result)
                        Model.Grouper = new TestResultGrouper();
                    else if (value == TestListGrouping.TestCase)
                        Model.Grouper = new TestCaseGrouper();
                    else
                        throw new NotImplementedException(string.Format(
                            CultureInfo.InvariantCulture,
                            "No grouper registered for grouping mode {0}", value
                        ));

                    GroupingChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TestCaseCollection TestCases => Model.TestCases;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<Test> AllTests => AllTestsInternal.ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<Test> PassedTests => AllTestsInternal.Where(test => test.Status == TestStatus.Passed).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<Test> FailedTests => AllTestsInternal.Where(test => test.Status == TestStatus.Failed).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<Test> NotExecutedTests => AllTestsInternal.Where(test => test.Status == TestStatus.NotExecuted).ToList();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<Test> SelectedTests => TestEntries
            .Where(entry => entry.IsSelected || entry.Group.IsSelected)
            .Select(entry => entry.Test)
            .ToList();

        private IEnumerable<Test> AllTestsInternal => TestEntries.Select(entry => entry.Test);

        private IEnumerable<TestListTestEntry> TestEntries => Model.Entries.OfType<TestListTestEntry>();

        private void OnModelChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnVisibleEntriesChanged(object sender, EventArgs e)
        {
            AutoScrollMinSize = new Size(0, Model.VisibleEntries.Count * EntryHeight);
            Invalidate();
        }

        private void OnModelSelectionChanged(object sender, EventArgs e)
        {
            Controller.HandleSelectionChanged(sender, e);
        }

        private void OnControllerSelectionChanged(object sender, EventArgs e)
        {
            Invalidate();
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnControllerTestActivate(object sender, TestEventArgs e)
        {
            TestActivate?.Invoke(this, e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (e == null)
                return;

            var g = e.Graphics;

            using (var brush = new SolidBrush(BackColor))
            {
                g.FillRectangle(brush, e.ClipRectangle);
            }

            using (var options = new TestListPaintOptions(this, e.ClipRectangle, Model.FocusedEntry))
            {
                int width = ClientSize.Width;
                int offset = VerticalScroll.Value;
                int startIndex = (e.ClipRectangle.Top + offset) / EntryHeight;
                startIndex = Math.Max(0, startIndex);
                int endIndex = (e.ClipRectangle.Bottom + offset) / EntryHeight;
                endIndex = Math.Min(endIndex, Model.VisibleEntries.Count - 1);

                for (int i = startIndex; i <= endIndex; i++)
                {
                    var bounds = new Rectangle(0, i * EntryHeight - offset, width, EntryHeight);
                    var entry = Model.VisibleEntries[i];
                    PaintEntryBackground(g, entry, bounds, options);

                    var testEntry = entry as TestListTestEntry;
                    if (testEntry != null)
                    {
                        PaintTestEntry(g, testEntry, bounds, options);
                        continue;
                    }

                    var groupEntry = entry as TestListGroupEntry;
                    if (groupEntry != null)
                    {
                        PaintGroupEntry(g, groupEntry, bounds, options);
                        continue;
                    }
                }
            }
        }

        private void PaintTestEntry(Graphics g, TestListTestEntry entry, Rectangle bounds, TestListPaintOptions options)
        {
            int left = bounds.Left + EntryPadding + 20;
            int right = bounds.Right - EntryPadding;
            int y = bounds.Top + EntryPadding;

            g.DrawImage(GetTestEntryImage(entry.Test.Status), left, y);
            left += 16 + EntryPadding;

            var textBrush = options.GetTextBrush(entry);
            if (left < right && entry.Test.Result != null)
            {
                string duration = entry.Test.Result.Duration.Format();
                int durationWidth = (int)Math.Ceiling(g.MeasureString(duration, options.EntryFont).Width);
                int durationX = Math.Max(left, right - durationWidth);
                g.DrawString(duration, options.EntryFont, textBrush, durationX, y);
                right = durationX - EntryPadding;
            }

            if (left < right)
            {
                var nameBounds = new RectangleF(left, y, right - left, 16);
                g.DrawString(entry.Test.Name, options.EntryFont, textBrush, nameBounds, options.EntryFormat);
            }
        }

        private void PaintGroupEntry(Graphics g, TestListGroupEntry group, Rectangle bounds, TestListPaintOptions options)
        {
            int x = bounds.Left + EntryPadding;
            int y = bounds.Top + EntryPadding;

            var image = GetGroupEntryImage(group.IsExpanded, group == Model.HighlightedIconEntry);
            g.DrawImage(image, x, y);
            x += 16 + EntryPadding;

            var textBrush = options.GetTextBrush(group);
            int nameWidth = (int)Math.Ceiling(g.MeasureString(group.Name, options.HeaderFont).Width);
            g.DrawString(group.Name, options.HeaderFont, textBrush, x, y);
            x += nameWidth;

            var count = " (" + group.Entries.Count + ")";
            g.DrawString(count, options.EntryFont, textBrush, x, y);
        }

        private void PaintEntryBackground(Graphics g, TestListEntry entry, Rectangle bounds, TestListPaintOptions options)
        {
            if (entry.IsSelected)
            {
                g.FillRectangle(options.SelectedBackBrush, bounds);
            }
            else if (entry == options.FocusedEntry)
            {
                bounds.Width -= 1;
                bounds.Height -= 1;
                g.DrawRectangle(options.FocusRectanglePen, bounds);
            }
        }

        private Image GetTestEntryImage(TestStatus status)
        {
            if (status == TestStatus.NotExecuted)
                return EntryImages.Images[NotExecutedImageKey];
            else if (status == TestStatus.Passed)
                return EntryImages.Images[PassedImageKey];
            else
                return EntryImages.Images[FailedImageKey];
        }

        private Image GetGroupEntryImage(bool expanded, bool highlighted)
        {
            string key;
            if (expanded)
                key = highlighted ? ExpandedHighlightImageKey : ExpandedImageKey;
            else
                key = highlighted ? CollapsedHighlightImageKey : CollapsedImageKey;
            return EntryImages.Images[key];
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            Invalidate(); // We need to repaint everything because of the ellipsis characters in too long test names
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override bool IsInputKey(Keys keys)
        {
            switch (keys)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Control | Keys.Up:
                case Keys.Control | Keys.Down:
                case Keys.Control | Keys.Left:
                case Keys.Control | Keys.Right:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Right:
                    return true;
                default:
                    return base.IsInputKey(keys);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Controller.HandleKeyDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Controller.HandleMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Controller.HandleMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Select();
            Controller.HandleMouseClick(e, ModifierKeys);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            Controller.HandleMouseDoubleClick(e, ModifierKeys);
        }
    }
}
