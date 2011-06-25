using System.Windows.Forms;
using PlagueEngine.Editor.TabPages;
using System.Threading;
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
        
        private EditorData _editorData;

        private EditorSniffer _sniffer;


        public GameObjectEditorWindow(Game game)
        {
            InitializeComponent();
            _editorData = new EditorData(game);
            _sniffer = new EditorSniffer(this);
            var editTab = new EditTab(_editorData);
            tabControlMain.TabPages.Add(new NewTab(_editorData));
            tabControlMain.TabPages.Add(editTab);
            var objectTree = new ObjectTree(_editorData);
            objectTree.Dock = DockStyle.Fill;
            objectTree.SelectedObjectCallback += editTab.SelectedObject;
            TreePanel.Controls.Add(objectTree);
            _sniffer.EditorReloadCallback += objectTree.FillAllObjectsId;
            resize();
        }

        private void GameObjectEditorWindow_Resize(object sender, System.EventArgs e)
        {
            resize();
        }
        private void resize(){
            int height = this.Size.Height;
            int width = this.Size.Width;
            if (width > 240 && height > 10)
            {
                MainPanel.Width = width - TreePanel.MinimumSize.Width;
                TreePanel.Location = new System.Drawing.Point(width - TreePanel.MinimumSize.Width, 0);
            }
        }

    }

}