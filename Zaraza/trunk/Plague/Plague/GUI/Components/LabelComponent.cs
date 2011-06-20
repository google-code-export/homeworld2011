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
        private string text;


        private int width, height,x,y;
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
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
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




        /********************************************************************************/
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                Control.Text = GlobalGameObjects.StringManager.Load<string>(value);
                text = value;
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                Control.Bounds = new UniRectangle(new UniScalar(value),
                                  new UniScalar(y),
                                  new UniScalar(width),
                                  new UniScalar(height));
                x = value;
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                Control.Bounds = new UniRectangle(new UniScalar(x),
                                  new UniScalar(value),
                                  new UniScalar(width),
                                  new UniScalar(height));
                y = value;
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                Control.Bounds = new UniRectangle(new UniScalar(x),
                                  new UniScalar(y),
                                  new UniScalar(value),
                                  new UniScalar(height));
                width = value;
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                Control.Bounds = new UniRectangle(new UniScalar(x),
                                  new UniScalar(y),
                                  new UniScalar(width),
                                  new UniScalar(value));
                height = value;
            }
        }
        /********************************************************************************/


    }
}
