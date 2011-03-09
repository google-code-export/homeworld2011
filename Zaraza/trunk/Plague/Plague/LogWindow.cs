using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlagueEngine
{
    public partial class LogWindow : Form
    {
        public TextBox TextBox
        {
            get
            {
                return this.textBox;
            }
        }
        
        public LogWindow()
        {
            InitializeComponent();
        }
    }
}
