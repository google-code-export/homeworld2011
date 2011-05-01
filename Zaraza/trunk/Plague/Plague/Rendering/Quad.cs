using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// Quad
    /********************************************************************************/
    class Quad
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private VertexBuffer vertexBuffer = null;
        private IndexBuffer  indexBuffer  = null;
        
        public static Renderer renderer = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Quad(float left, float top, float right, float bottom)
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(right,bottom,0),new Vector2(1,1)),
                new VertexPositionTexture(new Vector3(left ,bottom,0),new Vector2(0,1)),
                new VertexPositionTexture(new Vector3(left ,top,   0),new Vector2(0,0)),
                new VertexPositionTexture(new Vector3(right,top,   0),new Vector2(1,0)),
            };

            ushort[] indices = new ushort[] {0,1,2,2,3,0};

            vertexBuffer = new VertexBuffer(renderer.Device, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
            vertexBuffer.SetData <VertexPositionTexture>(vertices);

            indexBuffer = new IndexBuffer(renderer.Device, typeof(ushort), 6, BufferUsage.WriteOnly);
            indexBuffer.SetData<ushort>(indices);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public void Draw()
        {
            renderer.Device.SetVertexBuffer(vertexBuffer);
            renderer.Device.Indices = indexBuffer;

            renderer.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Set Buffers
        /****************************************************************************/
        public void SetBuffers()
        {
            renderer.Device.SetVertexBuffer(vertexBuffer);
            renderer.Device.Indices = indexBuffer;            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Just Draw
        /****************************************************************************/
        public void JustDraw()
        {
            renderer.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }
        /****************************************************************************/
    }
    /********************************************************************************/

}
/************************************************************************************/