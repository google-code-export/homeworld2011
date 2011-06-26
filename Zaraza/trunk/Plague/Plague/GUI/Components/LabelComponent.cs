using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;
using PlagueEngine.LowLevelGameFlow;

/************************************************************************************/
// PlagueEngine.GUI.Components
/************************************************************************************/
namespace PlagueEngine.GUI.Components
{
    /********************************************************************************/
    // GUI Label Component
    /********************************************************************************/
    class LabelComponent : GUIComponent
    {
        /****************************************************************************/
        // Fields
        /****************************************************************************/
        public LabelControl Control { get; protected set; }
        private string _text;


        private int _width, _height,_x,_y;
        /****************************************************************************/

        
        /****************************************************************************/
        // Constructor
        /****************************************************************************/
        public LabelComponent(string text, int x, int y, int width, int height)
            : base(x,y,width,height)
        {
            Control = new LabelControl
                          {
                              Bounds = new UniRectangle(new UniScalar(x),
                                                        new UniScalar(y),
                                                        new UniScalar(width),
                                                        new UniScalar(height))
                          };
            Text = text;
        }
        /****************************************************************************/

        public void Refresh()
        {
            Control.Text = GlobalGameObjects.StringManager.Load<string>(_text);
        }

        /****************************************************************************/
        // register
        /****************************************************************************/
        public override void Register()
        {
            GUI.Manager.Screen.Desktop.Children.Add(Control);
        }
        /****************************************************************************/

        /****************************************************************************/
        // Unregister
        /****************************************************************************/
        public void Unregister()
        {
            GUI.Manager.Screen.Desktop.Children.Remove(Control);

        }
        /****************************************************************************/


        /****************************************************************************/
        // Release Me 
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
                return _text;
            }
            set
            {
                Control.Text = GlobalGameObjects.StringManager.Load<string>(value);
                _text = value;
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
       new  public int Y
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
}
