using System;
using PlagueEngine.EventsSystem;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor
{
    internal class EditorSniffer : EventsSniffer
    {
        public delegate void EditorReload();
        public delegate void EditorSelectiveDrawing(bool flag, int objectId);
        public EditorReload EditorReloadCallback;
        public EditorSelectiveDrawing EditorSelectiveDrawingCallback;
        private readonly GameObjectEditorWindow _editor;
        private bool _reload;

        public EditorSniffer(GameObjectEditorWindow editor)
        {
            _editor = editor;
            SubscribeAll();
            SubscribeEvents(typeof(GameObjectReleased), typeof(GameObjectClicked), typeof(CreateEvent), typeof(DestroyEvent));
            TimeControlSystem.TimeControl.CreateTimer(TimeSpan.FromSeconds(1), -1, CheckToReload);
        }

        public void CheckToReload()
        {
            if (!_reload || _editor == null || !_editor.Visible) return;
            if (EditorReloadCallback != null) EditorReloadCallback();
            _reload = false;
        }

        public override void OnSniffedEvent(EventsSender sender, IEventsReceiver receiver, EventArgs e)
        {
            if (_editor == null || !_editor.Visible) return;
            var type = e.GetType();
            if (type.Equals(typeof(GameObjectClicked)))
            {
                if (EditorSelectiveDrawingCallback != null) EditorSelectiveDrawingCallback(true, ((GameObjectClicked)e).gameObjectID);
                return;
            }
            if (type.Equals(typeof(GameObjectReleased)))
            {
                if (EditorSelectiveDrawingCallback != null) EditorSelectiveDrawingCallback(false, -1);
                return;
            }
            if (type.Equals(typeof(CreateEvent)) || type.Equals(typeof(DestroyEvent)))
            {
                _reload = true;
                return;
            }
        }
    }
}