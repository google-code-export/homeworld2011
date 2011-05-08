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
        public ButtonControl button {  get; private set; }
        //public static GuiManager manager { get; set; }
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
        public bool Initialize(String text, UniRectangle bounds)
        {
            if (button != null && gui!=null)
            {
                button.Text = text;
                button.Bounds = new UniRectangle(new UniScalar(1.0f, -180.0f), new UniScalar(1.0f, -40.0f), 80, 24);//bounds;
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

        public override void register()
        {
            gui.window.Children.Add(this.button);
        }

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
