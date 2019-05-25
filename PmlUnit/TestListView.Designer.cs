using System.CodeDom.Compiler;

namespace PmlUnit
{
    partial class TestListView
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
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
            this.TestStatusImageList = new System.Windows.Forms.ImageList(this.components);
            this.TestList = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // TestStatusImageList
            // 
            this.TestStatusImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.TestStatusImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.TestStatusImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // TestList
            // 
            this.TestList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TestList.FullRowSelect = true;
            this.TestList.HideSelection = false;
            this.TestList.ImageIndex = 0;
            this.TestList.ImageList = this.TestStatusImageList;
            this.TestList.Location = new System.Drawing.Point(0, 0);
            this.TestList.Name = "TestList";
            this.TestList.SelectedImageIndex = 0;
            this.TestList.ShowRootLines = false;
            this.TestList.Size = new System.Drawing.Size(150, 150);
            this.TestList.TabIndex = 0;
            this.TestList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnAfterSelect);
            // 
            // TestListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TestList);
            this.Name = "TestListView";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList TestStatusImageList;
        private System.Windows.Forms.TreeView TestList;
    }
}
