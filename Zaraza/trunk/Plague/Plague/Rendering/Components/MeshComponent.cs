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
    /// Mesh Component
    /********************************************************************************/
    class MeshComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private PlagueEngineModel model    = null;
        private TexturesPack      textures = null;
        private Renderer          renderer = null;
        
        private Techniques technique;
        private readonly InstancingModes instancingMode;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public MeshComponent(GameObjectInstance gameObject, 
                             Renderer           renderer, 
                             PlagueEngineModel  model,
                             TexturesPack       textures,
                             InstancingModes    instancingMode,
                             Techniques         technique)
            : base(gameObject)
        {
            this.renderer       = renderer;
            this.model          = model;
            this.textures       = textures;
            this.instancingMode = instancingMode;
            this.technique      = technique;
        }        
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.batchedMeshes.RemoveMeshComponent(instancingMode, technique, this);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public PlagueEngineModel Model          { get { return model;          } }
        public TexturesPack      Textures       { get { return textures;       } }
        public InstancingModes   InstancingMode { get { return instancingMode; } }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/