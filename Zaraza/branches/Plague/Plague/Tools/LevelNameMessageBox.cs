﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


/********************************************************************************/
///  PlagueEngine.Tools
/********************************************************************************/
namespace PlagueEngine.Tools
{

    /********************************************************************************/
    /// LevelNameMessageBox
    /********************************************************************************/
    public partial class LevelNameMessageBox : Form
    {


        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        public string levelName = string.Empty;
        public bool canceled = true;
        /********************************************************************************/



        /********************************************************************************/
        /// Constructor[1]
        /********************************************************************************/
        public LevelNameMessageBox()
        {
            InitializeComponent();
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Constructor[2]
        /********************************************************************************/
        public LevelNameMessageBox(string labelText)
        {
            InitializeComponent();
            label1.Text = labelText;
        }
        /********************************************************************************/




        /********************************************************************************/
        ///  Button Accept Click
        /********************************************************************************/
        private void buttonAccept_Click(object sender, EventArgs e)
        {
            if (textBoxLevelName.Text != string.Empty)
            {
                this.levelName = textBoxLevelName.Text;
                this.canceled = false;
                
                this.Close();
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Button Cancel Click
        /********************************************************************************/
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.canceled = true;
            this.Close();
        }
        /********************************************************************************/



    }
    /********************************************************************************/


}
/********************************************************************************/