using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PlagueEngine.LowLevelGameFlow;
using System.IO;
using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.Editor.MessageBoxs;
using System.Reflection;

namespace PlagueEngine.Editor.Controls
{
    partial class NewPropertyGridControl : UserControl
    {
        private GameObjectDefinition _currentDefinition;
        private GameObjectClassName _currentClassName;
        private EditorData _editorData;
        public NewPropertyGridControl(EditorData editorData)
        {
            _editorData = editorData;
            InitializeComponent();
            fillObjectsName();
        }
        private void fillObjectsName()
        {
            foreach (var gameObject in _editorData.GameObjectClassNames)
            {
                this.comboBoxObjectsNames.Items.Add(gameObject.ClassName);
            }
        }
        private void buttonDeleteDefinition_Click(object sender, EventArgs e)
        {
            if (_currentDefinition != null )
            {
                var definitionCounters = new List<DefinitionCounter>();
                int allDefinitions = 0;
                string selectedDefinition = comboBoxDefinitions.SelectedItem.ToString();
                foreach (FileInfo levelFile in _editorData.Levels)
                {
                    DefinitionCounter dc = new DefinitionCounter(levelFile.Name);
                    if (levelFile.Name.Equals(_editorData.Level.CurrentLevel))
                    {
                        foreach (var gameObject in _editorData.Level.GameObjects.Values)
                        {
                            if (!String.IsNullOrWhiteSpace(gameObject.Definition) && gameObject.Definition.Equals(selectedDefinition))
                            {
                                dc.Add();
                                allDefinitions++;
                            }
                        }
                    }
                    else
                    {
                        LevelData levelData = _editorData.ContentManager.LoadLevel(levelFile.Name);
                        if (levelData != null)
                        {
                            foreach (var gameObjectdata in levelData.gameObjects)
                            {
                                if (!String.IsNullOrWhiteSpace(gameObjectdata.Definition) && gameObjectdata.Definition.Equals(selectedDefinition))
                                {
                                    dc.Add();
                                    allDefinitions++;
                                }
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
                    DeleteDefinition deleteDefinition = new DeleteDefinition(_currentDefinition, definitionCounters);
                    deleteDefinition.YesButtonClickedCallback += this.deleteDefinition;

                }
                else
                {
                    deleteDefinition(_currentDefinition);
                }
            }
        }
        private void deleteDefinition(GameObjectDefinition definition)
        {
            if (definition != null && !String.IsNullOrWhiteSpace(_currentDefinition.Name))
            {
                _editorData.ContentManager.GameObjectsDefinitions.Remove(_currentDefinition.Name);
                _editorData.ContentManager.SaveGameObjectsDefinitions();
                comboBoxDefinitions.Items.Remove(_currentDefinition.Name);
                comboBoxDefinitions.ClearSelection();
            }
        }

        private void comboBoxDefinitions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDefinitions.SelectedIndex != -1 && propertyGrid.SelectedObject != null)
            {
                if (_currentDefinition != null)
                {
                    PropertyInfo[] propINFO = _currentClassName.DataClassType.GetProperties();
                    foreach (PropertyInfo pI in propINFO)
                    {
                        if (_currentDefinition.Properties.ContainsKey(pI.Name))
                        {

                            pI.SetValue(propertyGrid.SelectedObject, null, null);
                        }
                    }
                }

                _currentDefinition = _editorData.ContentManager.GameObjectsDefinitions[(String)comboBoxDefinitions.SelectedItem];
                ((GameObjectInstanceData)propertyGrid.SelectedObject).Definition = (String)comboBoxDefinitions.SelectedItem;

                PropertyInfo[] propInfo = _currentClassName.DataClassType.GetProperties();

                foreach (PropertyInfo pf in propInfo)
                {
                    if (_currentDefinition.Properties.ContainsKey(pf.Name))
                    {
                        pf.SetValue(propertyGrid.SelectedObject, _currentDefinition.Properties[pf.Name], null);
                    }
                }
                propertyGrid.Refresh();
            }
            else
            {
                _currentDefinition = null;
            }
        }

        private void comboBoxObjectsNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxObjectsNames.SelectedIndex != -1)
            {
                string gameObjectClass = (String)comboBoxObjectsNames.SelectedItem;

                _currentClassName = _editorData.GetClass(gameObjectClass);
                if (_currentClassName != null && _currentClassName.DataClassType != null)
                {
                    propertyGrid.SelectedObject = (GameObjectInstanceData)(Activator.CreateInstance(_currentClassName.DataClassType));
                    loadDefinitionForClass(gameObjectClass);
                }
            }
        }
        private void loadDefinitionForClass(string gameObjectClass)
        {
            comboBoxDefinitions.ClearSelection();
            comboBoxDefinitions.Items.Clear();

            foreach (GameObjectDefinition definition in _editorData.ContentManager.GameObjectsDefinitions.Values)
            {
                if (definition.GameObjectClass.Equals(gameObjectClass))
                {
                    comboBoxDefinitions.Items.Add(definition.Name);
                }
            }
        }
    }
}
