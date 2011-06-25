using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.MessageBoxs
{
    partial class DeleteDefinition : Form
    {
        public delegate void ButtonClicked(GameObjectDefinition definition);
        public ButtonClicked YesButtonClickedCallback;
        private GameObjectDefinition _definition;
        public DeleteDefinition(GameObjectDefinition definition, List<DefinitionCounter> definitionCounters)
        {
            InitializeComponent();
            if (definition != null)
            {
                _definition = definition;
                this.labelText1.Text = string.Format("Objects using definition [{0}]:", _definition.Name);
            }
            dataGridViewLevelDefinition.DataSource = definitionCounters;
            Show();

        }

        public void buttonYes_Click(object sender, EventArgs e)
        {
            YesButtonClickedCallback(_definition);
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
