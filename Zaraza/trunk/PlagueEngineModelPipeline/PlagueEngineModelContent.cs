using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;


/************************************************************************************/
/// Plague Engine Model Pipeline
/************************************************************************************/
namespace PlagueEngineModelPipeline
{

    /********************************************************************************/
    /// Plague Engine Model Content
    /********************************************************************************/
    [ContentSerializerRuntimeType("PlagueEngine.Rendering.PlagueEngineModel, Plague")]
    public class PlagueEngineModelContent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public int                  TriangleCount;       
        public int                  VertexCount;        
        public VertexBufferContent  VertexBufferContent;        
        public IndexCollection      IndexCollection;
        public String               Name;
        public BoundingBox          BoundingBox;
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/