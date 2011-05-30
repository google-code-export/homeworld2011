using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.ArtificialInteligence.Controllers;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Audio.Components;
using PlagueEngine.Physics;
using PlagueEngine.EventsSystem;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class AbstractLivingBeing : GameObjectInstance
    {
        protected IAIController objectAIController;

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        
        /****************************************************************************/



    
        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public uint MaxHP { get; protected set; }
        public uint HP { get; protected set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Components
        /****************************************************************************/
        public SkinnedMeshComponent mesh;
        public CapsuleBodyComponent body;
        public SoundEffectComponent SoundEffectComponent;
        public SkinnedMeshComponent Mesh { get { return this.mesh; } protected set { this.mesh = value; } }
        public CapsuleBodyComponent Body { get { return this.body; } protected set { this.body = value; } }
        public PhysicsController Controller { get; protected set; }
        /****************************************************************************/

    }
}
