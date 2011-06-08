using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// Plague Engine Model
    /********************************************************************************/
    public class PlagueEngineModel
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public int TriangleCount;
        public int VertexCount;
        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;
        public String Name;
        public BoundingBox BoundingBox;
        public List<Vector3> VertexList;
        public List<Vector3> NormalList;
        public Dictionary<uint, List<uint>> Adjacency;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor (for use only by the XNB deserializer)
        /****************************************************************************/
        private PlagueEngineModel()
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