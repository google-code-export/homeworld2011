using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using PlagueEngine.Audio.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;
using PlagueEngine.TimeControlSystem;

using PlagueEngine.HighLevelGameFlow;
using System.Collections.Generic;


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
                        Vector3 explosionPosition)
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
                    return new[] { "Examine", "Activate" };
                }
            }
            return new[] { "Examine" };
            
        }
        /****************************************************************************/


        private void DoBoom()
        {
            this.body.DisableBody();
            foreach (var stone in stones)
            {
                SphericalBodyMesh stoneGO = (SphericalBodyMesh)level.GameObjects[stone];
                stoneGO.body.Immovable = false;
            }

            ExplosionManager.CreateExplosion(explosionPosition, explosionForce, explosionRadius);

        }


        /****************************************************************************/
        /// On Action
        /****************************************************************************/
        protected override void OnActivation()
        {

            TimeControl.CreateTimer(TimeSpan.FromSeconds(timer), 1, DoBoom);



        }



        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {


 
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
    }
    /********************************************************************************/

}
/************************************************************************************/

