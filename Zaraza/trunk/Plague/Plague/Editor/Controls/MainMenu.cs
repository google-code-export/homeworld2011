using System;
using System.Windows.Forms;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.Controls
{
    partial class MainMenu : BeseEditorDataControl
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        public override void SetEditorData(EditorData editorData)
        {
            base.SetEditorData(editorData);
            gameInputButton.Text = _editorData.Game.Input.Enabled ? "ON" : "OFF";
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void gameInputButton_Click(object sender, EventArgs e)
        {
            _editorData.Game.Input.Enabled = !_editorData.Game.Input.Enabled;
            gameInputButton.Text = _editorData.Game.Input.Enabled ? "ON" : "OFF";
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Level file|*.lvl";
            saveFileDialog.Title = "Save level as:";
            saveFileDialog.InitialDirectory = Application.StartupPath + "\\" + _editorData.Game.ContentManager.LevelDirectory;
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                _editorData.EditorEventSender.SendEvent(new SaveLevelEvent(System.IO.Path.GetFileName(saveFileDialog.FileName)), EventsSystem.Priority.High, GlobalGameObjects.GameController);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Level file|*.lvl";
            openFileDialog.Title = "Open level:";
            openFileDialog.InitialDirectory = Application.StartupPath + "\\" + _editorData.Game.ContentManager.LevelDirectory;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _editorData.EditorEventSender.SendEvent(new ChangeLevelEvent(System.IO.Path.GetFileName(openFileDialog.FileName)), EventsSystem.Priority.High, GlobalGameObjects.GameController);
            }
        }
    }
}