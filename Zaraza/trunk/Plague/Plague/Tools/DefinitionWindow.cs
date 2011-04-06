using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using PlagueEngine.LowLevelGameFlow;

/********************************************************************************/
/// PlagueEngine.Tools
/********************************************************************************/
namespace PlagueEngine.Tools
{
    /********************************************************************************/
    /// DefinitionWindow
    /********************************************************************************/
    public partial class DefinitionWindow : Form
    {


        /********************************************************************************/
        /// Field class
        /********************************************************************************/
        public class Field
        {
            public CheckBox checkbox = new CheckBox();
            public Label value = new Label();
            public Label label = new Label();
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        public List<Field> fields = new List<Field>();

        public Button create = new Button();
        public Button cancel = new Button();
        public TextBox textbox = new TextBox();
        public Label definitionName = new Label();

        public bool canceled = false;

        public GameObjectInstanceData currentObject = null;
        /********************************************************************************/



        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public DefinitionWindow(List<PropertyInfo> PropertyInfo, GameObjectInstanceData currentObject)
        {
            InitializeComponent();

            this.currentObject = currentObject;

            this.Size = new Size(300, 90 + PropertyInfo.Count * 25);

            this.create.Text = "Create";
            this.cancel.Text = "Cancel";
            this.definitionName.Text = "Name:";

            this.textbox.Size = new Size(200,20);
            this.definitionName.Size = new Size(40, 20);

            
            this.definitionName.Location = new Point(20, 10);
            this.textbox.Location = new Point(90, 10);
            this.create.Location = new Point(20, 35 + PropertyInfo.Count * 25);
            this.cancel.Location = new Point(150, 35 + PropertyInfo.Count * 25);

            this.create.Click += new EventHandler(Create_Click);
            this.cancel.Click += new EventHandler(Cancel_Click);

            Controls.Add(definitionName);
            Controls.Add(create);
            Controls.Add(cancel);
            Controls.Add(textbox);


            for (int i = 0; i < PropertyInfo.Count; i++)
            {
                
                    Field field = new Field();

                    field.label.Text = PropertyInfo[i].Name;
                    field.label.Location = new Point(20, 35 + 25 * i);
                    field.label.Size = new Size(130, 20);

                    try//nullReferenceExcepton? :O
                    {
                        field.value.Text = PropertyInfo[i].GetValue(currentObject, null).ToString();
                    }
                    catch
                    {
                        field.value.Text = "";
                    }


                    field.value.Location = new Point(150, 35 + 25 * i);
                    field.value.Size = new Size(110, 20);
                    
                    field.checkbox.Location = new Point(270, 35 + 25 * i);
                    field.checkbox.Checked = false;

                    Controls.Add(field.label);
                    Controls.Add(field.checkbox);
                    Controls.Add(field.value);

                    fields.Add(field);
                
            }
        }
        /********************************************************************************/



        /********************************************************************************/
        /// Create button click
        /********************************************************************************/
        private void Create_Click(object sender, System.EventArgs e)
        {
            if (this.textbox.Text != string.Empty)
            {
                bool badData = false;

                foreach (Field f in fields)
                {
                    if (f.checkbox.Checked)
                    {
                        if (f.value.Text == string.Empty)
                        {
                            badData = true;
                        }
                    }
                }

                if (badData)
                {
                    MessageBox.Show("Bad data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    this.Close();
                }
            }
        }
        /********************************************************************************/



        /********************************************************************************/
        /// Cancel button click
        /********************************************************************************/
        private void Cancel_Click(object sender, System.EventArgs e)
        {
            canceled = true;
            this.Close();
            
        }
        /********************************************************************************/


    }
    /********************************************************************************/


}
/********************************************************************************/