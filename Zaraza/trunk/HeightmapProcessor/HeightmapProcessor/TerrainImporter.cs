#region File Description
//-----------------------------------------------------------------------------
// TerrainProcessor.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
#endregion

namespace HeightmapProcessor
{
    /// <summary>
    /// Custom content processor for creating terrain meshes. Given an
    /// input heightfield texture, this processor uses the MeshBuilder
    /// class to programatically generate terrain geometry.
    /// </summary>
    [ContentProcessor(DisplayName = "Plague Engine Heightmap Processor")]
    public class TerrainProcessor : ContentProcessor<Texture2DContent, ModelContent>
    {
        const float texCoordScale = 1.0f;


        /// <summary>
        /// Generates a terrain mesh from an input heightfield texture.
        /// </summary>
        public override ModelContent Process(Texture2DContent input,
                                             ContentProcessorContext context)
        {
            MeshBuilder builder = MeshBuilder.StartMesh("terrain");

            // Convert the input texture to float format, for ease of processing.
            input.ConvertBitmapType(typeof(PixelBitmapContent<float>));

            PixelBitmapContent<float> heightfield;
            heightfield = (PixelBitmapContent<float>)input.Mipmaps[0];

            // Create the terrain vertices.
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    Vector3 position;

                    // position the vertices so that the heightfield is centered
                    // around x=0,z=0
                    position.X =   ((x*heightfield.Width/256) - ((heightfield.Width - 1) / 2.0f));
                    position.Z =   ((y*heightfield.Height/256) - ((heightfield.Height - 1) / 2.0f));

                    position.Y = (heightfield.GetPixel(x * heightfield.Width / 256, y * heightfield.Height / 256) - 1);

                    builder.CreatePosition(position);
                }
            }

      
            string directory = Path.GetDirectoryName(input.Identity.SourceFilename);
           

            // Create a vertex channel for holding texture coordinates.
            int texCoordId = builder.CreateVertexChannel<Vector2>(
                                            VertexChannelNames.TextureCoordinate(0));

            // Create the individual triangles that make up our terrain.
            for (int y = 0; y < 255; y++)
            {
                for (int x = 0; x < 255; x++)
                {
                    AddVertex(builder, texCoordId, 256, x , y  );
                    AddVertex(builder, texCoordId, 256, x  + 1 , y  );
                    AddVertex(builder, texCoordId, 256, x  + 1 , y   + 1  );

                    AddVertex(builder, texCoordId, 256, x , y );
                    AddVertex(builder, texCoordId, 256, x  + 1 , y   + 1  );
                    AddVertex(builder, texCoordId, 256, x , y   + 1  );
                }
            }

            // Chain to the ModelProcessor so it can convert the mesh we just generated.
            MeshContent terrainMesh = builder.FinishMesh();

            ModelContent model = context.Convert<MeshContent, ModelContent>(terrainMesh,
                                                              "ModelProcessor");

            // generate information about the height map, and attach it to the finished
            // model's tag.
            model.Tag = new HeightMapInfoContent(heightfield, 1,
                1);

            return model;
        }


        /// <summary>
        /// Helper for adding a new triangle vertex to a MeshBuilder,
        /// along with an associated texture coordinate value.
        /// </summary>
        static void AddVertex(MeshBuilder builder, int texCoordId, int w, int x, int y)
        {
            builder.SetVertexChannelData(texCoordId, new Vector2(x, y) * texCoordScale);

            builder.AddTriangleVertex(x + y * w);
        }
    }
}
