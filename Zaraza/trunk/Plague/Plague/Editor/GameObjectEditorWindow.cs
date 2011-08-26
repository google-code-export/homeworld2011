using System;
using System.Windows.Forms;
using PlagueEngine.Editor.Controls;

/********************************************************************************/
// PlagueEngine.Editor
/********************************************************************************/

namespace PlagueEngine.Editor
{
    /********************************************************************************/
    // Game Object Editor Window
    /********************************************************************************/

    partial class GameObjectEditorWindow : BetterForm
    {
        private readonly EditorData _editorData;

        private readonly EditorSniffer _sniffer;

        public GameObjectEditorWindow(Game game)
        {
            InitializeComponent();
            _editorData = new EditorData(game);
            _sniffer = new EditorSniffer(this);
            mainMenu.SetEditorData(_editorData);
            objectTree.SetEditorData(_editorData);
            FillTabs();
            _sniffer.EditorReloadCallback += objectTree.FillAllObjectsId;
        }

        private void FillTabs()
        {
            foreach (Type type in _editorData.ExecutingAssembly.GetTypes())
            {
                if (!String.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.Equals("PlagueEngine.Editor.TabPages"))
                {
                    Control control = Activator.CreateInstance(type) as Control;
                    OnGameObjectSelection selectionControl = control as OnGameObjectSelection;
                    if (selectionControl != null)
                    {
                        objectTree.SelectedObjectCallback += selectionControl.OnObjectSelection;
                    }
                    BeseEditorDataControl beseEditorDataControl = control as BeseEditorDataControl;
                    if (beseEditorDataControl != null)
                    {
                        beseEditorDataControl.SetEditorData(_editorData);
                    }
                    if (control != null)
                    {
                        TabPage tab = new TabPage();
                        control.Dock = DockStyle.Fill;
                        tab.Text = control.Text;
                        tab.Controls.Add(control);
                        tabControlMain.TabPages.Add(tab);
                    }
                }
            }
        }
    }
}