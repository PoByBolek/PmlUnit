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
            this.TestList = new System.Windows.Forms.ListView();
            this.TestNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ExecutionTimeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TestStatusImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // TestList
            // 
            this.TestList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TestList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TestNameColumn,
            this.ExecutionTimeColumn});
            this.TestList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TestList.FullRowSelect = true;
            this.TestList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.TestList.HideSelection = false;
            this.TestList.Location = new System.Drawing.Point(0, 0);
            this.TestList.Name = "TestList";
            this.TestList.Size = new System.Drawing.Size(150, 150);
            this.TestList.SmallImageList = this.TestStatusImageList;
            this.TestList.TabIndex = 0;
            this.TestList.UseCompatibleStateImageBehavior = false;
            this.TestList.View = System.Windows.Forms.View.Details;
            this.TestList.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            // 
            // TestNameColumn
            // 
            this.TestNameColumn.Text = "Name";
            // 
            // ExecutionTimeColumn
            // 
            this.ExecutionTimeColumn.Text = "Time";
            // 
            // TestStatusImageList
            // 
            this.TestStatusImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // TestListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TestList);
            this.Name = "TestListView";
            this.SizeChanged += new System.EventHandler(this.OnSizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView TestList;
        private System.Windows.Forms.ColumnHeader TestNameColumn;
        private System.Windows.Forms.ColumnHeader ExecutionTimeColumn;
        private System.Windows.Forms.ImageList TestStatusImageList;
    }
}
