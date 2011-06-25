using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PlagueEngine.Editor.Controls;

namespace PlagueEngine.Editor.TabPages
{
    partial class NewTab : TabPage
    {
        private EditorData _editorData;
        public NewTab(EditorData editorData)
        {
            _editorData = editorData;
            InitializeComponent();
            this.Name = "NewObjectTab";
            this.Text = "Create new object";
            NewPropertyGridControl newPropertyGridControl = new NewPropertyGridControl(_editorData);
            newPropertyGridControl.Dock = DockStyle.Fill;
            this.Controls.Add(newPropertyGridControl);
        }
    }
}
