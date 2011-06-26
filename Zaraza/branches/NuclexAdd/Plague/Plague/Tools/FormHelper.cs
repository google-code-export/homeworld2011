using System.Windows.Forms;

namespace PlagueEngine.Tools
{
    static class FormHelper
    {
        delegate void CloseMethod(Form form);

        public static void CloseForm(Form form)
        {
            if (form.IsDisposed) return;
            if (form.InvokeRequired)
            {
                var method = new CloseMethod(CloseForm);
                form.Invoke(method, new object[] { form });
            }
            else
            {
                form.Close();
            }
        }
    }
}
