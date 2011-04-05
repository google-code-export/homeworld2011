﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Resources;
using PlagueEngine.Rendering.Components;


/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// Rendering Components Factory
    /********************************************************************************/
    class RenderingComponentsFactory
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Renderer        renderer    = null;
        private ContentManager  content     = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public RenderingComponentsFactory(Renderer renderer)
        {
            this.renderer   = renderer;
            this.content    = renderer.contentManager;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create CameraComponent
        /****************************************************************************/
        public CameraComponent CreateCameraComponent(GameObjectInstance gameObject,
                                                     float fov,
                                                     float zNear,
                                                     float zFar)
        {
            CameraComponent result = new CameraComponent(gameObject, renderer, fov, zNear, zFar);
            
            if (renderer.CurrentCamera == null) renderer.CurrentCamera = result;
            
            return result;
        }                            
        /****************************************************************************/


        /****************************************************************************/
        /// Create TerrainComponent
        /****************************************************************************/
        public TerrainComponent CreateTerrainComponent( GameObjectInstance gameObject,
                                                        String heightMap,      
                                                        String basetexture,
                                                        String rtexture,
                                                        String gtexture,
                                                        String btexture,
                                                        String weightMap,                                    
                                                        int width,
                                                        int length,
                                                        float height,
                                                        float cellSize,
                                                        float textureTiling)
        {
            TerrainComponent result = new TerrainComponent( gameObject,
                                                            content.LoadTexture2D(heightMap),
                                                            content.LoadTexture2D(basetexture),
                                                            content.LoadTexture2D(rtexture),
                                                            content.LoadTexture2D(gtexture),
                                                            content.LoadTexture2D(btexture),
                                                            content.LoadTexture2D(weightMap),
                                                            width,
                                                            length,
                                                            height,
                                                            cellSize,
                                                            textureTiling,
                                                            content.LoadEffect("TerrainEffect"));
            result.ComputeMesh();
            return result;                                                                                      
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create WaterSurfaceComponent
        /****************************************************************************/
        public WaterSurfaceComponent CreateWaterSurfaceComponent(GameObjectInstance gameObject,
                                                                 float width,
                                                                 float length,
                                                                 float level,
                                                                 Vector3 color,
                                                                 float colorAmount,
                                                                 float waveLength,
                                                                 float waveHeight,
                                                                 float waveSpeed,                                  
                                                                 String normalMap,
                                                                 float bias,
                                                                 float textureTiling,
                                                                 float clipPlaneAdjustemnt,
                                                                 float specularStrength)
        {
            WaterSurfaceComponent result = new WaterSurfaceComponent(gameObject,
                                                                     width,
                                                                     length,
                                                                     level,
                                                                     color,
                                                                     colorAmount,
                                                                     waveLength,
                                                                     waveHeight,
                                                                     waveSpeed,
                                                                     content.LoadTexture2D(normalMap),
                                                                     bias,
                                                                     textureTiling,
                                                                     clipPlaneAdjustemnt,
                                                                     specularStrength,
                                                                     content.LoadEffect("ReflectiveWaterEffect"));
            
            renderer.preRender.Add(result);
            return result;
        }                                                                 
        /****************************************************************************/


        /****************************************************************************/
        /// Create SunlightComponent
        /****************************************************************************/
        public SunlightComponent CreateSunlightComponent(GameObjectInstance gameObject,
                                                         Vector3 diffuseColor,
                                                         Vector3 specularColor,
                                                         bool    enabled)
        {
            SunlightComponent result = new SunlightComponent(gameObject,
                                                             renderer,
                                                             diffuseColor,
                                                             specularColor,
                                                             enabled);
            renderer.Sunlight = result;
            return result;
        }                                                         
        /****************************************************************************/


        /****************************************************************************/
        /// Create MeshComponent
        /****************************************************************************/
        public MeshComponent CreateMeshComponent(GameObjectInstance gameObject,
                                                 String modelName,
                                                 String diffuseMap,
                                                 String specularMap,
                                                 String normalMap,
                                                 InstancingModes instancingMode)
        {
            MeshComponent result = null;
            
            Techniques technique = GuessTechnique(specularMap, normalMap);

            PlagueEngineModel model    = renderer.batchedMeshes.PickModel(instancingMode,technique,modelName);
            TexturesPack      textures = renderer.batchedMeshes.PickTexturesPack(instancingMode, technique, model, new String[] { diffuseMap, specularMap, normalMap });

            result = new MeshComponent(gameObject,
                                       model,
                                       textures,
                                       instancingMode,
                                       technique);                    
            return result;
        }                                            
        /****************************************************************************/


        /****************************************************************************/
        /// Guess Technique
        /****************************************************************************/
        private Techniques GuessTechnique(String specularMap, String normalMap)
        {
            if ( String.IsNullOrEmpty(specularMap) &&  String.IsNullOrEmpty(normalMap)) return Techniques.Diffuse;
            if (!String.IsNullOrEmpty(specularMap) &&  String.IsNullOrEmpty(normalMap)) return Techniques.DiffuseSpecular;
            if ( String.IsNullOrEmpty(specularMap) && !String.IsNullOrEmpty(normalMap)) return Techniques.DiffuseNormal;
            if (!String.IsNullOrEmpty(specularMap) && !String.IsNullOrEmpty(normalMap)) return Techniques.DiffuseSpecularNormal;
            
            return Techniques.Diffuse;
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/