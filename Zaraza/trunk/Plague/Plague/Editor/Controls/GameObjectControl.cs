using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PlagueEngine.Editor.Controls.GameObjectsControls;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.Controls
{
    partial class GameObjectControl : BeseEditorDataControl, OnGameObjectSelection
    {
        private List<Item> _itemsList;
        private GameObjectInstanceData _selectedObject;

        internal GameObjectInstanceData SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (_selectedObject != null || value != null)
                {
                    this.controlsPanel.Controls.Clear();
                }
                _selectedObject = value;
                if (_selectedObject != null)
                {
                    _itemsList = new List<Item>();
                    GameObjectClassName gameObjectClass = _editorData.GetClassByData(_selectedObject.GetType().Name);
                    if (gameObjectClass != null)
                    {
                        foreach (var control in _editorData.GameObjectControls)
                        {
                            if (control.IsForGameObject(gameObjectClass))
                            {
                                AddControl(_selectedObject, control.GetType());
                            }
                        }
                    }
                }
                else
                {
                    this.controlsPanel.Controls.Add(noObjectLabel);
                }
            }
        }

        public GameObjectControl()
        {
            InitializeComponent();
        }

        public void OnObjectSelection(GameObjectInstance selectedGameObject)
        {
            if (selectedGameObject != null)
            {
                SelectedObject = selectedGameObject.GetData();
                noObjectLabel.Visible = false;
            }
            else
            {
                SelectedObject = null;
                noObjectLabel.Visible = true;
            }
        }

        private void AddControl(GameObjectInstanceData gameObject, Type type)
        {
            var control = (BaseControl)Activator.CreateInstance(type);
            control.SetObjectData(gameObject);
            var item = new Item(control);
            item.Dock = DockStyle.Top;
            item.DataChangedCallback += DataChanged;
            _itemsList.Add(item);
            this.controlsPanel.Controls.Add(item);
            if (control.GetName().Equals("PropertyGrid"))
            {
                item.Expanded = true;
            }
        }

        public void DataChanged()
        {
            foreach (Item listItem in _itemsList)
            {
                listItem.Refresh();
            }
        }

        public void ClearModification()
        {
            foreach (Item listItem in _itemsList)
            {
                listItem.ClearModification();
            }
        }
    }
}