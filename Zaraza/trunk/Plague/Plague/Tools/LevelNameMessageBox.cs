using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlagueEngine.Tools
{
    public partial class LevelNameMessageBox : Form
    {
        public string levelName = string.Empty;
        public bool canceled = true;
        public LevelNameMessageBox()
        {
            InitializeComponent();
        }

        public LevelNameMessageBox(string labelText)
        {
            InitializeComponent();
            label1.Text = labelText;
        }
        private void buttonAccept_Click(object sender, EventArgs e)
        {
            if (textBoxLevelName.Text != string.Empty)
            {
                this.levelName = textBoxLevelName.Text;
                this.canceled = false;
                
                this.Hide();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.canceled = true;
            this.Hide();
        }


    }

}
