using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;

/************************************************************************************/
/// PlagueEngine.GUI.Components
/************************************************************************************/
namespace PlagueEngine.GUI.Components
{
    /********************************************************************************/
    /// GUI Label Component
    /********************************************************************************/
    class LabelComponent : GUIComponent
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public LabelControl label {  get; private set; }
        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public LabelComponent():base(null)
        {
            label = new LabelControl();
        }
        /****************************************************************************/

        /****************************************************************************/
        /// initialize
        /****************************************************************************/
        public bool Initialize(String text, UniRectangle bounds)
        {
            if (label != null && gui!=null)
            {
                label.Text = text;
                label.Bounds = bounds;
#if DEBUG
                Diagnostics.PushLog("Label component initialized successfully");
#endif
                return true;
            }
#if DEBUG
            Diagnostics.PushLog("Label component wasn't initialized");
#endif
            return false;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// register
        /****************************************************************************/
        public override void register()
        {
            gui.Manager.Screen.Desktop.Children.Add(this.label);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Release Me 
        /****************************************************************************/
        public override void ReleaseMe()
        {   
            gui.Manager.Screen.Desktop.Children.Remove(this.label);
            base.ReleaseMe();
        }
        /****************************************************************************/
    }
}
