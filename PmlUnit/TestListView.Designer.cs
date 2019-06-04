using System.CodeDom.Compiler;

namespace PmlUnit
{
    partial class TestListView
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        [GeneratedCode("Windows Form Designer generated code", "1.0")]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ExpanderImageList = new System.Windows.Forms.ImageList(this.components);
            this.StatusImageList = new System.Windows.Forms.ImageList(this.components);
            this.GroupPanel = new PmlUnit.StretchingVerticalFlowLayoutPanel();
            this.SuspendLayout();
            // 
            // ExpanderImageList
            // 
            this.ExpanderImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ExpanderImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.ExpanderImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // StatusImageList
            // 
            this.StatusImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.StatusImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.StatusImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // GroupPanel
            // 
            this.GroupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupPanel.Location = new System.Drawing.Point(0, 0);
            this.GroupPanel.Margin = new System.Windows.Forms.Padding(0);
            this.GroupPanel.Name = "GroupPanel";
            this.GroupPanel.Size = new System.Drawing.Size(150, 0);
            this.GroupPanel.TabIndex = 0;
            // 
            // TestListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.GroupPanel);
            this.Name = "TestListView";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList ExpanderImageList;
        private System.Windows.Forms.ImageList StatusImageList;
        private StretchingVerticalFlowLayoutPanel GroupPanel;
    }
}
