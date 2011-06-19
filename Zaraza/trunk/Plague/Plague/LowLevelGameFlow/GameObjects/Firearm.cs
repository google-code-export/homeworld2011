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
using PlagueEngine.ArtificialIntelligence.Controllers;
using PlagueEngine.TimeControlSystem;
using PlagueEngine.Particles.Components;
using PlagueEngine.Audio.Components;
using JigLibX.Collision;


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
        public PointLightComponent          light   = null;
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
        private bool isOn = false;

        private Dictionary<uint, AmmunitionVersionInfo> AmmunitionVersionData;

        public SoundEffectComponent sounds;
        private Vector3 FireOffset;
        private float MaxDispersion;
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
                         PointLightComponent light,
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
                         Vector3             ammoClipTranslation,
                         bool                on,
                         Dictionary<uint, AmmunitionVersionInfo> ammunitionVersionData,
                         Vector3 fireOffset,
                         float maxDispersion)
        {
            this.mesh = mesh;
            this.body = body;
            this.light = light;
            //this.attacks = attacks;
            isOn = on;
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

            AmmunitionVersionData = ammunitionVersionData;
            Ammunition = ammunitionInfo;
            AmmoClip   = ammoClip;
            AmmoClipTranslation = ammoClipTranslation;

            Accessories = accessories;
            FireOffset = fireOffset;
            MaxDispersion = MathHelper.ToRadians(maxDispersion);
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

            sounds = new SoundEffectComponent();
            sounds.LoadFolder("Firearms", 0.6f, 0, 0,true);
            
            Init(icon, slotsIcon, description, descriptionWindowWidth, descriptionWindowHeight, particle);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Fire
        /****************************************************************************/
        public bool Fire()
        {
            return SingleFireshot();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Single Fireshot
        /****************************************************************************/
        private bool SingleFireshot()
        {
            if (AmmoClip == null)
            {
                sounds.PlaySound("Firearms", "DryFire");
                return false;
            }
            else if (AmmoClip.Content.Count == 0)
            {
                sounds.PlaySound("Firearms", "DryFire");
                return false;
            }
            else
            {
                sounds.PlaySound("Firearms", "Fireshot");
                light.Enabled = true;                                
                
                AmmunitionVersionInfo bulletInfo = AmmunitionVersionData[AmmoClip.Content.Pop().Version];
                Matrix world = GetWorld();
                world = Matrix.CreateTranslation(FireOffset) * world;
                Vector3 position = world.Translation;
                float dist;
                CollisionSkin skin;
                Vector3 pos, nor;
                Random random = new Random();
                
                Vector3 dispersion = new Vector3((float)(2 * random.NextDouble() - 1) * (float)(MaxDispersion / Math.PI) * Math.Max(1 - bulletInfo.Accuracy * AccuracyModulation, 0),
                                                 (float)(2 * random.NextDouble() - 1) * (float)(MaxDispersion / Math.PI) * Math.Max(1 - bulletInfo.Accuracy * AccuracyModulation, 0),
                                                 -1);                                        
                
                dispersion.Normalize();                
                world.Translation = Vector3.Zero;
                dispersion = Vector3.Transform(dispersion, world);
                dispersion.Normalize();

                PhysicsUlitities.RayTest(position,
                                         position + bulletInfo.Range * RangeModulation * dispersion,
                                         out dist,
                                         out skin,
                                         out pos,
                                         out nor);

                if (skin != null)
                {
                    if(skin.ExternalData != null)
                    {
                        Diagnostics.PushLog(LoggingLevel.INFO, skin.ExternalData.ToString() + " | " + pos.ToString() + " | " + dist.ToString());
                    }

                    if ((skin.ExternalData as IShootable) != null)
                    {
                        (skin.ExternalData as IShootable).OnShoot(bulletInfo.Damage        * DamageModulation, 
                                                                  bulletInfo.StoppingPower * StoppingPowerModulation);

                        if (skin.ExternalData as IPenetrable != null)
                        {
                            (skin.ExternalData as IPenetrable).GetArmorClass();
                        }

                        if (bulletInfo.Version == 7 && (skin.ExternalData as IFlammable) != null) 
                        {
                            (skin.ExternalData as IFlammable).SetOnFire();
                        }
                        
                    }
                }

                TimeControl.CreateTimer(TimeSpan.FromSeconds(0.1f), 0, delegate() { light.Enabled = false; });
                return true;
            }
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

            for (int i = 0; i < Accessories.Length; i++)
            {
                if (Accessories[i].Accessory != null)
                {
                    AddAccessoryModulation(Accessories[i].Accessory);
                }
            }

            data.AmmoClip       = AmmoClip == null ? 0 : AmmoClip.ID;
            data.AmmunitionName = Ammunition == null ? String.Empty: Ammunition.Name;

            data.SelectiveFireMode = SelectiveFireMode;
            data.SelectiveFire = SelectiveFire;

            data.AmmoClipTranslation = AmmoClipTranslation;
            data.On = isOn;

            
            data.Color = light.Color;
            data.LightRadius = light.Radius;
            data.LinearAttenuation = light.LinearAttenuation;
            data.QuadraticAttenuation = light.QuadraticAttenuation;
            data.Intensity = light.Intensity;
            data.LightLocalPoistion = light.LocalPosition;

            data.MaxDispersion = MathHelper.ToDegrees(MaxDispersion);
            data.FireOffset = FireOffset;

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

            if (light != null)
            {
                light.ReleaseMe();
                light = null;
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

            foreach (var accessory in Accessories)
            {
                if (accessory.Accessory != null) accessory.Accessory.OnOwnerStoring();
            }

            if (AmmoClip != null)
            {
                AmmoClip.body.DisableBody();
                AmmoClip.mesh.Enabled = false;
            }

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

            Switch(true);

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
        /// On Placing
        /****************************************************************************/
        public virtual void OnPlacing()
        {
            foreach (var accessory in Accessories)
            {
                if (accessory.Accessory != null)
                {
                    accessory.Accessory.OnOwnerPlacing();
                }
            }

            if (AmmoClip != null)
            {
                AmmoClip.body.DisableBody();
                AmmoClip.mesh.Enabled = true;
            }
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
            accessory.Switch(isOn);
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
        public AmmoClip DetachClip()
        {
            AmmoClip result = AmmoClip;
            AmmoClip.OnStoring();            
            AmmoClip = null;
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Switch
        /****************************************************************************/
        public void Switch(bool on)
        {
            foreach (var accessory in Accessories)
            {
                if (accessory.Accessory != null)
                {
                    accessory.Accessory.Switch(on);
                }
            }
            isOn = on;
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

        [CategoryAttribute("Firearm Parameters")]
        public Vector3 FireOffset { get; set; }

        [CategoryAttribute("Firearm Parameters")]
        public float MaxDispersion { get; set; }


        [CategoryAttribute("Ammo Clip")]
        public int AmmoClip { get; set; }
        [CategoryAttribute("Ammo Clip")]
        public Vector3 AmmoClipTranslation { get; set; }

        [CategoryAttribute("Firearm Parameters")]
        public String AmmunitionName { get; set; }

        [CategoryAttribute("Misc")]
        public bool On { get; set; }


        [CategoryAttribute("Light")]
        public Vector3 Color { get; set; }
        [CategoryAttribute("Light")]
        public float Intensity { get; set; }
        [CategoryAttribute("Light")]
        public float LightRadius { get; set; }
        [CategoryAttribute("Light")]
        public float LinearAttenuation { get; set; }
        [CategoryAttribute("Light")]
        public float QuadraticAttenuation { get; set; }
        [CategoryAttribute("Light")]
        public Vector3 LightLocalPoistion { get; set; }
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
