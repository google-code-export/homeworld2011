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



        protected string ActionToString(PlagueEngine.ArtificialIntelligence.Controllers.Action action)
        {
            string result;
            result = System.Enum.GetName(typeof(PlagueEngine.ArtificialIntelligence.Controllers.Action), action);
            return result;
        }

        public void GetData(AbstractLivingBeingData data)
        {
            base.GetData(data);

            #region CREATE STRINGS FROM ACTIONS
            List<AnimationBinding> AnimationMapping = new List<AnimationBinding>();
            foreach (KeyValuePair<PlagueEngine.ArtificialIntelligence.Controllers.Action, String> pair in ObjectAIController.AnimationToActionMapping)
            {
                AnimationMapping.Add(new AnimationBinding(ActionToString(pair.Key), pair.Value));
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

        protected PlagueEngine.ArtificialIntelligence.Controllers.Action StringToAction(string String)
        {
            PlagueEngine.ArtificialIntelligence.Controllers.Action result;
            System.Enum.TryParse<PlagueEngine.ArtificialIntelligence.Controllers.Action>(String, false, out result);
            return result;
        }

        public Dictionary<PlagueEngine.ArtificialIntelligence.Controllers.Action, string> GetAnimationMapping()
        {
            Dictionary<PlagueEngine.ArtificialIntelligence.Controllers.Action, string> result = new Dictionary<ArtificialIntelligence.Controllers.Action, string>();
            foreach (AnimationBinding binding in AnimationMapping)
            {
                result.Add(StringToAction(binding.Action), binding.Animation);
            }
            return result;
        }

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
