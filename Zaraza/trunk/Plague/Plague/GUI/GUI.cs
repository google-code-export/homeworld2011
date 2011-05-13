using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using PlagueEngine.Input.Components;

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
        private MouseListenerComponent mouseListenerComponent = null;

        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GUI(Microsoft.Xna.Framework.Game game, GameServiceContainer services, MouseListenerComponent mouseListenerComponent)
        {
            ComponentsFactory = new GUIComponentsFactory(this);
            Manager = new GuiManager(services);
            this.mouseListenerComponent = mouseListenerComponent;
            this.mouseListenerComponent.SubscribeKeys(OnMouseKey, MouseKeyAction.RightClick, MouseKeyAction.LeftClick,MouseKeyAction.MiddleClick);
        }
        /****************************************************************************/

        private bool _updateable = true;

        public bool Updateable
        {
            get { return _updateable; }
            set { _updateable = value; }
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            
            GUIComponent.gui = this;
            
            Viewport viewport = graphicsDevice.Viewport;
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
            
             //Button through which the user can quit the application
            ButtonControl quitButton = new ButtonControl();
            quitButton.Text = "Exit";
            quitButton.Bounds = new UniRectangle(
              new UniScalar(1.0f, -80.0f), new UniScalar(1.0f, -32.0f), 80, 32
            );
            quitButton.Pressed += delegate(object sender, EventArgs arguments) { 
#if DEBUG
                Diagnostics.PushLog("Quit button clicked!!");
#endif
            };

            InputControl inputControl = new InputControl();
            inputControl.Enabled = true;
            inputControl.Bounds = new UniRectangle(
              new UniScalar(1.0f, -180.0f), new UniScalar(1.0f, -132.0f), 150, 32
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
            if (Updateable)
            {
                Manager.Update(gameTime);
            }
            // Blokowanie wykonywania innych akcji przy wpisywaniu tekstu w pole input... 
            if (Manager.Screen.FocusedControl is InputControl)
            {
                if (!Input.Input.inTextInputMode)
                {
                    Input.Input.inTextInputMode = true;
                }
            }
            else
            {
                Input.Input.inTextInputMode = false;
            }
        }

        // Odznaczanie zaznaczonych kontrolek przy kliknięciu dowolnym przyciskiem myszy nie na kontrolce
        private void OnMouseKey(MouseKeyAction mouseKeyAction, ExtendedMouseKeyState mouseKeyState)
        {
            if (Updateable)
            {
                if (!Manager.Screen.IsMouseOverGui && mouseKeyState.WasPressed())
                {
                    Manager.Screen.FocusedControl = null;
                }
            }
        }
    }
}
