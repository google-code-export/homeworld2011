using System.Windows.Forms;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Editor.Controls
{
    partial class ObjectTree : BeseEditorDataControl
    {
        public delegate void SelectedObject(GameObjectInstance currentSelectedGameObject);
        public SelectedObject SelectedObjectCallback;

        private delegate void UpdaterDelegate();

        public ObjectTree()
        {
            InitializeComponent();
        }

        public override void SetEditorData(EditorData editorData)
        {
            base.SetEditorData(editorData);
            FillAllObjectsId();
        }

        public void FillAllObjectsId()
        {
            if (_editorData != null && _editorData.Level != null && _editorData.Level.GameObjects != null)
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
                                                                      Text = "[" + gameObject.ID + "] " + gameObject.Name
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
            GameObjectInstance gameObject = null;

            if (e.Node.Parent != null)
            {
                gameObject = _editorData.Level.GameObjects[(int)e.Node.Tag];
            }
            if (SelectedObjectCallback != null)
            {
                SelectedObjectCallback(gameObject);
            }
        }
    }
}