﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;
using PlagueEngine.ArtificialIntelligence.Controllers;

using PlagueEngine.Particles.Components;

// TODO: Akcesoria

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Firearm
    /********************************************************************************/
    class Firearm : StorableObject
    {   

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public MeshComponent                mesh    = null;
        public SquareBodyComponent          body    = null;
        //public List<Attack>                 attacks = null;
        
        public AmmunitionInfo Ammunition { get; private set; }
        public AmmoClip       AmmoClip = null;
        private Vector3 AmmoClipTranslation;

        public float Condition      { get; private set; }
        public float Reliability    { get; private set; }
        public float RateOfFire     { get; private set; }
        public float Ergonomy       { get; private set; }
        public float ReloadingTime  { get; private set; }

        public float DamageModulation        { get; private set; }
        public float AccuracyModulation      { get; private set; }
        public float RangeModulation         { get; private set; }
        public float PenetrationModulation   { get; private set; }
        public float RecoilModulation        { get; private set; }
        public float StoppingPowerModulation { get; private set; }

        public AttachedAccessory[] Accessories = new AttachedAccessory[0];

        public List<int> SelectiveFire     { get; private set; }
        public int       SelectiveFireMode { get; set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public bool SideArm { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent       mesh,
                         SquareBodyComponent body,
                         AttachedAccessory[] accessories,
                         Rectangle           icon,
                         Rectangle           slotsIcon,
                         String              description,
                         int                 descriptionWindowWidth,
                         int                 descriptionWindowHeight,
                         bool                sideArm,
                         List<int>           selectiveFire,
                         int                 selectiveFireMode,
                         ParticleEmitterComponent particle,
                         float               condition,
                         float               reliability,
                         float               rateOfFire,
                         float               ergonomy,
                         float               reloadingTime,
                         float               damageModulation,
                         float               accuracyModulation,
                         float               rangeModulation,
                         float               penetrationModulation,
                         float               recoilModulation,
                         float               stoppingPowerModulation,
                         AmmoClip            ammoClip,
                         AmmunitionInfo      ammunitionInfo,
                         Vector3             ammoClipTranslation)
        {
            this.mesh = mesh;
            this.body = body;

            //this.attacks = attacks;

            Condition               = condition;
            Reliability             = reliability;
            RateOfFire              = rateOfFire;
            Ergonomy                = ergonomy;
            ReloadingTime           = reloadingTime;
            DamageModulation        = damageModulation;
            AccuracyModulation      = accuracyModulation;
            RangeModulation         = rangeModulation;
            PenetrationModulation   = penetrationModulation;
            RecoilModulation        = recoilModulation;
            StoppingPowerModulation = stoppingPowerModulation;

            Ammunition = ammunitionInfo;
            AmmoClip   = ammoClip;
            AmmoClipTranslation = ammoClipTranslation;

            Accessories = accessories;

            SelectiveFire     = selectiveFire;
            SelectiveFireMode = selectiveFireMode;
            
            //if (SelectiveFireMode == 0) SelectiveFireMode = SelectiveFire.ElementAt(0);

            SideArm = sideArm;

            if (!body.Immovable)
            {
                body.SubscribeCollisionEvent(typeof(Terrain));
            }
                        
            for (int i = 0; i < Accessories.Length; i++)
            {
                if (Accessories[i].Accessory != null)
                {
                    AddAccessoryModulation(Accessories[i].Accessory);
                }
            }

            Init(icon, slotsIcon, description, descriptionWindowWidth, descriptionWindowHeight, particle);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Owning
        /****************************************************************************/
        protected override void OnOwning(GameObjectInstance owner)
        {
            if (owner != null)
            {
                World = Matrix.Identity;
                World *= Matrix.CreateRotationY(MathHelper.ToRadians(-90));
                if (mesh != null) mesh.Enabled = true;
                if (body != null) body.DisableBody();
            }
            else
            {
                if (getWorld != null) World = GetWorld();
                if (body     != null) body.EnableBody();
                if (mesh     != null) mesh.Enabled = true;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetData
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            FirearmData data = new FirearmData();
            GetData(data);
            data.Model    = mesh.Model.Name;
            data.Diffuse  = (mesh.Textures.Diffuse  == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals  = (mesh.Textures.Normals  == null ? String.Empty : mesh.Textures.Normals.Name);

            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);

            //data.Attacks = this.attacks;

            data.Mass             = body.Mass;
            data.Elasticity       = body.Elasticity;
            data.StaticRoughness  = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Lenght           = body.Length;
            data.Width            = body.Width;
            data.Height           = body.Height;
            data.Immovable        = body.Immovable;
            data.Translation      = body.SkinTranslation;
            data.SkinPitch        = body.Pitch;
            data.SkinRoll         = body.Roll;
            data.SkinYaw          = body.Yaw;
            data.EnabledPhysics   = body.Enabled;
            data.EnabledMesh      = mesh.Enabled;
            
            data.SideArm          = SideArm;

            List<AccessoryInfo> AvailableAccessories = new List<AccessoryInfo>();

            for (int i = 0; i < Accessories.Length; i++)
            {
                AccessoryInfo info = new AccessoryInfo();
                info.AccessoryID = (Accessories[i].Accessory == null ? 0 : Accessories[i].Accessory.ID);
                info.Genre       = Accessories[i].Genre;
                info.Translation = Accessories[i].Translation;
                AvailableAccessories.Add(info);
                if (Accessories[i].Accessory != null)
                {
                    SubAccessoryModulation(Accessories[i].Accessory);
                }
            }

            data.AvailableAccessories = AvailableAccessories;

            data.Condition      = Condition;
            data.Reliability    = Reliability;
            data.RateOfFire     = RateOfFire;
            data.Ergonomy       = Ergonomy;
            data.ReloadingTime  = ReloadingTime;

            data.DamageModulation        = DamageModulation;
            data.AccuracyModulation      = AccuracyModulation;
            data.RangeModulation         = RangeModulation;
            data.PenetrationModulation   = PenetrationModulation;
            data.RecoilModulation        = RecoilModulation;
            data.StoppingPowerModulation = StoppingPowerModulation;

            data.AmmoClip       = AmmoClip == null ? 0 : AmmoClip.ID;
            data.AmmunitionName = Ammunition == null ? String.Empty: Ammunition.Name;

            data.SelectiveFireMode = SelectiveFireMode;
            data.SelectiveFire = SelectiveFire;

            data.AmmoClipTranslation = AmmoClipTranslation;

            return data;
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if(e.GetType().Equals(typeof(CollisionEvent)))
            {
                CollisionEvent CollisionEvent = e as CollisionEvent;
                if (CollisionEvent.gameObject.GetType().Equals(typeof(Terrain)))
                {
                    body.Immovable = true;
                    body.CancelCollisionWithGameObjectsType(typeof(Terrain));
                }
            }
            else
            {
                base.OnEvent(sender, e);
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

            base.ReleaseComponents();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnStoring
        /****************************************************************************/
        public override void OnStoring()
        {
            body.DisableBody();
            mesh.Enabled = false;

            base.OnStoring();
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// On Dropping
        /****************************************************************************/
        public override void OnDropping()
        {
            body.EnableBody();
            mesh.Enabled = true;
            
            body.Immovable = false;
            body.SubscribeCollisionEvent(typeof(Terrain));
            
            base.OnDropping();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Picking
        /****************************************************************************/
        public override void OnPicking()
        {
            mesh.Enabled = true;
            body.DisableBody();

            base.OnPicking();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// AttachedAccessory
        /****************************************************************************/
        public struct AttachedAccessory
        {
            public Accessory Accessory   { get; set; }
            public String    Genre       { get; set; }
            public Vector3   Translation { get; set; }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Add Accessory Modulation
        /****************************************************************************/
        private void AddAccessoryModulation(Accessory accessory)
        { 
            DamageModulation        += accessory.DamageModulation;
            AccuracyModulation      += accessory.AccuracyModulation;
            RangeModulation         += accessory.RangeModulation;
            PenetrationModulation   += accessory.PenetrationModulation;
            RecoilModulation        += accessory.RecoilModulation;
            StoppingPowerModulation += accessory.StoppingPowerModulation;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Sub Accessory Modulation
        /****************************************************************************/
        private void SubAccessoryModulation(Accessory accessory)
        {
            DamageModulation        -= accessory.DamageModulation;
            AccuracyModulation      -= accessory.AccuracyModulation;
            RangeModulation         -= accessory.RangeModulation;
            PenetrationModulation   -= accessory.PenetrationModulation;
            RecoilModulation        -= accessory.RecoilModulation;
            StoppingPowerModulation -= accessory.StoppingPowerModulation;        
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Attach Accessory
        /****************************************************************************/
        public void AttachAccessory(Accessory accessory,int accessorySlot)
        {
            accessory.OnAttach(this, Accessories[accessorySlot].Translation);
            Accessories[accessorySlot].Accessory = accessory;
            AddAccessoryModulation(accessory);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Detach Accessory
        /****************************************************************************/
        public void DetachAccessory(int accessorySlot)
        {
            Accessories[accessorySlot].Accessory.OnStoring();
            SubAccessoryModulation(Accessories[accessorySlot].Accessory);
            Accessories[accessorySlot].Accessory = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Attach Clip
        /****************************************************************************/
        public void AttachClip(AmmoClip clip)
        {
            clip.OnAttach(this, AmmoClipTranslation);
            AmmoClip = clip;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Detach Clip
        /****************************************************************************/
        public void DetachClip()
        {
            AmmoClip.OnStoring();
            AmmoClip = null;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// FirearmData
    /********************************************************************************/
    [Serializable]
    public class FirearmData : StorableObjectData
    {

        public FirearmData()
        {
            Type = typeof(Firearm);
            AvailableAccessories = new List<AccessoryInfo>();
            SelectiveFire = new List<int>();
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

        //[CategoryAttribute("Possible Attacks")]
        //public List<Attack> Attacks { get; set; }

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

        [CategoryAttribute("Weapon Genre")]
        public bool SideArm { get; set; }

        [CategoryAttribute("Accessories")]
        public List<AccessoryInfo> AvailableAccessories { get; set; }


        [CategoryAttribute("Firearm Parameters")]
        public float Condition { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float Reliability { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float RateOfFire { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float Ergonomy { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float ReloadingTime { get; set; }

        [CategoryAttribute("Firearm Parameters")]
        public float DamageModulation { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float AccuracyModulation { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float RangeModulation { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float PenetrationModulation { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float RecoilModulation { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float StoppingPowerModulation { get; set; }

        [CategoryAttribute("Firearm Parameters")]
        public List<int> SelectiveFire { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public int SelectiveFireMode { get; set; }

        [CategoryAttribute("Ammo Clip")]
        public int AmmoClip { get; set; }
        [CategoryAttribute("Ammo Clip")]
        public Vector3 AmmoClipTranslation { get; set; }

        [CategoryAttribute("Firearm Parameters")]
        public String AmmunitionName { get; set; }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// AccessoryInfo
    /********************************************************************************/
    [Serializable]
    public class AccessoryInfo
    {
        public int     AccessoryID { get; set; }
        public String  Genre       { get; set; }
        public Vector3 Translation { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/
