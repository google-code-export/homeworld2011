using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;


using PlagueEngine.Audio.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;
using PlagueEngine.TimeControlSystem;

using PlagueEngine.HighLevelGameFlow;
using System.Collections.Generic;

using PlagueEngine.Particles.Components;
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class Doors : ActivableObject
    {

        /****************************************************************************/
        // Fields
        /****************************************************************************/
        public SquareBodyComponent body;
        private int _keyId = -1;
        public SkinnedMeshComponent mesh;
        bool used = false;
        /****************************************************************************/



        /****************************************************************************/
        // Init
        /****************************************************************************/
        public void Init(
                         SquareBodyComponent body,
                         int keyID,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight,
                         SkinnedMeshComponent mesh)
        {

            this.body = body;
            _keyId = keyID;
            activationRecievers = new int[0];
            Init(activationRecievers, description, descriptionWindowWidth, descriptionWindowHeight);
            this.mesh = mesh;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public override string[] GetActions(Mercenary mercenary)
        {

            if (_keyId != -1)
            {
                if (mercenary.HasItem(_keyId) && !used)
                {

                    return new[] { "Examine", "Activate" };

                }
            }
            return new[] { "Examine" };

        }
        /****************************************************************************/





        /****************************************************************************/
        /// On Action
        /****************************************************************************/
        protected override void OnActivation()
        {

            used = true;
            Diagnostics.PushLog("DRZWI WYLAMANE");
        }








        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {

            if (mesh != null)
            {
                mesh.ReleaseMe();
                mesh = null;
            }

            if (body != null)
            {
                body.ReleaseMe();
                body = null;
            }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// GetData
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            var data = new DoorsData();
            GetData(data);

            data.Model = mesh.Model.Name;
            data.Diffuse = (mesh.Textures.Diffuse == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals = (mesh.Textures.Normals == null ? String.Empty : mesh.Textures.Normals.Name);


            data.TimeRatio = mesh.TimeRatio;
            data.CurrentClip = (mesh.currentAnimation.Clip == null ? String.Empty : mesh.currentAnimation.Clip.Name);
            data.CurrentTime = mesh.currentAnimation.ClipTime.TotalSeconds;
            data.CurrentKeyframe = mesh.currentAnimation.Keyframe;
            data.Pause = mesh.Pause;

            data.Blend = mesh.Blend;
            data.BlendDuration = mesh.BlendDuration.TotalSeconds;
            data.BlendTime = mesh.BlendTime.TotalSeconds;
            data.BlendClip = (mesh.blendAnimation.Clip == null ? String.Empty : mesh.blendAnimation.Clip.Name);
            data.BlendClipTime = mesh.blendAnimation.ClipTime.TotalSeconds;
            data.BlendKeyframe = mesh.blendAnimation.Keyframe;


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


            data.keyId = _keyId;



            return data;
        }
        /****************************************************************************/


    }



    /********************************************************************************/
    /// DoorsData
    /********************************************************************************/
    [Serializable]
    public class DoorsData : ActivableObjectData
    {
        public DoorsData()
        {
            Type = typeof(Doors);

        }


        [CategoryAttribute("Model")]
        public String Model { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse { get; set; }
        [CategoryAttribute("Textures")]
        public String Specular { get; set; }
        [CategoryAttribute("Textures")]
        public String Normals { get; set; }

        [CategoryAttribute("Animation")]
        public float TimeRatio { get; set; }
        [CategoryAttribute("Animation")]
        public String CurrentClip { get; set; }
        [CategoryAttribute("Animation")]
        public double CurrentTime { get; set; }
        [CategoryAttribute("Animation")]
        public int CurrentKeyframe { get; set; }
        [CategoryAttribute("Animation")]
        public bool Pause { get; set; }

        [CategoryAttribute("Animation Blending")]
        public bool Blend { get; set; }
        [CategoryAttribute("Animation Blending")]
        public double BlendDuration { get; set; }
        [CategoryAttribute("Animation Blending")]
        public double BlendTime { get; set; }
        [CategoryAttribute("Animation Blending")]
        public String BlendClip { get; set; }
        [CategoryAttribute("Animation Blending")]
        public double BlendClipTime { get; set; }
        [CategoryAttribute("Animation Blending")]
        public int BlendKeyframe { get; set; }

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

        public int keyId { get; set; }



    }
    /********************************************************************************/

}
/************************************************************************************/

