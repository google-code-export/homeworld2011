using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;


namespace PlagueEngine.Tools
{
    public partial class DefinitionWindow : Form
    {
        public class Field
        {
            public CheckBox checkbox = new CheckBox();
            public Label label = new Label();
        }

        public List<Field> fields = new List<Field>();

        public Button create = new Button();
        public Button cancel = new Button();
        public TextBox textbox = new TextBox();

        public bool canceled = false;

        public DefinitionWindow(List<PropertyInfo> fieldInfo)
        {
            InitializeComponent();

            this.Size = new Size(260, 90 + fieldInfo.Count * 25);

            this.create.Text = "Create";
            this.cancel.Text = "Cancel";

            this.textbox.Size = new Size(160,20);

            this.textbox.Location = new Point(60, 10);
            this.create.Location = new Point(20, 35 + fieldInfo.Count  * 25);
            this.cancel.Location = new Point(150, 35 + fieldInfo.Count * 25);

            this.create.Click += new EventHandler(Create_Click);
            this.cancel.Click += new EventHandler(Cancel_Click);

            Controls.Add(create);
            Controls.Add(cancel);
            Controls.Add(textbox);


            for (int i = 0; i < fieldInfo.Count; i++)
            {
                
                    Field field = new Field();

                    field.label.Text = fieldInfo[i].Name;
                    field.label.Location = new Point(20, 35 + 25 * i);
                    field.label.Size = new Size(130, 20);

                    field.checkbox.Location = new Point(190, 35 + 25 * i);
                    field.checkbox.Checked = false;

                    Controls.Add(field.label);
                    Controls.Add(field.checkbox);

                    fields.Add(field);
                
            }
        }

        private void Create_Click(object sender, System.EventArgs e)
        {
            this.Hide();
        }
        private void Cancel_Click(object sender, System.EventArgs e)
        {
            canceled = true;
            this.Hide();
            
        }

    }
}
