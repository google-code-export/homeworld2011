using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.LowLevelGameFlow;


/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// SpotLightComponent
    /********************************************************************************/
    class SpotLightComponent : GameObjectComponent
    {
        
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public static Renderer renderer = null;

        private Matrix scale;
        private float radius    = 0;
        private float nearPlane = 0;
        private float farPlane  = 0;
        private float angleCos  = 0;
        private Texture2D Attenuation = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SpotLightComponent(GameObjectInstance gameObject,
                                   bool              enabled,
                                   Vector3           color,
                                   bool              specular,
                                   float             intensity,
                                   float             radius,
                                   float             nearPlane,
                                   float             farPlane,
                                   float             linearAttenuation,
                                   float             quadraticAttenuation,
                                   Matrix            localTransform,
                                   Texture2D         attenuation,
                                   bool              shadows)
            : base(gameObject)
        {
            Enabled              = enabled;
            Color                = color;
            Intensity            = intensity;
            this.radius          = radius;
            this.nearPlane       = nearPlane;
            this.farPlane        = farPlane;
            LinearAttenuation    = linearAttenuation;
            QuadraticAttenuation = quadraticAttenuation;
            LocalTransform       = localTransform;
            Attenuation          = attenuation;
            Specular             = specular;
                     
            CalculateProjection();
            CalculateScale();
            angleCos = (float)Math.Cos(MathHelper.ToRadians(radius / 2));

            ShadowMap = renderer.lightsManager.AddSpotLight(this, Attenuation, Specular, shadows);

            if (ShadowMap >= 0)
            {
                Vector2 offset = new Vector2(ShadowMap % 4,ShadowMap / 4);
                ShadowMapOffset = offset;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.lightsManager.RemoveSpotLight(this, Attenuation, Specular, ShadowsEnabled);
            base.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Calculate Projection
        /****************************************************************************/
        private void CalculateProjection()
        {

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(radius),
                                                             1.0f, 
                                                             nearPlane, 
                                                             farPlane);         
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Calculate Scale
        /****************************************************************************/
        private void CalculateScale()
        {
            scale = Matrix.CreateScale(radius / 45.0f, radius / 45.0f, 1);
            scale *= Matrix.CreateScale(farPlane, farPlane, farPlane);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public bool     Enabled              { get; set; }
        public Vector3  Color                { get; set; }
        public float    Intensity            { get; set; }

        public float    LinearAttenuation    { get; set; }
        public float    QuadraticAttenuation { get; set; }
              
        public Matrix   LocalTransform   { get; private set; }
        public Matrix   Projection       { get; private set; }
       
        public Matrix   World            
        { 
            get 
            {                 
                return scale * LocalTransform * gameObject.GetWorld();
            } 
        }

        public Matrix ViewProjection
        {
            get
            {
                return Matrix.Invert(LocalTransform * gameObject.GetWorld()) * Projection;
            }
        }

        public String AttenuationTexture { get { return Attenuation.Name; } }
        public float  AngleCos           { get { return angleCos; } }

        public float Radius    
        {
            get { return radius; }
            set
            {
                radius = value;
                CalculateProjection();
                CalculateScale();
                angleCos = (float)Math.Cos(MathHelper.ToRadians(radius / 2));
            }
        }

        public float NearPlane 
        {
            get { return nearPlane; }
            set
            {
                nearPlane = value;
                CalculateProjection();
            }
        }

        public float FarPlane  
        {
            get { return farPlane; }
            set
            {
                farPlane = value;
                CalculateProjection();
                CalculateScale();
            }
        }
        
        public bool    Specular        { get; private set; }
        public int     ShadowMap       { get; private set; }
        public Vector2 ShadowMapOffset { get; private set; }
        public bool    ShadowsEnabled  { get { return (ShadowMap >= 0 ? true : false); } }        
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/