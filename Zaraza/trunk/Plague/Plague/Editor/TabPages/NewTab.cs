using System.Windows.Forms;
using PlagueEngine.Editor.Controls;

namespace PlagueEngine.Editor.TabPages
{
    partial class NewTab : TabPage
    {
        private readonly EditorData _editorData;
        public NewTab(EditorData editorData)
        {
            _editorData = editorData;
            InitializeComponent();
            Name = "NewObjectTab";
            Text = "Create new object";
            var newPropertyGridControl = new NewPropertyGridControl(_editorData) {Dock = DockStyle.Fill};
            Controls.Add(newPropertyGridControl);
        }
    }
}
