using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using PlagueEngine.GUI.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Tools;
using Nuclex.UserInterface.Visuals.Flat;

namespace PlagueEngine.GUI
{
    /********************************************************************************/
    /// GUI
    /********************************************************************************/
    class GUI
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public GuiManager Manager = null;
        public GUIComponentsFactory ComponentsFactory = null;
        public WindowControl window = null;
#if DEBUG
        public NonConsumingInputManager input = null;
#else
        public InputManager input = null; 
#endif
        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GUI(PlagueEngine.Game game, GameServiceContainer Services)
        {
            ComponentsFactory = new GUIComponentsFactory(this);
#if DEBUG
            input = new NonConsumingInputManager(Services);       
#else
            input = new InputManager(Services, game.Window.Handle); 
#endif
            Manager = new GuiManager(Services);
        }
        /****************************************************************************/

        public bool updateable = true;

        public void Initialize(GraphicsDevice GraphicsDevice)
        {
            
            GUIComponent.gui = this;
            
            this.Manager.DrawOrder = 1000;
            Viewport viewport = GraphicsDevice.Viewport;
            Screen mainScreen = new Screen(viewport.Width, viewport.Height);
            this.Manager.Screen = mainScreen;

            // Each screen has a 'desktop' control. This invisible control by default
            // stretches across the whole screen and serves as the root of the control
            // tree in which all visible controls are managed. All controls are positioned
            // using a system of fractional coordinates and pixel offset coordinates.
            // We now adjust the position of the desktop window to prevent GUI or HUD
            // elements from appearing outside of the title-safe area.
            mainScreen.Desktop.Bounds = new UniRectangle(
              new UniScalar(0.1f, 0.0f), new UniScalar(0.1f, 0.0f), // x and y = 10%
              new UniScalar(0.8f, 0.0f), new UniScalar(0.8f, 0.0f) // width and height = 80%
            );
            this.Manager.Initialize();


            //TODO: Usunąć ten hard-kodowany guzik po zrobieniu GUI do końca.
            
            // Button through which the user can quit the application
            ButtonControl quitButton = new ButtonControl();
            quitButton.Text = "Exit";
            quitButton.Bounds = new UniRectangle(
              new UniScalar(1.0f, -80.0f), new UniScalar(1.0f, -32.0f), 80, 32
            );
            quitButton.Pressed += delegate(object sender, EventArgs arguments) { 
                Diagnostics.PushLog("Quit button clicked!!");
            };

            InputControl inputControl = new InputControl();
            inputControl.Enabled = true;
            inputControl.Bounds = new UniRectangle(
              new UniScalar(1.0f, -180.0f), new UniScalar(1.0f, -132.0f), 80, 32
            );
            mainScreen.Desktop.Children.Add(inputControl);
            mainScreen.Desktop.Children.Add(quitButton);
           
        }

      
        internal void Draw(GameTime gameTime)
        {
            Manager.Draw(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            if (updateable)
            {
                Manager.Update(gameTime);
                input.Update();
                //Diagnostics.PushLog("UPDATE INPUT!!");
            }
        }
    }
}
