﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Nuclex.UserInterface;
using PlagueEngine.Input.Components;
using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.TimeControlSystem;

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

    delegate void OnMouseKey(MouseKeyAction mouseKeyAction, ref ExtendedMouseKeyState mouseKeyState);
    delegate void OnMouseMove(MouseMoveAction mouseMoveAction,ref ExtendedMouseMovementState mouseMovementState);
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

        internal Stack<MouseListenerComponent>    ModalsMouseListeners    = new Stack<MouseListenerComponent>();
        internal Stack<KeyboardListenerComponent> ModalsKeyboardListeners = new Stack<KeyboardListenerComponent>();

        private InputManager           inputManager;
        private Screen                 _guiScreen;   
        private Game                   game;
        private InputComponentsFactory componentsFactory;
        private KeyboardState          oldKeyboardState;
        private MouseState             oldMouseState;
        private int                    cursorLock;
        private bool                   enabled;
        private Clock                  clock;
        private TimeSpan               prevLeftButtonDown;
        private TimeSpan               prevMiddleButtonDown;
        private TimeSpan               prevRightButtonDown;
        private TimeSpan               doubleClickTime = TimeSpan.FromSeconds(0.25);


        private SpriteBatch             spriteBatch   = null;
        private Dictionary<String, int> cursors       = new Dictionary<String,int>();
        private Texture2D               cursorTexture = null;
        private Vector2                 cursorSize;
        private int[]                   cursorGrid    = new int[2];
        private int                     currentCursor = -1;
        private Vector2                 cursorPosition;
        internal bool                    cursorIsVisible = true;
        private Rectangle               cursorRect;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Input(Game game, GameServiceContainer services,GraphicsDevice device)
        {
            enabled              = true;
            this.game            = game;
            inputManager         = new InputManager(services, game.Window.Handle);
            componentsFactory    = new InputComponentsFactory(this);
            InputComponent.input = this;
            spriteBatch          = new SpriteBatch(device);
            clock = TimeControl.CreateClock();
            
            
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
            if (!enabled) return;
            inputManager.Update();
            CheckMouse();
            if (Input.inTextInputMode) return;
            CheckKeyboard();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Check Mouse 
        /****************************************************************************/
        private void CheckMouse()
        {
            MouseState state = Mouse.GetState();
            
            cursorPosition.X = state.X;
            cursorPosition.Y = state.Y;


            
            foreach(var mouseMoveAction in mouseMoveListeners.Keys)
            {
                var mouseMoveState = new ExtendedMouseMovementState(state.X, state.Y, state.ScrollWheelValue, oldMouseState.X, oldMouseState.Y, oldMouseState.ScrollWheelValue);
                foreach(var mouseMoveListener in mouseMoveListeners[mouseMoveAction] )
                {
                    if (ModalsMouseListeners.Count > 0)
                    {
                        if (mouseMoveListener.mouseListenerComponent.Active && ModalsMouseListeners.Peek() == mouseMoveListener.mouseListenerComponent)
                        {
                            mouseMoveListener.onMouseMove(mouseMoveAction, ref mouseMoveState);
                            break;
                        }                        
                    }
                    else
                    {
                        if (mouseMoveListener.mouseListenerComponent.Active) mouseMoveListener.onMouseMove(mouseMoveAction, ref mouseMoveState);
                        if (!mouseMoveState.Propagate) break;                    
                    }
                }
            }

            foreach (MouseKeyAction mouseKeyAction in mouseKeyListeners.Keys)
            {
                var currentState = ExtendedMouseKeyStateFactory(state, mouseKeyAction);
                foreach (var mouseKeyListener in mouseKeyListeners[mouseKeyAction])
                {
                    if (ModalsMouseListeners.Count > 0)
                    {
                        if (mouseKeyListener.mouseListenerComponent.Active && ModalsMouseListeners.Peek() == mouseKeyListener.mouseListenerComponent)
                        {
                            mouseKeyListener.onMouseKey(mouseKeyAction, ref currentState);
                            break;
                        }                        
                    }
                    else
                    {
                        if (mouseKeyListener.mouseListenerComponent.Active) mouseKeyListener.onMouseKey(mouseKeyAction, ref currentState);
                        if (!currentState.Propagate) break;                    
                    }
                }
            }

            if (cursorLock > 0) Mouse.SetPosition(oldMouseState.X, oldMouseState.Y);
            else oldMouseState = state;
        }
        /****************************************************************************/
        private ExtendedMouseKeyState ExtendedMouseKeyStateFactory(MouseState state, MouseKeyAction mouseKeyAction)
        {
            var down = false;
            var changed = false;
            var doubleClick = false;
            switch (mouseKeyAction)
            {

                case MouseKeyAction.LeftClick:
                    
                    down = (state.LeftButton == ButtonState.Pressed ? true : false);
                    changed = (oldMouseState.LeftButton != state.LeftButton ? true : false);

                    if (down && changed)
                    {
                        if ((clock.Time - prevLeftButtonDown)<doubleClickTime)
                        {
                            doubleClick = true;
                        }
                    }

                    if (down)
                    {
                        prevLeftButtonDown = clock.Time;
                    }
                    break;

                case MouseKeyAction.RightClick:
                    
                    down = (state.RightButton == ButtonState.Pressed ? true : false);
                    changed = (oldMouseState.RightButton != state.RightButton ? true : false);

                    if (down && changed)
                    {
                        if ((clock.Time - prevRightButtonDown) < doubleClickTime)
                        {
                            doubleClick = true;
                        }
                    }


                    if (down)
                    {
                        prevRightButtonDown = clock.Time;
                    }
                    break;

                case MouseKeyAction.MiddleClick:
                    
                    down = (state.MiddleButton == ButtonState.Pressed ? true : false);
                    changed = (oldMouseState.MiddleButton != state.MiddleButton ? true : false);

                    if (down && changed)
                    {
                        if ((clock.Time - prevMiddleButtonDown) < doubleClickTime)
                        {
                            doubleClick = true;
                        }
                    }

                    if (down)
                    {
                        prevMiddleButtonDown = clock.Time;
                    }
                    break;
            }
            return new ExtendedMouseKeyState(down, changed,doubleClick);
        }

        /****************************************************************************/
        /// Check Keyboard
        /****************************************************************************/
        private void CheckKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

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
                    if (ModalsKeyboardListeners.Count > 0)
                    {
                        if (keyboardListener.listener.Active && keyboardListener.listener == ModalsKeyboardListeners.Peek())
                        {
                            keyboardListener.onKey(key, keyState);
                            break;
                        }
                    }
                    else
                    {
                        if (keyboardListener.listener.Active) keyboardListener.onKey(key, keyState);
                    }
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
                cursorIsVisible = false;
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
            if (cursorLock == 0)
            {
                if(currentCursor < 0) game.IsMouseVisible = true;
                cursorIsVisible = true;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
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
                if (value) return;
                if (inputManager == null) return;
                var state = inputManager.GetKeyboard().GetState();

                foreach (var key in state.GetPressedKeys())
                {
                    if (GuiScreen != null) GuiScreen.InjectKeyRelease(key);
                    var index = 0;
                    for (; index < keyListeners[key].Count; index++)
                    {
                        var keyboardListener = keyListeners[key][index];
                        if (keyboardListener.listener.Active)
                            keyboardListener.onKey(key, new ExtendedKeyState(false, false));
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


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public void Draw()
        {
            if (currentCursor >= 0 && cursorIsVisible)
            {
                if (spriteBatch.GraphicsDevice.IsDisposed)
                {
                    spriteBatch = new SpriteBatch(game.GraphicsDevice);
                }

                spriteBatch.Begin();

                if (currentCursor == cursors["Targeting"]) spriteBatch.Draw(cursorTexture, cursorPosition + new Vector2(-16,-16), cursorRect, Color.White);
                else spriteBatch.Draw(cursorTexture, cursorPosition, cursorRect, Color.White);                
                spriteBatch.End();
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Set Cursor Texture
        /****************************************************************************/
        public void SetCursorTexture(Texture2D texture, int width, int height, String[] cursors)
        {
            int i = 0;
            foreach (String cursor in cursors)
            {
                this.cursors.Add(cursor, i++);
            }

            this.cursorTexture = texture;
            this.cursorSize.X  = texture.Width  / width;
            this.cursorSize.Y  = texture.Height / height;
            cursorGrid[0] = width;
            cursorGrid[1] = height;

            currentCursor = 0;

            cursorRect = new Rectangle((int)((currentCursor % cursorGrid[0]) * cursorSize.X),
                                        (int)((currentCursor / cursorGrid[1]) * cursorSize.Y),
                                        (int)cursorSize.X,
                                        (int)cursorSize.Y);

            game.IsMouseVisible = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Set Cursor
        /****************************************************************************/
        public void SetCursor(String cursor)
        {
            if (String.IsNullOrEmpty(cursor))
            {
                currentCursor = -1;
                game.IsMouseVisible = true;
            }
            else
            {
                if (currentCursor != cursors[cursor])
                {
                    currentCursor = cursors[cursor];
                    game.IsMouseVisible = false;

                    cursorRect = new Rectangle((int)((currentCursor % cursorGrid[0]) * cursorSize.X),
                                                (int)((currentCursor / cursorGrid[1]) * cursorSize.Y),
                                                (int) cursorSize.X,
                                                (int) cursorSize.Y);
                }
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/