namespace PlagueEngine.Editor.Controls
{
    partial class GameObjectControl
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
            this.controlsPanel = new System.Windows.Forms.Panel();
            this.noObjectLabel = new System.Windows.Forms.Label();
            this.controlsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlsPanel
            // 
            this.controlsPanel.AutoScroll = true;
            this.controlsPanel.Controls.Add(this.noObjectLabel);
            this.controlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlsPanel.Location = new System.Drawing.Point(0, 0);
            this.controlsPanel.Name = "controlsPanel";
            this.controlsPanel.Size = new System.Drawing.Size(383, 164);
            this.controlsPanel.TabIndex = 0;
            // 
            // noObjectLabel
            // 
            this.noObjectLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noObjectLabel.Location = new System.Drawing.Point(0, 0);
            this.noObjectLabel.Name = "noObjectLabel";
            this.noObjectLabel.Size = new System.Drawing.Size(383, 164);
            this.noObjectLabel.TabIndex = 0;
            this.noObjectLabel.Text = "Game object hasn\'t been selected...";
            this.noObjectLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GameObjectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.controlsPanel);
            this.Name = "GameObjectControl";
            this.Size = new System.Drawing.Size(383, 164);
            this.controlsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel controlsPanel;
        private System.Windows.Forms.Label noObjectLabel;


    }
}
