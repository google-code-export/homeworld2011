using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlagueEngine.Editor.Controls.GameObjectsControls
{
    public partial class Bloom : BaseControl
    {
        
        public Bloom()
        {
            InitializeComponent();
        }

        public override bool IsForGameObject(GameObjectClassName gameObjectClass)
        {
            return gameObjectClass.ClassName.Equals("Bloom");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _dataChanged = true;
            _objectData.Position = new Microsoft.Xna.Framework.Vector3(12, 10, 4);
            if (DataChangedCallback != null)
            {
                DataChangedCallback();
            }
        }
    }
}
