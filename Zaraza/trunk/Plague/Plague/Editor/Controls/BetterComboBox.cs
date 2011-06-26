using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlagueEngine.Editor.Controls
{
    class BetterComboBox : ComboBox
    {
        public void ClearSelection()
        {
            this.SelectedIndex = -1;
            this.SelectedIndex = -1;
        }
    }
}
