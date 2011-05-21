using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.Physics;
using PlagueEngine.ArtificialIntelligence.Controllers;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Mercenary
    /********************************************************************************/
    class Mercenary : AbstractPerson, IActiveGameObject
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/                        
        /*
        private Vector3            target;
        private GameObjectInstance objectTarget;
        
        private int    moving = 0;

        private float rotationSpeed  = 0;
        private float movingSpeed    = 0;
        private float distance       = 0;
        private float anglePrecision = 0;
        */
        private MercenaryController controller;
        /****************************************************************************/



        /****************************************************************************/
        /// Slots
        /****************************************************************************/
        //private GameObjectInstance currentObject = null;
        //private GameObjectInstance currentObject = null;
        //private GameObjectInstance currentObject = null;

        //private String gripBone;
        /****************************************************************************/


        /****************************************************************************/
        /// Marker
        /****************************************************************************/
        public bool                 Marker     { get; set; }
        private FrontEndComponent marker = null;
        private Vector3 markerLocalPosition;        
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        //public uint      MaxHP { get; private set; }
        //public uint      HP    { get; private set; }
        //public Rectangle Icon  { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Components
        /****************************************************************************/
        //public SkinnedMeshComponent Mesh       { get; private set; }
        //public CapsuleBodyComponent Body       { get; private set; }
        //public PhysicsController    Controller { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(SkinnedMeshComponent mesh, 
                         CapsuleBodyComponent body,
                         FrontEndComponent    frontEndComponent,
                         Vector3 markerPosition,
                         float rotationSpeed,
                         float movingSpeed,
                         float distance,
                         float angle,
                         String gripBone,
                         uint maxHP,
                         uint HP,
                         Rectangle icon)
        {
            Mesh = mesh;
            Body = body;

            this.controller = new MercenaryController(this, rotationSpeed, movingSpeed, distance, angle);

            /*this.rotationSpeed  = rotationSpeed;
            this.movingSpeed    = movingSpeed;
            this.distance       = distance;
            this.anglePrecision = angle;
            */
            this.gripBone       = gripBone;
            
            this.HP = HP;
            MaxHP = maxHP;
            Icon = icon;

            this.marker = frontEndComponent;
            markerLocalPosition = markerPosition;
            marker.Draw = MarkerDraw;
            Marker = false;

            Controller = new PhysicsController(Body);
            Controller.EnableControl();

            Mesh.StartClip("Idle");
            
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
                return Mesh.WorldTransforms[bone];
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            this.controller.OnEvent(sender, e);
            /*
            #region Move to point (1)
            
            if (e.GetType().Equals(typeof(MoveToPointCommandEvent)))
            {
                MoveToPointCommandEvent moveToPointCommandEvent = e as MoveToPointCommandEvent;

                target = moveToPointCommandEvent.point;
                moving = 1;
            }
            #endregion

            #region Grab Object (2)

            else if (e.GetType().Equals(typeof(GrabObjectCommandEvent)))
            {
                GrabObjectCommandEvent moveToObjectCommandEvent = e as GrabObjectCommandEvent;

                if (moveToObjectCommandEvent.gameObject != this)
                {
                    objectTarget = moveToObjectCommandEvent.gameObject;
                    moving = 2;
                    Body.SubscribeCollisionEvent(objectTarget.ID);
                }
            }
            #endregion

            #region Collision detected (ends 2 & 4)
            else if (e.GetType().Equals(typeof(CollisionEvent)))
            {
                CollisionEvent collisionEvent = e as CollisionEvent;

                if (collisionEvent.gameObject == objectTarget)
                {
                    Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                    if (moving == 2)
                    {
                        #region Pick object if pickable
                        if (objectTarget.Status == GameObjectStatus.Pickable)
                        {
                            #region Drop current object if present
                            if (currentObject != null)
                            {
                                currentObject.World.Translation += Vector3.Normalize(World.Forward) * 2;
                                currentObject.Owner = null;
                                currentObject.OwnerBone = -1;
                            }
                            #endregion

                            currentObject = objectTarget;
                            objectTarget.Owner = this;
                            objectTarget.OwnerBone = Mesh.BoneMap[gripBone];
                        }
                        #endregion
                    }
                    else if (moving == 4)
                    {
                        #region Examine object
                        SendEvent(new ExamineEvent(), EventsSystem.Priority.Normal, objectTarget);
                        #endregion
                    }
                    #region Turn to idle
                    objectTarget = null;
                    moving = 0;
                    Controller.StopMoving();
                    Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                    #endregion
                }
            }
            #endregion

            #region Follow object (3)
            else if (e.GetType().Equals(typeof(FollowObjectCommandEvent)))
            {
                FollowObjectCommandEvent FollowObjectCommandEvent = e as FollowObjectCommandEvent;

                if (FollowObjectCommandEvent.gameObject != this)
                {
                    objectTarget = FollowObjectCommandEvent.gameObject;
                    moving = 3;
                }
            }
            #endregion

            #region Examine Object (4)
            else if (e.GetType().Equals(typeof(ExamineObjectCommandEvent)))
            {
                ExamineObjectCommandEvent ExamineObjectCommandEvent = e as ExamineObjectCommandEvent;

                objectTarget = ExamineObjectCommandEvent.gameObject;                    
                moving = 4;
                Body.SubscribeCollisionEvent(objectTarget.ID);
            }
            #endregion

            */
        }
        /****************************************************************************/

        /*
        private void move(TimeSpan deltaTime)
        {
            if (Vector2.Distance(new Vector2(World.Translation.X,
                                                 World.Translation.Z),
                                     new Vector2(target.X,
                                                 target.Z)) < distance)
            {
                moving = 0;
                Controller.StopMoving();
                Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
            }
            else
            {
                Vector3 direction = World.Translation - target;
                Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                Vector2 v2 = Vector2.Normalize(new Vector2(World.Forward.X, World.Forward.Z));

                float det = v1.X * v2.Y - v1.Y * v2.X;
                float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                if (det < 0) angle = -angle;

                if (Math.Abs(angle) > anglePrecision) Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);

                Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                #region Blend to Run
                if (Mesh.CurrentClip != "Run")
                {
                    Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
                }
                #endregion 
            }
        }

        private void grabOrExamine(TimeSpan deltaTime)
        {
            if (objectTarget.IsDisposed() || objectTarget.Owner != null)
            {
                #region Cancel Grab or Examine
                objectTarget = null;
                moving = 0;
                Controller.StopMoving();
                Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                #endregion
                return;
            }

            Vector3 direction = World.Translation - objectTarget.World.Translation;
            Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
            Vector2 v2 = Vector2.Normalize(new Vector2(World.Forward.X, World.Forward.Z));

            float det = v1.X * v2.Y - v1.Y * v2.X;
            float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

            if (det < 0) angle = -angle;

            if (Math.Abs(angle) > anglePrecision) Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);

            Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

            #region Blend to Run
            if (Mesh.CurrentClip != "Run")
            {
                Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
            }
            #endregion
        }

        private void follow(TimeSpan deltaTime)
        {
            if (objectTarget.IsDisposed())
            {
                #region Turn to Idle
                objectTarget = null;
                moving = 0;
                Controller.StopMoving();
                Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                #endregion
                return;
            }
            else
            {
                double currentDistance = Vector2.Distance(new Vector2(World.Translation.X, World.Translation.Z),
                                     new Vector2(objectTarget.World.Translation.X, objectTarget.World.Translation.Z));
                if (Mesh.CurrentClip == "Idle" && currentDistance > 8)
                {
                    #region Resume Chase
                    Vector3 direction = World.Translation - objectTarget.World.Translation;
                    Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                    Vector2 v2 = Vector2.Normalize(new Vector2(World.Forward.X, World.Forward.Z));

                    float det = v1.X * v2.Y - v1.Y * v2.X;
                    float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                    if (det < 0)
                    {
                        angle = -angle;
                    }
                    if (Math.Abs(angle) > anglePrecision)
                    {
                        Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
                    }

                    Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                    Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.3f));

                    #endregion
                }
                else if (Mesh.CurrentClip != "Idle")
                {
                    if (currentDistance < 4)
                    {
                        #region Pause Chase
                        Controller.StopMoving();
                        Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        #endregion
                        return;
                    }
                    else
                    {
                        #region Continue Running
                        Vector3 direction = World.Translation - objectTarget.World.Translation;
                        Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        Vector2 v2 = Vector2.Normalize(new Vector2(World.Forward.X, World.Forward.Z));

                        float det = v1.X * v2.Y - v1.Y * v2.X;
                        float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                        if (det < 0)
                        {
                            angle = -angle;
                        }

                        if (Math.Abs(angle) > anglePrecision)
                        {
                            Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
                        }

                        Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                        if (Mesh.CurrentClip != "Run")
                        {
                            Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.3f));
                        }
                        #endregion
                    }
                }
            }
        }
        */

        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {
            /*switch (moving)
            {
                case 1:
                    move(deltaTime);
                    break;
                case 2:
                case 4:
                    grabOrExamine(deltaTime);
                    break;
                case 3:
                    follow(deltaTime);
                    break;
            }*/
            this.controller.Update(deltaTime);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// MarkerDraw
        /****************************************************************************/
        public void MarkerDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {
            if (!Marker) return;
            Vector4 position = Vector4.Transform(Vector3.Transform(markerLocalPosition,World), ViewProjection);
            Vector2 pos2;

            pos2.X = MathHelper.Clamp(0.5f * ((position.X / Math.Abs(position.W)) + 1.0f), 0.01f, 0.99f);
            pos2.X *= screenWidth;
            pos2.X -= marker.Texture.Width / 2;

            pos2.Y = MathHelper.Clamp(1.0f - (0.5f * ((position.Y / Math.Abs(position.W)) + 1.0f)), 0.01f, 0.99f);
            pos2.Y *= screenHeight;
            pos2.Y -= marker.Texture.Height / 2;

            spriteBatch.Draw(marker.Texture, pos2, Color.White);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            if (Mesh != null)
            {
                Mesh.ReleaseMe();
                Mesh = null;
            }

            if (Body != null)
            {
                Body.ReleaseMe();
                Body = null;
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

            data.Model = Mesh.Model.Name;

            data.Diffuse  = (Mesh.Textures.Diffuse == null ? String.Empty : Mesh.Textures.Diffuse.Name);
            data.Specular = (Mesh.Textures.Specular == null ? String.Empty : Mesh.Textures.Specular.Name);
            data.Normals  = (Mesh.Textures.Normals == null ? String.Empty : Mesh.Textures.Normals.Name);

            data.TimeRatio       = Mesh.TimeRatio;
            data.CurrentClip     = (Mesh.currentAnimation.Clip == null ? String.Empty : Mesh.currentAnimation.Clip.Name);
            data.CurrentTime     = Mesh.currentAnimation.ClipTime.TotalSeconds;
            data.CurrentKeyframe = Mesh.currentAnimation.Keyframe;
            data.Pause           = Mesh.Pause;

            data.Blend          = Mesh.Blend;
            data.BlendDuration  = Mesh.BlendDuration.TotalSeconds;
            data.BlendTime      = Mesh.BlendTime.TotalSeconds;
            data.BlendClip      = (Mesh.blendAnimation.Clip == null ? String.Empty : Mesh.blendAnimation.Clip.Name);
            data.BlendClipTime  = Mesh.blendAnimation.ClipTime.TotalSeconds;
            data.BlendKeyframe  = Mesh.blendAnimation.Keyframe;

            data.Immovable        = Body.Immovable;
            data.IsEnabled        = Body.IsEnabled;
            data.Elasticity       = Body.Elasticity;
            data.StaticRoughness  = Body.StaticRoughness;
            data.DynamicRoughness = Body.DynamicRoughness;
            data.Mass             = Body.Mass;
            data.Translation      = Body.SkinTranslation;
            data.SkinPitch        = Body.Pitch;
            data.SkinRoll         = Body.Roll;
            data.SkinYaw          = Body.Yaw;

            data.Radius = Body.Radius;
            data.Length = Body.Length;

            data.MarkerPosition = markerLocalPosition;
            data.GripBone       = gripBone;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public String[] GetActions()
        {
            return new String[] { "Follow" };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public string[] GetActions(Mercenary mercenary)
        {
            if (mercenary == this) return new String[] { };
            else return new String[] { "Follow" };
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

        [CategoryAttribute("HP")]
        public uint MaxHP { get;  set; }
        [CategoryAttribute("HP")]
        public uint HP { get;  set; }

        [CategoryAttribute("Icon")]
        public Rectangle Icon { get;  set; }
    }
    /********************************************************************************/

}
/************************************************************************************/