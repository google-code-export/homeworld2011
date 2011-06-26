using System;
using System.Windows.Forms;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.Controls
{
    partial class ObjectTree : UserControl
    {
        public delegate void SelectedObject(GameObjectInstanceData currentSelectedGameObject);
        public SelectedObject SelectedObjectCallback;

        private delegate void UpdaterDelegate();
        private readonly EditorData _editorData;

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
                    treeViewObjects.Invoke(new UpdaterDelegate(UpdateTreeView));
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
                foreach (var gameObjectClassName in _editorData.GameObjectClassNames)
                {
                    var gameObjectClassTreeNode = new TreeNode
                                                      {
                                                          Text = gameObjectClassName.ClassName,
                                                          Tag = gameObjectClassName.ClassName
                                                      };
                    lock (_editorData.Level.GameObjects)
                    {
                        foreach (var gameObject in _editorData.Level.GameObjects.Values)
                        {
                            if (gameObject.GetType().Name.Equals(gameObjectClassName.ClassName))
                            {
                                var gameObjectTreeNode = new TreeNode
                                                                  {
                                                                      Tag = gameObject.ID,
                                                                      Text =@"[" + String.Format("{0:0000}", gameObject.ID) +@"] " + gameObject.Name
                                                                  };
                                gameObjectClassTreeNode.Nodes.Add(gameObjectTreeNode);
                            }
                        }
                    }
                    treeViewObjects.Nodes.Add(gameObjectClassTreeNode);
                }
            }

            
            treeViewObjects.Sort();
            treeViewObjects.ResumeLayout();
        }

        private void TreeViewObjectsAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null) return;
            var result = e.Node.Text.Split(']');
            if (result.Length <= 0) return;
            int id;
            if (!int.TryParse(result[0].Substring(1, result[0].Length - 1), out id)) return;
            if (SelectedObjectCallback != null)
            {
                SelectedObjectCallback(_editorData.Level.GameObjects[id].GetData());
            }
        }
    }
}
