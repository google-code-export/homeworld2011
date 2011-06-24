using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PlagueEngine.Tools;

namespace PlagueEngine
{
    public partial class LogWindow : Form
    {
        public ScrollingRichTextBox TextBox
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

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            this.textBox.ScrollToBottom();
        }
    }
}
