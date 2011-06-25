using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PlagueEngine.Editor.Controls;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.TabPages
{
     partial class EditTab : TabPage
    {
        private EditorData _editorData;
        private EditPropertyGridControl _newPropertyGridControl;
        public EditTab(EditorData editorData)
        {
            _editorData = editorData;
            InitializeComponent();
            this.Name = "EditObjectTab";
            this.Text = "Edit object";
            _newPropertyGridControl = new EditPropertyGridControl(_editorData);
            _newPropertyGridControl.Dock = DockStyle.Fill;
            this.Controls.Add(_newPropertyGridControl);
        }
        public void SelectedObject(GameObjectInstanceData currentSelectedGameObject)
        {
            _newPropertyGridControl.SelectedObject(currentSelectedGameObject);
        }
    }
}
