using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PlagueEngine.LowLevelGameFlow;



namespace PlagueEngine.Editor.Controls
{

    partial class EditPropertyGridControl : UserControl
    {

        private GameObjectClassName currentClassNameEdit = null;
        private GameObjectInstanceData currentEditGameObject = null;
        
        private EditorData _editorData;
        public EditPropertyGridControl(EditorData editorData)
        {
            _editorData = editorData;
            InitializeComponent();
        }

        public void SelectedObject(GameObjectInstanceData currentSelectedGameObject){
            propertyGrid.SelectedObject = currentSelectedGameObject;
        }

    }
}
