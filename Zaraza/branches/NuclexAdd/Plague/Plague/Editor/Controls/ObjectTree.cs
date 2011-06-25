using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.Controls
{
    partial class ObjectTree : UserControl
    {
        public delegate void SelectedObject(GameObjectInstanceData currentSelectedGameObject);
        public SelectedObject SelectedObjectCallback;

        private delegate void UpdaterDelegate();
        private EditorData _editorData;

        public ObjectTree(EditorData editorData)
        {
            _editorData = editorData;
            InitializeComponent();
            FillAllObjectsId();
        }

        public void FillAllObjectsId()
        {
            if (_editorData != null &&  _editorData.Level != null && _editorData.Level.GameObjects != null)
            {
                if (treeViewObjects.InvokeRequired)
                {
                    treeViewObjects.Invoke(new UpdaterDelegate(delegate { UpdateTreeView(); }));
                    return;
                }
                UpdateTreeView();
            }

        }

        private void UpdateTreeView()
        {
            treeViewObjects.SuspendLayout();
            treeViewObjects.Nodes.Clear();
            lock (_editorData.GameObjectClassNames)
            {
                TreeNode GameObjectClassTreeNode;
                TreeNode GameObjectTreeNode;
                foreach (var GameObjectClassName in _editorData.GameObjectClassNames)
                {
                    GameObjectClassTreeNode = new TreeNode();
                    GameObjectClassTreeNode.Text = GameObjectClassName.ClassName;
                    GameObjectClassTreeNode.Tag = GameObjectClassName.ClassName;
                    lock (_editorData.Level.GameObjects)
                    {
                        foreach (var gameObject in _editorData.Level.GameObjects.Values)
                        {
                            if (gameObject.GetType().Name.Equals(GameObjectClassName.ClassName))
                            {
                                GameObjectTreeNode = new TreeNode();
                                GameObjectTreeNode.Tag = gameObject.ID;
                                GameObjectTreeNode.Text = "[" + String.Format("{0:0000}", gameObject.ID) + "] " + gameObject.Name;
                                GameObjectClassTreeNode.Nodes.Add(GameObjectTreeNode);
                            }
                        }
                    }
                    treeViewObjects.Nodes.Add(GameObjectClassTreeNode);
                }
            }

            
            treeViewObjects.Sort();
            treeViewObjects.ResumeLayout();
        }

        private void treeViewObjects_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                int id;
                string[] result = e.Node.Text.Split(']');
                if (result.Length > 0)
                {
                    if (int.TryParse(result[0].Substring(1, result[0].Length - 1), out id))
                    {
                        if (SelectedObjectCallback != null)
                        {
                            SelectedObjectCallback(_editorData.Level.GameObjects[id].GetData());
                        }
                    }
                }

            }
        }
    }
}
