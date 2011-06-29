using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;


using PlagueEngine.Audio.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;
using PlagueEngine.TimeControlSystem;

using PlagueEngine.HighLevelGameFlow;
using System.Collections.Generic;

using PlagueEngine.Particles.Components;
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class BoomTriger : ActivableObject
    {

        /****************************************************************************/
        // Fields
        /****************************************************************************/
        public SquareBodyComponent body;
        private int _keyId = -1;
        int[] stones;
        Level level;
        float timer;

        public float explosionForce;
        public float explosionRadius;
        public Vector3 explosionPosition;
        Mercenary merc;
        StorableObject objectToDestroy;
        int bombId;
        SoundEffectComponent sounds;

        ParticleEmitterComponent emitter;
        /****************************************************************************/


        
        /****************************************************************************/
        // Init
        /****************************************************************************/
        public void Init(
                         SquareBodyComponent body,
                         int keyID,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight,
                         int[] stones,
                         Level level,
                         float timer,
                        float explosionForce,
                        float explosionRadius,
                        Vector3 explosionPosition,
                        ParticleEmitterComponent emitter)
        {
          
            this.body = body;
            _keyId = keyID;
            activationRecievers = new int[0];
            Init(activationRecievers, description, descriptionWindowWidth, descriptionWindowHeight);
            this.level = level;
            this.stones = stones;
            this.timer = timer;
            this.explosionRadius = explosionRadius;
            this.explosionForce = explosionForce;
            this.explosionPosition = explosionPosition;

            sounds = new SoundEffectComponent();
            sounds.LoadFolder("Misc", 1, 0, 0);


            this.emitter = emitter;
            //emitter.EnableEmitter();
            emitter.DisableEmitter();
        }
        /****************************************************************************/

        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public override string[] GetActions(Mercenary mercenary)
        {

            if (_keyId != -1)
            {
                if (mercenary.HasItem(_keyId))
                {
                    merc = mercenary;
                    return new[] { "Examine", "Activate" };
                    
                }
            }
            return new[] { "Examine" };
            
        }
        /****************************************************************************/

        private void remove()
        {
            this.SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.Normal, GlobalGameObjects.GameController);
           
        }

        private void removeParticles()
        {
            //emitter.DisableEmitter();
        }
        private void DoBoom()
        {
            this.body.DisableBody();
            foreach (var stone in stones)
            {
                SphericalBodyMesh stoneGO = (SphericalBodyMesh)level.GameObjects[stone];
                stoneGO.body.Immovable = false;
            }
            sounds.StopAllSounds();
            ExplosionManager.CreateExplosion(explosionPosition, explosionForce, explosionRadius);
            sounds.SetPosition(this.World.Translation);
            sounds.PlaySound("Misc", "Bomb", false, 200);
            body.dontCollide = true;
            TimeControl.CreateTimer(TimeSpan.FromSeconds(5), 1, remove);
            TimeControl.CreateTimer(TimeSpan.FromSeconds(0.5f), 1, removeParticles);
            emitter.EnableEmitter();
            this.SendEvent(new DestroyObjectEvent(bombId), EventsSystem.Priority.Normal, GlobalGameObjects.GameController);
           
        }


        /****************************************************************************/
        /// On Action
        /****************************************************************************/
        protected override void OnActivation()
        {
            sounds.SetPosition(this.World.Translation);
            sounds.PlaySound("Misc", "Clock", true, 50);

            TimeControl.CreateTimer(TimeSpan.FromSeconds(timer), 1, DoBoom);
            CreateBomb();

            if (merc != null)
            {

                foreach (var item in merc.Items.Keys)
                {
                    if (item.ID == _keyId)
                    {
                        objectToDestroy = item;
                    }
                }

                if (objectToDestroy != null)
                {
                    merc.Items.Remove(objectToDestroy);
                }
            }

        }


        private void CreateBomb()
        {
            StaticMeshData data = new StaticMeshData();
            data.EnabledMesh = true;
            data.Type = typeof(StaticMesh);
            data.World = Matrix.CreateTranslation(new Vector3(282.1997f, 61.11f, 210.139984f));
            data.Model = @"Misc\timeBomb";
            data.Diffuse = @"Misc\timeBomb.diff";
            data.Normals = @"Misc\timeBomb.norm";
            bombId= level.GameObjectsFactory.Create(data).ID;
            //SendEvent(new CreateObjectEvent(data), EventsSystem.Priority.Normal, GlobalGameObjects.GameController);
        }


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            sounds.ReleaseMe();
            emitter.ReleaseMe();
 
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
            var data = new BoomTrigerData();
            GetData(data);
     


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
            data.timer = timer;

            data.keyId = _keyId;
            data.Stones = stones;

            data.explosionForce = explosionForce;
            data.explosionPosition = explosionPosition;
            data.explosionRadius = explosionRadius;



            if (emitter.particleSystem.settings.BlendState == BlendState.Additive)
            {
                data.BlendState = 1;
            }
            else if (emitter.particleSystem.settings.BlendState == BlendState.AlphaBlend)
            {
                data.BlendState = 2;
            }
            else if (emitter.particleSystem.settings.BlendState == BlendState.NonPremultiplied)
            {
                data.BlendState = 3;
            }
            else if (emitter.particleSystem.settings.BlendState == BlendState.Opaque)
            {
                data.BlendState = 4;
            }

            data.Duration = emitter.particleSystem.settings.DurationInSeconds;
            data.DurationRandomnes = emitter.particleSystem.settings.DurationRandomness;
            data.EmitterVelocitySensitivity = emitter.particleSystem.settings.EmitterVelocitySensitivity;
            data.VelocityEnd = emitter.particleSystem.settings.EndVelocity;
            data.Gravity = emitter.particleSystem.settings.Gravity;
            data.ColorMax = emitter.particleSystem.settings.MaxColor;
            data.EndSizeMax = emitter.particleSystem.settings.MaxEndSize;
            data.VelocityHorizontalMax = emitter.particleSystem.settings.MaxHorizontalVelocity;
            data.ParticlesMax = emitter.particleSystem.settings.MaxParticles;
            data.RotateSpeedMax = emitter.particleSystem.settings.MaxRotateSpeed;
            data.StartSizeMax = emitter.particleSystem.settings.MaxStartSize;
            data.VelocityVerticalMax = emitter.particleSystem.settings.MaxVerticalVelocity;
            data.ColorMin = emitter.particleSystem.settings.MinColor;
            data.EndSizeMin = emitter.particleSystem.settings.MinEndSize;
            data.VelocityHorizontalMin = emitter.particleSystem.settings.MinHorizontalVelocity;
            data.RotateSpeedMin = emitter.particleSystem.settings.MinRotateSpeed;
            data.StartSizeMin = emitter.particleSystem.settings.MinStartSize;
            data.VelocityVerticalMin = emitter.particleSystem.settings.MinVerticalVelocity;
            data.ParticleTexture = emitter.particleSystem.settings.TextureName;
            data.ParticlesPerSecond = emitter.particlesPerSecond;
            data.EmitterTranslation = emitter.particleTranslation;
            data.ParticlesEnabled = emitter.enabled;
            data.Technique = emitter.particleSystem.settings.Technique;



            return data;
        }
        /****************************************************************************/


    }



    /********************************************************************************/
    /// BoomTrigerData
    /********************************************************************************/
    [Serializable]
    public class BoomTrigerData : ActivableObjectData
    {
        public BoomTrigerData()
        {
            Type = typeof(BoomTriger);
            
        }


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

        public  int []Stones { get; set; }
        public int keyId { get; set; }
        public float timer { get; set; }

        public float explosionForce { get; set; }
        public float explosionRadius { get; set; }
        public Vector3 explosionPosition { get; set; }




        [CategoryAttribute("Particles")]
        public bool ParticlesEnabled { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("How long these particles will last.")]
        public double Duration { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("If greater than zero, some particles will last a shorter time than others")]
        public float DurationRandomnes { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("Translation against gameObject position.")]
        public Vector3 EmitterTranslation { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Alpha blending settings.1 - Additive, 2 - AlphaBlend, 3 - NonPremultiplied, 4 - Opaque. ")]
        public int BlendState { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Maximum number of particles that can be displayed at one time.")]
        public int ParticlesMax { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Name of the texture used by this particle system.")]
        public String ParticleTexture { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Frequency at which particles will be created.")]
        public float ParticlesPerSecond { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling the particle color and alpha. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public Color ColorMin { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling the particle color and alpha. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public Color ColorMax { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how big the particles are when first created. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float StartSizeMin { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how big the particles are when first created. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float StartSizeMax { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how big particles become at the end of their life. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float EndSizeMin { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how big particles become at the end of their life. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float EndSizeMax { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("Controls how the particle velocity will change over their lifetime. If set to 1, particles will keep going at the same speed as when they were created.If set to 0, particles will come to a complete stop right before they die. Values greater than 1 make the particles speed up over time.")]
        public float VelocityEnd { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how much Y axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float VelocityVerticalMin { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float VelocityVerticalMax { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float VelocityHorizontalMin { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float VelocityHorizontalMax { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("Direction and strength of the gravity effect. Note that this can point in any direction, not just down! The fire effect points it upward to make the flames rise, and the smoke plume points it sideways to simulate wind.")]
        public Vector3 Gravity { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Controls how much particles are influenced by the velocity of the object which created them. You can see this in action with the explosion effect, where the flames continue to move in the same direction as the source projectile. The projectile trail particles, on the other hand, set this value very low so they are less affected by the velocity of the projectile.")]
        public float EmitterVelocitySensitivity { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how fast the particles rotate. Values for individual particles are randomly chosen from somewhere between min and max. If both these values are set to 0, the particle system will automatically switch to an alternative shader technique that does not support rotation, and thus requires significantly less GPU power. This means if you don't need the rotation effect, you may get a performance boost from leaving these values at 0.")]
        public float RotateSpeedMax { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how fast the particles rotate. Values for individual particles are randomly chosen from somewhere between min and max. If both these values are set to 0, the particle system will automatically switch to an alternative shader technique that does not support rotation, and thus requires significantly less GPU power. This means if you don't need the rotation effect, you may get a performance boost from leaving these values at 0.")]
        public float RotateSpeedMin { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("0 - FacedToScreen 1 - FacedUp")]
        public int Technique { get; set; }

    }
    /********************************************************************************/

}
/************************************************************************************/

