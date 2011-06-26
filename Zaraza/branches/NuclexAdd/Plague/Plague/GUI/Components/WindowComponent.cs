using System;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Controls.Desktop;

/************************************************************************************/
// PlagueEngine.GUI.Components
/************************************************************************************/
namespace PlagueEngine.GUI.Components
{

    /********************************************************************************/
    // WindowComponent
    /********************************************************************************/
    class WindowComponent : GUIComponent
    {

        /****************************************************************************/
        // Fields
        /****************************************************************************/
        public WindowControl Control { get; private set; }
        private int _width, _height, _x, _y;
        private String _title;
        /****************************************************************************/


        /****************************************************************************/
        // Constructor
        /****************************************************************************/
        public WindowComponent(String title, int x, int y, int width, int height, bool enableDragging)            
        {
            Control = new WindowControl
                          {
                              Title = title,
                              Bounds = new UniRectangle(new UniScalar(x),
                                                        new UniScalar(y),
                                                        new UniScalar(width),
                                                        new UniScalar(height)),
                              EnableDragging = enableDragging
                          };
            _height = height;
            _width = width;
            _x = x;
            _y = y;
            Register();
        }
        /****************************************************************************/





                
        /****************************************************************************/
        // Register
        /****************************************************************************/
        public override sealed void Register()
        {
            GUI.Manager.Screen.Desktop.Children.Add(Control);
        }
        /****************************************************************************/

        /****************************************************************************/
        // Unregister
        /****************************************************************************/
        public  void Unregister()
        {
            GUI.Manager.Screen.Desktop.Children.Remove(Control);

        }
        /****************************************************************************/

        /****************************************************************************/
        // Add Control
        /****************************************************************************/
        public void AddControl(Control control)
        {
            Control.Children.Add(control);
        }
        /****************************************************************************/


        /****************************************************************************/
        // Remove Control
        /****************************************************************************/
        public void RemoveControl(Control control)
        {
            Control.Children.Remove(control);
        }
        /****************************************************************************/


        /****************************************************************************/
        // Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            GUI.Manager.Screen.Desktop.Children.Remove(Control);                
            base.ReleaseMe();
        }
        /****************************************************************************/





        /********************************************************************************/
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                Control.Title = value;
                _title = value;
            }
        }
        /********************************************************************************/





        /********************************************************************************/
       new public int X
        {
            get
            {
                return _x;
            }
            set
            {
                Control.Bounds = new UniRectangle(new UniScalar(value),
                                  new UniScalar(_y),
                                  new UniScalar(_width),
                                  new UniScalar(_height));
                _x = value;
            }
        }
        /********************************************************************************/





        /********************************************************************************/
        new public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                Control.Bounds = new UniRectangle(new UniScalar(_x),
                                  new UniScalar(value),
                                  new UniScalar(_width),
                                  new UniScalar(_height));
                _y = value;
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        new public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                Control.Bounds = new UniRectangle(new UniScalar(_x),
                                  new UniScalar(_y),
                                  new UniScalar(value),
                                  new UniScalar(_height));
                _width = value;
            }
        }
        /********************************************************************************/


        /********************************************************************************/
       new public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                Control.Bounds = new UniRectangle(new UniScalar(_x),
                                  new UniScalar(_y),
                                  new UniScalar(_width),
                                  new UniScalar(value));
                _height = value;
            }
        }
        /********************************************************************************/



    }
    /********************************************************************************/

}
/************************************************************************************/