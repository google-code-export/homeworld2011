using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Particles;

/************************************************************************************/
/// Plague Engine Model Pipeline
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// GlowStick
    /********************************************************************************/
    class ParticleEmitterGO : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        ParticleEmitter particleEmitter = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(ParticleEmitter particleEmitter)
        {
            this.particleEmitter = particleEmitter;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            ParticleEmitterGOData data = new ParticleEmitterGOData();

            GetData(data);

          
                    if(particleEmitter.particleSystem.settings.BlendState==BlendState.Additive)
                    {
                        data.blendState=1;
                    }
                    else if(particleEmitter.particleSystem.settings.BlendState==BlendState.AlphaBlend)
                    {
                        data.blendState=2;
                    }
                    else if(particleEmitter.particleSystem.settings.BlendState==BlendState.Opaque)
                    {
                        data.blendState=4;
                    }
                    else
                    {
                        data.blendState=3;
                    }
                  

            data.duration=particleEmitter.particleSystem.settings.DurationInSeconds;
            data.durationRandomnes=particleEmitter.particleSystem.settings.DurationRandomness;
            data.emitterVelocitySensitivity=particleEmitter.particleSystem.settings.EmitterVelocitySensitivity;
            data.endVelocity=particleEmitter.particleSystem.settings.EndVelocity;
            data.gravity=particleEmitter.particleSystem.settings.Gravity;
            data.maxColor=particleEmitter.particleSystem.settings.MaxColor;
            data.maxEndSize=particleEmitter.particleSystem.settings.MaxEndSize;
            data.maxHorizontalVelocity=particleEmitter.particleSystem.settings.MaxHorizontalVelocity;
            data.maxParticles=particleEmitter.particleSystem.settings.MaxParticles;
            data.maxRotateSpeed=particleEmitter.particleSystem.settings.MaxRotateSpeed;
            data.maxStartSize=particleEmitter.particleSystem.settings.MaxStartSize;
            data.maxVerticalVelocity=particleEmitter.particleSystem.settings.MaxVerticalVelocity;
            data.minColor=particleEmitter.particleSystem.settings.MinColor;
            data.minEndSize=particleEmitter.particleSystem.settings.MinEndSize;
            data.minHorizontalVelocity=particleEmitter.particleSystem.settings.MinHorizontalVelocity;
            data.minRotateSpeed=particleEmitter.particleSystem.settings.MinRotateSpeed;
            data.minStartSize=particleEmitter.particleSystem.settings.MinStartSize;
            data.minVerticalVelocity=particleEmitter.particleSystem.settings.MinVerticalVelocity;
            data.particleTexture=particleEmitter.particleSystem.settings.TextureName;
            data.particlesPerSecond=particleEmitter.particlesPerSecond;
            data.initialPosition = particleEmitter.previousPosition;




            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// GlowStickData
    /********************************************************************************/
    [Serializable]
    public class ParticleEmitterGOData : GameObjectInstanceData
    {

           public int blendState { get; set; }
           public double duration { get; set; }
           public float durationRandomnes { get; set; }
           public float emitterVelocitySensitivity { get; set; }
           public float endVelocity { get; set; }
           public Vector3 gravity { get; set; }
           public Color maxColor { get; set; }
           public float maxEndSize { get; set; }
           public float maxHorizontalVelocity { get; set; }
           public int maxParticles { get; set; }
           public float maxRotateSpeed { get; set; }
           public float maxStartSize { get; set; }
           public float maxVerticalVelocity { get; set; }
           public Color minColor { get; set; }
           public float minEndSize { get; set; }
           public float minHorizontalVelocity { get; set; }
           public float minRotateSpeed { get; set; }
           public float minStartSize { get; set; }
           public float minVerticalVelocity { get; set; }
           public String particleTexture { get; set; }
           public float particlesPerSecond { get; set; }
           public Vector3 initialPosition { get; set; }


    }
    /********************************************************************************/

}
/************************************************************************************/