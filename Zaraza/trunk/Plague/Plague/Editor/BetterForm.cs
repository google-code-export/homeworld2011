using System.Windows.Forms;

namespace PlagueEngine.Editor
{
    class BetterForm:Form
    {
        delegate void CloseMethod();

        public void CloseForm()
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                Invoke(new CloseMethod(CloseForm));
            }
            else
            {
                Close();
            }
        }

        public void StartForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(this);
        }
    }
}
