using System.Windows.Forms;
using PlagueEngine.Editor.TabPages;
using PlagueEngine.Editor.Controls;


/********************************************************************************/
// PlagueEngine.Editor
/********************************************************************************/
namespace PlagueEngine.Editor
{
    /********************************************************************************/
    // Game Object Editor Window
    /********************************************************************************/
    partial class GameObjectEditorWindow : Form
    {
        
        private readonly EditorData _editorData;

        private readonly EditorSniffer _sniffer;


        public GameObjectEditorWindow(Game game)
        {
            InitializeComponent();
            _editorData = new EditorData(game);
            _sniffer = new EditorSniffer(this);
            var editTab = new EditTab(_editorData);
            tabControlMain.TabPages.Add(new NewTab(_editorData));
            tabControlMain.TabPages.Add(editTab);
            var objectTree = new ObjectTree(_editorData) {Dock = DockStyle.Fill};
            objectTree.SelectedObjectCallback += editTab.SelectedObject;
            TreePanel.Controls.Add(objectTree);
            _sniffer.EditorReloadCallback += objectTree.FillAllObjectsId;
            WindowResize();
        }

        private void GameObjectEditorWindowResize(object sender, System.EventArgs e)
        {
            WindowResize();
        }
    }

}