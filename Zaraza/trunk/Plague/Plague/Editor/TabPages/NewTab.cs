﻿using System;
using System.Collections.Generic;
using PlagueEngine.Editor.Controls;
using PlagueEngine.Editor.MessageBoxs;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.TabPages
{
    partial class NewTab : BeseEditorDataControl
    {
        private GameObjectDefinition _currentDefinition;
        private GameObjectClassName _currentClassName;

        public NewTab()
        {
            InitializeComponent();
            Name = "NewObjectTab";
            Text = "Create new object";
        }

        private void FillObjectsName()
        {
            foreach (var gameObject in _editorData.GameObjectClassNames)
            {
                if (!gameObject.CanBeCreated) continue;
                comboBoxObjectsNames.Items.Add(gameObject.ClassName);
            }
        }

        public override void SetEditorData(EditorData editorData)
        {
            base.SetEditorData(editorData);
            FillObjectsName();
        }

        private void ButtonDeleteDefinitionClick(object sender, EventArgs e)
        {
            if (_currentDefinition == null) return;
            var definitionCounters = new List<DefinitionCounter>();
            var allDefinitions = 0;
            var selectedDefinition = comboBoxDefinitions.SelectedItem.ToString();
            foreach (var levelFile in _editorData.Levels)
            {
                var dc = new DefinitionCounter(levelFile.Name);
                if (levelFile.Name.Equals(_editorData.Level.CurrentLevel))
                {
                    foreach (var gameObject in _editorData.Level.GameObjects.Values)
                    {
                        if (String.IsNullOrWhiteSpace(gameObject.Definition) || !gameObject.Definition.Equals(selectedDefinition)) continue;
                        dc.Add();
                        allDefinitions++;
                    }
                }
                else
                {
                    var levelData = _editorData.ContentManager.LoadLevel(levelFile.Name);
                    if (levelData != null)
                    {
                        foreach (var gameObjectdata in levelData.gameObjects)
                        {
                            if (String.IsNullOrWhiteSpace(gameObjectdata.Definition) || !gameObjectdata.Definition.Equals(selectedDefinition)) continue;
                            dc.Add();
                            allDefinitions++;
                        }
                    }
                }
                if (dc.Count > 0)
                {
                    definitionCounters.Add(dc);
                }
            }

            if (allDefinitions != 0)
            {
                var deleteDefinition = new DeleteDefinition(_currentDefinition, definitionCounters);
                deleteDefinition.YesButtonClickedCallback += DeleteDefinition;
            }
            else
            {
                DeleteDefinition(_currentDefinition);
            }
        }

        private void DeleteDefinition(GameObjectDefinition definition)
        {
            if (definition == null || String.IsNullOrWhiteSpace(_currentDefinition.Name)) return;
            _editorData.ContentManager.GameObjectsDefinitions.Remove(_currentDefinition.Name);
            _editorData.ContentManager.SaveGameObjectsDefinitions();
            comboBoxDefinitions.Items.Remove(_currentDefinition.Name);
            comboBoxDefinitions.ClearSelection();
        }

        private void ComboBoxDefinitionsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDefinitions.SelectedIndex != -1 && gameObjectControl.SelectedObject != null)
            {
                var propertyInfos = _currentClassName.DataClassType.GetProperties();
                if (_currentDefinition != null)
                {
                    foreach (var pI in propertyInfos)
                    {
                        if (_currentDefinition.Properties.ContainsKey(pI.Name))
                        {
                            pI.SetValue(gameObjectControl.SelectedObject, null, null);
                        }
                    }
                }

                _currentDefinition = _editorData.ContentManager.GameObjectsDefinitions[(String)comboBoxDefinitions.SelectedItem];
                gameObjectControl.SelectedObject.Definition = (String)comboBoxDefinitions.SelectedItem;

                foreach (var pf in propertyInfos)
                {
                    if (!_currentDefinition.Properties.ContainsKey(pf.Name)) continue;
                    pf.SetValue(gameObjectControl.SelectedObject, _currentDefinition.Properties[pf.Name], null);
                }
            }
            else
            {
                _currentDefinition = null;
            }
            this.gameObjectControl.DataChanged();
        }

        private void ComboBoxObjectsNamesSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxObjectsNames.SelectedIndex == -1) return;
            var gameObjectClass = (String)comboBoxObjectsNames.SelectedItem;

            _currentClassName = _editorData.GetClass(gameObjectClass);
            if (_currentClassName == null || _currentClassName.DataClassType == null) return;
            gameObjectControl.SelectedObject = (GameObjectInstanceData)(Activator.CreateInstance(_currentClassName.DataClassType));
            LoadDefinitionForClass(gameObjectClass);
        }

        private void LoadDefinitionForClass(string gameObjectClass)
        {
            comboBoxDefinitions.ClearSelection();
            comboBoxDefinitions.Items.Clear();

            foreach (var definition in _editorData.ContentManager.GameObjectsDefinitions.Values)
            {
                if (definition.GameObjectClass == null || !definition.GameObjectClass.Equals(gameObjectClass)) continue;
                comboBoxDefinitions.Items.Add(definition.Name);
            }
            var flag = comboBoxDefinitions.Items.Count != 0;

            comboBoxDefinitions.Enabled = flag;
            buttonDeleteDefinition.Enabled = flag;
        }

        private void buttonCreateObject_Click(object sender, EventArgs e)
        {
            _editorData.EditorEventSender.SendEvent(new CreateObjectEvent(gameObjectControl.SelectedObject), EventsSystem.Priority.High, GlobalGameObjects.GameController);
        }

        private void buttonCreateDefinition_Click(object sender, EventArgs e)
        {
        }
    }
}