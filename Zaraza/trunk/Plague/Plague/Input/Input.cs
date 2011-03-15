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
        /// Fields
        /****************************************************************************/
        private Dictionary<Keys, List<KeyboardListener>> keyListeners = new Dictionary<Keys,List<KeyboardListener>>();

        private InputComponentsFactory componentsFactory;
        private KeyboardState          oldKeyboardState;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Input()
        {
            componentsFactory = new InputComponentsFactory(this);
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
        /// Update
        /****************************************************************************/
        public void Update()
        {
            CheckKeyboard();
        }
        /****************************************************************************/


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