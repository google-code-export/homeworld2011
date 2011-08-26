namespace PlagueEngine.Editor.Controls.GameObjectsControls
{
    partial class PropertyGrid
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
            this.Grid = new PlagueEngine.Editor.Controls.BetterPropertyGrid();
            this.SuspendLayout();
            // 
            // Grid
            // 
            this.Grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grid.Location = new System.Drawing.Point(0, 0);
            this.Grid.Name = "Grid";
            this.Grid.Size = new System.Drawing.Size(430, 430);
            this.Grid.TabIndex = 1;
            this.Grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.Grid_PropertyValueChanged);
            // 
            // PropertyGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Grid);
            this.Name = "PropertyGrid";
            this.Size = new System.Drawing.Size(430, 430);
            this.ResumeLayout(false);

        }

        #endregion

        private BetterPropertyGrid Grid;
    }
}
