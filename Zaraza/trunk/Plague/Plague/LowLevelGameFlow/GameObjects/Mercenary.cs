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
        
        private Vector3            target;
        private GameObjectInstance objectTarget;
        
        private int    moving = 0;

        private float rotationSpeed  = 0;
        private float movingSpeed    = 0;
        private float distance       = 0;
        private float anglePrecision = 0;

        private GameObjectInstance currentObject = null;
        private String gripBone;
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Marker Marker { get { return marker; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(SkinnedMeshComponent mesh, 
                         CapsuleBodyComponent body,
                         Vector3 markerPosition,
                         float rotationSpeed,
                         float movingSpeed,
                         float distance,
                         float angle,
                         String gripBone)
        {
            this.mesh = mesh;
            this.body = body;
            
            this.rotationSpeed  = rotationSpeed;
            this.movingSpeed    = movingSpeed;
            this.distance       = distance;
            this.anglePrecision = angle;
            this.gripBone       = gripBone;

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
                moving = 1;
            }
            /*************************************/
            /// MoveToObjectCommandEvent
            /*************************************/
            if (e.GetType().Equals(typeof(MoveToObjectCommandEvent)))
            {
                MoveToObjectCommandEvent moveToObjectCommandEvent = e as MoveToObjectCommandEvent;

                if (moveToObjectCommandEvent.gameObject != this)
                {
                    objectTarget = moveToObjectCommandEvent.gameObject;
                    moving = 2;
                    body.SubscribeCollisionEvent(objectTarget.ID);
                }
            }
            /*************************************/
            /// CollisionEvent
            /*************************************/
            if (e.GetType().Equals(typeof(CollisionEvent)))
            {
                CollisionEvent collisionEvent = e as CollisionEvent;

                if (collisionEvent.gameObject == objectTarget)
                {
                    body.CancelSubscribeCollisionEvent(objectTarget.ID);

                    if (objectTarget.Status == GameObjectStatus.Pickable)
                    {
                        if (currentObject != null)
                        {
                            currentObject.Owner = null;
                            currentObject.OwnerBone = -1;
                        }

                        currentObject = objectTarget;
                        objectTarget.Owner = this;
                        objectTarget.OwnerBone = mesh.Model.SkinningData.BoneMap[gripBone];
                    }

                    objectTarget = null;
                    moving = 0;
                    controller.StopMoving();
                    mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                }
            }
            /*************************************/            
            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {           
            if (moving == 1)
            {
                if (Vector2.Distance(new Vector2(World.Translation.X,
                                                 World.Translation.Z),
                                     new Vector2(target.X,
                                                 target.Z)) < distance)
                {
                    moving = 0;
                    controller.StopMoving();
                    mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                }
                else
                {
                    Vector3 direction = World.Translation - target;
                    Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                    Vector2 v2 = Vector2.Normalize(new Vector2(World.Forward.X, World.Forward.Z));

                    float det = v1.X * v2.Y - v1.Y * v2.X;
                    float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                    if (det < 0) angle = -angle;

                    if (Math.Abs(angle) > anglePrecision) controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
                    
                    controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);
                                        
                    if (mesh.CurrentClip != "Run")
                    {
                        mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
                    }                    
                }
            }
            else if (moving == 2)
            {
                if (objectTarget.IsDisposed())
                {
                    objectTarget = null;
                    moving = 0;
                    return;
                }

                Vector3 direction = World.Translation - objectTarget.World.Translation;
                Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                Vector2 v2 = Vector2.Normalize(new Vector2(World.Forward.X, World.Forward.Z));

                float det = v1.X * v2.Y - v1.Y * v2.X;
                float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                if (det < 0) angle = -angle;

                if (Math.Abs(angle) > anglePrecision) controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);

                controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                if (mesh.CurrentClip != "Run")
                {
                    mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
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
            data.GripBone       = gripBone;

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

        [CategoryAttribute("Movement")]
        public float RotationSpeed      { get; set; }
        [CategoryAttribute("Movement")]
        public float MovingSpeed        { get; set; }
        [CategoryAttribute("Movement")]
        public float DistancePrecision  { get; set; }
        [CategoryAttribute("Movement")]
        public float AnglePrecision     { get; set; }

        [CategoryAttribute("Grip Bone")]
        public String GripBone { get; set; }
    }
    /********************************************************************************/
}
/************************************************************************************/