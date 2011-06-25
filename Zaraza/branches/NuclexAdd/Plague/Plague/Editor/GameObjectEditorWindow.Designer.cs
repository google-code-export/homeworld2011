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
            this.MainPanel = new System.Windows.Forms.Panel();
            this.MainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(440, 600);
            this.tabControlMain.TabIndex = 0;
            // 
            // TreePanel
            // 
            this.TreePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TreePanel.Location = new System.Drawing.Point(440, 0);
            this.TreePanel.MaximumSize = new System.Drawing.Size(260, 0);
            this.TreePanel.MinimumSize = new System.Drawing.Size(260, 600);
            this.TreePanel.Name = "TreePanel";
            this.TreePanel.Padding = new System.Windows.Forms.Padding(10, 20, 10, 10);
            this.TreePanel.Size = new System.Drawing.Size(260, 600);
            this.TreePanel.TabIndex = 5;
            // 
            // MainPanel
            // 
            this.MainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MainPanel.Controls.Add(this.tabControlMain);
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.MinimumSize = new System.Drawing.Size(0, 600);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(440, 600);
            this.MainPanel.TabIndex = 6;
            // 
            // GameObjectEditorWindow
            // 
            this.ClientSize = new System.Drawing.Size(700, 600);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.TreePanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(700, 600);
            this.Name = "GameObjectEditorWindow";
            this.Text = "Game-Object-Editor";
            this.Resize += new System.EventHandler(this.GameObjectEditorWindow_Resize);
            this.MainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.Panel TreePanel;
        private System.Windows.Forms.Panel MainPanel;


    }
}