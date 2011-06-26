namespace PlagueEngine.Editor.Controls
{
    partial class EditPropertyGridControl
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
            this.buttonCreateDefinition = new System.Windows.Forms.Button();
            this.buttonSaveObject = new System.Windows.Forms.Button();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // buttonCreateDefinition
            // 
            this.buttonCreateDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateDefinition.Location = new System.Drawing.Point(324, 524);
            this.buttonCreateDefinition.Name = "buttonCreateDefinition";
            this.buttonCreateDefinition.Size = new System.Drawing.Size(113, 23);
            this.buttonCreateDefinition.TabIndex = 5;
            this.buttonCreateDefinition.Text = "Create definition";
            this.buttonCreateDefinition.UseVisualStyleBackColor = true;
            // 
            // buttonSaveObject
            // 
            this.buttonSaveObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveObject.Location = new System.Drawing.Point(203, 524);
            this.buttonSaveObject.Name = "buttonSaveObject";
            this.buttonSaveObject.Size = new System.Drawing.Size(115, 23);
            this.buttonSaveObject.TabIndex = 4;
            this.buttonSaveObject.Text = "Save object";
            this.buttonSaveObject.UseVisualStyleBackColor = true;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.Location = new System.Drawing.Point(6, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(431, 515);
            this.propertyGrid.TabIndex = 0;
            // 
            // EditPropertyGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonCreateDefinition);
            this.Controls.Add(this.buttonSaveObject);
            this.Controls.Add(this.propertyGrid);
            this.Name = "EditPropertyGridControl";
            this.Size = new System.Drawing.Size(440, 550);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCreateDefinition;
        private System.Windows.Forms.Button buttonSaveObject;
        private System.Windows.Forms.PropertyGrid propertyGrid;

    }
}
