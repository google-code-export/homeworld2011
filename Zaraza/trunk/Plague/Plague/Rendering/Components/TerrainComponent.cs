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
        private float        height         = 0;
        
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

        private Vector3[]    objectSpaceBBCorners = null;
        private Vector3[]    BBCorners            = new Vector3[8];
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
                                float              width,
                                float              height,
                                int                segments,
                                float              textureTiling,
                                Effect             effect) 

            : base(gameObject,effect)
        {
            Width          = width;
            this.height    = height;
            Segments       = segments;

            this.heightMap      = heightMap;
            this.baseTexture    = baseTexture;
            this.rTexture       = rTexture;
            this.gTexture       = gTexture;
            this.bTexture       = bTexture;
            this.weightMap      = weightMap;
            this.textureTiling  = textureTiling;

            this.effect         = effect;

            vertexCount    = segments * segments;
            indexCount     = (segments - 1) * (segments - 1) * 6;
            trianglesCount = indexCount/3;

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
            float cellSize = Width / (float)Segments;

            for (int z = 0; z < Segments; z++)            
            {
                for (int x = 0; x < Segments ; x++)
                {

                    int t1 = (heightMap.Height > Segments  ? z * (heightMap.Height/Segments) : z / (Segments/heightMap.Height));
                    int t2 = (heightMap.Width  > Segments  ? x * (heightMap.Width /Segments) : x / (Segments/heightMap.Width));
                    vertexHeight = heightMapData[(t1 * heightMap.Width) + t2].R;
                    vertexHeight /= 255.0f;
                    vertexHeight *= height;

                    vertices[(z * Segments) + x].Position = new Vector3(x * cellSize, vertexHeight, z * cellSize);
                    vertices[(z * Segments) + x].TextureCoordinate = new Vector2(vertices[(z * Segments) + x].Position.X + gameObject.World.Translation.X, 
                                                                                       vertices[(z * Segments) + x].Position.Z + gameObject.World.Translation.Z);
                }
            }


            objectSpaceBBCorners = BoundingBox.CreateFromPoints(new Vector3[] { new Vector3(Width,height,Width),
                                                                                new Vector3(0,0,0) }
                                                               ).GetCorners();


            //float[] tempHeights = new float[vertexCount];

            //for (int i = 0; i < vertexCount; i++)
            //{
            //    tempHeights[i] = vertices[i].Position.Y;
            //}

            //for (int x = 5; x < Segments - 5; x++)
            //{
            //    for (int y = 5; y < Segments - 5; y++)
            //    {
            //        float h = 0;

            //        for (int i = -5; i <= 5; i++)
            //        {
            //            for (int j = -5; j <= 5; j++)
            //            {
            //                h += vertices[((x + i) * Segments) + y + j].Position.Y;
            //            }
            //        }

            //        h /= 121.0f;
            //        tempHeights[(x * Segments) + y] = h;
            //    }
            //}

            //for (int i = 0; i < vertexCount; i++)
            //{
            //    vertices[i].Position.Y = tempHeights[i];
            //}

            int[] indices = new int[indexCount];

            {
                int i = 0;

                for (int x = 0; x < Segments - 1; x++)
                {
                    for (int z = 0; z < Segments - 1; z++)
                    {
                        int upperLeft  = z * Segments + x;
                        int upperRight = upperLeft + 1;
                        int lowerLeft  = upperLeft + Segments;
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
        public float Width    { get; private set; }
        public int   Segments { get; private set; }

        public float Height        { get { return height;           } }
        public float TextureTiling { get { return textureTiling;    } }

        public String HeightMap    { get { return heightMap.Name;   } }
        public String BaseTexture  { get { return baseTexture.Name; } }
        public String RTexture     { get { return (rTexture == null ? String.Empty : rTexture.Name);    } }
        public String GTexture     { get { return (gTexture == null ? String.Empty : rTexture.Name);    } }
        public String BTexture     { get { return (bTexture == null ? String.Empty : rTexture.Name);    } }
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


        /****************************************************************************/
        /// FrustrumInteresction
        /****************************************************************************/
        public override bool FrustrumInteresction(BoundingFrustum frustrum)
        {
            Vector3.Transform(objectSpaceBBCorners,ref gameObject.World, BBCorners);
            return frustrum.Intersects(BoundingBox.CreateFromPoints(BBCorners));       
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw Depth
        /****************************************************************************/
        public override void DrawDepth(ref Matrix ViewProjection, ref Vector3 LightPosition,float depthPrecision,bool directional)
        {
            effect.Parameters["World"].SetValue(gameObject.World);
            effect.Parameters["ViewProjection"].SetValue(ViewProjection);
            effect.Parameters["LightPosition"].SetValue(LightPosition);
            effect.Parameters["DepthPrecision"].SetValue(depthPrecision);

            if (directional)
                effect.CurrentTechnique = effect.Techniques["DepthWrite2"];
            else
                effect.CurrentTechnique = effect.Techniques["DepthWrite"];
            
            effect.CurrentTechnique.Passes[0].Apply();
            
            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);
            
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, trianglesCount);

            effect.CurrentTechnique = effect.Techniques[0];
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/
