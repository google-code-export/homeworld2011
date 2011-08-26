namespace PlagueEngine.Editor
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
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.TreePanel = new System.Windows.Forms.Panel();
            this.objectTree = new PlagueEngine.Editor.Controls.ObjectTree();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.mainMenu = new PlagueEngine.Editor.Controls.MainMenu();
            this.TreePanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(424, 585);
            this.tabControlMain.TabIndex = 0;
            // 
            // TreePanel
            // 
            this.TreePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TreePanel.Controls.Add(this.objectTree);
            this.TreePanel.Location = new System.Drawing.Point(424, 25);
            this.TreePanel.MaximumSize = new System.Drawing.Size(260, 0);
            this.TreePanel.MinimumSize = new System.Drawing.Size(260, 0);
            this.TreePanel.Name = "TreePanel";
            this.TreePanel.Padding = new System.Windows.Forms.Padding(10);
            this.TreePanel.Size = new System.Drawing.Size(260, 585);
            this.TreePanel.TabIndex = 5;
            // 
            // objectTree
            // 
            this.objectTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectTree.Location = new System.Drawing.Point(10, 10);
            this.objectTree.Name = "objectTree";
            this.objectTree.Size = new System.Drawing.Size(240, 565);
            this.objectTree.TabIndex = 0;
            // 
            // MainPanel
            // 
            this.MainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MainPanel.Controls.Add(this.tabControlMain);
            this.MainPanel.Location = new System.Drawing.Point(5, 25);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(424, 585);
            this.MainPanel.TabIndex = 6;
            // 
            // mainMenu
            // 
            this.mainMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(684, 25);
            this.mainMenu.TabIndex = 7;
            // 
            // GameObjectEditorWindow
            // 
            this.ClientSize = new System.Drawing.Size(684, 612);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.TreePanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(700, 650);
            this.Name = "GameObjectEditorWindow";
            this.Text = "Game-Object-Editor";
            this.TreePanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.Panel TreePanel;
        private System.Windows.Forms.Panel MainPanel;

        private Controls.ObjectTree objectTree;
        private Controls.MainMenu mainMenu;
    }
}