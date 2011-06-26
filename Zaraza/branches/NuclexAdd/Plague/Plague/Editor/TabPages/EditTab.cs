using System.Windows.Forms;
using PlagueEngine.Editor.Controls;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.TabPages
{
     partial class EditTab : TabPage
    {
        private readonly EditorData _editorData;
        private readonly EditPropertyGridControl _newPropertyGridControl;
        public EditTab(EditorData editorData)
        {
            _editorData = editorData;
            InitializeComponent();
            Name = "EditObjectTab";
            Text = "Edit object";
            _newPropertyGridControl = new EditPropertyGridControl(_editorData) {Dock = DockStyle.Fill};
            Controls.Add(_newPropertyGridControl);
        }
        public void SelectedObject(GameObjectInstanceData currentSelectedGameObject)
        {
            _newPropertyGridControl.SelectedObject(currentSelectedGameObject);
        }
    }
}
