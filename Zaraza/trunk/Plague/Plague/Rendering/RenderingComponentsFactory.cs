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
                                                                content.Load<Model>(asset),
                                                                RemoveBasicMeshComponent);            

            renderer.basicMeshComponents.Add(result);
            
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
        /// Remove BasicMeshComponent
        /****************************************************************************/
        public void RemoveBasicMeshComponent(BasicMeshComponent component)
        {
            renderer.basicMeshComponents.Remove(component);
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/