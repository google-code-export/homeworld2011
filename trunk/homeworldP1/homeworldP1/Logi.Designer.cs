namespace homeworldP1
{
    partial class Logi
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
            this.Loggi = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // Loggi
            // 
            this.Loggi.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Loggi.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.Loggi.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Loggi.Cursor = System.Windows.Forms.Cursors.Cross;
            this.Loggi.Location = new System.Drawing.Point(0, 0);
            this.Loggi.Name = "Loggi";
            this.Loggi.Size = new System.Drawing.Size(437, 420);
            this.Loggi.TabIndex = 0;
            this.Loggi.Text = "";
            // 
            // Logi
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(434, 419);
            this.ControlBox = false;
            this.Controls.Add(this.Loggi);
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.Name = "Logi";
            this.Text = "Logi";
            this.ResumeLayout(false);

        }

        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tekst"></param>
        public void setText(string tekst)
        {
            this.Loggi.Text = tekst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tekst"></param>
        public void addText(string tekst)
        {
            this.Loggi.Text = tekst + this.Loggi.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tekst"></param>
        public void setTitle(string tekst)
        {
            this.Text = tekst;
        }


        private System.Windows.Forms.RichTextBox Loggi;
    }
}