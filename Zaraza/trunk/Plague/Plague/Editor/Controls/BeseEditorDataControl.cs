using System.Windows.Forms;

namespace PlagueEngine.Editor.Controls
{
    partial class BeseEditorDataControl : UserControl
    {
        protected EditorData _editorData;

        public BeseEditorDataControl()
        {
            InitializeComponent();
        }

        public virtual void SetEditorData(EditorData editorData)
        {
            _editorData = editorData;
            foreach (Control cont in this.Controls)
            {
                var control = cont as BeseEditorDataControl;
                if (control != null && _editorData!=null)
                {
                    control.SetEditorData(_editorData);
                }
            }
        }

        private void BeseEditorDataControl_ControlAdded(object sender, ControlEventArgs e)
        {
            var control = e.Control as BeseEditorDataControl;
            if (control != null && _editorData != null)
            {
                control.SetEditorData(_editorData);
            }
        }
    }
}