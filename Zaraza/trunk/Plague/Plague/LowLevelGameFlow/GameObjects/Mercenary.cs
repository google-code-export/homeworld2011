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
        /// Slots
        /****************************************************************************/
        public StorableObject CurrentObject = null;

        public Firearm Weapon   = null;
        public Firearm SideArm  = null;
        public Armor   Armor    = null;

        public String Grip        { get; private set; }
        public String SideArmGrip { get; private set; }
        public String WeaponGrip  { get; private set; }

        public Dictionary<StorableObject, ItemPosition> Items { get; private set; }
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
        public Rectangle Icon          { get; private set; }
        public Rectangle InventoryIcon { get; private set; }
        public uint      TinySlots     { get; private set; }
        public uint      Slots         { get; private set; }
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
                         String sideArmBone,
                         String weaponBone,
                         uint maxHP,
                         uint HP,
                         Rectangle icon,
                         Rectangle inventoryIcon,
                         uint tinySlots,
                         uint slots,
                         Dictionary<StorableObject, ItemPosition> items,
                         StorableObject currentObject,
                         Firearm weapon,
                         Firearm sideArm)
        {
            this.objectAIController = new MercenaryController(this, rotationSpeed, movingSpeed, distance, angle);
            Mesh = mesh;
            Body = body;
            SoundEffectComponent = new SoundEffectComponent();
            SoundEffectComponent.CreateNewSoundFromFolder("Mercenary", 0.2f, 0, 0);            
            InventoryIcon  = inventoryIcon;
            TinySlots      = tinySlots;
            Slots          = slots;
            CurrentObject  = currentObject;
            Weapon         = weapon;
            SideArm        = sideArm;

            Grip        = gripBone;
            SideArmGrip = sideArmBone;
            WeaponGrip  = weaponBone;

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
            /// TODO: Wpisałem to w Merca bo w sumie średnio to się ma do zachowań

            /*************************************/
            /// DropItemCommandEvent
            /*************************************/
            if (e.GetType().Equals(typeof(DropItemCommandEvent)))
            {
                DropItem();
                SendEvent(new ActionDoneEvent(), Priority.High, sender as IEventsReceiver);
            }
            /*************************************/
            /// SwitchToWeaponCommandEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(SwitchToWeaponCommandEvent)))
            {
                if ((CurrentObject == null && Weapon != null))
                {
                    Firearm weapon = Weapon;
                    StoreItem(1);
                    PlaceItem(weapon, 0);
                }
                else if (CurrentObject != null)
                {
                    Firearm firearm = CurrentObject as Firearm;
                    if (firearm != null)
                    {
                        if (!firearm.SideArm)
                        {
                            Firearm weapon = Weapon;
                            StoreItem(0);
                            if (Weapon != null)
                            {
                                StoreItem(1);
                                PlaceItem(weapon, 0);
                            }
                            PlaceItem(firearm, 1);
                        }
                        else if (SideArm == null && Weapon != null)
                        {
                            Firearm weapon = Weapon;
                            StoreItem(0);
                            StoreItem(1);
                            PlaceItem(weapon, 0);
                            PlaceItem(firearm, 2);
                        }
                    }
                }
                SendEvent(new ActionDoneEvent(), Priority.High, sender as IEventsReceiver);
            }
            /*************************************/
            /// SwitchToSideArmCommandEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(SwitchToSideArmCommandEvent)))
            {
                if ((CurrentObject == null && SideArm != null))
                {
                    Firearm weapon = SideArm;
                    StoreItem(2);
                    PlaceItem(weapon, 0);
                }
                else if (CurrentObject != null)
                {
                    Firearm firearm = CurrentObject as Firearm;
                    if (firearm != null)
                    {
                        if (firearm.SideArm)
                        {
                            Firearm weapon = SideArm;
                            StoreItem(0);
                            if (SideArm != null)
                            {
                                StoreItem(2);
                                PlaceItem(weapon, 0);
                            }
                            PlaceItem(firearm, 2);
                        }
                        else if (Weapon == null && SideArm != null)
                        {
                            Firearm weapon = SideArm;
                            StoreItem(0);
                            StoreItem(2);
                            PlaceItem(weapon, 0);
                            PlaceItem(firearm, 1);
                        }
                    }
                }
                SendEvent(new ActionDoneEvent(), Priority.High, sender as IEventsReceiver);
            }
            /*************************************/
            else this.objectAIController.OnEvent(sender, e);                
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
        public void PickItem(StorableObject item)
        {
            CurrentObject  = item;
            item.Owner     = this;
            item.OwnerBone = Mesh.BoneMap[Grip];
            item.OnPicking();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Place Item
        /****************************************************************************/
        public void PlaceItem(StorableObject item, uint slot)
        {
            switch (slot)
            { 
                /***************/
                /// Current
                /***************/
                case 0: 
                    CurrentObject  = item;
                    item.Owner     = this;
                    item.OwnerBone = Mesh.BoneMap[Grip];
                    break;
                /***************/
                /// Weapon                
                /***************/
                case 1:
                    Weapon         = item as Firearm;
                    item.Owner     = this;
                    item.OwnerBone = Mesh.BoneMap[WeaponGrip];
                    break;
                /***************/
                /// Side Arm                
                /***************/
                case 2:                                         
                    SideArm        = item as Firearm;
                    item.Owner     = this;
                    item.OwnerBone = Mesh.BoneMap[SideArmGrip];
                    break;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Drop Item
        /****************************************************************************/
        public void DropItem(StorableObject item = null)
        {
            if (item != null)
            {

                item.World = Matrix.Identity;
                item.World.Translation = World.Translation +
                                         Vector3.Normalize(World.Backward) * 2 +
                                         Vector3.Normalize(World.Up) * 2;

                item.OnDropping();                

            }
            else if (CurrentObject != null)
            {

                CurrentObject.World = Matrix.Identity;
                CurrentObject.World.Translation = World.Translation +
                                                  Vector3.Normalize(World.Backward) * 2 +
                                                  Vector3.Normalize(World.Up) * 2;
                CurrentObject.OnDropping();

                CurrentObject           = null;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Store Item
        /****************************************************************************/
        public void StoreItem(uint slot)
        {
            switch (slot)
            { 
                case 0:
                    if (CurrentObject != null)
                    {
                        CurrentObject.OnStoring();
                        CurrentObject = null;
                    }
                    break;
                case 1:
                    if (Weapon != null)
                    {
                        Weapon.OnStoring();
                        Weapon = null;
                    }
                    break;
                case 2:
                    if (SideArm != null)
                    {
                        SideArm.OnStoring();
                        SideArm = null;
                    }
                    break;     
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
            data.Grip           = Grip;
            data.SideArmGrip    = SideArmGrip;
            data.WeaponGrip     = WeaponGrip;

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


            data.CurrentItem = CurrentObject == null ? 0 : CurrentObject.ID;
            data.SideArm     = SideArm       == null ? 0 : SideArm.ID;
            data.Weapon      = Weapon        == null ? 0 : Weapon.ID;

            data.Items = new List<int[]>();

            foreach (KeyValuePair<StorableObject, ItemPosition> item in Items)
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
            List<String> actions = new List<String>();

            actions.Add("Inventory");

            if (mercenary != null && mercenary != this)
            {
                actions.Add("Follow");
                actions.Add("Exchange Items");
            }
            else
            {
                if (CurrentObject == null)
                {
                    if (Weapon != null) actions.Add("Switch to Weapon");
                    if (SideArm != null) actions.Add("Switch to Side Arm");
                }
                else
                {
                    Firearm firearm = CurrentObject as Firearm;
                    if (firearm != null)
                    {
                        if (!firearm.SideArm || (firearm.SideArm && SideArm == null && Weapon != null)) actions.Add("Switch to Weapon");
                        if (firearm.SideArm || (!firearm.SideArm && Weapon == null && SideArm != null)) actions.Add("Switch to Side Arm");

                        actions.Add("Reload");
                    }
                    actions.Add("Drop Item");
                }
            }

            return actions.ToArray();
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

        [CategoryAttribute("Grips")]
        public String Grip        { get; set; }
        [CategoryAttribute("Grips")]
        public String SideArmGrip { get; set; }
        [CategoryAttribute("Grips")]
        public String WeaponGrip  { get; set; }

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
        [CategoryAttribute("Inventory")]
        public int Weapon { get; set; }
        [CategoryAttribute("Inventory")]
        public int SideArm { get; set; }
    }
    /********************************************************************************/
    
}
/************************************************************************************/