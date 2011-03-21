using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

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
        /// Create BasicMeshComponent        
        /****************************************************************************/
        public BasicMeshComponent CreateBasicMeshComponent(GameObjectInstance gameObject,
                                                           String asset)
        {
            BasicMeshComponent result = new BasicMeshComponent( gameObject,
                                                                renderer,
                                                                content.Load<Model>(asset));
          
            return result;
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
                                                            renderer,
                                                            content.Load<Texture2D>(heightMap),
                                                            content.Load<Texture2D>(basetexture),
                                                            content.Load<Texture2D>(rtexture),
                                                            content.Load<Texture2D>(gtexture),
                                                            content.Load<Texture2D>(btexture),
                                                            content.Load<Texture2D>(weightMap),
                                                            width,
                                                            length,
                                                            height,
                                                            cellSize,
                                                            textureTiling,
                                                            content.Load<Effect>("TerrainEffect"));
            result.ComputeMesh();          
            return result;                                                                                      
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/