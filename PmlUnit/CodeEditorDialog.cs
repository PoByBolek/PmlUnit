// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PmlUnit
{
    class CodeEditorDialog : Component
    {
        private readonly Form Dialog;
        private readonly CodeEditorControl Control;

        public CodeEditorDialog()
        {
            Control = CreateControl();
            try
            {
                Dialog = CreateDialog(Control);
            }
            catch
            {
                Control.Dispose();
                throw;
            }
        }

        public CodeEditorDialog(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            Control = CreateControl();
            try
            {
                Dialog = CreateDialog(Control);
                container.Add(this);
            }
            catch
            {
                if (Dialog != null)
                    Dialog.Dispose();
                Control.Dispose();
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Dialog.Dispose();
            }
        }

        public Font Font
        {
            get { return Dialog.Font; }
            set { Dialog.Font = value; }
        }

        public CodeEditorDescriptor Descriptor
        {
            get { return Control.Descriptor; }
            set { Control.Descriptor = value; }
        }

        public DialogResult ShowDialog()
        {
            return Dialog.ShowDialog();
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            return Dialog.ShowDialog(owner);
        }

        private static CodeEditorControl CreateControl()
        {
            var result = new CodeEditorControl();
            try
            {
                result.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                result.Location = new Point(12, 12);
                result.Name = "CodeEditorControl";
                result.TabIndex = 0;
            }
            catch
            {
                result.Dispose();
                throw;
            }
            return result;
        }

        private Form CreateDialog(CodeEditorControl control)
        {
            var result = new Form();
            Button okButton = null;
            Button cancelButton = null;
            try
            {
                //
                // okButton
                //
                okButton = new Button();
                okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                okButton.DialogResult = DialogResult.OK;
                okButton.Location = new Point(306, 114);
                okButton.Margin = new Padding(3, 10, 3, 3);
                okButton.Name = "okButton";
                okButton.Size = new Size(75, 23);
                okButton.TabIndex = 1;
                okButton.Text = "&OK";
                okButton.UseVisualStyleBackColor = true;
                okButton.Click += OnOkButtonClick;
                //
                // cancelButton
                //
                cancelButton = new Button();
                cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(387, 114);
                cancelButton.Margin = new Padding(3, 10, 3, 3);
                cancelButton.Name = "cancelButton";
                cancelButton.Size = new Size(75, 23);
                cancelButton.TabIndex = 2;
                cancelButton.Text = "&Cancel";
                cancelButton.UseVisualStyleBackColor = true;
                //
                // form
                //
                result.AcceptButton = okButton;
                result.AutoScaleDimensions = new SizeF(6F, 13F);
                result.AutoScaleMode = AutoScaleMode.Font;
                result.CancelButton = cancelButton;
                result.ClientSize = new Size(474, 149);
                result.Controls.Add(cancelButton);
                result.Controls.Add(okButton);
                result.Controls.Add(control);
                result.Name = "Dialog";
                result.MaximizeBox = false;
                result.MaximumSize = new Size(1920, 189);
                result.MinimizeBox = false;
                result.MinimumSize = new Size(389, 189);
                result.ShowIcon = false;
                result.ShowInTaskbar = false;
                result.Text = "Select Editor";
                return result;
            }
            catch
            {
                if (okButton != null)
                    okButton.Dispose();
                if (cancelButton != null)
                    cancelButton.Dispose();

                result.Dispose();
                throw;
            }
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            if (!Control.ValidateChildren())
                Dialog.DialogResult = DialogResult.None;
        }
    }
}
