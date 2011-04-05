using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.Resources;

/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// Terrain Component
    /********************************************************************************/
    class TerrainComponent : RenderableComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private int          width          = 0;
        private int          length         = 0;
        private float        height         = 0;
        private float        cellSize       = 0;
        
        private VertexBuffer vertexBuffer   = null;
        private IndexBuffer  indexBuffer    = null;
        private int          vertexCount    = 0;
        private int          indexCount     = 0;
        private int          trianglesCount = 0;
        
        private Texture2D    heightMap      = null;
        private Texture2D    baseTexture    = null;
        private Texture2D    rTexture       = null;
        private Texture2D    gTexture       = null;
        private Texture2D    bTexture       = null;
        private Texture2D    weightMap      = null;
        private float        textureTiling  = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public TerrainComponent(GameObjectInstance gameObject,
                                Texture2D          heightMap,
                                Texture2D          baseTexture,
                                Texture2D          rTexture,
                                Texture2D          gTexture,
                                Texture2D          bTexture,
                                Texture2D          weightMap,
                                int                width,
                                int                length,
                                float              height,
                                float              cellSize,
                                float              textureTiling,
                                Effect             effect) : base(gameObject,effect)
        {
            this.width          = width;
            this.length         = length;
            this.height         = height;
            this.cellSize       = cellSize;

            this.heightMap      = heightMap;
            this.baseTexture    = baseTexture;
            this.rTexture       = rTexture;
            this.gTexture       = gTexture;
            this.bTexture       = bTexture;
            this.weightMap      = weightMap;
            this.textureTiling  = textureTiling;

            this.effect         = effect;

            vertexCount    = width * length;
            indexCount     = (width - 1) * (length - 1) * 6;
            trianglesCount = indexCount / 3;

            SetEffect();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Compute Mesh
        /****************************************************************************/
        public void ComputeMesh()
        {
            if (vertexBuffer != null) vertexBuffer.Dispose();
            if (indexBuffer  != null) indexBuffer.Dispose();

            Color[] heightMapData = new Color[heightMap.Width * heightMap.Height];
            heightMap.GetData<Color>(heightMapData);

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[vertexCount];

            float vertexHeight;

            for (int z = 0; z < length; z++)            
            {
                for (int x = 0; x < width; x++)
                {


                    int t1 = (heightMap.Height > length ? z * (heightMap.Height / length) : z / (length / heightMap.Height));
                    int t2 = (heightMap.Width  > width  ? x * (heightMap.Width  / width)  : x / (width  / heightMap.Width ));
                    vertexHeight = heightMapData[(t1 * heightMap.Width) + t2].R;
                    vertexHeight /= 255;
                    vertexHeight *= height;

                    vertices[(z * width) + x].Position          = new Vector3(x * cellSize, vertexHeight, z * cellSize);
                    vertices[(z * width) + x].TextureCoordinate = new Vector2((float)x/width ,(float)z/length);
                }
            }

            int[] indices = new int[indexCount];

            {
                int i = 0;

                for (int x = 0; x < width - 1; x++)
                {
                    for (int z = 0; z < length - 1; z++)
                    {
                        int upperLeft = z * (int)width + x;
                        int upperRight = upperLeft + 1;
                        int lowerLeft = upperLeft + (int)width;
                        int lowerRight = lowerLeft + 1;

                        indices[i++] = upperLeft;
                        indices[i++] = upperRight;
                        indices[i++] = lowerLeft;

                        indices[i++] = lowerLeft;
                        indices[i++] = upperRight;
                        indices[i++] = lowerRight;
                    }
                }
            }
            

            for (int i = 0; i < indexCount; i += 3)
            {
                // Find the position of each corner of the triangle
                Vector3 v1 = vertices[indices[i]].Position;
                Vector3 v2 = vertices[indices[i + 1]].Position;
                Vector3 v3 = vertices[indices[i + 2]].Position;
                // Cross the vectors between the corners to get the normal
                Vector3 normal = Vector3.Cross(v1 - v2, v1 - v3);
                normal.Normalize();
                // Add the influence of the normal to each vertex in the
                // triangle
                vertices[indices[i]].Normal += normal;
                vertices[indices[i + 1]].Normal += normal;
                vertices[indices[i + 2]].Normal += normal;
            }
            // Average the influences of the triangles touching each
            // vertex
            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i].Normal.Normalize();
            }

            vertexBuffer = new VertexBuffer(renderer.Device, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
            
            indexBuffer = new IndexBuffer(renderer.Device, IndexElementSize.ThirtyTwoBits, indexCount,BufferUsage.WriteOnly);
            indexBuffer.SetData<int>(indices);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public override void Draw()
        {
            effect.Parameters["World"].SetValue(gameObject.World);
            effect.CurrentTechnique.Passes[0].Apply();
            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, trianglesCount);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Set Effect
        /****************************************************************************/
        private void SetEffect()
        {
            effect.Parameters["TextureTiling"].SetValue(textureTiling);
            effect.Parameters["BaseTexture"].SetValue(baseTexture);
            effect.Parameters["RTexture"].SetValue(rTexture);
            effect.Parameters["GTexture"].SetValue(gTexture);
            effect.Parameters["BTexture"].SetValue(bTexture);
            effect.Parameters["WeightMap"].SetValue(weightMap);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public int   Width         { get { return width;            } }
        public int   Length        { get { return length;           } }
        public float Height        { get { return height;           } }
        public float CellSize      { get { return cellSize;         } }
        public float TextureTiling { get { return textureTiling;    } }

        public String HeightMap    { get { return heightMap.Name;   } }
        public String BaseTexture  { get { return baseTexture.Name; } }
        public String RTexture     { get { return rTexture.Name;    } }
        public String GTexture     { get { return gTexture.Name;    } }
        public String BTexture     { get { return bTexture.Name;    } }
        public String WeightMap    { get { return weightMap.Name;   } }       
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            base.ReleaseMe();
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/
