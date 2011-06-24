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
using PlagueEngine.ArtificialIntelligence.Controllers;

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
        public Pathfinder.PathfinderComponent PathfinderComponent;
        public SkinnedMeshComponent Mesh { get { return this.mesh; } protected set { this.mesh = value; } }
        public CapsuleBodyComponent Body { get { return this.body; } protected set { this.body = value; } }
        public PhysicsController Controller { get; protected set; }
        /****************************************************************************/

        public void GetData(AbstractLivingBeingData data)
        {
            base.GetData(data);

            #region CREATE STRINGS FROM ACTIONS
            List<AnimationBinding> AnimationMapping = new List<AnimationBinding>();
            foreach (KeyValuePair<PlagueEngine.ArtificialIntelligence.Controllers.Action, String> pair in ObjectAIController.AnimationToActionMapping)
            {
                switch (pair.Key)
                {
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.ACTIVATE:
                        AnimationMapping.Add(new AnimationBinding("ACTIVATE", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.ATTACK:
                        AnimationMapping.Add(new AnimationBinding("ATTACK", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.ATTACK_IDLE:
                        AnimationMapping.Add(new AnimationBinding("ATTACK_IDLE", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.ENGAGE:
                        AnimationMapping.Add(new AnimationBinding("ENGAGE", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.EXAMINE:
                        AnimationMapping.Add(new AnimationBinding("EXAMINE", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.EXCHANGE:
                        AnimationMapping.Add(new AnimationBinding("EXCHANGE", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.FOLLOW:
                        AnimationMapping.Add(new AnimationBinding("FOLLOW", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.IDLE:
                        AnimationMapping.Add(new AnimationBinding("IDLE", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.MOVE:
                        AnimationMapping.Add(new AnimationBinding("MOVE", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.OPEN:
                        AnimationMapping.Add(new AnimationBinding("OPEN", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.PICK:
                        AnimationMapping.Add(new AnimationBinding("PICK", pair.Value));
                        break;
                    case PlagueEngine.ArtificialIntelligence.Controllers.Action.TO_IDLE:
                        AnimationMapping.Add(new AnimationBinding("TO_IDLE", pair.Value));
                        break;
                    case ArtificialIntelligence.Controllers.Action.TACTICAL_MOVE_CARABINE:
                        AnimationMapping.Add(new AnimationBinding("TACTICAL_MOVE_CARABINE", pair.Value));
                        break;
                    case ArtificialIntelligence.Controllers.Action.TACTICAL_MOVE_SIDEARM:
                        AnimationMapping.Add(new AnimationBinding("TACTICAL_MOVE_SIDEARM", pair.Value));
                        break;
                    case ArtificialIntelligence.Controllers.Action.WOUNDED_MOVE:
                        AnimationMapping.Add(new AnimationBinding("WOUNDED_MOVE", pair.Value));
                        break;
                    default:
                        break;
                }

            }
            #endregion
            
            data.AnimationMapping = AnimationMapping;
            //data.animationMapping = null;
        }
    }

    [Serializable]
    public class AnimationBinding
    {
        public AnimationBinding()
        {
            this.Action = "";
            this.Animation = "";
        }
        public AnimationBinding(string Action, string Animation)
        {
            this.Action = Action;
            this.Animation = Animation;
        }
        [CategoryAttribute("Binding")]
        public string Action { get; set; }
        [CategoryAttribute("Binding")]
        public string Animation { get; set; }
    }

    [Serializable]
    public class AbstractLivingBeingData : GameObjectInstanceData
    {

        public AbstractLivingBeingData()
        {
            this.AnimationMapping = new List<AnimationBinding>();
            //animationMapping = new Dictionary<ArtificialIntelligence.Controllers.Action, string>();
        }

        [CategoryAttribute("AI")]
        public List<AnimationBinding> AnimationMapping { get; set; }
        

        //[CategoryAttribute("AI")]
        //public Dictionary<PlagueEngine.ArtificialIntelligence.Controllers.Action, string> animationMapping { get; set; }
        
    }
}
