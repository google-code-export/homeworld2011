using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;

using PlagueEngine.Input.Components;
using PlagueEngine.LowLevelGameFlow;


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
        public ButtonControl Control { get; protected set; }
        
        public String Tag            { get; protected set; }
        public String Text           { get; protected set; }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public ButtonComponent(String text, int x, int y, int width, int height, String tag)
            : base(x,y,width,height)
        {
            Control = new ButtonControl();
            Control.Text = GlobalGameObjects.StringManager.Load<string> (text);
            Control.Bounds = new UniRectangle(new UniScalar(x),
                                              new UniScalar(y),
                                              new UniScalar(width),
                                              new UniScalar(height));
            Tag = tag;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// register
        /****************************************************************************/
        public override void Register()
        {
            gui.Manager.Screen.Desktop.Children.Add(this.Control);
            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Unregister
        /****************************************************************************/
        public  void Unregister()
        {
            gui.Manager.Screen.Desktop.Children.Remove(this.Control);

        }
        /****************************************************************************/

        /****************************************************************************/
        /// setDelegate
        /****************************************************************************/
        public void setDelegate(EventHandler handler)
        {
            Control.Pressed += handler;            
        }
        /****************************************************************************/
        


        /****************************************************************************/
        /// Release Me 
        /****************************************************************************/
        public override void ReleaseMe()
        {
            Control.Parent.Children.Remove(Control);
            base.ReleaseMe();
        }
        /****************************************************************************/
    }
    /********************************************************************************/
}
