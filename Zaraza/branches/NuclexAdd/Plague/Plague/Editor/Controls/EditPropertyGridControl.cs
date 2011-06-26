using System.Windows.Forms;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.Controls
{
    partial class EditPropertyGridControl : UserControl
    {
        private GameObjectClassName _currentClassNameEdit;
        private GameObjectInstanceData _currentEditGameObject;
        
        private readonly EditorData _editorData;
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
