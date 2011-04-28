using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna;

using PlagueEngine.LowLevelGameFlow;


/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// Mesh Component
    /********************************************************************************/
    class MeshComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private PlagueEngineModel model    = null;
        private TexturesPack      textures = null;
        
        internal static Renderer   renderer = null;
        
        private Techniques technique;
        private readonly InstancingModes instancingMode;
        private Vector3[] corners = new Vector3[8];
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public MeshComponent(GameObjectInstance gameObject, 
                             PlagueEngineModel  model,
                             TexturesPack       textures,
                             InstancingModes    instancingMode,
                             Techniques         technique)
            : base(gameObject)
        {
            this.model          = model;
            this.textures       = textures;
            this.instancingMode = instancingMode;
            this.technique      = technique;

            renderer.batchedMeshes.AddMeshComponent(instancingMode, technique, this);
        }        
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.batchedMeshes.RemoveMeshComponent(instancingMode, technique, this);
            base.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Bounding Box
        /****************************************************************************/
        public BoundingBox BoundingBox 
        {
            get
            {                
                Vector3.Transform(model.BoundingBox.GetCorners().ToArray(), ref gameObject.World, corners);
                return BoundingBox.CreateFromPoints(corners);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public PlagueEngineModel Model          { get { return model; } }
        public TexturesPack      Textures       { get { return textures;       } }
        public InstancingModes   InstancingMode { get { return instancingMode; } }        
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/