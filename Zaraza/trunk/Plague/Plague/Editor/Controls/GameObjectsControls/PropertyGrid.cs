using System.Windows.Forms;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.Controls.GameObjectsControls
{
    public partial class PropertyGrid : BaseControl
    {
        public PropertyGrid()
        {
            InitializeComponent();
        }

        override public void SetObjectData(GameObjectInstanceData objectData)
        {
            base.SetObjectData(objectData);
            Grid.SelectedObject = objectData;
        }

        private void Grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _dataChanged = true;
            if (DataChangedCallback != null)
            {
                DataChangedCallback();
            }
        }

        override public string GetName()
        {
            return "PropertyGrid";
        }

        override public bool IsForGameObject(GameObjectClassName gameObjectClass)
        {
            return true;
        }
    }
}