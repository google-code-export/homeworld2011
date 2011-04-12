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
    class Button 
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public ButtonControl button {  get; private set; }
        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Button(Screen screen, String text)
        {
            button = new ButtonControl();
            button.Text = text;
            button.Bounds = new UniRectangle(new UniScalar(1.0f, -190.0f), new UniScalar(1.0f, -32.0f), 100, 32);
            //screen.Desktop.Children.Add(button);  
        }

        public void setDelegate(EventHandler handler)
        {
            button.Pressed += handler;
        }


    }
}
