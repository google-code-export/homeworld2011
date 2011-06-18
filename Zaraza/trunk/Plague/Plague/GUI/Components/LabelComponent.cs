using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;
using PlagueEngine.LowLevelGameFlow;

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
        public LabelControl Control { get; protected set; }
        public string Text { get; protected set; }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public LabelComponent(string text, int x, int y, int width, int height)
            : base(x,y,width,height)
        {
            Control = new LabelControl();
            Control.Bounds = new UniRectangle(new UniScalar(x),
                                              new UniScalar(y),
                                              new UniScalar(width),
                                              new UniScalar(height));
            Control.Text = GlobalGameObjects.StringManager.Load<string>(text) ;
            this.Text = text;  
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
        public void Unregister()
        {
            gui.Manager.Screen.Desktop.Children.Remove(this.Control);

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me 
        /****************************************************************************/
        public override void ReleaseMe()
        {
            if (Control.Parent != null)
            {
                Control.Parent.Children.Remove(Control);
            }
            base.ReleaseMe();
        }
        /****************************************************************************/
    }
}
