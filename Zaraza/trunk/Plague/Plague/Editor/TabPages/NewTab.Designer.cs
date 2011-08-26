namespace PlagueEngine.Editor.TabPages
{
    partial class NewTab
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
            this.buttonCreateObject = new System.Windows.Forms.Button();
            this.gameObjectControl = new PlagueEngine.Editor.Controls.GameObjectControl();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDeleteDefinition = new System.Windows.Forms.Button();
            this.comboBoxDefinitions = new PlagueEngine.Editor.Controls.BetterComboBox();
            this.comboBoxObjectsNames = new PlagueEngine.Editor.Controls.BetterComboBox();
            this.SuspendLayout();
            // 
            // buttonCreateDefinition
            // 
            this.buttonCreateDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateDefinition.Location = new System.Drawing.Point(496, 448);
            this.buttonCreateDefinition.Name = "buttonCreateDefinition";
            this.buttonCreateDefinition.Size = new System.Drawing.Size(113, 23);
            this.buttonCreateDefinition.TabIndex = 21;
            this.buttonCreateDefinition.Text = "Create definition";
            this.buttonCreateDefinition.UseVisualStyleBackColor = true;
            this.buttonCreateDefinition.Click += new System.EventHandler(this.buttonCreateDefinition_Click);
            // 
            // buttonCreateObject
            // 
            this.buttonCreateObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateObject.Location = new System.Drawing.Point(375, 448);
            this.buttonCreateObject.Name = "buttonCreateObject";
            this.buttonCreateObject.Size = new System.Drawing.Size(115, 23);
            this.buttonCreateObject.TabIndex = 20;
            this.buttonCreateObject.Text = "Create object";
            this.buttonCreateObject.UseVisualStyleBackColor = true;
            this.buttonCreateObject.Click += new System.EventHandler(this.buttonCreateObject_Click);
            // 
            // gameObjectControl
            // 
            this.gameObjectControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gameObjectControl.Location = new System.Drawing.Point(0, 62);
            this.gameObjectControl.Name = "gameObjectControl";
            this.gameObjectControl.Size = new System.Drawing.Size(609, 384);
            this.gameObjectControl.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Definitions:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Game Object type:";
            // 
            // buttonDeleteDefinition
            // 
            this.buttonDeleteDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeleteDefinition.Location = new System.Drawing.Point(573, 28);
            this.buttonDeleteDefinition.Name = "buttonDeleteDefinition";
            this.buttonDeleteDefinition.Size = new System.Drawing.Size(24, 23);
            this.buttonDeleteDefinition.TabIndex = 18;
            this.buttonDeleteDefinition.Text = "X";
            this.buttonDeleteDefinition.UseVisualStyleBackColor = true;
            this.buttonDeleteDefinition.Click += new System.EventHandler(this.ButtonDeleteDefinitionClick);
            // 
            // comboBoxDefinitions
            // 
            this.comboBoxDefinitions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDefinitions.FormattingEnabled = true;
            this.comboBoxDefinitions.Location = new System.Drawing.Point(106, 30);
            this.comboBoxDefinitions.Name = "comboBoxDefinitions";
            this.comboBoxDefinitions.Size = new System.Drawing.Size(461, 21);
            this.comboBoxDefinitions.Sorted = true;
            this.comboBoxDefinitions.TabIndex = 16;
            this.comboBoxDefinitions.SelectedIndexChanged += new System.EventHandler(this.ComboBoxDefinitionsSelectedIndexChanged);
            // 
            // comboBoxObjectsNames
            // 
            this.comboBoxObjectsNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxObjectsNames.FormattingEnabled = true;
            this.comboBoxObjectsNames.Location = new System.Drawing.Point(106, 3);
            this.comboBoxObjectsNames.Name = "comboBoxObjectsNames";
            this.comboBoxObjectsNames.Size = new System.Drawing.Size(491, 21);
            this.comboBoxObjectsNames.Sorted = true;
            this.comboBoxObjectsNames.TabIndex = 14;
            this.comboBoxObjectsNames.SelectedIndexChanged += new System.EventHandler(this.ComboBoxObjectsNamesSelectedIndexChanged);
            // 
            // NewTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.buttonCreateDefinition);
            this.Controls.Add(this.buttonCreateObject);
            this.Controls.Add(this.gameObjectControl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonDeleteDefinition);
            this.Controls.Add(this.comboBoxDefinitions);
            this.Controls.Add(this.comboBoxObjectsNames);
            this.Name = "NewTab";
            this.Size = new System.Drawing.Size(612, 474);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonDeleteDefinition;
        private Controls.BetterComboBox comboBoxDefinitions;
        private Controls.BetterComboBox comboBoxObjectsNames;
        private Controls.GameObjectControl gameObjectControl;
        private System.Windows.Forms.Button buttonCreateDefinition;
        private System.Windows.Forms.Button buttonCreateObject;
    }
}
