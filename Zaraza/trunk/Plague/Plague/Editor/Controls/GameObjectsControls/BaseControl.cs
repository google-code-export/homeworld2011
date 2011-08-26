using System.Windows.Forms;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.Controls.GameObjectsControls
{
    public partial class BaseControl : UserControl
    {
        protected bool _dataChanged;
        protected GameObjectInstanceData _objectData;

        public BaseControl()
        {
            InitializeComponent();
        }

        public virtual bool IsForGameObject(GameObjectClassName gameObjectClass)
        {
            return false;
        }

        public bool IsDataChanged()
        {
            return _dataChanged;
        }

        public virtual string GetName()
        {
            return "Default component name";
        }

        public virtual void SetObjectData(GameObjectInstanceData objectData)
        {
            _objectData = objectData;
        }
        public override void Refresh()
        {
            base.Refresh();
            foreach (Control control in this.Controls)
            {
                control.Refresh();
            }
        }
        public delegate void DataChanged();
        public DataChanged DataChangedCallback;
    }
}