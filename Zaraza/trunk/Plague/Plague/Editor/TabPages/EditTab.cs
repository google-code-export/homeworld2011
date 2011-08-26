using System;
using PlagueEngine.Editor.Controls;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.TabPages
{
    partial class EditTab : BeseEditorDataControl, OnGameObjectSelection
    {
        public EditTab()
        {
            InitializeComponent();
            Name = "EditObjectTab";
            Text = "Edit object";
        }

        public void OnObjectSelection(GameObjectInstance selectedGameObject)
        {
            gameObjectControl.OnObjectSelection(selectedGameObject);
            if (selectedGameObject != null)
            {
                gameObjectName.Text = "Type: " + StringValiding(selectedGameObject.GetType().Name, " - ") + " ID: " + selectedGameObject.ID + " Name:" + StringValiding(selectedGameObject.Name, " - ");
            }
            else
            {
                gameObjectName.Text = "";
            }
        }

        private string StringValiding(string text, string defaultValue)
        {
            return String.IsNullOrWhiteSpace(text) ? defaultValue : text;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            RecreateGameObject(gameObjectControl.SelectedObject);
            gameObjectControl.ClearModification();
        }

        private void RecreateGameObject(GameObjectInstanceData data)
        {
            _editorData.EditorEventSender.SendEvent(new DestroyObjectEvent(data.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
            _editorData.EditorEventSender.SendEvent(new CreateObjectEvent(data), EventsSystem.Priority.Low, GlobalGameObjects.GameController);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            _editorData.EditorEventSender.SendEvent(new DestroyObjectEvent(gameObjectControl.SelectedObject.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
            OnObjectSelection(null);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            OnObjectSelection(_editorData.Level.GameObjects[gameObjectControl.SelectedObject.ID]);
        }

        private void buttonForceUpdate_Click(object sender, EventArgs e)
        {
            RecreateGameObject(gameObjectControl.SelectedObject);
        }

    }
}