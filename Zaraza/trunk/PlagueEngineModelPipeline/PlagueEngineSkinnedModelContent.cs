using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using PlagueEngine.Rendering;

/************************************************************************************/
/// Plague Engine Model Pipeline
/************************************************************************************/
namespace PlagueEngineModelPipeline
{

    /********************************************************************************/
    // Plague Engine Skinned Model Content
    /********************************************************************************/
    [ContentSerializerRuntimeType("PlagueEngine.Rendering.PlagueEngineSkinnedModel, Plague")]
    public class PlagueEngineSkinnedModelContent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public int                  TriangleCount;
        public int                  VertexCount;
        public VertexBufferContent  VertexBufferContent;
        public IndexCollection      IndexCollection;
        public String               Name;
        public SkinningData         SkinningData;
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/