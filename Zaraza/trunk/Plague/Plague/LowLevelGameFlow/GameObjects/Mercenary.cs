using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.Physics;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Mercenary
    /********************************************************************************/
    class Mercenary : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private SkinnedMeshComponent mesh       = null;
        private CapsuleBodyComponent body       = null;
        private PhysicsController    controller = null;
        private Marker               marker     = null;
        
        private Vector3 target;
        private bool    moving = false;
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Marker Marker { get { return marker; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(SkinnedMeshComponent mesh, CapsuleBodyComponent body,Vector3 markerPosition)
        {
            this.mesh = mesh;
            this.body = body;

            this.marker = new Marker(this.GetWorld, markerPosition, false);

            controller = new PhysicsController(body);
            controller.EnableControl();

            mesh.StartClip("Idle");

            RequiresUpdate = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get World
        /****************************************************************************/
        protected override Matrix GetMyWorld(int bone)
        {
            if (bone == -1)
                return World;
            else
                return mesh.WorldTransforms[bone];
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {                        
            /*************************************/
            /// MoveToPointCommandEvent
            /*************************************/
            if (e.GetType().Equals(typeof(MoveToPointCommandEvent)))
            {
                MoveToPointCommandEvent moveToPointCommandEvent = e as MoveToPointCommandEvent;

                target = moveToPointCommandEvent.point;
                moving = true;
            }
            /*************************************/

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {
            // TODO: zrobić zeby się poruszało

            float rotationSpeed = 0.1f;
            float movingSpeed = 10.0f;
            float distance = 5.0f;
            float anglePrecision = 0.1f;

            if (moving)
            {
                if (Vector3.Distance(World.Translation, target) < distance)
                {
                    moving = false;
                    controller.StopMoving();
                    mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.5f));
                }

                Vector3 direction = World.Translation - target;

                Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                Vector2 v2 = Vector2.Normalize(new Vector2(World.Forward.X, World.Forward.Z));

                float angle = (float)Math.Acos((double)Vector2.Dot(v1,v2));

                //Diagnostics.PushLog(MathHelper.ToDegrees(angle).ToString() + " : " + dot.ToString());

                if (angle > MathHelper.Pi) angle -= (float)Math.PI*2;

                if (Math.Abs(angle) > anglePrecision) controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed);
                                
                controller.MoveForward(movingSpeed);

                if (mesh.currentAnimation.Clip.Name != "Run")
                {
                    mesh.StartClip("Run");
                }
            }
        }
        /****************************************************************************/


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

            if (marker != null)
            {
                marker.ReleaseMe();
                marker = null;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            MercenaryData data = new MercenaryData();
            GetData(data);

            data.Model = mesh.Model.Name;

            data.Diffuse  = (mesh.Textures.Diffuse == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals  = (mesh.Textures.Normals == null ? String.Empty : mesh.Textures.Normals.Name);

            data.TimeRatio       = mesh.TimeRatio;
            data.CurrentClip     = (mesh.currentAnimation.Clip == null ? String.Empty : mesh.currentAnimation.Clip.Name);
            data.CurrentTime     = mesh.currentAnimation.ClipTime.TotalSeconds;
            data.CurrentKeyframe = mesh.currentAnimation.Keyframe;
            data.Pause           = mesh.Pause;

            data.Blend          = mesh.Blend;
            data.BlendDuration  = mesh.BlendDuration.TotalSeconds;
            data.BlendTime      = mesh.BlendTime.TotalSeconds;
            data.BlendClip      = (mesh.blendAnimation.Clip == null ? String.Empty : mesh.blendAnimation.Clip.Name);
            data.BlendClipTime  = mesh.blendAnimation.ClipTime.TotalSeconds;
            data.BlendKeyframe  = mesh.blendAnimation.Keyframe;

            data.Immovable        = body.Immovable;
            data.IsEnabled        = body.IsEnabled;
            data.Elasticity       = body.Elasticity;
            data.StaticRoughness  = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Mass             = body.Mass;
            data.Translation      = body.SkinTranslation;
            data.SkinPitch        = body.Pitch;
            data.SkinRoll         = body.Roll;
            data.SkinYaw          = body.Yaw;

            data.Radius = body.Radius;
            data.Length = body.Length;

            data.MarkerPosition = marker.LocalPosition;

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Mercenary Data
    /********************************************************************************/
    [Serializable]
    public class MercenaryData : GameObjectInstanceData
    {
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
        public bool Immovable { get; set; }
        [CategoryAttribute("Physics")]
        public bool IsEnabled { get; set; }
        [CategoryAttribute("Physics")]
        public float Elasticity { get; set; }
        [CategoryAttribute("Physics")]
        public float StaticRoughness { get; set; }
        [CategoryAttribute("Physics")]
        public float DynamicRoughness { get; set; }
        [CategoryAttribute("Physics")]
        public float Mass { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Length { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float Radius { get; set; }
        [CategoryAttribute("Collision Skin")]
        public Vector3 Translation { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float SkinYaw { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float SkinPitch { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float SkinRoll { get; set; }


        [CategoryAttribute("Marker")]
        public Vector3 MarkerPosition { get; set; }
    }
    /********************************************************************************/
}
/************************************************************************************/