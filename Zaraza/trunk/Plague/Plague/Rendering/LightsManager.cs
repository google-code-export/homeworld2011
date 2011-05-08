using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.Rendering.Components;
using PlagueEngine.Resources;

// TODO: Instancjonować światła (o ile się da i opłaca)

/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// LightsManager
    /********************************************************************************/
    class LightsManager
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private RenderTarget2D     normal          = null;
        private RenderTarget2D     depth           = null;
        private RenderTarget2D     light           = null;
        private RenderTarget2D     shadowMapBlur   = null;        

        private PlagueEngineModel pointLightModel  = null;
        private PlagueEngineModel spotLightModel   = null;

        private Effect            directionalLight = null;
        private Effect            pointLight       = null;
        private Effect            spotLight        = null;
        private Effect            blur             = null;

        private BlendState        lightBlendState  = null;

        private Quad              fullScreenQuad   = null;

        internal SunlightComponent         sunlight        = null;

        private List<PointLightComponent> pointLightsDiff = new List<PointLightComponent>();
        private List<PointLightComponent> pointLightsSpec = new List<PointLightComponent>();

        private Dictionary<Texture2D, List<SpotLightComponent>> spotLightsDiff = new Dictionary<Texture2D, List<SpotLightComponent>>();
        private Dictionary<Texture2D, List<SpotLightComponent>> spotLightsSpec = new Dictionary<Texture2D, List<SpotLightComponent>>();

        private List<SpotLightComponent> shadowCasters = new List<SpotLightComponent>();

        private Renderer renderer = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public LightsManager(RenderTarget2D normalRT, RenderTarget2D depthRT, RenderTarget2D lightRT, Quad fullScreenQuad, Vector2 halfPixel, Renderer renderer, ContentManager content)
        {
            this.renderer       = renderer;
            this.fullScreenQuad = fullScreenQuad;
            
            normal = normalRT;
            depth  = depthRT;
            light  = lightRT;

            directionalLight = content.LoadEffect("DSDirectionalLight");
            pointLight       = content.LoadEffect("DSPointLight");
            spotLight        = content.LoadEffect("DSSpotLight");
            blur             = content.LoadEffect("GaussianBlur");

            directionalLight.Parameters["GBufferNormal"].SetValue(normal);
            directionalLight.Parameters["GBufferDepth" ].SetValue(depth);
            directionalLight.Parameters["HalfPixel"    ].SetValue(halfPixel);

            pointLight.Parameters["GBufferNormal"].SetValue(normal);
            pointLight.Parameters["GBufferDepth" ].SetValue(depth);
            pointLight.Parameters["HalfPixel"    ].SetValue(halfPixel);

            spotLight.Parameters["GBufferNormal"].SetValue(normal);
            spotLight.Parameters["GBufferDepth" ].SetValue(depth);
            spotLight.Parameters["HalfPixel"    ].SetValue(halfPixel);

            pointLightModel = content.LoadModel("Sphere");
            spotLightModel  = content.LoadModel("Cone");

            shadowMapBlur = new RenderTarget2D(renderer.Device,
                                               512,
                                               512,
                                               false,
                                               SurfaceFormat.HalfVector2,
                                               DepthFormat.None);

            lightBlendState = new BlendState();

            lightBlendState.ColorSourceBlend      = Blend.One;
            lightBlendState.ColorDestinationBlend = Blend.One;
            lightBlendState.ColorBlendFunction    = BlendFunction.Add;
            lightBlendState.AlphaSourceBlend      = Blend.One;
            lightBlendState.AlphaDestinationBlend = Blend.One;
            lightBlendState.AlphaBlendFunction    = BlendFunction.Add;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Render Lights
        /****************************************************************************/
        public void RenderLights(ref Matrix ViewProjection, ref Matrix InverseViewProjection, ref Vector3 CameraPosition, BoundingFrustum frustrum)
        {
            renderer.Device.SetRenderTarget(light);
            renderer.Device.Clear(Color.Transparent);
            
            renderer.Device.BlendState        = lightBlendState;
            renderer.Device.DepthStencilState = DepthStencilState.DepthRead;


            /*********************************/
            /// Directional Light
            /*********************************/
            if (sunlight != null)
            {
                if (sunlight.Enabled)
                {
                    directionalLight.Parameters["InverseViewProjection"].SetValue(InverseViewProjection);
                    directionalLight.Parameters["CameraPosition"].SetValue(CameraPosition);                                        
                    directionalLight.Parameters["LightDirection"].SetValue(sunlight.Direction);
                    directionalLight.Parameters["LightColor"].SetValue(sunlight.DiffuseColor);
                    directionalLight.Parameters["LightIntensity"].SetValue(sunlight.Intensity);

                    directionalLight.Techniques[0].Passes[0].Apply();
                    fullScreenQuad.Draw();
                }
            }
            /*********************************/


            /*********************************/
            #region Point Lights
            /*********************************/
            pointLight.Parameters["ViewProjection"       ].SetValue(ViewProjection);
            pointLight.Parameters["InverseViewProjection"].SetValue(InverseViewProjection);
            pointLight.Parameters["CameraPosition"       ].SetValue(CameraPosition);

            renderer.Device.SetVertexBuffer(pointLightModel.VertexBuffer);
            renderer.Device.Indices = pointLightModel.IndexBuffer;


            /***********************/
            #region Diffuse
            /***********************/
            pointLight.CurrentTechnique = pointLight.Techniques["LambertTechnique"];

            foreach (PointLightComponent pointLightComponent in pointLightsDiff)
            {
                if (!pointLightComponent.Enabled) continue;
                if (!frustrum.Intersects(pointLightComponent.BoundingSphere)) continue;

                pointLight.Parameters["World"               ].SetValue(pointLightComponent.World);
                pointLight.Parameters["LightPosition"       ].SetValue(pointLightComponent.Position);
                pointLight.Parameters["LightRadius"         ].SetValue(pointLightComponent.Radius);
                pointLight.Parameters["LightColor"          ].SetValue(pointLightComponent.Color);
                pointLight.Parameters["LightIntensity"      ].SetValue(pointLightComponent.Intensity);
                pointLight.Parameters["LinearAttenuation"   ].SetValue(pointLightComponent.LinearAttenuation);
                pointLight.Parameters["QuadraticAttenuation"].SetValue(pointLightComponent.QuadraticAttenuation);

                if (Vector3.Distance(CameraPosition, pointLightComponent.Position) < pointLightComponent.Radius * 3)
                {
                    renderer.Device.RasterizerState = RasterizerState.CullClockwise;
                }
                else
                {
                    renderer.Device.RasterizerState = RasterizerState.CullCounterClockwise;
                }

                pointLight.CurrentTechnique.Passes[0].Apply();

                renderer.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, pointLightModel.VertexCount, 0, pointLightModel.TriangleCount);
            }
            /***********************/
            #endregion
            /***********************/


            /***********************/
            #region Specular
            /***********************/
            pointLight.CurrentTechnique = pointLight.Techniques["PhongTechnique"];

            foreach (PointLightComponent pointLightComponent in pointLightsSpec)
            {
                if (!pointLightComponent.Enabled) continue;
                if (!frustrum.Intersects(pointLightComponent.BoundingSphere)) continue;

                pointLight.Parameters["World"               ].SetValue(pointLightComponent.World);
                pointLight.Parameters["LightPosition"       ].SetValue(pointLightComponent.Position);
                pointLight.Parameters["LightRadius"         ].SetValue(pointLightComponent.Radius);
                pointLight.Parameters["LightColor"          ].SetValue(pointLightComponent.Color);
                pointLight.Parameters["LightIntensity"      ].SetValue(pointLightComponent.Intensity);
                pointLight.Parameters["LinearAttenuation"   ].SetValue(pointLightComponent.LinearAttenuation);
                pointLight.Parameters["QuadraticAttenuation"].SetValue(pointLightComponent.QuadraticAttenuation);

                if (Vector3.Distance(CameraPosition, pointLightComponent.Position) < pointLightComponent.Radius * 3)
                {
                    renderer.Device.RasterizerState = RasterizerState.CullClockwise;
                }
                else
                {
                    renderer.Device.RasterizerState = RasterizerState.CullCounterClockwise;
                }

                pointLight.CurrentTechnique.Passes[0].Apply();

                renderer.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, pointLightModel.VertexCount, 0, pointLightModel.TriangleCount);
            }
            /***********************/
            #endregion
            /***********************/

            /*********************************/
            #endregion
            /*********************************/


            /*********************************/
            #region Spot Lights
            /*********************************/
            spotLight.Parameters["ViewProjection"       ].SetValue(ViewProjection);
            spotLight.Parameters["InverseViewProjection"].SetValue(InverseViewProjection);
            spotLight.Parameters["CameraPosition"       ].SetValue(CameraPosition);

            renderer.Device.SetVertexBuffer(spotLightModel.VertexBuffer);
            renderer.Device.Indices = spotLightModel.IndexBuffer;

            /************************/
            /// Helpers
            /************************/
            Matrix          LightViewProjection;
            Matrix          World;
            Vector3         CamDir;
            float           DL;
            Vector3         Direction;
            BoundingFrustum LightFrustrum = new BoundingFrustum(Matrix.Identity);
            /************************/


            /************************/
            #region Diffuse
            /************************/
            spotLight.CurrentTechnique = spotLight.Techniques["LambertTechnique"];

            foreach (KeyValuePair<Texture2D, List<SpotLightComponent>> pair in spotLightsDiff)
            {
                spotLight.Parameters["AttenuationTexture"].SetValue(pair.Key);

                foreach (SpotLightComponent spotLightcomponent in pair.Value)
                {

                    LightViewProjection = spotLightcomponent.ViewProjection;
                    LightFrustrum.Matrix = LightViewProjection;

                    if (!spotLightcomponent.Enabled) continue;
                    if (!frustrum.Intersects(LightFrustrum)) continue;

                    World = spotLightcomponent.World;
                    Direction = World.Forward;
                    Direction.Normalize();

                    spotLight.Parameters["World"               ].SetValue(World);
                    spotLight.Parameters["LightColor"          ].SetValue(spotLightcomponent.Color);
                    spotLight.Parameters["LightIntensity"      ].SetValue(spotLightcomponent.Intensity);
                    spotLight.Parameters["LightPosition"       ].SetValue(World.Translation);
                    spotLight.Parameters["LightDirection"      ].SetValue(Direction);
                    spotLight.Parameters["LightRadius"         ].SetValue(spotLightcomponent.Radius);
                    spotLight.Parameters["LightAngleCos"       ].SetValue(spotLightcomponent.AngleCos);
                    spotLight.Parameters["LightFarPlane"       ].SetValue(spotLightcomponent.FarPlane);
                    spotLight.Parameters["LinearAttenuation"   ].SetValue(spotLightcomponent.LinearAttenuation);
                    spotLight.Parameters["QuadraticAttenuation"].SetValue(spotLightcomponent.QuadraticAttenuation);
                    spotLight.Parameters["LightViewProjection" ].SetValue(LightViewProjection);

                    if (spotLightcomponent.ShadowsEnabled)
                    {
                        spotLight.Parameters["ShadowsEnabled"].SetValue(true);
                        spotLight.Parameters["DepthPrecision"].SetValue(spotLightcomponent.FarPlane);
                        spotLight.Parameters["ShadowMap"].SetValue(spotLightcomponent.ShadowMap);
                    }
                    else
                    {
                        spotLight.Parameters["ShadowsEnabled"].SetValue(false);
                    }

                    CamDir = World.Translation - CameraPosition;
                    CamDir.Normalize();
                    DL = Math.Abs(Vector3.Dot(Direction, -CamDir));
                    if (DL * 2 > spotLightcomponent.AngleCos)
                    {
                        renderer.Device.RasterizerState = RasterizerState.CullClockwise;
                    }
                    else
                    {
                        renderer.Device.RasterizerState = RasterizerState.CullCounterClockwise;
                    }

                    spotLight.CurrentTechnique.Passes[0].Apply();
                    renderer.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, spotLightModel.VertexCount, 0, spotLightModel.TriangleCount);
                }
            }
            /************************/
            #endregion
            /************************/


            /************************/
            #region Specular
            /************************/
            spotLight.CurrentTechnique = spotLight.Techniques["PhongTechnique"];

            foreach (KeyValuePair<Texture2D, List<SpotLightComponent>> pair in spotLightsSpec)
            {
                spotLight.Parameters["AttenuationTexture"].SetValue(pair.Key);

                foreach (SpotLightComponent spotLightcomponent in pair.Value)
                {

                    LightViewProjection = spotLightcomponent.ViewProjection;
                    LightFrustrum.Matrix = LightViewProjection;

                    if (!spotLightcomponent.Enabled) continue;
                    if (!frustrum.Intersects(LightFrustrum)) continue;

                    World = spotLightcomponent.World;
                    Direction = World.Forward;
                    Direction.Normalize();

                    spotLight.Parameters["World"               ].SetValue(World);
                    spotLight.Parameters["LightColor"          ].SetValue(spotLightcomponent.Color);
                    spotLight.Parameters["LightIntensity"      ].SetValue(spotLightcomponent.Intensity);
                    spotLight.Parameters["LightPosition"       ].SetValue(World.Translation);
                    spotLight.Parameters["LightDirection"      ].SetValue(Direction);
                    spotLight.Parameters["LightRadius"         ].SetValue(spotLightcomponent.Radius);
                    spotLight.Parameters["LightAngleCos"       ].SetValue(spotLightcomponent.AngleCos);
                    spotLight.Parameters["LightFarPlane"       ].SetValue(spotLightcomponent.FarPlane);
                    spotLight.Parameters["LinearAttenuation"   ].SetValue(spotLightcomponent.LinearAttenuation);
                    spotLight.Parameters["QuadraticAttenuation"].SetValue(spotLightcomponent.QuadraticAttenuation);
                    spotLight.Parameters["LightViewProjection" ].SetValue(LightViewProjection);

                    if (spotLightcomponent.ShadowsEnabled)
                    {
                        spotLight.Parameters["ShadowsEnabled"].SetValue(true);
                        spotLight.Parameters["DepthPrecision"].SetValue(spotLightcomponent.FarPlane);
                        spotLight.Parameters["ShadowMap"].SetValue(spotLightcomponent.ShadowMap);
                    }
                    else
                    {
                        spotLight.Parameters["ShadowsEnabled"].SetValue(false);
                    }

                    CamDir = World.Translation - CameraPosition;
                    CamDir.Normalize();
                    DL = Math.Abs(Vector3.Dot(Direction, -CamDir));
                    if (DL * 2 > spotLightcomponent.AngleCos)
                    {
                        renderer.Device.RasterizerState = RasterizerState.CullClockwise;
                    }
                    else
                    {
                        renderer.Device.RasterizerState = RasterizerState.CullCounterClockwise;
                    }

                    spotLight.CurrentTechnique.Passes[0].Apply();
                    renderer.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, spotLightModel.VertexCount, 0, spotLightModel.TriangleCount);
                }
            }
            /************************/
            #endregion
            /************************/
            /*********************************/
            #endregion
            /*********************************/

            
            renderer.Device.BlendState        = BlendState.Opaque;
            renderer.Device.DepthStencilState = DepthStencilState.Default;
            renderer.Device.RasterizerState   = RasterizerState.CullCounterClockwise;
            renderer.Device.SetRenderTarget(null);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Render Shadows
        /****************************************************************************/
        public void RenderShadows(BoundingFrustum frustrum)
        {
            BoundingFrustum LightFrustrum = new BoundingFrustum(Matrix.Identity);
            Matrix          LightViewProjection;
            float           depthPrecision;
            Vector3         Positon;

            foreach (SpotLightComponent spotLight in shadowCasters)
            {
                LightViewProjection  = spotLight.ViewProjection;
                LightFrustrum.Matrix = LightViewProjection;

                if (!spotLight.Enabled) continue;
                if (!frustrum.Intersects(LightFrustrum)) continue;

                renderer.Device.SetRenderTarget(spotLight.ShadowMap);
                
                depthPrecision = spotLight.FarPlane;
                Positon        = spotLight.World.Translation;

                /*********************************/
                /// Renderable Components
                /*********************************/
                foreach (RenderableComponent renderableComponent in renderer.renderableComponents)
                {
                    if (!renderableComponent.FrustrumInteresction(LightFrustrum)) continue;

                    renderableComponent.DrawDepth(ref LightViewProjection, ref Positon, depthPrecision);
                }
                /*********************************/


                /*********************************/
                /// Batched Meshes
                /*********************************/
                renderer.batchedMeshes.DrawDepth(LightViewProjection,
                                                 LightFrustrum,
                                                 Positon,
                                                 depthPrecision);
                /*********************************/


                /*********************************/
                /// Blur
                /*********************************/
                fullScreenQuad.SetBuffers();

                renderer.Device.SetRenderTarget(shadowMapBlur);
                blur.Parameters["Texture"].SetValue(spotLight.ShadowMap);
                blur.CurrentTechnique.Passes[0].Apply();
                fullScreenQuad.JustDraw();

                renderer.Device.SetRenderTarget(spotLight.ShadowMap);
                blur.Parameters["Texture"].SetValue(shadowMapBlur);
                blur.CurrentTechnique.Passes[1].Apply();
                fullScreenQuad.JustDraw();
                /*********************************/
                
            }

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Add Point Light
        /****************************************************************************/
        public void AddPointLight(PointLightComponent light, bool specular)
        {
            if (specular) pointLightsSpec.Add(light);
            else pointLightsDiff.Add(light);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Point Light
        /****************************************************************************/
        public void RemovePointLight(PointLightComponent light, bool specular)
        {
            if (specular) pointLightsSpec.Remove(light);
            else pointLightsDiff.Remove(light);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Add Spot Light
        /****************************************************************************/
        public void AddSpotLight(SpotLightComponent light, Texture2D attenuationTexture, bool specular, bool shadows)
        {
            if (specular)
            {
                if (!spotLightsSpec.ContainsKey(attenuationTexture))
                { 
                    spotLightsSpec.Add(attenuationTexture,new List<SpotLightComponent>());                    
                }
                spotLightsSpec[attenuationTexture].Add(light);
            }
            else
            {
                if (!spotLightsDiff.ContainsKey(attenuationTexture))
                {
                    spotLightsDiff.Add(attenuationTexture, new List<SpotLightComponent>());
                }
                spotLightsDiff[attenuationTexture].Add(light);
            }

            if (shadows) shadowCasters.Add(light);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Spot Light
        /****************************************************************************/
        public void RemoveSpotLight(SpotLightComponent light, Texture2D attenuationTexture, bool specular, bool shadows)
        {
            if (specular)
            {
                if (spotLightsSpec.ContainsKey(attenuationTexture))
                {
                    spotLightsSpec[attenuationTexture].Remove(light);
                }

                if (spotLightsSpec[attenuationTexture].Count == 0)
                {
                    spotLightsSpec.Remove(attenuationTexture);
                }
            }
            else
            {
                if (spotLightsDiff.ContainsKey(attenuationTexture))
                {
                    spotLightsDiff[attenuationTexture].Remove(light);
                }

                if (spotLightsDiff[attenuationTexture].Count == 0)
                {
                    spotLightsDiff.Remove(attenuationTexture);
                }
            }

            if (shadows) shadowCasters.Remove(light);
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/