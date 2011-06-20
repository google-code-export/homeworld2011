using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Controls.Desktop;

/************************************************************************************/
/// PlagueEngine.GUI.Components
/************************************************************************************/
namespace PlagueEngine.GUI.Components
{

    /********************************************************************************/
    /// WindowComponent
    /********************************************************************************/
    class WindowComponent : GUIComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public WindowControl Control { get; private set; }
        private int width, height, x, y;
        private String title;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public WindowComponent(String title, int x, int y, int width, int height, bool enableDragging)            
        {
            Control                = new WindowControl();
            Control.Title          = title;
            Control.Bounds = new UniRectangle(new UniScalar(x),
                                              new UniScalar(y),
                                              new UniScalar(width),
                                              new UniScalar(height));
            Control.EnableDragging = enableDragging;
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
            Register();
        }
        /****************************************************************************/





                
        /****************************************************************************/
        /// Register
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
        /// Add Control
        /****************************************************************************/
        public void AddControl(Control control)
        {
            Control.Children.Add(control);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Control
        /****************************************************************************/
        public void RemoveControl(Control control)
        {
            Control.Children.Remove(control);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            gui.Manager.Screen.Desktop.Children.Remove(Control);                
            base.ReleaseMe();
        }
        /****************************************************************************/





        /********************************************************************************/
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                Control.Title = value;
                title = value;
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
    /********************************************************************************/

}
/************************************************************************************/