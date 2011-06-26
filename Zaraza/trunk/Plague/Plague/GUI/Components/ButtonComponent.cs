using System;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using PlagueEngine.LowLevelGameFlow;


/************************************************************************************/
// PlagueEngine.GUI.Components
/************************************************************************************/
namespace PlagueEngine.GUI.Components
{

    /********************************************************************************/
    // GUI Button Component
    /********************************************************************************/
    class ButtonComponent : GUIComponent
    {
        /****************************************************************************/
        // Fields
        /****************************************************************************/
        public ButtonControl Control { get; protected set; }
        
        public String Tag            { get; protected set; }
        private string _text;
        /****************************************************************************/

        
        /****************************************************************************/
        // Constructor
        /****************************************************************************/
        public ButtonComponent(String text, int x, int y, int width, int height, String tag)
            : base(x,y,width,height)
        {
            Control = new ButtonControl();
            Text = text;
            Control.Bounds = new UniRectangle(new UniScalar(x),
                                              new UniScalar(y),
                                              new UniScalar(width),
                                              new UniScalar(height));
            Tag = tag;
            
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
        public  void Unregister()
        {
            GUI.Manager.Screen.Desktop.Children.Remove(Control);

        }
        /****************************************************************************/

        /****************************************************************************/
        // setDelegate
        /****************************************************************************/
        public void SetDelegate(EventHandler handler)
        {
            Control.Pressed += handler;            
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
    }
    /********************************************************************************/
}
