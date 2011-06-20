using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.Audio.Components;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;
using PlagueEngine.EventsSystem;
using PlagueEngine.ArtificialIntelligence.Controllers;


/************************************************************************************/
// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Mercenary
    /********************************************************************************/
    class Mercenary : AbstractLivingBeing, IActiveGameObject, IPenetrable
    {

        public delegate void OnSomething();

        /****************************************************************************/
        // Slots
        /****************************************************************************/
        public StorableObject CurrentObject;

        public Firearm Weapon;
        public Firearm SideArm;
        public Armor Armor;

        public String Grip { get; private set; }
        public String SideArmGrip { get; private set; }
        public String WeaponGrip { get; private set; }

        public Dictionary<StorableObject, ItemPosition> Items { get; private set; }
        public OnSomething UpdateInventory = null;
        /****************************************************************************/


        /****************************************************************************/
        // Marker
        /****************************************************************************/
        public bool Marker { get; set; }
        private FrontEndComponent _marker;
        private Vector3 _markerLocalPosition;
        /****************************************************************************/


        /****************************************************************************/
        // Properties
        /****************************************************************************/
        public Rectangle Icon { get; private set; }
        public Rectangle InventoryIcon { get; private set; }
        public uint TinySlots { get; private set; }
        public uint Slots { get; private set; }
        public new bool IsDisposed { get; set; } //Zmienna nadpisuje istenejace metode.
        /****************************************************************************/


        /****************************************************************************/
        // Initialization
        /****************************************************************************/
        public void Init(SkinnedMeshComponent mesh,
                         CapsuleBodyComponent body,
                         FrontEndComponent frontEndComponent,
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
                         Firearm sideArm,
                         List<AnimationBinding> animationMapping
            )
        {
            ObjectAIController = new MercenaryController(this, rotationSpeed, movingSpeed, distance, angle, maxHP, HP, animationMapping);
            Mesh = mesh;
            Body = body;
            SoundEffectComponent = new SoundEffectComponent();
            SoundEffectComponent.LoadFolderTree("Mercenary", 1f, 0, 0, 2);
            InventoryIcon = inventoryIcon;
            TinySlots = tinySlots;
            Slots = slots;
            CurrentObject = currentObject;
            Weapon = weapon;
            SideArm = sideArm;

            Grip = gripBone;
            SideArmGrip = sideArmBone;
            WeaponGrip = weaponBone;

            Items = items;

            Icon = icon;

            _marker = frontEndComponent;
            _markerLocalPosition = markerPosition;
            _marker.Draw = MarkerDraw;
            Marker = false;

            Controller = new PhysicsController(Body);
            Controller.EnableControl();

            RequiresUpdate = true;
            IsDisposed = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        // Get World
        /****************************************************************************/
        protected override Matrix GetMyWorld(int bone)
        {
            if (IsDisposed)
            {
                return Matrix.Identity;
            }
            return bone == -1 ? World : Mesh.WorldTransforms[bone];
        }
        /****************************************************************************/


        /****************************************************************************/
        // On Event
        /****************************************************************************/
        public override void OnEvent(EventsSender sender, EventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }
            // TODO: Wpisałem to w Merca bo w sumie średnio to się ma do zachowań
            /*************************************/
            // DropItemCommandEvent
            /*************************************/
            if (e.GetType().Equals(typeof(DropItemCommandEvent)))
            {
                DropItem();
                SendEvent(new ActionDoneEvent(), Priority.High, sender as IEventsReceiver);
            }
            /*************************************/
            // SwitchToWeaponCommandEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(SwitchToWeaponCommandEvent)))
            {
                if ((CurrentObject == null && Weapon != null))
                {
                    var weapon = Weapon;
                    StoreItem(1);
                    PlaceItem(weapon, 0);
                }
                else if (CurrentObject != null)
                {
                    var firearm = CurrentObject as Firearm;
                    if (firearm != null)
                    {
                        if (!firearm.SideArm)
                        {
                            var weapon = Weapon;
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
                            var weapon = Weapon;
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
            // SwitchToSideArmCommandEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(SwitchToSideArmCommandEvent)))
            {
                if ((CurrentObject == null && SideArm != null))
                {
                    var weapon = SideArm;
                    StoreItem(2);
                    PlaceItem(weapon, 0);
                }
                else if (CurrentObject != null)
                {
                    var firearm = CurrentObject as Firearm;
                    if (firearm != null)
                    {
                        if (firearm.SideArm)
                        {
                            var weapon = SideArm;
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
                            var weapon = SideArm;
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
            else ObjectAIController.OnEvent(sender, e);
        }
        /****************************************************************************/


        /****************************************************************************/
        // Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {
            ObjectAIController.Update(deltaTime);
        }
        /****************************************************************************/
        /// <summary>
        /// Sprawdzamy czy posiada obiekt danego typu.
        /// Przeszukuje ręce, schowek, wszystko.
        /// </summary>
        /// <param name="itemType">Typ poszukiwanego obiektu</param>
        /// <returns>
        ///   <c>true</c> jeżeli posiada obiekt danego typu; inaczej, <c>false</c>.
        /// </returns>
        public bool HasItemType(Type itemType)
        {
            if (CurrentObject != null && CurrentObject.GetType().Assembly.Equals(itemType))
            {
                return true;
            }
            if (Weapon != null && Weapon.GetType().Assembly.Equals(itemType))
            {
                return true;
            }
            if (SideArm != null && SideArm.GetType().Assembly.Equals(itemType))
            {
                return true;
            }
            foreach (var item in Items.Keys)
            {
                if (item.GetType().Assembly.Equals(itemType))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Sprawdzamy czy posiada obiekt o podanym Id.
        /// </summary>
        /// <param name="itemId">ID poszukiwanego obiektu</param>
        /// <returns>
        ///   <c>true</c> jeżli posiadny obiekt ma takie ID; inaczej, <c>false</c>.
        /// </returns>
        public bool HasItem(int itemId)
        {
            if (CurrentObject != null && CurrentObject.ID == itemId)
            {
                return true;
            }
            if (Weapon != null && Weapon.ID == itemId)
            {
                return true;
            }
            if (SideArm != null && SideArm.ID == itemId)
            {
                return true;
            }
            foreach (var item in Items.Keys)
            {
                if (item.ID == itemId)
                {
                    return true;
                }
            }
            return false;
        }
        /****************************************************************************/
        // Pick Item
        /****************************************************************************/
        public void PickItem(StorableObject item)
        {
            if (!GroupAmmo(item as AmmoBox))
            {
                FindPlaceForItem(item, true);
            }

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
                // Current
                /***************/
                case 0:
                    if (Grip != null && Mesh.BoneMap.ContainsKey(Grip))
                    {
                        CurrentObject = item;
                        item.Owner = this;
                        item.OwnerBone = Mesh.BoneMap[Grip];
                        if (item.GetType().Equals(typeof(Firearm)))
                        {
                            (item as Firearm).Switch(true);
                            (item as Firearm).OnPlacing();
                        }
                    }
#if DEBUG
                    else
                    {
                        Diagnostics.PushLog(LoggingLevel.WARN, this, "Nie ma określonej kości uchwytu Grip. Nie można przypisać przedmiotów.");
                    }
#endif
                    break;
                /***************/
                // Weapon                
                /***************/
                case 1:
                    if (WeaponGrip != null && Mesh.BoneMap.ContainsKey(WeaponGrip))
                    {
                        Weapon = item as Firearm;
                        item.Owner = this;
                        item.OwnerBone = Mesh.BoneMap[WeaponGrip];
                        if (item.GetType().Equals(typeof(Firearm)))
                        {
                            (item as Firearm).Switch(false);
                            (item as Firearm).OnPlacing();
                        }
                    }
#if DEBUG
                    else
                    {
                        Diagnostics.PushLog(LoggingLevel.WARN,this, "Nie ma określonej kości uchwytu WeaponGrip. Nie można przypisać przedmiotów.");
                    }
#endif
                    break;
                /***************/
                // Side Arm                
                /***************/
                case 2:
                    if (SideArmGrip != null && Mesh.BoneMap.ContainsKey(SideArmGrip))
                    {
                        SideArm = item as Firearm;
                        item.Owner = this;
                        item.OwnerBone = Mesh.BoneMap[SideArmGrip];
                        if (item.GetType().Equals(typeof(Firearm)))
                        {
                            (item as Firearm).Switch(false);
                            (item as Firearm).OnPlacing();
                        }
                    }
#if DEBUG
                    else
                    {
                        Diagnostics.PushLog(LoggingLevel.WARN, this, "Nie ma określonej kości uchwytu SideArmGrip. Nie można przypisać przedmiotów.");
                    }
#endif
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

                CurrentObject = null;
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
        /// Find Place For Item
        /****************************************************************************/
        public bool FindPlaceForItem(StorableObject item, bool put)
        {

            /******************************/
            // Check Weapon Slots
            /******************************/
            if (item.GetType().Equals(typeof(Firearm)))
            {
                if ((item as Firearm).SideArm)
                {
                    if (SideArm == null)
                    {
                        if(put)PlaceItem(item, 2);
                        return true;
                    }
                }
                else
                {
                    if (Weapon == null)
                    {
                        if(put)PlaceItem(item, 1);
                        return true;                    
                    }
                }
            }
            /******************************/

            int width  = item.SlotsIcon.Width/32;
            int height = item.SlotsIcon.Height/32;

            int slots = (int)(TinySlots + Slots);

            bool[,] SlotsContent = new bool[11, ((slots % 11) > 0 ? 1 : 1) + slots / 11];

            foreach (var pair in Items)
            {
                List<int> itemSlots = Inventory.CalculateSlots(pair.Key, pair.Value.Slot, pair.Value.Orientation, true);
                foreach (int slot in itemSlots)
                {
                    SlotsContent[slot % 11, slot / 11] = true;
                }
            }

            bool blocked = false;

            /******************/
            #region Tiny Slots
            /******************/
            if (width < 3 && height < 3)
            {
                for (int i = 0; i < TinySlots; i++)
                {
                    blocked = false;

                    // Normal Orientation
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            /* C# w swej pełnej wspaniałości, ma pewną wadę
                             * skurwysyn nie zatrzymuje się przy sprawdzaniu 
                             * warunków tylko leci wszytskie, nawet jeżeli 
                             * dane zadanie już jet prawdziwe/nieprawdziwe.
                             */

                            if (i + x + (y * 11) >= TinySlots ||
                                (i % 11) + x > 10)
                            {
                                blocked = true;
                                break;                           
                            }
                            else if (SlotsContent[(i + x) % 11, (i/ 11) + y])
                            {
                                blocked = true;
                                break;
                            }
                            
                            /* I właśnie taka sytuacja, normalnie w cywilizowanym
                             * jezyku, te dwa warunki wsadziłoby się w jeden 'if'
                             * i nie było by porblemu z tym ze cwaniak przekroczy 
                             * zakres tablicy. Tutaj jednak, wpierw muszę sprawdzić 
                             * czy nie przekroczył i dopiero potem zastanawiać się
                             * nad resztą.
                             */
                        }
                        if (blocked) break;
                    }

                    if (!blocked)
                    {
                        if (put)
                        {
                            item.OnStoring();
                            Items.Add(item,new ItemPosition(i,1));
                            if (UpdateInventory != null) UpdateInventory();
                        }

                        return true;
                    }

                    blocked = false;

                    // Rotated
                    for (int x = 0; x < height; x++)
                    {
                        for (int y = 0; y < width; y++)
                        {
                            if (i + x + (y * 11) >= TinySlots ||
                                (i % 11) + x > 10)
                            {
                                blocked = true;
                                break;
                            }
                            else if (SlotsContent[(i + x) % 11, (i/11) + y])
                            {
                                blocked = true;
                                break;
                            }
                        }
                        if (blocked) break;
                    }

                    if (!blocked)
                    {
                        if (put)
                        {
                            item.OnStoring();
                            Items.Add(item, new ItemPosition(i, -1));
                            if (UpdateInventory != null) UpdateInventory();
                        }

                        return true;
                    }
                }
            }
            /******************/
            #endregion
            /******************/
            #region Slots
            /******************/
            for (int i = (int)TinySlots; i < Slots + TinySlots; i++)
            {
                blocked = false;

                // Normal Orientation
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (i + x + (y * 11) >= Slots + TinySlots ||
                            (i % 11) + x > 10)
                        {
                            blocked = true;
                            break;
                        }
                        else if (SlotsContent[(i + x) % 11, (i/11) + y])
                        {
                            blocked = true;
                            break;
                        }
                    }
                    if (blocked) break;
                }

                if (!blocked)
                {
                    if (put)
                    {
                        item.OnStoring();
                        Items.Add(item, new ItemPosition(i, 1));
                        if (UpdateInventory != null) UpdateInventory();
                    }

                    return true;
                }

                blocked = false;

                // Rotated
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        if (i + x + (y * 11) >= Slots + TinySlots ||
                            (i % 11) + x > 10)
                        {
                            blocked = true;
                            break;
                        }
                        else if (SlotsContent[(i + x) % 11, (i/11) + y])
                        {
                            blocked = true;
                            break;
                        }
                    }
                    if (blocked) break;
                }

                if (!blocked)
                {
                    if (put)
                    {
                        item.OnStoring();
                        Items.Add(item, new ItemPosition(i, -1));
                        if (UpdateInventory != null) UpdateInventory();
                    }

                    return true;
                }
            }
            /******************/
            #endregion
            /******************/


            /******************************/
            /// Check Current Item
            /******************************/
            if (CurrentObject == null)
            {
                if(put) PlaceItem(item, 0);
                return true;
            }
            /******************************/

            return false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GroupAmmo
        /****************************************************************************/
        public bool GroupAmmo(AmmoBox ammo)
        {
            if(ammo == null) return false;
            if (ammo.Amount == ammo.Capacity) return false;

            if (CurrentObject != null)
            {
                if (CurrentObject.GetType().Equals(typeof(AmmoBox)))
                {
                    AmmoBox ammoBox = CurrentObject as AmmoBox;

                    if (ammoBox.AmmunitionVersionInfo == ammoBox.AmmunitionVersionInfo &&
                        ammoBox.PPP == ammo.PPP &&
                        ammoBox.Amount < ammoBox.Capacity)
                    {
                        int diff = ammoBox.Capacity - ammoBox.Amount;

                        ammoBox.Amount += ammo.Amount > diff ? diff : ammo.Amount;
                        ammo.Amount -= diff;
                        if (ammo.Amount <= 0)
                        {
                            SendEvent(new DestroyObjectEvent(ammo.ID), Priority.High, GlobalGameObjects.GameController);
                            return true;
                        }
                    }
                }
            }

            foreach (var item in Items)
            {                 
                if(item.Key.GetType().Equals(typeof(AmmoBox)))
                {
                    AmmoBox ammoBox = item.Key as AmmoBox;

                    if (ammoBox.AmmunitionVersionInfo == ammo.AmmunitionVersionInfo &&
                        ammoBox.PPP                   == ammo.PPP                   &&
                        ammoBox.Amount                 < ammoBox.Capacity)
                    {
                        int diff = ammoBox.Capacity - ammoBox.Amount;

                        ammoBox.Amount += ammo.Amount > diff ? diff : ammo.Amount;
                        ammo.Amount -= diff;
                        if (ammo.Amount <= 0)
                        {
                            SendEvent(new DestroyObjectEvent(ammo.ID), Priority.High, GlobalGameObjects.GameController);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        /****************************************************************************/


        /****************************************************************************/
        // MarkerDraw
        /****************************************************************************/
        public void MarkerDraw(SpriteBatch spriteBatch, ref Matrix viewProjection, int screenWidth, int screenHeight)
        {
            if (IsDisposed) return;
            if (!Marker) return;
            Vector2 pos2;

            var position = Vector4.Transform(Vector3.Transform(_markerLocalPosition, World), viewProjection);

            pos2.X = MathHelper.Clamp(0.5f * ((position.X / Math.Abs(position.W)) + 1.0f), 0.01f, 0.99f);
            pos2.X *= screenWidth;
            pos2.X -= _marker.Texture.Width / 2;

            pos2.Y = MathHelper.Clamp(1.0f - (0.5f * ((position.Y / Math.Abs(position.W)) + 1.0f)), 0.01f, 0.99f);
            pos2.Y *= screenHeight;
            pos2.Y -= _marker.Texture.Height / 2;

            spriteBatch.Draw(_marker.Texture, pos2, Color.White);
        }
        /****************************************************************************/


        /****************************************************************************/
        // Release Components
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

            if (_marker != null)
            {
                _marker.ReleaseMe();
                _marker = null;
            }


        }
        /****************************************************************************/


        /****************************************************************************/
        // Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            var data = new MercenaryData();
            GetData(data);

            data.Model = Mesh.Model.Name;

            data.Diffuse = (Mesh.Textures.Diffuse == null ? String.Empty : Mesh.Textures.Diffuse.Name);
            data.Specular = (Mesh.Textures.Specular == null ? String.Empty : Mesh.Textures.Specular.Name);
            data.Normals = (Mesh.Textures.Normals == null ? String.Empty : Mesh.Textures.Normals.Name);

            data.TimeRatio = Mesh.TimeRatio;
            data.CurrentClip = (Mesh.currentAnimation.Clip == null ? String.Empty : Mesh.currentAnimation.Clip.Name);
            data.CurrentTime = Mesh.currentAnimation.ClipTime.TotalSeconds;
            data.CurrentKeyframe = Mesh.currentAnimation.Keyframe;
            data.Pause = Mesh.Pause;

            data.Blend = Mesh.Blend;
            data.BlendDuration = Mesh.BlendDuration.TotalSeconds;
            data.BlendTime = Mesh.BlendTime.TotalSeconds;
            data.BlendClip = (Mesh.blendAnimation.Clip == null ? String.Empty : Mesh.blendAnimation.Clip.Name);
            data.BlendClipTime = Mesh.blendAnimation.ClipTime.TotalSeconds;
            data.BlendKeyframe = Mesh.blendAnimation.Keyframe;

            data.Immovable = Body.Immovable;
            data.Elasticity = Body.Elasticity;
            data.StaticRoughness = Body.StaticRoughness;
            data.DynamicRoughness = Body.DynamicRoughness;
            data.Mass = Body.Mass;
            data.Translation = Body.SkinTranslation;
            data.SkinPitch = Body.Pitch;
            data.SkinRoll = Body.Roll;
            data.SkinYaw = Body.Yaw;

            data.Radius = Body.Radius;
            data.Length = Body.Length;

            data.MarkerPosition = _markerLocalPosition;
            data.Grip = Grip;
            data.SideArmGrip = SideArmGrip;
            data.WeaponGrip = WeaponGrip;

            data.HP = (ObjectAIController).HP;
            data.MaxHP = (ObjectAIController).MaxHP;
            data.Icon = Icon;
            data.InventoryIcon = InventoryIcon;

            data.TinySlots = TinySlots;
            data.Slots = Slots;
            data.MovingSpeed = ObjectAIController.MovingSpeed;
            data.RotationSpeed = ObjectAIController.RotationSpeed;
            data.DistancePrecision = ObjectAIController.DistancePrecision;
            data.AnglePrecision = ObjectAIController.AnglePrecision;


            data.CurrentItem = CurrentObject == null ? 0 : CurrentObject.ID;
            data.SideArm = SideArm == null ? 0 : SideArm.ID;
            data.Weapon = Weapon == null ? 0 : Weapon.ID;

            data.Items = new List<int[]>();

            foreach (var item in Items)
            {
                data.Items.Add(new[] { item.Key.ID, item.Value.Slot, item.Value.Orientation });
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
            return new[] { "Inventory", "Follow", "Exchange Items" };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public string[] GetActions(Mercenary mercenary)
        {
            var actions = new List<String> { "Inventory" };

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
                    var firearm = CurrentObject as Firearm;
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


        /****************************************************************************/
        /// Reload
        /****************************************************************************/
        public void Reload()
        {
            if (CurrentObject as Firearm == null) return;
            foreach (var item in Items)
            {
                if (item.Key as AmmoClip != null && item.Value.Slot < TinySlots)
                {
                    if ((item.Key as AmmoClip).Compability.Contains(CurrentObject.Name) &&
                        (item.Key as AmmoClip).Content.Count > 0)
                    {
                        Items.Remove(item.Key);
                        Firearm firearm = CurrentObject as Firearm;
                        AmmoClip ammoClip = firearm.DetachClip();
                        firearm.AttachClip(item.Key as AmmoClip);
                        FindPlaceForItem(ammoClip, true);
                        return;
                    }
                }
            }
        }
        /****************************************************************************/

        public void Recoil(float recoil)
        { 
            // TODO: Efekt na odrzut
            Mesh.BlendTo("Fire_Carabine", TimeSpan.FromSeconds(0.1f));
        }

        public float GetAccuracyModulation(Firearm firearm)
        {
            // TODO: Obsłużyć to
            return 1;
        }

        public float GetArmorClass()
        {
            return 1;
        }

        public void OnShoot(float damage, float stoppingPower, Vector3 position, Vector3 direction)
        {
            ObjectAIController.OnEvent(null, new ArtificialIntelligence.TakeDamage(damage, null));
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// Mercenary Data
    /********************************************************************************/
    [Serializable]
    public class MercenaryData : AbstractLivingBeingData
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
        public float RotationSpeed { get; set; }
        [CategoryAttribute("Movement")]
        public float MovingSpeed { get; set; }
        [CategoryAttribute("Movement")]
        public float DistancePrecision { get; set; }
        [CategoryAttribute("Movement")]
        public float AnglePrecision { get; set; }

        [CategoryAttribute("Grips")]
        public String Grip { get; set; }
        [CategoryAttribute("Grips")]
        public String SideArmGrip { get; set; }
        [CategoryAttribute("Grips")]
        public String WeaponGrip { get; set; }

        [CategoryAttribute("HP")]
        public uint MaxHP { get; set; }
        [CategoryAttribute("HP")]
        public uint HP { get; set; }

        [CategoryAttribute("Icon")]
        public Rectangle Icon { get; set; }
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