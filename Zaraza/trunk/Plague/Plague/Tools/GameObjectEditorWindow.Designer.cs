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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
            this.panel5 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonDeleteObject = new System.Windows.Forms.Button();
            this.buttonCreateDefinitionEdit = new System.Windows.Forms.Button();
            this.checkBoxDisableEditing = new System.Windows.Forms.CheckBox();
            this.buttonForceUpdate = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBoxLevelNames = new System.Windows.Forms.ListBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.buttonSaveAs = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonNew = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.inputEnable = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBoxGamePaused = new System.Windows.Forms.CheckBox();
            this.checkBoxShowCollisionSkin = new System.Windows.Forms.CheckBox();
            this.buttonCommitMeshTransforms = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label6 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.tabControl1.Size = new System.Drawing.Size(410, 493);
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
            this.tabPage1.Size = new System.Drawing.Size(402, 467);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Create GameObject";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 62);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(396, 374);
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
            this.panel2.Size = new System.Drawing.Size(396, 59);
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
            this.panel1.Location = new System.Drawing.Point(3, 436);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(396, 28);
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
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.propertyGrid2);
            this.tabPage3.Controls.Add(this.panel5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(402, 467);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Edit GameObject";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(173, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "                      ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(217, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(82, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Currently editing:";
            // 
            // propertyGrid2
            // 
            this.propertyGrid2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid2.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid2.Name = "propertyGrid2";
            this.propertyGrid2.Size = new System.Drawing.Size(402, 398);
            this.propertyGrid2.TabIndex = 18;
            this.propertyGrid2.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid2_PropertyValueChanged);
            this.propertyGrid2.SelectedObjectsChanged += new System.EventHandler(this.propertyGrid2_SelectedObjectsChanged);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.button2);
            this.panel5.Controls.Add(this.buttonDeleteObject);
            this.panel5.Controls.Add(this.buttonCreateDefinitionEdit);
            this.panel5.Controls.Add(this.checkBoxDisableEditing);
            this.panel5.Controls.Add(this.buttonForceUpdate);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 398);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(402, 69);
            this.panel5.TabIndex = 21;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(237, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 23);
            this.button2.TabIndex = 21;
            this.button2.Text = "Refresh";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonDeleteObject
            // 
            this.buttonDeleteObject.Location = new System.Drawing.Point(237, 38);
            this.buttonDeleteObject.Name = "buttonDeleteObject";
            this.buttonDeleteObject.Size = new System.Drawing.Size(99, 23);
            this.buttonDeleteObject.TabIndex = 14;
            this.buttonDeleteObject.Text = "Delete";
            this.buttonDeleteObject.UseVisualStyleBackColor = true;
            this.buttonDeleteObject.Click += new System.EventHandler(this.buttonDeleteObject_Click);
            // 
            // buttonCreateDefinitionEdit
            // 
            this.buttonCreateDefinitionEdit.Location = new System.Drawing.Point(109, 38);
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
            this.checkBoxDisableEditing.Location = new System.Drawing.Point(8, 7);
            this.checkBoxDisableEditing.Name = "checkBoxDisableEditing";
            this.checkBoxDisableEditing.Size = new System.Drawing.Size(95, 17);
            this.checkBoxDisableEditing.TabIndex = 19;
            this.checkBoxDisableEditing.Text = "Disable editing";
            this.checkBoxDisableEditing.UseVisualStyleBackColor = true;
            this.checkBoxDisableEditing.CheckedChanged += new System.EventHandler(this.checkBoxDisableEditing_CheckedChanged);
            // 
            // buttonForceUpdate
            // 
            this.buttonForceUpdate.Location = new System.Drawing.Point(109, 4);
            this.buttonForceUpdate.Name = "buttonForceUpdate";
            this.buttonForceUpdate.Size = new System.Drawing.Size(104, 23);
            this.buttonForceUpdate.TabIndex = 18;
            this.buttonForceUpdate.Text = "Force update";
            this.buttonForceUpdate.UseVisualStyleBackColor = true;
            this.buttonForceUpdate.Click += new System.EventHandler(this.buttonForceUpdate_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBoxLevelNames);
            this.tabPage2.Controls.Add(this.panel4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(402, 467);
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
            this.listBoxLevelNames.Size = new System.Drawing.Size(396, 432);
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
            this.panel4.Location = new System.Drawing.Point(3, 435);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(396, 29);
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
            this.tabPage4.Controls.Add(this.groupBox1);
            this.tabPage4.Controls.Add(this.button3);
            this.tabPage4.Controls.Add(this.inputEnable);
            this.tabPage4.Controls.Add(this.checkBox2);
            this.tabPage4.Controls.Add(this.checkBox1);
            this.tabPage4.Controls.Add(this.checkBoxGamePaused);
            this.tabPage4.Controls.Add(this.checkBoxShowCollisionSkin);
            this.tabPage4.Controls.Add(this.buttonCommitMeshTransforms);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(402, 467);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Game Properites";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox5);
            this.groupBox1.Controls.Add(this.checkBox3);
            this.groupBox1.Controls.Add(this.checkBox4);
            this.groupBox1.Location = new System.Drawing.Point(185, 165);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 130);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FreeCamera";
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(18, 79);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(80, 17);
            this.checkBox5.TabIndex = 13;
            this.checkBox5.Text = "JiglibX drag";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox5_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Location = new System.Drawing.Point(18, 33);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(79, 17);
            this.checkBox3.TabIndex = 11;
            this.checkBox3.Text = "Draw icons";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(18, 56);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(101, 17);
            this.checkBox4.TabIndex = 12;
            this.checkBox4.Text = "JiglibX selection";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(8, 66);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(147, 23);
            this.button3.TabIndex = 10;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // inputEnable
            // 
            this.inputEnable.AutoSize = true;
            this.inputEnable.Location = new System.Drawing.Point(204, 113);
            this.inputEnable.Name = "inputEnable";
            this.inputEnable.Size = new System.Drawing.Size(80, 17);
            this.inputEnable.TabIndex = 9;
            this.inputEnable.Text = "Game input";
            this.inputEnable.UseVisualStyleBackColor = true;
            this.inputEnable.CheckedChanged += new System.EventHandler(this.inputEnable_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
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
            this.panel6.Controls.Add(this.splitContainer1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(624, 493);
            this.panel6.TabIndex = 22;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView1);
            this.splitContainer1.Size = new System.Drawing.Size(624, 493);
            this.splitContainer1.SplitterDistance = 410;
            this.splitContainer1.TabIndex = 4;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(210, 493);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(252, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "            ";
            // 
            // GameObjectEditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 493);
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
            this.tabPage3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.Button buttonDeleteObject;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.PropertyGrid propertyGrid2;
        private System.Windows.Forms.ListBox listBoxLevelNames;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button buttonSaveAs;
        private System.Windows.Forms.Button buttonCommitMeshTransforms;
        private System.Windows.Forms.CheckBox checkBoxShowCollisionSkin;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.CheckBox checkBoxGamePaused;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox inputEnable;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button buttonCreateDefinitionEdit;
        private System.Windows.Forms.CheckBox checkBoxDisableEditing;
        private System.Windows.Forms.Button buttonForceUpdate;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;


    }
}