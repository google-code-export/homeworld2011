using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using PlagueEngine.LowLevelGameFlow;


/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// Dynamic Mesh Component
    /********************************************************************************/
    class DynamicMeshComponent : RenderableComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private PlagueEngineModel model    = null;
        
        private Texture2D         diffuse  = null;
        private Texture2D         specular = null;
        private Texture2D         normals  = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public DynamicMeshComponent(GameObjectInstance gameObject, 
                                    Renderer           renderer, 
                                    PlagueEngineModel  model, 
                                    Texture2D          diffuse,
                                    Texture2D          specular,
                                    Texture2D          normals,
                                    Effect             effect)
            : base(gameObject, renderer, effect)
        {
            this.model    = model;
            this.diffuse  = diffuse;
            this.specular = specular;
            this.normals  = normals;
        }        
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            base.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Name        
        /****************************************************************************/
        public String Name
        {
            get
            {
                return model.Name;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public override void Draw()
        {
            effect.Parameters["World"].SetValue(gameObject.World);
            effect.Parameters["DiffuseMap"].SetValue(diffuse);
            effect.Parameters["SpecularMap"].SetValue(specular);
            effect.Parameters["NormalsMap"].SetValue(normals);
            effect.CurrentTechnique.Passes[0].Apply();
            device.Indices = model.IndexBuffer;
            device.SetVertexBuffer(model.VertexBuffer);
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.VertexCount, 0, model.TriangleCount);
        }
        /****************************************************************************/

    }
    /********************************************************************************/
}
/************************************************************************************/