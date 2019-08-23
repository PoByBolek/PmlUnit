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
            this.EntryImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // EntryImages
            // 
            this.EntryImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.EntryImages.ImageSize = new System.Drawing.Size(16, 16);
            this.EntryImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // TestListView
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList EntryImages;
    }
}
