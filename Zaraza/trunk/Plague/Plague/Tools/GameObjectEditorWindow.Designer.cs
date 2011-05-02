namespace PlagueEngine.Tools
{
    partial class GameObjectEditorWindow
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonDeleteDefinition = new System.Windows.Forms.Button();
            this.gameObjectsName = new System.Windows.Forms.ComboBox();
            this.ComboboxDefinitions = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonCreateDefinition = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel5 = new System.Windows.Forms.Panel();
            this.buttonCreateDefinitionEdit = new System.Windows.Forms.Button();
            this.checkBoxDisableEditing = new System.Windows.Forms.CheckBox();
            this.buttonForceUpdate = new System.Windows.Forms.Button();
            this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonDeleteObject = new System.Windows.Forms.Button();
            this.comboBoxFilterId = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboboxGameObjectId = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBoxLevelNames = new System.Windows.Forms.ListBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.buttonSaveAs = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonNew = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBoxGamePaused = new System.Windows.Forms.CheckBox();
            this.checkBoxShowCollisionSkin = new System.Windows.Forms.CheckBox();
            this.buttonCommitMeshTransforms = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(389, 492);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.propertyGrid1);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(381, 466);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Create GameObject";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 62);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(375, 372);
            this.propertyGrid1.TabIndex = 16;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged_1);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.buttonDeleteDefinition);
            this.panel2.Controls.Add(this.gameObjectsName);
            this.panel2.Controls.Add(this.ComboboxDefinitions);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(375, 59);
            this.panel2.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "GameObject:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Definitions:";
            // 
            // buttonDeleteDefinition
            // 
            this.buttonDeleteDefinition.Location = new System.Drawing.Point(346, 30);
            this.buttonDeleteDefinition.Name = "buttonDeleteDefinition";
            this.buttonDeleteDefinition.Size = new System.Drawing.Size(24, 23);
            this.buttonDeleteDefinition.TabIndex = 13;
            this.buttonDeleteDefinition.Text = "X";
            this.buttonDeleteDefinition.UseVisualStyleBackColor = true;
            this.buttonDeleteDefinition.Click += new System.EventHandler(this.buttonDeleteDefinition_Click);
            // 
            // gameObjectsName
            // 
            this.gameObjectsName.FormattingEnabled = true;
            this.gameObjectsName.Location = new System.Drawing.Point(85, 6);
            this.gameObjectsName.Name = "gameObjectsName";
            this.gameObjectsName.Size = new System.Drawing.Size(282, 21);
            this.gameObjectsName.Sorted = true;
            this.gameObjectsName.TabIndex = 3;
            this.gameObjectsName.SelectedIndexChanged += new System.EventHandler(this.FillNames);
            // 
            // ComboboxDefinitions
            // 
            this.ComboboxDefinitions.FormattingEnabled = true;
            this.ComboboxDefinitions.Location = new System.Drawing.Point(85, 32);
            this.ComboboxDefinitions.Name = "ComboboxDefinitions";
            this.ComboboxDefinitions.Size = new System.Drawing.Size(255, 21);
            this.ComboboxDefinitions.Sorted = true;
            this.ComboboxDefinitions.TabIndex = 10;
            this.ComboboxDefinitions.SelectedIndexChanged += new System.EventHandler(this.ComboboxDefinitions_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.buttonCreateDefinition);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 434);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 29);
            this.panel1.TabIndex = 14;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(61, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Create";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonCreateDefinition
            // 
            this.buttonCreateDefinition.Location = new System.Drawing.Point(189, 3);
            this.buttonCreateDefinition.Name = "buttonCreateDefinition";
            this.buttonCreateDefinition.Size = new System.Drawing.Size(119, 23);
            this.buttonCreateDefinition.TabIndex = 12;
            this.buttonCreateDefinition.Text = "Create definition";
            this.buttonCreateDefinition.UseVisualStyleBackColor = true;
            this.buttonCreateDefinition.Click += new System.EventHandler(this.buttonCreateDefinition_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.panel5);
            this.tabPage3.Controls.Add(this.propertyGrid2);
            this.tabPage3.Controls.Add(this.panel3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(381, 466);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Edit GameObject";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.buttonCreateDefinitionEdit);
            this.panel5.Controls.Add(this.checkBoxDisableEditing);
            this.panel5.Controls.Add(this.buttonForceUpdate);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 437);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(381, 29);
            this.panel5.TabIndex = 21;
            // 
            // buttonCreateDefinitionEdit
            // 
            this.buttonCreateDefinitionEdit.Location = new System.Drawing.Point(234, 3);
            this.buttonCreateDefinitionEdit.Name = "buttonCreateDefinitionEdit";
            this.buttonCreateDefinitionEdit.Size = new System.Drawing.Size(104, 23);
            this.buttonCreateDefinitionEdit.TabIndex = 20;
            this.buttonCreateDefinitionEdit.Text = "Create definition";
            this.buttonCreateDefinitionEdit.UseVisualStyleBackColor = true;
            this.buttonCreateDefinitionEdit.Click += new System.EventHandler(this.buttonCreateDefinitionEdit_Click);
            // 
            // checkBoxDisableEditing
            // 
            this.checkBoxDisableEditing.AutoSize = true;
            this.checkBoxDisableEditing.Location = new System.Drawing.Point(33, 7);
            this.checkBoxDisableEditing.Name = "checkBoxDisableEditing";
            this.checkBoxDisableEditing.Size = new System.Drawing.Size(95, 17);
            this.checkBoxDisableEditing.TabIndex = 19;
            this.checkBoxDisableEditing.Text = "Disable editing";
            this.checkBoxDisableEditing.UseVisualStyleBackColor = true;
            this.checkBoxDisableEditing.CheckedChanged += new System.EventHandler(this.checkBoxDisableEditing_CheckedChanged);
            // 
            // buttonForceUpdate
            // 
            this.buttonForceUpdate.Location = new System.Drawing.Point(134, 3);
            this.buttonForceUpdate.Name = "buttonForceUpdate";
            this.buttonForceUpdate.Size = new System.Drawing.Size(94, 23);
            this.buttonForceUpdate.TabIndex = 18;
            this.buttonForceUpdate.Text = "Force update";
            this.buttonForceUpdate.UseVisualStyleBackColor = true;
            this.buttonForceUpdate.Click += new System.EventHandler(this.buttonForceUpdate_Click);
            // 
            // propertyGrid2
            // 
            this.propertyGrid2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid2.Location = new System.Drawing.Point(0, 63);
            this.propertyGrid2.Name = "propertyGrid2";
            this.propertyGrid2.Size = new System.Drawing.Size(381, 403);
            this.propertyGrid2.TabIndex = 18;
            this.propertyGrid2.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid2_PropertyValueChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.buttonDeleteObject);
            this.panel3.Controls.Add(this.comboBoxFilterId);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.comboboxGameObjectId);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(381, 63);
            this.panel3.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "GameObject:";
            // 
            // buttonDeleteObject
            // 
            this.buttonDeleteObject.Location = new System.Drawing.Point(349, 30);
            this.buttonDeleteObject.Name = "buttonDeleteObject";
            this.buttonDeleteObject.Size = new System.Drawing.Size(24, 23);
            this.buttonDeleteObject.TabIndex = 14;
            this.buttonDeleteObject.Text = "X";
            this.buttonDeleteObject.UseVisualStyleBackColor = true;
            this.buttonDeleteObject.Click += new System.EventHandler(this.buttonDeleteObject_Click);
            // 
            // comboBoxFilterId
            // 
            this.comboBoxFilterId.FormattingEnabled = true;
            this.comboBoxFilterId.Location = new System.Drawing.Point(91, 3);
            this.comboBoxFilterId.Name = "comboBoxFilterId";
            this.comboBoxFilterId.Size = new System.Drawing.Size(282, 21);
            this.comboBoxFilterId.Sorted = true;
            this.comboBoxFilterId.TabIndex = 15;
            this.comboBoxFilterId.SelectedIndexChanged += new System.EventHandler(this.LoadFilteredID);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Filter:";
            // 
            // comboboxGameObjectId
            // 
            this.comboboxGameObjectId.FormattingEnabled = true;
            this.comboboxGameObjectId.Location = new System.Drawing.Point(91, 32);
            this.comboboxGameObjectId.Name = "comboboxGameObjectId";
            this.comboboxGameObjectId.Size = new System.Drawing.Size(256, 21);
            this.comboboxGameObjectId.TabIndex = 6;
            this.comboboxGameObjectId.SelectedIndexChanged += new System.EventHandler(this.comboboxGameObjectId_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBoxLevelNames);
            this.tabPage2.Controls.Add(this.panel4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(381, 466);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Level";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listBoxLevelNames
            // 
            this.listBoxLevelNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLevelNames.FormattingEnabled = true;
            this.listBoxLevelNames.Location = new System.Drawing.Point(3, 3);
            this.listBoxLevelNames.Margin = new System.Windows.Forms.Padding(15);
            this.listBoxLevelNames.Name = "listBoxLevelNames";
            this.listBoxLevelNames.Size = new System.Drawing.Size(375, 431);
            this.listBoxLevelNames.TabIndex = 6;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.buttonSaveAs);
            this.panel4.Controls.Add(this.buttonLoad);
            this.panel4.Controls.Add(this.buttonDelete);
            this.panel4.Controls.Add(this.buttonNew);
            this.panel4.Controls.Add(this.buttonSave);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(3, 434);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(375, 29);
            this.panel4.TabIndex = 5;
            // 
            // buttonSaveAs
            // 
            this.buttonSaveAs.Location = new System.Drawing.Point(227, 3);
            this.buttonSaveAs.Name = "buttonSaveAs";
            this.buttonSaveAs.Size = new System.Drawing.Size(61, 23);
            this.buttonSaveAs.TabIndex = 5;
            this.buttonSaveAs.Text = "Save as";
            this.buttonSaveAs.UseVisualStyleBackColor = true;
            this.buttonSaveAs.Click += new System.EventHandler(this.buttonSaveAs_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(80, 3);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(68, 23);
            this.buttonLoad.TabIndex = 1;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(294, 3);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(61, 23);
            this.buttonDelete.TabIndex = 4;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonNew
            // 
            this.buttonNew.Location = new System.Drawing.Point(13, 3);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(61, 23);
            this.buttonNew.TabIndex = 2;
            this.buttonNew.Text = "New";
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(154, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(67, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.checkBox2);
            this.tabPage4.Controls.Add(this.checkBox1);
            this.tabPage4.Controls.Add(this.checkBoxGamePaused);
            this.tabPage4.Controls.Add(this.checkBoxShowCollisionSkin);
            this.tabPage4.Controls.Add(this.buttonCommitMeshTransforms);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(381, 466);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Game Properites";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(204, 89);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(55, 17);
            this.checkBox2.TabIndex = 8;
            this.checkBox2.Text = "SSAO";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(204, 43);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(167, 17);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "Show heightmap collision skin";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBoxGamePaused
            // 
            this.checkBoxGamePaused.AutoSize = true;
            this.checkBoxGamePaused.Location = new System.Drawing.Point(204, 66);
            this.checkBoxGamePaused.Name = "checkBoxGamePaused";
            this.checkBoxGamePaused.Size = new System.Drawing.Size(92, 17);
            this.checkBoxGamePaused.TabIndex = 6;
            this.checkBoxGamePaused.Text = "Game paused";
            this.checkBoxGamePaused.UseVisualStyleBackColor = true;
            this.checkBoxGamePaused.CheckedChanged += new System.EventHandler(this.checkBoxGamePaused_CheckedChanged);
            // 
            // checkBoxShowCollisionSkin
            // 
            this.checkBoxShowCollisionSkin.AutoSize = true;
            this.checkBoxShowCollisionSkin.Location = new System.Drawing.Point(204, 20);
            this.checkBoxShowCollisionSkin.Name = "checkBoxShowCollisionSkin";
            this.checkBoxShowCollisionSkin.Size = new System.Drawing.Size(115, 17);
            this.checkBoxShowCollisionSkin.TabIndex = 5;
            this.checkBoxShowCollisionSkin.Text = "Show collision skin";
            this.checkBoxShowCollisionSkin.UseVisualStyleBackColor = true;
            this.checkBoxShowCollisionSkin.CheckedChanged += new System.EventHandler(this.checkBoxShowCollisionSkin_CheckedChanged);
            // 
            // buttonCommitMeshTransforms
            // 
            this.buttonCommitMeshTransforms.Location = new System.Drawing.Point(8, 14);
            this.buttonCommitMeshTransforms.Name = "buttonCommitMeshTransforms";
            this.buttonCommitMeshTransforms.Size = new System.Drawing.Size(147, 23);
            this.buttonCommitMeshTransforms.TabIndex = 4;
            this.buttonCommitMeshTransforms.Text = "Commit Mesh Transforms";
            this.buttonCommitMeshTransforms.UseVisualStyleBackColor = true;
            this.buttonCommitMeshTransforms.Click += new System.EventHandler(this.buttonCommitMeshTransforms_Click);
            // 
            // panel6
            // 
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(389, 492);
            this.panel6.TabIndex = 22;
            // 
            // GameObjectEditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 492);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel6);
            this.Name = "GameObjectEditorWindow";
            this.Text = "Game-Object-Editor";
            this.Activated += new System.EventHandler(this.GameObjectEditorWindow_Activated);
            this.Deactivate += new System.EventHandler(this.GameObjectEditorWindow_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameObjectEditorWindow_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox gameObjectsName;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ComboboxDefinitions;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonNew;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonCreateDefinition;
        private System.Windows.Forms.Button buttonDeleteDefinition;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboboxGameObjectId;
        private System.Windows.Forms.Button buttonDeleteObject;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxFilterId;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.PropertyGrid propertyGrid2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListBox listBoxLevelNames;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.CheckBox checkBoxDisableEditing;
        private System.Windows.Forms.Button buttonForceUpdate;
        private System.Windows.Forms.Button buttonCreateDefinitionEdit;
        private System.Windows.Forms.Button buttonSaveAs;
        private System.Windows.Forms.Button buttonCommitMeshTransforms;
        private System.Windows.Forms.CheckBox checkBoxShowCollisionSkin;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.CheckBox checkBoxGamePaused;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;


    }
}