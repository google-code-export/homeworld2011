using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlagueEngine.Input.Components;
using PlagueEngine.LowLevelGameFlow;

/************************************************************************************/
/// PlagueEngine.Input
/************************************************************************************/
namespace PlagueEngine.Input
{

    /********************************************************************************/
    /// InputComponentsFactory
    /********************************************************************************/
    class InputComponentsFactory
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Input input = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public InputComponentsFactory(Input input)
        {
            this.input = input;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Keyboard Listener Component
        /****************************************************************************/
        public KeyboardListenerComponent CreateKeyboardListenerComponent(GameObjectInstance gameObject, bool active)
        {
            return new KeyboardListenerComponent(gameObject, input, active);
        }
        /****************************************************************************/

        public MouseListenerComponent CreateMouseListenerComponent(GameObjectInstance gameObject, bool active)
        {
            return new MouseListenerComponent(gameObject, input, active);
        }
    }
    /********************************************************************************/

}
/************************************************************************************/