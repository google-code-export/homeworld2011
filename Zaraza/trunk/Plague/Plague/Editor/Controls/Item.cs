using System;
using System.Drawing;
using System.Windows.Forms;
using PlagueEngine.Editor.Controls.GameObjectsControls;

namespace PlagueEngine.Editor.Controls
{
    public partial class Item : UserControl
    {
        BaseControl _control;
        bool _expanded;

        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                _expanded = value;

                if (_expanded)
                {
                    this.Height = this.Height + _control.Height + 20;
                    expand.Text = "-";
                }
                else
                {
                    this.Height = this.Height - _control.Height - 20;
                    expand.Text = "+";
                }
                this.Invalidate();
            }
        }

        public Item(BaseControl control)
        {
            InitializeComponent();

            _control = control;
            this.name.Text = _control.GetName();
            _control.Location = new Point(5, 30);
            _control.DataChangedCallback += ControlDataChanged;
            if (this.Width > 10)
            {
                _control.Width = this.Width - 10;
            }
            _control.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(_control);
        }

        private void expand_Click(object sender, EventArgs e)
        {
            Expanded = !_expanded;
        }

        private void ControlDataChanged()
        {
            if (!modification.Visible)
            {
                modification.Visible = true;
            }
            if (DataChangedCallback != null)
            {
                DataChangedCallback();
            }
        }

        public void ClearModification()
        {
            modification.Visible = false;
        }

        public override void Refresh()
        {
            base.Refresh();
            _control.Refresh();
        }

        private void Item_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        public delegate void DataChanged();
        public DataChanged DataChangedCallback;
    }
}