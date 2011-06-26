namespace PlagueEngine.Editor.MessageBoxs
{
    partial class DeleteDefinition
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridViewLevelDefinition = new System.Windows.Forms.DataGridView();
            this.labelText1 = new System.Windows.Forms.Label();
            this.labelText2 = new System.Windows.Forms.Label();
            this.buttonYes = new System.Windows.Forms.Button();
            this.buttonNo = new System.Windows.Forms.Button();
            this.LevelName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DefinitionCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLevelDefinition)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewLevelDefinition
            // 
            this.dataGridViewLevelDefinition.AllowUserToAddRows = false;
            this.dataGridViewLevelDefinition.AllowUserToDeleteRows = false;
            this.dataGridViewLevelDefinition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLevelDefinition.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LevelName,
            this.DefinitionCount});
            this.dataGridViewLevelDefinition.Location = new System.Drawing.Point(12, 25);
            this.dataGridViewLevelDefinition.Name = "dataGridViewLevelDefinition";
            this.dataGridViewLevelDefinition.ReadOnly = true;
            this.dataGridViewLevelDefinition.RowHeadersWidth = 4;
            this.dataGridViewLevelDefinition.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewLevelDefinition.Size = new System.Drawing.Size(429, 199);
            this.dataGridViewLevelDefinition.TabIndex = 0;
            // 
            // labelText1
            // 
            this.labelText1.AutoSize = true;
            this.labelText1.Location = new System.Drawing.Point(9, 9);
            this.labelText1.Name = "labelText1";
            this.labelText1.Size = new System.Drawing.Size(138, 13);
            this.labelText1.TabIndex = 1;
            this.labelText1.Text = "Objects using this definition:";
            // 
            // labelText2
            // 
            this.labelText2.AutoSize = true;
            this.labelText2.Location = new System.Drawing.Point(12, 227);
            this.labelText2.Name = "labelText2";
            this.labelText2.Size = new System.Drawing.Size(267, 26);
            this.labelText2.TabIndex = 2;
            this.labelText2.Text = "Delete this definition?\r\n(Clicking yes can leads to error while loading level dat" +
                "a)";
            // 
            // buttonYes
            // 
            this.buttonYes.Location = new System.Drawing.Point(15, 267);
            this.buttonYes.Name = "buttonYes";
            this.buttonYes.Size = new System.Drawing.Size(75, 23);
            this.buttonYes.TabIndex = 3;
            this.buttonYes.Text = "Yes";
            this.buttonYes.UseVisualStyleBackColor = true;
            this.buttonYes.Click += new System.EventHandler(this.ButtonYesClick);
            // 
            // buttonNo
            // 
            this.buttonNo.Location = new System.Drawing.Point(366, 267);
            this.buttonNo.Name = "buttonNo";
            this.buttonNo.Size = new System.Drawing.Size(75, 23);
            this.buttonNo.TabIndex = 4;
            this.buttonNo.Text = "No";
            this.buttonNo.UseVisualStyleBackColor = true;
            this.buttonNo.Click += new System.EventHandler(this.ButtonNoClick);
            // 
            // LevelName
            // 
            this.LevelName.DataPropertyName = "LevelName";
            this.LevelName.HeaderText = "Level Name";
            this.LevelName.MinimumWidth = 150;
            this.LevelName.Name = "LevelName";
            this.LevelName.ReadOnly = true;
            this.LevelName.Width = 200;
            // 
            // DefinitionCount
            // 
            this.DefinitionCount.DataPropertyName = "Count";
            this.DefinitionCount.HeaderText = "Definition Count";
            this.DefinitionCount.MinimumWidth = 150;
            this.DefinitionCount.Name = "DefinitionCount";
            this.DefinitionCount.ReadOnly = true;
            this.DefinitionCount.Width = 200;
            // 
            // DeleteDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 299);
            this.ControlBox = false;
            this.Controls.Add(this.buttonNo);
            this.Controls.Add(this.buttonYes);
            this.Controls.Add(this.labelText2);
            this.Controls.Add(this.labelText1);
            this.Controls.Add(this.dataGridViewLevelDefinition);
            this.Name = "DeleteDefinition";
            this.Text = "DeleteDefinition";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLevelDefinition)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewLevelDefinition;
        private System.Windows.Forms.Label labelText1;
        private System.Windows.Forms.Label labelText2;
        private System.Windows.Forms.Button buttonYes;
        private System.Windows.Forms.Button buttonNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn LevelName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefinitionCount;
    }
}