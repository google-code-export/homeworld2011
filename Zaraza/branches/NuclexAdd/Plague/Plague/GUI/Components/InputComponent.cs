using System;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;

/************************************************************************************/
// PlagueEngine.GUI.Components
/************************************************************************************/
namespace PlagueEngine.GUI.Components
{
    /********************************************************************************/
    // GUI Input Component
    /********************************************************************************/
    class InputComponent : GUIComponent
    {
        /****************************************************************************/
        // Fields
        /****************************************************************************/
        public InputControl Input {  get; private set; }
        /****************************************************************************/
        
        /****************************************************************************/
        // Constructor
        /****************************************************************************/
        public InputComponent()
        {
            Input = new InputControl();
        }
        /****************************************************************************/

        /****************************************************************************/
        // initialize
        /****************************************************************************/
        public bool Initialize(String text, UniRectangle bounds)
        {
            if (Input != null && GUI!=null)
            {
                Input.Text = text;
                Input.Bounds = bounds;
#if DEBUG
                Diagnostics.PushLog("Input component initialized successfully");
#endif
                return true;
            }
#if DEBUG
            Diagnostics.PushLog("Input component wasn't initialized");
#endif
            return false;
        }
        /****************************************************************************/



        /****************************************************************************/
        // initialize
        /****************************************************************************/
        public void Initialize(string text, int x, int y, int width, int height)
        {
            Input.Text = text;
            Input.Bounds = new UniRectangle(new UniScalar(x),
                                              new UniScalar(y),
                                              new UniScalar(width),
                                              new UniScalar(height));
         
        }
        /****************************************************************************/


        /****************************************************************************/
        // register
        /****************************************************************************/
        public override void Register()
        {
            GUI.Manager.Screen.Desktop.Children.Add(Input);
        }
        /****************************************************************************/


        /****************************************************************************/
        // Unregister
        /****************************************************************************/
        public void Unregister()
        {
            GUI.Manager.Screen.Desktop.Children.Remove(Input);

        }
        /****************************************************************************/

        /****************************************************************************/
        // Release Me 
        /****************************************************************************/
        public override void ReleaseMe()
        {   
            GUI.Manager.Screen.Desktop.Children.Remove(Input);
            base.ReleaseMe();
        }
        /****************************************************************************/
    }
}
