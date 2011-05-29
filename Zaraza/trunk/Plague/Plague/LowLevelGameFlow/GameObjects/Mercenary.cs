using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.Audio.Components;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.Physics;
using PlagueEngine.EventsSystem;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Mercenary
    /********************************************************************************/
    class Mercenary : GameObjectInstance, IActiveGameObject
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/                        
        private Vector3            target;
        private GameObjectInstance objectTarget;
        
        private int    moving = 0;

        private float rotationSpeed  = 0;
        private float movingSpeed    = 0;
        private float distance       = 0;
        private float anglePrecision = 0;

        private IEventsReceiver receiver = null;
        /****************************************************************************/



        /****************************************************************************/
        /// Slots
        /****************************************************************************/
        public GameObjectInstance currentObject   = null;

        public Firearm Weapon   = null;
        public Firearm sideArm  = null;
        public Armor   armor    = null;

        private String gripBone;

        public Dictionary<IStorable, ItemPosition> Items { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Marker
        /****************************************************************************/
        public bool                 Marker { get; set; }
        private FrontEndComponent   marker = null;
        private Vector3             markerLocalPosition;        
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public uint      MaxHP         { get; private set; }
        public uint      HP            { get; private set; }
        public Rectangle Icon          { get; private set; }
        public Rectangle InventoryIcon { get; private set; }
        public uint      TinySlots     { get; private set; }
        public uint      Slots         { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Components
        /****************************************************************************/
        public SkinnedMeshComponent mesh;
        public CapsuleBodyComponent body;
        public SoundEffectComponent SoundEffectComponent;
        public SkinnedMeshComponent Mesh { get { return this.mesh; } private set { this.mesh = value; } }
        public CapsuleBodyComponent Body { get { return this.body; } private set { this.body = value; } }
        public PhysicsController    Controller { get; private set; }
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
                         Rectangle icon,
                         Rectangle inventoryIcon,
                         uint tinySlots,
                         uint slots)
        {
            Mesh = mesh;
            Body = body;
            SoundEffectComponent = new SoundEffectComponent();
            SoundEffectComponent.CreateNewSoundFromFolder("Mercenary", 1, 0, 0);
            this.rotationSpeed  = rotationSpeed;
            this.movingSpeed    = movingSpeed;
            this.distance       = distance;
            this.anglePrecision = angle;
            this.gripBone       = gripBone;
            this.InventoryIcon  = inventoryIcon;
            this.TinySlots      = tinySlots;
            this.Slots          = slots;

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

            Items = new Dictionary<IStorable, ItemPosition>();

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

            /*************************************/
            /// MoveToPointCommandEvent
            /*************************************/
            if (e.GetType().Equals(typeof(MoveToPointCommandEvent)))
            {
                SoundEffectComponent.PlaySound("yesSir");
                MoveToPointCommandEvent moveToPointCommandEvent = e as MoveToPointCommandEvent;

                receiver = sender as IEventsReceiver;
                target = moveToPointCommandEvent.point;
                moving = 1;
            }
            /*************************************/
            /// GrabObjectCommandEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(GrabObjectCommandEvent)))
            {
                GrabObjectCommandEvent moveToObjectCommandEvent = e as GrabObjectCommandEvent;

                receiver = sender as IEventsReceiver;

                if (moveToObjectCommandEvent.gameObject != this)
                {
                    objectTarget = moveToObjectCommandEvent.gameObject;
                    moving = 2;
                    Body.SubscribeCollisionEvent(objectTarget.ID);
                }
            }
            /*************************************/
            /// CollisionEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(CollisionEvent)))
            {
                CollisionEvent collisionEvent = e as CollisionEvent;

                if (collisionEvent.gameObject == objectTarget)
                {
                    if (moving == 2)
                    {
                        Body.CancelSubscribeCollisionEvent(objectTarget.ID);

                        if (objectTarget.Status == GameObjectStatus.Pickable)
                        {
                            if (currentObject != null)
                            {
                                currentObject.World.Translation += Vector3.Normalize(World.Forward) * 2;
                                currentObject.Owner = null;
                                currentObject.OwnerBone = -1;
                            }

                            currentObject = objectTarget;
                            objectTarget.Owner = this;
                            objectTarget.OwnerBone = Mesh.BoneMap[gripBone];
                        }

                        objectTarget = null;
                        moving = 0;
                        body.Immovable = true;
                        Controller.StopMoving();
                        Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        SendEvent(new ActionDoneEvent(), Priority.High, receiver);
                    }
                    else if (moving == 4)
                    {
                        Body.CancelSubscribeCollisionEvent(objectTarget.ID);

                        SendEvent(new ExamineEvent(), EventsSystem.Priority.Normal, objectTarget);
                                            
                        objectTarget = null;
                        moving = 0;
                        Controller.StopMoving();
                        Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        SendEvent(new ActionDoneEvent(), Priority.High, receiver);                    
                    }
                }
            }
            /*************************************/
            /// FollowObjectCommandEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(FollowObjectCommandEvent)))
            {
                FollowObjectCommandEvent FollowObjectCommandEvent = e as FollowObjectCommandEvent;

                receiver = sender as IEventsReceiver;

                if (FollowObjectCommandEvent.gameObject != this)
                {
                    objectTarget = FollowObjectCommandEvent.gameObject;
                    moving = 3;
                }
            }
            /*************************************/
            /// ExamineObjectCommandEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(ExamineObjectCommandEvent)))
            {
                ExamineObjectCommandEvent ExamineObjectCommandEvent = e as ExamineObjectCommandEvent;

                receiver = sender as IEventsReceiver;

                objectTarget = ExamineObjectCommandEvent.gameObject;                    
                moving = 4;
                Body.SubscribeCollisionEvent(objectTarget.ID);                
            }
            /*************************************/
            /// StopActionEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(StopActionEvent)))
            {
                moving       = 0;
                objectTarget = null;
                Controller.StopMoving();
                Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));     
            }
            /*************************************/

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {
            

            SoundEffectComponent.SetPosiotion(World.Translation);
            if (moving == 1)
            {
                if (Vector2.Distance(new Vector2(World.Translation.X,
                                                 World.Translation.Z),
                                     new Vector2(target.X,
                                                 target.Z)) < distance)
                {
                    moving = 0;
                    Controller.StopMoving();
                    Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                    SendEvent(new ActionDoneEvent(), Priority.High, receiver);
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
                                        
                    if (Mesh.CurrentClip != "Run")
                    {
                        Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));                        
                    }                    
                }
            }
            else if (moving == 2 || moving == 4)
            {
                if (objectTarget.IsDisposed() || objectTarget.Owner != null)
                {
                    objectTarget = null;
                    moving = 0;
                    Controller.StopMoving();
                    Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));                    
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

                if (Mesh.CurrentClip != "Run")
                {
                    Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));                    
                }                                    
            }
            else if (moving == 3)
            {
                if (objectTarget.IsDisposed())
                {
                    objectTarget = null;
                    moving = 0;
                    Controller.StopMoving();
                    Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                    return;
                }
                else if (Vector2.Distance(new Vector2(World.Translation.X, World.Translation.Z),
                                         new Vector2(objectTarget.World.Translation.X, objectTarget.World.Translation.Z)) < 4)
                {
                    Controller.StopMoving();
                    if (Mesh.CurrentClip != "Idle")
                    {
                        Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                    }
                    return;
                }
                else if (Mesh.CurrentClip == "Idle" && Vector2.Distance(new Vector2(World.Translation.X, World.Translation.Z), new Vector2(objectTarget.World.Translation.X, objectTarget.World.Translation.Z)) > 8)
                {
                    Vector3 direction = World.Translation - objectTarget.World.Translation;
                    Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                    Vector2 v2 = Vector2.Normalize(new Vector2(World.Forward.X, World.Forward.Z));

                    float det = v1.X * v2.Y - v1.Y * v2.X;
                    float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                    if (det < 0) angle = -angle;

                    if (Math.Abs(angle) > anglePrecision) Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);

                    Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                    if (Mesh.CurrentClip != "Run")
                    {
                        Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.3f));
                    }
                }
                else if (Mesh.CurrentClip != "Idle")
                {
                    Vector3 direction = World.Translation - objectTarget.World.Translation;
                    Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                    Vector2 v2 = Vector2.Normalize(new Vector2(World.Forward.X, World.Forward.Z));

                    float det = v1.X * v2.Y - v1.Y * v2.X;
                    float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                    if (det < 0) angle = -angle;

                    if (Math.Abs(angle) > anglePrecision) Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);

                    Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                    if (Mesh.CurrentClip != "Run")
                    {
                        Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.3f));
                    }                
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Item
        /****************************************************************************/
        public void PickItem(GameObjectInstance item)
        {
            currentObject  = item;
            item.Owner     = this;
            item.OwnerBone = Mesh.BoneMap[gripBone];
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Drop Item
        /****************************************************************************/
        public void DropItem(GameObjectInstance item = null)
        {
            if (item != null)
            {
                currentObject.World.Translation += Vector3.Normalize(World.Forward) * 2;
                currentObject.Owner = null;
                currentObject.OwnerBone = -1;            
            }
            else if (currentObject != null)
            {
                currentObject.World.Translation += Vector3.Normalize(World.Forward) * 2;
                currentObject.Owner = null;
                currentObject.OwnerBone = -1;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Store Item
        /****************************************************************************/
        public void StoreCurrentItem()
        {
            IStorable item = currentObject as IStorable;
            if (item != null)
            {
                item.OnStoring();
                currentObject = null;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// MarkerDraw
        /****************************************************************************/
        public void MarkerDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {
            if (!Marker) return;
            Vector4 position;
            Vector2 pos2;

            position = Vector4.Transform(Vector3.Transform(markerLocalPosition,World), ViewProjection);

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

            data.Diffuse  = (Mesh.Textures.Diffuse  == null ? String.Empty : Mesh.Textures.Diffuse.Name );
            data.Specular = (Mesh.Textures.Specular == null ? String.Empty : Mesh.Textures.Specular.Name);
            data.Normals  = (Mesh.Textures.Normals  == null ? String.Empty : Mesh.Textures.Normals.Name );

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

            data.HP            = HP;
            data.MaxHP         = MaxHP;
            data.Icon          = Icon;
            data.InventoryIcon = InventoryIcon;

            data.TinySlots          = TinySlots;
            data.Slots              = Slots;
            data.MovingSpeed        = movingSpeed;
            data.RotationSpeed      = rotationSpeed;
            data.DistancePrecision  = distance;
            data.AnglePrecision     = anglePrecision;


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
        [CategoryAttribute("Icon")]
        public Rectangle InventoryIcon { get; set; }

        [CategoryAttribute("Slots")]
        public uint TinySlots { get; set; }
        [CategoryAttribute("Slots")]
        public uint Slots { get; set; }

    }
    /********************************************************************************/

}
/************************************************************************************/