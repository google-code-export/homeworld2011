using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;
using PlagueEngine.TimeControlSystem;


namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class Bulldozer:ActivableObject
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public MeshComponent mesh = null;
        public SquareBodyComponent body = null;
        private PhysicsController controller=null;

        private int keyId = -1;
        uint timerID = 0;
        float forceForward = 0;
        float moveDuration = 0;
        
        bool used = false;
        /****************************************************************************/



        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent mesh,
                         SquareBodyComponent body,
                         int[] activationRecievers,
                         int keyID,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight,float forceForward,float moveDuration)
        {
            this.mesh = mesh;
            this.body = body;
            this.keyId = keyID;
            //controller = new PhysicsController(body);
            //controller.DisableControl();
            this.forceForward = forceForward;
            this.moveDuration = moveDuration;
            base.Init(activationRecievers,description,descriptionWindowWidth,descriptionWindowHeight);
            
        }
        /****************************************************************************/


        private void StopMoving()
        {

            controller.DisableControl();
            body.Immovable = false;
            TimeControl.ReleaseFrameCounter(timerID);
        }


        private void Move()
        {

            controller.MoveBackward(forceForward);
        }

        /****************************************************************************/
        /// On Action
        /****************************************************************************/
        protected override void OnActivation()
        {
            if (!used)
            {
                if (controller == null)
                {
                    controller = new PhysicsController(body);
                    controller.EnableControl();
                }
                timerID = TimeControl.CreateFrameCounter(1, -1, Move);
                TimeControl.CreateTimer(TimeSpan.FromSeconds(moveDuration), 1, StopMoving);
                used = true;
            }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public override string[] GetActions(Mercenary mercenary)
        {
            if (keyId != -1 && !used)
            {
                foreach (StorableObject item in mercenary.Items.Keys)
                {
                    if (item.ID == keyId)
                    {

                        return new String[] {"Examine", "Activate" };
                    }
                }
            }

            return new String[] { "Examine"};
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            if (controller != null)
            {
                controller.DisableControl();
                controller.DisableController();
                controller.RealeseMe();
            }

            if (body != null)
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
            BulldozerData data = new BulldozerData();
            GetData(data);
            data.Model = mesh.Model.Name;
            data.Diffuse = (mesh.Textures.Diffuse == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals = (mesh.Textures.Normals == null ? String.Empty : mesh.Textures.Normals.Name);

            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);


            
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
            data.EnabledMesh = mesh.Enabled;


            data.ActivationRecievers = this.activationRecievers;
            data.keyId = keyId;
            data.ForceForward = forceForward;
            data.MoveDuration = moveDuration;

            return data;
        }
        /****************************************************************************/


    }


    
    /********************************************************************************/
    /// BulldozerData
    /********************************************************************************/
    [Serializable]
    public class BulldozerData : ActivableObjectData
    {
        public BulldozerData()
        {
            Type = typeof(Bulldozer);
        }


        [CategoryAttribute("Model")]
        public String Model { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse { get; set; }
        [CategoryAttribute("Textures")]
        public String Specular { get; set; }
        [CategoryAttribute("Textures")]
        public String Normals { get; set; }

        [CategoryAttribute("Instancing"),
        DescriptionAttribute("1 - No Instancing, 2 - Static Instancing, 3 - Dynamic Instancing.")]
        public uint InstancingMode { get; set; }

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

        [CategoryAttribute("EnabledMesh")]
        public bool EnabledMesh { get; set; }

        public int[] ActivationRecievers { get; set; }
        public int keyId { get; set; }
        public float ForceForward { get; set; }
        public float MoveDuration { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/

