using System.CodeDom.Compiler;

namespace PmlUnit
{
    partial class TestListView
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        [GeneratedCode("Windows Form Designer generated code", "1.0")]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TestStatusImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // TestStatusImageList
            // 
            this.TestStatusImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.TestStatusImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.TestStatusImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // TestListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Name = "TestListView";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList TestStatusImageList;
    }
}
