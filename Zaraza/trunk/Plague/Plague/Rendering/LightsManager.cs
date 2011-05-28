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
        internal RenderTarget2D shadowMapBlur = null;
        internal RenderTarget2D shadowMap = null;
        internal RenderTarget2D sunlightShadowMap = null;        

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

        private SpotLightComponent[] shadowCasters      = new SpotLightComponent[16];
        private List<int>            freeShadowCasterID = new List<int>();
        private int                  lastShadowCasterID = 0;

        private Renderer renderer = null;
        private Matrix SunlightViewProjection;
        private Vector3 VectorZero = Vector3.Zero;
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
            blur             = content.LoadEffect("GaussianBlur7");

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
                                               2048,
                                               2048,
                                               false,
                                               SurfaceFormat.HalfVector2,
                                               DepthFormat.Depth24);

            shadowMap = new RenderTarget2D(renderer.Device,
                                           2048,
                                           2048,
                                           false,
                                           SurfaceFormat.HalfVector2,
                                           DepthFormat.Depth24);

            sunlightShadowMap = new RenderTarget2D(renderer.Device,
                                                   2048,
                                                   2048,
                                                   false,
                                                   SurfaceFormat.Single,
                                                   DepthFormat.Depth24);

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
                    directionalLight.Parameters["ShadowMap"].SetValue(sunlightShadowMap);
                    directionalLight.Parameters["LightViewProjection"].SetValue(SunlightViewProjection);
                    directionalLight.Parameters["DepthBias"].SetValue(sunlight.DepthBias);       
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
                        spotLight.Parameters["ShadowMap"].SetValue(shadowMap);
                        spotLight.Parameters["DepthBias"].SetValue(spotLightcomponent.DepthBias);
                        spotLight.Parameters["ShadowMapOffset"].SetValue(spotLightcomponent.ShadowMapOffset);
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
                        spotLight.Parameters["ShadowsEnabled" ].SetValue(true);
                        spotLight.Parameters["DepthPrecision" ].SetValue(spotLightcomponent.FarPlane);
                        spotLight.Parameters["ShadowMap"      ].SetValue(shadowMap);
                        spotLight.Parameters["DepthBias"].SetValue(spotLightcomponent.DepthBias);
                        spotLight.Parameters["ShadowMapOffset"].SetValue(spotLightcomponent.ShadowMapOffset);
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

            renderer.Device.SetRenderTarget(shadowMap);

            Viewport viewport = renderer.Device.Viewport;

            viewport.Width  = 512;
            viewport.Height = 512;
            
            foreach (SpotLightComponent spotLight in shadowCasters)
            {
                if (spotLight == null) continue;

                LightViewProjection  = spotLight.ViewProjection;
                LightFrustrum.Matrix = LightViewProjection;

                if (!spotLight.Enabled) continue;
                if (!frustrum.Intersects(LightFrustrum)) continue;                             
                
                depthPrecision = spotLight.FarPlane;
                Positon        = spotLight.World.Translation;

                viewport.X = 512 * (int)spotLight.ShadowMapOffset.X;
                viewport.Y = 512 * (int)spotLight.ShadowMapOffset.Y;

                renderer.Device.Viewport = viewport;
                
                /*********************************/
                /// Renderable Components
                /*********************************/
                foreach (RenderableComponent renderableComponent in renderer.renderableComponents)
                {
                    if (!renderableComponent.FrustrumInteresction(LightFrustrum)) continue;

                    renderableComponent.DrawDepth(ref LightViewProjection, ref Positon, depthPrecision,false);
                }
                /*********************************/


                /*********************************/
                /// Batched Meshes
                /*********************************/
                renderer.batchedMeshes.DrawDepth(LightViewProjection,
                                                 LightFrustrum,
                                                 Positon,
                                                 depthPrecision,false);
                /*********************************/


                /*********************************/
                /// Skinned Meshes
                /*********************************/
                renderer.batchedSkinnedMeshes.DrawDepth(LightViewProjection,
                                                        LightFrustrum,
                                                        Positon,
                                                        depthPrecision,false);
                /*********************************/
            }

            /*********************************/
            /// Blur
            /*********************************/
            fullScreenQuad.SetBuffers();

            renderer.Device.SetRenderTarget(shadowMapBlur);
            blur.Parameters["Texture"].SetValue(shadowMap);
            blur.CurrentTechnique.Passes[0].Apply();
            fullScreenQuad.JustDraw();

            renderer.Device.SetRenderTarget(shadowMap);
            blur.Parameters["Texture"].SetValue(shadowMapBlur);
            blur.CurrentTechnique.Passes[1].Apply();
            fullScreenQuad.JustDraw();
            /*********************************/


            if (sunlight != null)
            {
                if (sunlight.Enabled)
                {
                    CreateSunlightViewProjectionMatrix(-sunlight.Direction, frustrum);
                    
                    LightFrustrum.Matrix = SunlightViewProjection;                    

                    renderer.Device.SetRenderTarget(sunlightShadowMap);
                    renderer.Device.Clear(Color.White);


                    /*********************************/
                    /// Renderable Components
                    /*********************************/
                    foreach (RenderableComponent renderableComponent in renderer.renderableComponents)
                    {
                        if (!renderableComponent.FrustrumInteresction(LightFrustrum)) continue;

                        renderableComponent.DrawDepth(ref SunlightViewProjection, ref VectorZero, 0, true);
                    }
                    /*********************************/


                    /*********************************/
                    /// Batched Meshes
                    /*********************************/
                    renderer.batchedMeshes.DrawDepth(SunlightViewProjection,
                                                     LightFrustrum,
                                                     VectorZero,
                                                     0, true);
                    /*********************************/


                    /*********************************/
                    /// Skinned Meshes
                    /*********************************/
                    renderer.batchedSkinnedMeshes.DrawDepth(SunlightViewProjection,
                                                            LightFrustrum,
                                                            VectorZero,
                                                            0, true);
                    /*********************************/
                    
                }
            }

        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateSunlightViewProjectionMatrix
        /****************************************************************************/        
        void CreateSunlightViewProjectionMatrix(Vector3 lightDir,BoundingFrustum frustrum)
        {            
            Matrix lightRotation = Matrix.CreateLookAt(Vector3.Zero,
                                                       -lightDir,
                                                       Vector3.Up);

            Vector3[] frustumCorners = frustrum.GetCorners();

            for (int i = 0; i < frustumCorners.Length; i++)
            {
                frustumCorners[i] = Vector3.Transform(frustumCorners[i], lightRotation);
            }

            BoundingBox lightBox = BoundingBox.CreateFromPoints(frustumCorners);

            Vector3 boxSize = lightBox.Max - lightBox.Min;
            Vector3 halfBoxSize = boxSize * 0.5f;

            Vector3 lightPosition = lightBox.Min + halfBoxSize;
            lightPosition.Z = lightBox.Min.Z;

            lightPosition = Vector3.Transform(lightPosition,
                                              Matrix.Invert(lightRotation));

            Matrix lightView = Matrix.CreateLookAt(lightPosition,
                                                   lightPosition - lightDir,
                                                   Vector3.Up);

            Matrix lightProjection = Matrix.CreateOrthographic(boxSize.X, boxSize.Y,
                                                               -boxSize.Z * 2, 0);

            SunlightViewProjection = lightView * lightProjection;
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
        public int AddSpotLight(SpotLightComponent light, Texture2D attenuationTexture, bool specular, bool shadows)
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

            if (shadows)
            {
                if (freeShadowCasterID.Count > 0)
                {
                    int id = freeShadowCasterID[0];
                    shadowCasters[id] = light;
                    freeShadowCasterID.RemoveAt(0);
                    return id;
                }
                               
                if (lastShadowCasterID == 16) return -1;

                shadowCasters[lastShadowCasterID++] = light;

                return lastShadowCasterID - 1;
            }
            else return -1;
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

            if (shadows)
            {
                shadowCasters[light.ShadowMap] = null;
                freeShadowCasterID.Add(light.ShadowMap);
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/