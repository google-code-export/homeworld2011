using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics;
using PlagueEngine.Physics.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    enum TacticalAction { IDLE, MOVE, GRAB, FOLLLOW, EXAMINE, DROP, ATTACK }; 
    class LivingBeing : GameObjectInstance
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/                        
        /*private Vector3            target;
        private GameObjectInstance objectTarget;
        
        protected TacticalAction    moving = TacticalAction.IDLE;
        */
        /*
        private float rotationSpeed  = 0;
        private float movingSpeed    = 0;
        private float distance       = 0;
        private float anglePrecision = 0;
        */
        /****************************************************************************/

        /****************************************************************************/
        /// Slots
        /****************************************************************************/
        //private GameObjectInstance currentObject = null;

        public String gripBone {get; protected set;}
        /****************************************************************************/

        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public uint      MaxHP { get; protected set; }
        public uint      HP    { get; protected set; }
        public Rectangle Icon  { get; protected set; }
        /****************************************************************************/

        /****************************************************************************/
        /// Components
        /****************************************************************************/
        public SkinnedMeshComponent Mesh       { get; protected set; }
        public CapsuleBodyComponent Body       { get; protected set; }
        public PhysicsController Controller    { get; protected set; }
        /****************************************************************************/


    }
}
