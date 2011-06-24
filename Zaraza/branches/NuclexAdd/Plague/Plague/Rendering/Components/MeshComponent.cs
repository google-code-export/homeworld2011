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
        private Vector3[] _bbCorners = new Vector3[8];
        private BoundingBox PrecomputedBoundingBox;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public MeshComponent(GameObjectInstance gameObject, 
                             PlagueEngineModel  model,
                             TexturesPack       textures,
                             InstancingModes    instancingMode,
                             Techniques         technique,
                             bool               enabled,
                             bool               staticMesh)
            : base(gameObject)
        {
            this.model          = model;
            this.textures       = textures;
            this.instancingMode = instancingMode;
            this.technique      = technique;
            Enabled = enabled;
            Static = staticMesh;
            
            if (Static)
            { 
                var world = gameObject.GetWorld(); 
                _bbCorners = Model.BoundingBox.GetCorners();
                Vector3.Transform(_bbCorners, ref world, corners);
                PrecomputedBoundingBox = BoundingBox.CreateFromPoints(corners);
            }

            _bbCorners = Model.BoundingBox.GetCorners();                        
            renderer.batchedMeshes.AddMeshComponent(instancingMode, technique, this);
            renderer.meshes.Add(this);        
        }        
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.batchedMeshes.RemoveMeshComponent(instancingMode, technique, this);
            renderer.meshes.Remove(this);

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
                if (Static) return PrecomputedBoundingBox;

                var world = gameObject.GetWorld(); 
                Vector3.Transform(_bbCorners, ref world, corners);
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
        public bool              Enabled        { get; set; }
        public bool Static                      { get; private set; }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/