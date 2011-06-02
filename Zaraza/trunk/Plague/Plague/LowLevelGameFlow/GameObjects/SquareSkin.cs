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
using PlagueEngine.Particles.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class SquareSkin:GameObjectInstance
    {
        public SquareBodyComponent body = null;



        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(SquareBodyComponent physcisComponent)
        {
            this.body = physcisComponent;
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





        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            SquareSkinData data = new SquareSkinData();
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
    /// SquareSkinMeshData
    /********************************************************************************/
    [Serializable]
    public class SquareSkinData : GameObjectInstanceData
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