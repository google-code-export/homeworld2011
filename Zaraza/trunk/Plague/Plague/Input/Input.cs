using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

using PlagueEngine.Input.Components;


// TODO: Rozwinąć input o obsługę myszy (odpowiednie komponenty) i mapowanie klawiszy


/************************************************************************************/
/// PlagueEngine.Input
/************************************************************************************/
namespace PlagueEngine.Input
{

    /********************************************************************************/
    /// Delegates
    /********************************************************************************/
    delegate void OnKey(Keys key, ExtendedKeyState state);

    delegate void onMouseKey(MouseKeyAction mouseKeyAction,ExtendedMouseKeyState mouseKeyState);
    delegate void onMouseMove(MouseMoveAction mouseMoveAction,ExtendedMouseMovementState mouseMovementState);
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
        class MouseMoveListener
        {
            public MouseListenerComponent mouseListenerComponent;
            public onMouseMove onMouseMove;

            public MouseMoveListener(MouseListenerComponent mouseListenerComponent,onMouseMove onMouseMove)
            {
                this.mouseListenerComponent=mouseListenerComponent;
                this.onMouseMove=onMouseMove;
            }
        }

        class MouseKeyListener
        {
            public MouseListenerComponent mouseListenerComponent;
            public onMouseKey onMouseKey;

            public MouseKeyListener(MouseListenerComponent mouseListenerComponent,onMouseKey onMouseKey)
            {
                this.mouseListenerComponent=mouseListenerComponent;
                this.onMouseKey=onMouseKey;
            }
        }



        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Dictionary<Keys, List<KeyboardListener>> keyListeners = new Dictionary<Keys,List<KeyboardListener>>();
        private Dictionary<MouseKeyAction,List<MouseKeyListener>> mouseKeyListeners = new Dictionary<MouseKeyAction,List<MouseKeyListener>>();
        private Dictionary<MouseMoveAction,List<MouseMoveListener>> mouseMoveListeners = new Dictionary<MouseMoveAction,List<MouseMoveListener>>();


        private InputComponentsFactory componentsFactory;
        private KeyboardState          oldKeyboardState;
        private MouseState             oldMouseState;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Input()
        {
            componentsFactory = new InputComponentsFactory(this);
        }
        /****************************************************************************/



        public void SubscribeMouseMoveListener(MouseListenerComponent listener, MouseMoveAction mouseMoveAction, onMouseMove onMouseMove)
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

        public void SubscribeMouseKeyListener(MouseListenerComponent listener, MouseKeyAction MouseKeyAction, onMouseKey onMouseKey)
        {
            if(mouseKeyListeners.Keys.Contains(MouseKeyAction))
            {
                mouseKeyListeners[MouseKeyAction].Add(new MouseKeyListener(listener,onMouseKey));
            }
            else
            {

                List<MouseKeyListener> list = new List<MouseKeyListener>();
                list.Add(new MouseKeyListener(listener,onMouseKey));
                mouseKeyListeners.Add(MouseKeyAction,list);
            }

        }


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

        public void ReleaseMouseListenerComponent(MouseListenerComponent component)
        {

            List<MouseKeyListener> keyListenersToDelete = new List<MouseKeyListener>();
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
        /// Update
        /****************************************************************************/
        public void Update()
        {
            CheckKeyboard();
            CheckMouse();
        }
        /****************************************************************************/

        private void CheckMouse()
        {
            MouseState state= Mouse.GetState();
            ExtendedMouseKeyState mouseKeyState;
            ExtendedMouseMovementState mouseMoveState;

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
                        down=(state.LeftButton == ButtonState.Pressed ? true : false);
                        changed=(down==(oldMouseState.LeftButton==ButtonState.Pressed) ? true : false);
                        break;

                    case MouseKeyAction.RightClick:
                        down=(state.RightButton == ButtonState.Pressed ? true : false);
                        changed=(down==(oldMouseState.RightButton==ButtonState.Pressed) ? true : false);
                        break;

                    case MouseKeyAction.MiddleClick:
                        down=(state.MiddleButton == ButtonState.Pressed ? true : false);
                        changed=(down==(oldMouseState.MiddleButton==ButtonState.Pressed) ? true : false);
                        break;
                }

                mouseKeyState=new ExtendedMouseKeyState(down,changed);
                foreach(MouseKeyListener mouseKeyListener in mouseKeyListeners[mouseKeyAction] )
                {
                    if(mouseKeyListener.mouseListenerComponent.Active)mouseKeyListener.onMouseKey(mouseKeyAction,mouseKeyState);
                }
            }



            oldMouseState=state;
        }
        /****************************************************************************/
        /// Check Keyboard
        /****************************************************************************/
        private void CheckKeyboard()
        {

            //TODO: Key-mapping ;>

            KeyboardState    state = Keyboard.GetState();
            
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
        /// Components Factory
        /****************************************************************************/
        public InputComponentsFactory ComponentsFactory
        {
            get
            {
                return componentsFactory;
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/