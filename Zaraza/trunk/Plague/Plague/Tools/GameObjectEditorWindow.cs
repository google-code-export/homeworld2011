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
    public partial class GameObjectEditorWindow : Form
    {

        private GameObjectEditor gameObjectEditor = null;




        public GameObjectEditorWindow(GameObjectEditor gameObjectEditor)
        {
            InitializeComponent();
            this.gameObjectEditor = gameObjectEditor;
            this.Visible = true;
        }

        public void addGameObjectName(string name)
        {
            gameObjectsName.Items.Add(name);
        }


        private void FillNames(object sender, EventArgs e)
        {
            string objectName = gameObjectsName.Items[gameObjectsName.SelectedIndex].ToString();
            List<string> fieldNames = gameObjectEditor.getClassFieldsName(objectName);

            GridNamesValues.Rows.Clear();

            for (int i = 0; i < fieldNames.Count; i++)
            {
                GridNamesValues.Rows.Add();
                GridNamesValues.Rows[i].Cells[0].Value = fieldNames[i];
            }
        }




    }
}

