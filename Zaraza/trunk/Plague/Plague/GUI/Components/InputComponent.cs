using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;

/************************************************************************************/
/// PlagueEngine.GUI.Components
/************************************************************************************/
namespace PlagueEngine.GUI.Components
{
    /********************************************************************************/
    /// GUI Input Component
    /********************************************************************************/
    class InputComponent : GUIComponent
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public InputControl input {  get; private set; }
        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public InputComponent():base(null)
        {
            input = new InputControl();
        }
        /****************************************************************************/

        /****************************************************************************/
        /// initialize
        /****************************************************************************/
        public bool Initialize(String text, UniRectangle bounds)
        {
            if (input != null && gui!=null)
            {
                input.Text = text;
                input.Bounds = bounds;
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
        /// register
        /****************************************************************************/
        public override void register()
        {
            gui.Manager.Screen.Desktop.Children.Add(this.input);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Release Me 
        /****************************************************************************/
        public override void ReleaseMe()
        {   
            gui.Manager.Screen.Desktop.Children.Remove(this.input);
            base.ReleaseMe();
        }
        /****************************************************************************/
    }
}
