namespace PlagueEngine.Editor.Controls
{
    partial class Item
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
            this.expand = new System.Windows.Forms.Button();
            this.name = new System.Windows.Forms.Label();
            this.modification = new System.Windows.Forms.Label();
            this.itemPanel = new System.Windows.Forms.Panel();
            this.itemPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // expand
            // 
            this.expand.Cursor = System.Windows.Forms.Cursors.Hand;
            this.expand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.expand.Location = new System.Drawing.Point(2, 8);
            this.expand.Margin = new System.Windows.Forms.Padding(0);
            this.expand.Name = "expand";
            this.expand.Size = new System.Drawing.Size(15, 15);
            this.expand.TabIndex = 0;
            this.expand.Text = "+";
            this.expand.UseCompatibleTextRendering = true;
            this.expand.UseVisualStyleBackColor = true;
            this.expand.Click += new System.EventHandler(this.expand_Click);
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.name.Location = new System.Drawing.Point(20, 9);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(46, 13);
            this.name.TabIndex = 1;
            this.name.Text = "default";
            // 
            // modification
            // 
            this.modification.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.modification.AutoSize = true;
            this.modification.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.modification.Location = new System.Drawing.Point(344, 8);
            this.modification.Name = "modification";
            this.modification.Size = new System.Drawing.Size(81, 13);
            this.modification.TabIndex = 2;
            this.modification.Text = "Zmodyfikowany";
            this.modification.Visible = false;
            // 
            // itemPanel
            // 
            this.itemPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.itemPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.itemPanel.Controls.Add(this.name);
            this.itemPanel.Controls.Add(this.modification);
            this.itemPanel.Controls.Add(this.expand);
            this.itemPanel.Location = new System.Drawing.Point(1, 1);
            this.itemPanel.Name = "itemPanel";
            this.itemPanel.Size = new System.Drawing.Size(428, 28);
            this.itemPanel.TabIndex = 3;
            // 
            // Item
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.itemPanel);
            this.Name = "Item";
            this.Size = new System.Drawing.Size(430, 30);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Item_Paint);
            this.itemPanel.ResumeLayout(false);
            this.itemPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button expand;
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.Label modification;
        private System.Windows.Forms.Panel itemPanel;
    }
}
