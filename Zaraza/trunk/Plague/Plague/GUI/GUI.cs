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
        public GuiManager Manager;
        public GUIComponentsFactory ComponentsFactory;        
        private readonly MouseListenerComponent _mouseListenerComponent;

        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GUI(GameServiceContainer services, MouseListenerComponent mouseListenerComponent)
        {
            ComponentsFactory = new GUIComponentsFactory(this);
            Manager = new GuiManager(services);
            _mouseListenerComponent = mouseListenerComponent;
            _mouseListenerComponent.SubscribeKeys(OnMouseKey, MouseKeyAction.RightClick, MouseKeyAction.LeftClick, MouseKeyAction.MiddleClick);
            _mouseListenerComponent.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move, MouseMoveAction.Scroll);
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
            Manager.Screen = mainScreen;

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
            Manager.Initialize();                        
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
        private void OnMouseKey(MouseKeyAction mouseKeyAction, ref ExtendedMouseKeyState mouseKeyState)
        {
            if (!Updateable) return;
            if (!mouseKeyState.WasPressed()) return;

            if(!Manager.Screen.IsMouseOverGui)
            {
                Manager.Screen.FocusedControl = null;
            }
            else
            {
                mouseKeyState.Propagate = false;
            }
        }

        private void OnMouseMove(MouseMoveAction mouseMoveAction, ref ExtendedMouseMovementState mouseMovementState)
        {
            if (!Updateable) return;

            if (!Manager.Screen.IsMouseOverGui) return;

            mouseMovementState.Propagate = false;
            _mouseListenerComponent.SetCursor("Default");
        }
    }
}
