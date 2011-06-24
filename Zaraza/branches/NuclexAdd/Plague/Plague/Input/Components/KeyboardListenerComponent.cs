using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Input;

/************************************************************************************/
/// PlagueEngine.Input.Components
/************************************************************************************/
namespace PlagueEngine.Input.Components
{

    /********************************************************************************/
    /// KeyboardListenerComponent
    /********************************************************************************/
    class KeyboardListenerComponent : InputComponent
    {
        
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private bool active = true;
        private bool modal = false;
        /****************************************************************************/


        /****************************************************************************/
        /// Keyboard Listener Component
        /****************************************************************************/
        public KeyboardListenerComponent(GameObjectInstance gameObject, bool active) : base(gameObject)
        {
            this.active = active;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Active
        /****************************************************************************/
        public bool Active
        {
            get
            {
                return active;
            }

            set
            {
                active = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Keys
        /****************************************************************************/
        public void SubscibeKeys(OnKey onKey, params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                input.SubscribeKeyboardListener(this, key, onKey);
            }
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Modal
        /****************************************************************************/
        public bool Modal
        {
            get { return modal; }

            set
            {
                if (!modal && value)
                {
                    input.ModalsKeyboardListeners.Push(this);
                    modal = true;
                }
                else if (modal && !value && input.ModalsKeyboardListeners.Peek() == this)
                {
                    input.ModalsKeyboardListeners.Pop();
                    modal = false;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            input.ReleaseKeyboardListenerComponent(this);
            base.ReleaseMe();
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/