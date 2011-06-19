using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.ArtificialIntelligence.Controllers;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Audio.Components;
using PlagueEngine.Physics;
using PlagueEngine.EventsSystem;
using System.ComponentModel;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class AbstractLivingBeing : GameObjectInstance
    {
        public AbstractAIController ObjectAIController { get; protected set; }
        /****************************************************************************/
        /// Fields
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

        public void GetData(AbstractLivingBeingData data)
        {
            base.GetData(data);
            data.animationMapping = this.ObjectAIController.AnimationBinding;
        }
    }

    [Serializable]
    public class AbstractLivingBeingData : GameObjectInstanceData
    {

        public AbstractLivingBeingData()
        {
            Type = typeof(AbstractLivingBeing);
            animationMapping = new Dictionary<ArtificialIntelligence.Controllers.Action, string>();
        }

        [CategoryAttribute("AI")]
        public Dictionary<ArtificialIntelligence.Controllers.Action, string> animationMapping { get; set; }

    }
}
