using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;
using PlagueEngine.Input.Components;


// TODO: Rozwinąć input o mapowanie klawiszy


/************************************************************************************/
/// PlagueEngine.Input
/************************************************************************************/
namespace PlagueEngine.Input
{

    /********************************************************************************/
    /// Delegates
    /********************************************************************************/
    delegate void OnKey(Keys key, ExtendedKeyState state);

    delegate void OnMouseKey (MouseKeyAction  mouseKeyAction, ExtendedMouseKeyState      mouseKeyState);
    delegate void OnMouseMove(MouseMoveAction mouseMoveAction,ExtendedMouseMovementState mouseMovementState);
    /********************************************************************************/


    /********************************************************************************/
    /// Input
    /********************************************************************************/
    class Input
    {
     
        /****************************************************************************/
        /// Keyboard Listener
        /****************************************************************************/
        class KeyboardListener
        {
                        
            /************************************************************************/
            /// Fields
            /************************************************************************/
            public KeyboardListenerComponent listener;
            public OnKey                     onKey;
            /************************************************************************/


            /************************************************************************/
            /// Constructor
            /************************************************************************/
            public KeyboardListener(KeyboardListenerComponent listener, OnKey onKey)
            {
                this.listener = listener;
                this.onKey    = onKey;                
            }
            /************************************************************************/

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Mouse Move Listener
        /****************************************************************************/
        class MouseMoveListener
        {

            /************************************************************************/
            /// Fields
            /************************************************************************/
            public MouseListenerComponent mouseListenerComponent;
            public OnMouseMove onMouseMove;
            /************************************************************************/


            /************************************************************************/
            /// Constructor
            /************************************************************************/
            public MouseMoveListener(MouseListenerComponent mouseListenerComponent,OnMouseMove onMouseMove)
            {
                this.mouseListenerComponent=mouseListenerComponent;
                this.onMouseMove=onMouseMove;
            }
            /************************************************************************/

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Mouse Key Listener
        /****************************************************************************/
        class MouseKeyListener
        {

            /************************************************************************/
            /// Fields
            /************************************************************************/
            public MouseListenerComponent mouseListenerComponent;
            public OnMouseKey onMouseKey;
            /************************************************************************/


            /************************************************************************/
            /// Constructor
            /************************************************************************/
            public MouseKeyListener(MouseListenerComponent mouseListenerComponent,OnMouseKey onMouseKey)
            {
                this.mouseListenerComponent=mouseListenerComponent;
                this.onMouseKey=onMouseKey;
            }
            /************************************************************************/

        }
        /****************************************************************************/


        /****************************************************************************/

        /// Fields
        /****************************************************************************/
        public static bool inTextInputMode = false;
        private Dictionary<Keys, List<KeyboardListener>>            keyListeners       = new Dictionary<Keys,List<KeyboardListener>>();
        private Dictionary<MouseKeyAction,List<MouseKeyListener>>   mouseKeyListeners  = new Dictionary<MouseKeyAction,List<MouseKeyListener>>();
        private Dictionary<MouseMoveAction,List<MouseMoveListener>> mouseMoveListeners = new Dictionary<MouseMoveAction,List<MouseMoveListener>>();

        private InputManager           inputManager;
        private Screen                 _guiScreen;   
        private Game                   game;
        private InputComponentsFactory componentsFactory;
        private KeyboardState          oldKeyboardState;
        private MouseState             oldMouseState;
        private int                    cursorLock;
        private bool                   enabled;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Input(Game game, GameServiceContainer services)
        {
            enabled = true;
            this.game         = game;
            inputManager = new InputManager(services, game.Window.Handle);
            componentsFactory = new InputComponentsFactory(this);
            InputComponent.input = this;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Mouse Move Listener
        /****************************************************************************/
        public void SubscribeMouseMoveListener(MouseListenerComponent listener, 
                                               MouseMoveAction        mouseMoveAction, 
                                               OnMouseMove            onMouseMove)
        {
            if(mouseMoveListeners.Keys.Contains(mouseMoveAction))
            {
                mouseMoveListeners[mouseMoveAction].Add(new MouseMoveListener(listener,onMouseMove));
            }
            else
            {

                List<MouseMoveListener> list = new List<MouseMoveListener>();
                list.Add(new MouseMoveListener(listener,onMouseMove));
                mouseMoveListeners.Add(mouseMoveAction,list);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Mouse Key Listener
        /****************************************************************************/
        public void SubscribeMouseKeyListener(MouseListenerComponent listener, 
                                              MouseKeyAction         mouseKeyAction, 
                                              OnMouseKey             onMouseKey)
        {
            if(mouseKeyListeners.Keys.Contains(mouseKeyAction))
            {
                mouseKeyListeners[mouseKeyAction].Add(new MouseKeyListener(listener,onMouseKey));
            }
            else
            {
                List<MouseKeyListener> list = new List<MouseKeyListener>();
                list.Add(new MouseKeyListener(listener,onMouseKey));
                mouseKeyListeners.Add(mouseKeyAction,list);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Keyboard Listener
        /****************************************************************************/
        public void SubscribeKeyboardListener(KeyboardListenerComponent listener,
                                              Keys                      key,
                                              OnKey                     onKey)
        {
            if (keyListeners.Keys.Contains(key))
            {
                keyListeners[key].Add(new KeyboardListener(listener, onKey));
            }
            else
            {
                List<KeyboardListener> list = new List<KeyboardListener>();
                list.Add(new KeyboardListener(listener, onKey));
                keyListeners.Add(key,list);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Keyboard Listener Component
        /****************************************************************************/
        public void ReleaseKeyboardListenerComponent(KeyboardListenerComponent component)
        {
            List<KeyboardListener> listenersToDelete = new List<KeyboardListener>();

            foreach (List<KeyboardListener> list in keyListeners.Values)
            {                
                foreach (KeyboardListener listener in list)
                {
                    if (listener.listener == component) listenersToDelete.Add(listener);
                }

                foreach (KeyboardListener listener in listenersToDelete)
                {
                    list.Remove(listener);
                }
                
                listenersToDelete.Clear();
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Mouse Listener Component
        /****************************************************************************/
        public void ReleaseMouseListenerComponent(MouseListenerComponent component)
        {
            List<MouseKeyListener>  keyListenersToDelete  = new List<MouseKeyListener>();
            List<MouseMoveListener> moveListenersToDelete = new List<MouseMoveListener>();

            foreach (List<MouseKeyListener> list in mouseKeyListeners.Values)
            {                
                foreach (MouseKeyListener listener in list)
                {
                    if (listener.mouseListenerComponent == component) keyListenersToDelete.Add(listener);
                }

                foreach (MouseKeyListener listener in keyListenersToDelete)
                {
                    list.Remove(listener);
                }
                
                keyListenersToDelete.Clear();
            }

            
            foreach (List<MouseMoveListener> list in mouseMoveListeners.Values)
            {                
                foreach (MouseMoveListener listener in list)
                {
                    if (listener.mouseListenerComponent == component) moveListenersToDelete.Add(listener);
                }

                foreach (MouseMoveListener listener in moveListenersToDelete)
                {
                    list.Remove(listener);
                }
                
                moveListenersToDelete.Clear();
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update()
        {
            if (enabled)
            {
                inputManager.Update();
                if (!Input.inTextInputMode)
                {
                    CheckKeyboard();
                }
                CheckMouse();
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Check Mouse 
        /****************************************************************************/
        private void CheckMouse()
        {
            MouseState state = inputManager.GetMouse().GetState();

            ExtendedMouseKeyState       mouseKeyState;
            ExtendedMouseMovementState  mouseMoveState;

            foreach(MouseMoveAction mouseMoveAction in mouseMoveListeners.Keys)
            {
                mouseMoveState=new ExtendedMouseMovementState(
                    state.X,state.Y,state.ScrollWheelValue,
                    oldMouseState.X,oldMouseState.Y,oldMouseState.ScrollWheelValue);

                foreach(MouseMoveListener mouseMoveListener in mouseMoveListeners[mouseMoveAction] )
                {
                    if(mouseMoveListener.mouseListenerComponent.Active) mouseMoveListener.onMouseMove(mouseMoveAction,mouseMoveState);
                }
            }
            
            foreach(MouseKeyAction mouseKeyAction in mouseKeyListeners.Keys)
            {
                bool down=false;
                bool changed=false;

                switch(mouseKeyAction)
                {

                    case MouseKeyAction.LeftClick:
                        down    = (state.LeftButton == ButtonState.Pressed ? true : false);
                        changed = (oldMouseState.LeftButton != state.LeftButton ? true : false);
                        break;

                    case MouseKeyAction.RightClick:
                        down    = (state.RightButton == ButtonState.Pressed ? true : false);
                        changed = (oldMouseState.RightButton != state.RightButton ? true : false);
                        break;

                    case MouseKeyAction.MiddleClick:
                        down    = (state.MiddleButton == ButtonState.Pressed ? true : false);
                        changed = (oldMouseState.MiddleButton != state.MiddleButton? true : false);
                        break;
                }


                mouseKeyState=new ExtendedMouseKeyState(down,changed);
                foreach(MouseKeyListener mouseKeyListener in mouseKeyListeners[mouseKeyAction] )
                {
                    if(mouseKeyListener.mouseListenerComponent.Active)mouseKeyListener.onMouseKey(mouseKeyAction,mouseKeyState);
                }
            }

            if (cursorLock > 0) Mouse.SetPosition(oldMouseState.X, oldMouseState.Y);
            else oldMouseState = state;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Check Keyboard
        /****************************************************************************/
        private void CheckKeyboard()
        {

            KeyboardState    state = inputManager.GetKeyboard().GetState();
            ExtendedKeyState keyState;
            foreach (Keys key in keyListeners.Keys)
            {

                if(state.IsKeyDown(key))
                {
                    if(oldKeyboardState.IsKeyDown(key))
                    {
                        keyState = new ExtendedKeyState(true,false);
                    }
                    else
                    {
                        keyState = new ExtendedKeyState(true,true);
                    }                    
                }
                else
                {
                    if(oldKeyboardState.IsKeyUp(key))
                    {
                        keyState = new ExtendedKeyState(false,false);
                    }
                    else
                    {
                        keyState = new ExtendedKeyState(false,true);
                    }                                    
                }

                foreach (KeyboardListener keyboardListener in keyListeners[key])
                {
                    if(keyboardListener.listener.Active) keyboardListener.onKey(key,keyState);
                }
            }

            oldKeyboardState = state;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Lock Cursor
        /****************************************************************************/
        public void LockCursor()
        {
            if (cursorLock == 0)
            {
                oldMouseState = Mouse.GetState();
                game.IsMouseVisible = false;
            }

            ++cursorLock;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Unlock Cursor
        /****************************************************************************/
        public void UnlockCursor()
        {
            if (cursorLock != 0) --cursorLock;             
            if (cursorLock == 0) game.IsMouseVisible = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Components Factory
        /****************************************************************************/
        public InputComponentsFactory ComponentsFactory
        {
            get
            {
                return componentsFactory;
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;

                if (inputManager != null)
                {
                    inputManager.Enable = value;
                }
                if (!value)
                {
                    if (inputManager != null)
                    {
                        KeyboardState state = inputManager.GetKeyboard().GetState();

                        foreach (Keys key in state.GetPressedKeys())
                        {
                            if (GuiScreen != null) GuiScreen.InjectKeyRelease(key);
                            foreach (KeyboardListener keyboardListener in keyListeners[key])
                            {
                                if (keyboardListener.listener.Active) keyboardListener.onKey(key, new ExtendedKeyState(false, false));
                            }
                        }
                    }
                }
            }
        }

        public InputManager InputManager
        {
            get { return inputManager; }
            set { inputManager = value; }
        }

        public Screen GuiScreen
        {
            get { return _guiScreen; }
            set { _guiScreen = value; }
        }

        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/