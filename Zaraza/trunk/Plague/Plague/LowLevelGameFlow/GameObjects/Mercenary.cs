using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.Audio.Components;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.Physics;
using PlagueEngine.EventsSystem;
using PlagueEngine.ArtificialInteligence.Controllers;


using PlagueEngine.AItest;
/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Mercenary
    /********************************************************************************/
    class Mercenary : AbstractLivingBeing, IActiveGameObject
    {

        /****************************************************************************/
        /// Fields => INHERITED
        /****************************************************************************/                        
        //private Vector3            target;
        //private GameObjectInstance objectTarget;
        
        //private int    moving = 0;

        //private float rotationSpeed  = 0;
        //private float movingSpeed    = 0;
        //private float distance       = 0;
        //private float anglePrecision = 0;

        //private IEventsReceiver receiver = null;
        /****************************************************************************/



        /****************************************************************************/
        /// Slots
        /****************************************************************************/
        public GameObjectInstance currentObject   = null;

        public Firearm Weapon   = null;
        public Firearm sideArm  = null;
        public Armor   armor    = null;

        public String gripBone;

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
        /// Properties => INHERITED
        /****************************************************************************/
        //public uint      MaxHP         { get; private set; }
        //public uint      HP            { get; private set; }
        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Rectangle Icon          { get; private set; }
        public Rectangle InventoryIcon { get; private set; }
        public uint      TinySlots     { get; private set; }
        public uint      Slots         { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Components => INHERITED
        /****************************************************************************/
        //public SkinnedMeshComponent mesh;
        //public CapsuleBodyComponent body;
        //public SoundEffectComponent SoundEffectComponent;
        //public SkinnedMeshComponent Mesh { get { return this.mesh; } private set { this.mesh = value; } }
        //public CapsuleBodyComponent Body { get { return this.body; } private set { this.body = value; } }
        //public PhysicsController    Controller { get; private set; }


        private EventsSnifferComponent sniffer = new EventsSnifferComponent();
        List<GameObjectInstance> barrels = new List<GameObjectInstance>();
        KeyboardListenerComponent keyboard;
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
                         uint slots,
                         Dictionary<IStorable, ItemPosition> items,
                         GameObjectInstance currentObject)
        {
            this.objectAIController = new MercenaryController(this, rotationSpeed, movingSpeed, distance, angle);
            Mesh = mesh;
            Body = body;
            SoundEffectComponent = new SoundEffectComponent();
            SoundEffectComponent.CreateNewSoundFromFolder("Mercenary", 0.2f, 0, 0);
            this.gripBone       = gripBone;
            this.InventoryIcon  = inventoryIcon;
            this.TinySlots      = tinySlots;
            this.Slots          = slots;
            this.currentObject  = currentObject;

            Items = items;

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
                        
            sniffer.SetOnSniffedEvent(OnSniffedEvent);
            sniffer.SubscribeEvents(typeof(CreateEvent),
                                    typeof(DestroyEvent));
            keyboard = new KeyboardListenerComponent(this, true);
            keyboard.SubscibeKeys(OnKey, Keys.T);

        }
        /****************************************************************************/


        private void OnKey(Keys key, ExtendedKeyState state)
        {
            if (key == Keys.T && state.WasPressed())
            {
                
                GameObjectInstance closesEnemy= AI.FindClosestVisible(barrels, this,this.World.Backward, 45, 150);
                if(closesEnemy!=null)
                Diagnostics.PushLog("CLOSEST ENEMY: " + closesEnemy.ID.ToString());
            }
        }
        private void OnSniffedEvent(EventsSender sender, IEventsReceiver receiver, EventArgs e)
        {

            /*************************************/
            /// CreateEvent
            /*************************************/
            if (e.GetType().Equals(typeof(CreateEvent)) && sender.GetType().Equals(typeof(CylindricalBodyMesh)))
            {
                barrels.Add(sender as CylindricalBodyMesh);
            }
            if (e.GetType().Equals(typeof(DestroyEvent)) && sender.GetType().Equals(typeof(CylindricalBodyMesh)))
            {
                barrels.Remove(sender as CylindricalBodyMesh);
            }
        }

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
            this.objectAIController.OnEvent(sender, e);          
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {
            this.objectAIController.Update(deltaTime);
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
            //data.IsEnabled        = Body.IsEnabled;
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
            data.MovingSpeed        = (objectAIController as MercenaryController).MovingSpeed;
            data.RotationSpeed      = (objectAIController as MercenaryController).RotationSpeed;
            data.DistancePrecision  = (objectAIController as MercenaryController).Distance;
            data.AnglePrecision     = (objectAIController as MercenaryController).AnglePrecision;
            

            data.CurrentItem = currentObject == null ? 0 : currentObject.ID;
            data.Items = new List<int[]>();

            foreach (KeyValuePair<IStorable, ItemPosition> item in Items)
            {
                data.Items.Add(new int[] { (item.Key as GameObjectInstance).ID, item.Value.Slot, item.Value.Orientation });
            }
            data.EnabledPhysics = body.Enabled;
            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public String[] GetActions()
        {
            return new String[] { "Inventory", "Follow" , "Exchange Items" };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public string[] GetActions(Mercenary mercenary)
        {
            if (mercenary == this) return new String[] { "Inventory" };
            else return new String[] { "Inventory", "Follow", "Exchange Items" };
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
        //[CategoryAttribute("Physics")]
        //public bool IsEnabled { get; set; }
        [CategoryAttribute("Physics")]
        public float Elasticity { get; set; }
        [CategoryAttribute("Physics")]
        public float StaticRoughness { get; set; }
        [CategoryAttribute("Physics")]
        public float DynamicRoughness { get; set; }
        [CategoryAttribute("Physics")]
        public float Mass { get; set; }

        [CategoryAttribute("Physics")]
        public bool EnabledPhysics { get; set; }


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

        [CategoryAttribute("Inventory")]
        public uint TinySlots { get; set; }
        [CategoryAttribute("Inventory")]
        public uint Slots { get; set; }
        [CategoryAttribute("Inventory")]
        public List<int[]> Items { get; set; }
        [CategoryAttribute("Inventory")]
        public int CurrentItem { get; set; }
    }
    /********************************************************************************/

    
}
/************************************************************************************/