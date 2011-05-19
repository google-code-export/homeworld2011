using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Input.Components;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// ActionSwitch
    /********************************************************************************/
    class ActionSwitch : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private FrontEndComponent frontEndComponent = null;
        private MouseListenerComponent    mouse     = null;
        private KeyboardListenerComponent keyboard  = null;
        private Vector2 position;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(FrontEndComponent  frontEndComponent,
                         Vector2            position,
                         String[]           actions,
                         GameObjectInstance feedback)
        { 
            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            frontEndComponent.ReleaseMe();
            frontEndComponent = null;
            mouse.ReleaseMe();
            mouse = null;
            keyboard.ReleaseMe();
            keyboard = null;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// ActionSwitchData
    /********************************************************************************/
    [Serializable]
    class ActionSwitchData : GameObjectInstanceData
    {
        public Vector2  Position { get; set; }
        public String[] Actions  { get; set; }
        public int      Feedback { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/