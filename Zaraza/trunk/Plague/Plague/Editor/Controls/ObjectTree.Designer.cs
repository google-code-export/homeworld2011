﻿namespace PlagueEngine.Editor.Controls
{
    partial class ObjectTree
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeViewObjects = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewObjects
            // 
            this.treeViewObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewObjects.Location = new System.Drawing.Point(0, 0);
            this.treeViewObjects.Name = "treeViewObjects";
            this.treeViewObjects.Size = new System.Drawing.Size(160, 170);
            this.treeViewObjects.TabIndex = 0;
            this.treeViewObjects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewObjectsAfterSelect);
            // 
            // ObjectTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewObjects);
            this.Name = "ObjectTree";
            this.Size = new System.Drawing.Size(160, 170);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewObjects;
    }
}
