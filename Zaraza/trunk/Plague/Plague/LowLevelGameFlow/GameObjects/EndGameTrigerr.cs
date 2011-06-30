using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Physics;
using PlagueEngine.Physics.Components;

using PlagueEngine.EventsSystem;
/********************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/********************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{


    /********************************************************************************/
    /// EndGameTrigerr
    /********************************************************************************/
    class EndGameTrigerr : GameObjectInstance
    {


        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        public SquareBodyComponent body = null;
        /********************************************************************************/



        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(SquareBodyComponent physcisComponent)
        {
           
            this.body = physcisComponent;
            body.dontCollide = true;
            body.SubscribeStartCollisionEvent(typeof(Mercenary));
        }
        /********************************************************************************/




        /********************************************************************************/
        /// ReleaseComponents
        /********************************************************************************/
        public override void ReleaseComponents()
        {
          
            body.ReleaseMe();
        }
        /********************************************************************************/



        /****************************************************************************/
        // On Event
        /****************************************************************************/
        public override void OnEvent(EventsSender sender, EventArgs e)
        {

            Broadcast(new FadeInEvent(), Priority.High);
        }




        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            EndGameTrigerrData data = new EndGameTrigerrData();
            GetData(data);
      
            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Lenght = body.Length;
            data.Width = body.Width;
            data.Height = body.Height;
            data.Immovable = body.Immovable;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;
            data.EnabledPhysics = body.Enabled;


            return data;
        }
        /********************************************************************************/



    }
    /********************************************************************************/




    /********************************************************************************/
    /// EndGameTrigerrData
    /********************************************************************************/
    [Serializable]
    public class EndGameTrigerrData : GameObjectInstanceData
    {

        [CategoryAttribute("Physics")]
        public float Mass { get; set; }

        [CategoryAttribute("Physics")]
        public float Elasticity { get; set; }

        [CategoryAttribute("Physics")]
        public float StaticRoughness { get; set; }

        [CategoryAttribute("Physics")]
        public float DynamicRoughness { get; set; }

        [CategoryAttribute("Physics")]
        public bool Immovable { get; set; }

        [CategoryAttribute("Physics")]
        public bool EnabledPhysics { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Lenght { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Width { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Height { get; set; }

        [CategoryAttribute("Collision Skin")]
        public Vector3 Translation { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinYaw { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinPitch { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinRoll { get; set; }
    }
    /********************************************************************************/



}
/********************************************************************************/