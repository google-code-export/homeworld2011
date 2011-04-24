using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;




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
        public static GuiManager manager { get; set; }
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
        public bool Initialize(String text)
        {
            if (button != null && manager!=null)
            {
                button.Text = text;
                button.Bounds = new UniRectangle(new UniScalar(1.0f, -190.0f),
                                            new UniScalar(1.0f, -32.0f), 100, 32);
                manager.Screen.Desktop.Children.Add(this.button);
                return true;
            }
            return false;
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
        }
        /****************************************************************************/
    }
    /********************************************************************************/
}
