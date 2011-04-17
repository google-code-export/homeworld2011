using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngineSkinning;

/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// Plague Engine Skinned Model
    /********************************************************************************/
    class PlagueEngineSkinnedModel
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public int          TriangleCount;
        public int          VertexCount;
        public VertexBuffer VertexBuffer;
        public IndexBuffer  IndexBuffer;
        public String       Name;
        public SkinningData SkinningData;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor (for use only by the XNB deserializer)
        /****************************************************************************/
        private PlagueEngineSkinnedModel()
        {
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public void ReleaseMe()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/