using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlagueEngine.LowLevelGameFlow;


/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// Skinned MeshC omponent
    /********************************************************************************/
    class SkinnedMeshComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        internal static Renderer renderer = null;

        private Techniques technique;
        /****************************************************************************/
        

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SkinnedMeshComponent(GameObjectInstance gameObject,
                                    PlagueEngineSkinnedModel model,
                                    TexturesPack textures,
                                    Techniques technique)
            : base(gameObject)
        {
            Model          = model;
            Textures       = textures;
            this.technique = technique;

            AnimationPlayer = new AnimationPlayer(model.SkinningData);

            renderer.batchedSkinnedMeshes.AddSkinnedMeshComponent(technique, this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Start Clip
        /****************************************************************************/
        public void StartClip(String name)
        {
            AnimationPlayer.StartClip(Model.SkinningData.AnimationClips[name]);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.batchedSkinnedMeshes.RemoveSkinnedMeshComponent(technique, this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public PlagueEngineSkinnedModel Model           { get; private set; }
        public TexturesPack             Textures        { get; private set; }
        public AnimationPlayer          AnimationPlayer { get; private set; }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/