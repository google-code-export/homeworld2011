namespace PlagueEngine.Editor.Controls
{
    partial class NewPropertyGridControl
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
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.buttonCreateObject = new System.Windows.Forms.Button();
            this.buttonCreateDefinition = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonDeleteDefinition = new System.Windows.Forms.Button();
            this.comboBoxDefinitions = new PlagueEngine.Editor.Controls.BetterComboBox();
            this.comboBoxObjectsNames = new PlagueEngine.Editor.Controls.BetterComboBox();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.Location = new System.Drawing.Point(6, 66);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(430, 452);
            this.propertyGrid.TabIndex = 0;
            // 
            // buttonCreateObject
            // 
            this.buttonCreateObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateObject.Location = new System.Drawing.Point(203, 524);
            this.buttonCreateObject.Name = "buttonCreateObject";
            this.buttonCreateObject.Size = new System.Drawing.Size(115, 23);
            this.buttonCreateObject.TabIndex = 1;
            this.buttonCreateObject.Text = "Create object";
            this.buttonCreateObject.UseVisualStyleBackColor = true;
            // 
            // buttonCreateDefinition
            // 
            this.buttonCreateDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateDefinition.Location = new System.Drawing.Point(324, 524);
            this.buttonCreateDefinition.Name = "buttonCreateDefinition";
            this.buttonCreateDefinition.Size = new System.Drawing.Size(113, 23);
            this.buttonCreateDefinition.TabIndex = 2;
            this.buttonCreateDefinition.Text = "Create definition";
            this.buttonCreateDefinition.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "GameObject:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Definitions:";
            // 
            // buttonDeleteDefinition
            // 
            this.buttonDeleteDefinition.Location = new System.Drawing.Point(340, 37);
            this.buttonDeleteDefinition.Name = "buttonDeleteDefinition";
            this.buttonDeleteDefinition.Size = new System.Drawing.Size(24, 23);
            this.buttonDeleteDefinition.TabIndex = 13;
            this.buttonDeleteDefinition.Text = "X";
            this.buttonDeleteDefinition.UseVisualStyleBackColor = true;
            this.buttonDeleteDefinition.Click += new System.EventHandler(this.buttonDeleteDefinition_Click);
            // 
            // comboBoxDefinitions
            // 
            this.comboBoxDefinitions.FormattingEnabled = true;
            this.comboBoxDefinitions.Location = new System.Drawing.Point(79, 39);
            this.comboBoxDefinitions.Name = "comboBoxDefinitions";
            this.comboBoxDefinitions.Size = new System.Drawing.Size(255, 21);
            this.comboBoxDefinitions.Sorted = true;
            this.comboBoxDefinitions.TabIndex = 10;
            this.comboBoxDefinitions.SelectedIndexChanged += new System.EventHandler(this.comboBoxDefinitions_SelectedIndexChanged);
            // 
            // comboBoxObjectsNames
            // 
            this.comboBoxObjectsNames.FormattingEnabled = true;
            this.comboBoxObjectsNames.Location = new System.Drawing.Point(79, 10);
            this.comboBoxObjectsNames.Name = "comboBoxObjectsNames";
            this.comboBoxObjectsNames.Size = new System.Drawing.Size(285, 21);
            this.comboBoxObjectsNames.Sorted = true;
            this.comboBoxObjectsNames.TabIndex = 3;
            this.comboBoxObjectsNames.SelectedIndexChanged += new System.EventHandler(this.comboBoxObjectsNames_SelectedIndexChanged);
            // 
            // NewPropertyGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCreateDefinition);
            this.Controls.Add(this.buttonDeleteDefinition);
            this.Controls.Add(this.buttonCreateObject);
            this.Controls.Add(this.comboBoxDefinitions);
            this.Controls.Add(this.comboBoxObjectsNames);
            this.Controls.Add(this.propertyGrid);
            this.Name = "NewPropertyGridControl";
            this.Size = new System.Drawing.Size(440, 550);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button buttonCreateObject;
        private System.Windows.Forms.Button buttonCreateDefinition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonDeleteDefinition;
        private BetterComboBox comboBoxObjectsNames;
        private BetterComboBox comboBoxDefinitions;
    }
}
