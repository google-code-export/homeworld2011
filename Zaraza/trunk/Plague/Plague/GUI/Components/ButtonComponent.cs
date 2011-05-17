using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;

using PlagueEngine.Input.Components;


/************************************************************************************/
/// PlagueEngine.GUI.Components
/************************************************************************************/
namespace PlagueEngine.GUI.Components
{

    /********************************************************************************/
    /// GUI Button Component
    /********************************************************************************/
    class ButtonComponent : GUIComponent
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        //TODO: sprawdzić czemu tu public
        public ButtonControl button {  get; private set; }
        public String tag {get; private set; }
        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public ButtonComponent():base(null)
        {
            button = new ButtonControl();
        }
        /****************************************************************************/

        /****************************************************************************/
        /// initialize
        /****************************************************************************/
        public bool Initialize(String text, UniRectangle bounds, String tag)
        {
            if (button != null && gui!=null)
            {
                button.Text = text;
                button.Bounds = bounds;
                this.tag = tag;
#if DEBUG
                Diagnostics.PushLog("Button component initialized successfully");
#endif
                return true;
            }
#if DEBUG
            Diagnostics.PushLog("Button component wasn't initialized");
#endif
            return false;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// register
        /****************************************************************************/
        public override void register()
        {
            gui.Manager.Screen.Desktop.Children.Add(this.button);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// setDelegate
        /****************************************************************************/
        public void setDelegate(EventHandler handler)
        {
            button.Pressed += handler;
        }
        /****************************************************************************/
        


        /****************************************************************************/
        /// Release Me 
        /****************************************************************************/
        public override void ReleaseMe()
        {   
            gui.Manager.Screen.Desktop.Children.Remove(this.button);
            base.ReleaseMe();
        }
        /****************************************************************************/
    }
    /********************************************************************************/
}
