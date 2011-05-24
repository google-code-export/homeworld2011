using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;


/************************************************************************************/
/// Plague Engine Model Pipeline
/************************************************************************************/
namespace PlagueEngineModelPipeline
{

    /********************************************************************************/
    /// Plague Engine Model Processor
    /********************************************************************************/
    [ContentProcessor(DisplayName = "Plague Engine Model Processor")]
    public class PlagueEngineModelProcessor : ContentProcessor<NodeContent, PlagueEngineModelContent>
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private ContentProcessorContext  context;
        private PlagueEngineModelContent outputModel;
        private bool                     geometryGrabbed = false;
        /****************************************************************************/
        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        [DisplayName("VertexList")]
        [DefaultValue(true)]
        [Description("Enables runtime Vertex List inside Model")]
        public bool hasList{get; set;}
        

        /****************************************************************************/
        /// Process
        /****************************************************************************/
        public override PlagueEngineModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            this.context     = context;
            outputModel      = new PlagueEngineModelContent();
            outputModel.Name = input.Name;
            
            ProcessNode(input);
            
            return outputModel;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Process Node
        /****************************************************************************/
        private void ProcessNode(NodeContent node)
        {
            if (geometryGrabbed) return;

            MeshHelper.TransformScene(node, node.Transform);
            node.Transform = Matrix.Identity;

            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                MeshHelper.OptimizeForCache(mesh);
                if (mesh.Geometry.Count != 0)
                {
                    MeshHelper.CalculateNormals(mesh, false);

                    MeshHelper.CalculateTangentFrames(mesh, VertexChannelNames.TextureCoordinate(0), VertexChannelNames.Tangent(0), VertexChannelNames.Binormal(0));
                    
                    GeometryContent geometry = mesh.Geometry[0];

                    outputModel.TriangleCount       = geometry.Indices.Count / 3;
                    outputModel.VertexCount         = geometry.Vertices.VertexCount;
                    outputModel.IndexCollection     = geometry.Indices;
                    outputModel.VertexBufferContent = geometry.Vertices.CreateVertexBuffer();
                    outputModel.BoundingBox         = BoundingBox.CreateFromPoints(geometry.Vertices.Positions);

                    if (true)
                    {
                        outputModel.VertexList = geometry.Vertices.Positions.ToList();
                        //Teoretycznie, powinna siê tu zbudowaæ poprawnie lista...
                        Dictionary<uint, List<uint>> temp = new Dictionary<uint, List<uint>>();
                        int[] array = geometry.Vertices.PositionIndices.ToArray();
                        for (int i = 2; i < array.Length; i += 3)
                        {
                            if (!temp.ContainsKey((uint)array[i]))
                            {
                                temp.Add((uint)array[i], new List<uint>());
                            }
                            temp[(uint)array[i]].Add((uint)array[i - 1]);
                            temp[(uint)array[i]].Add((uint)array[i - 2]);

                            if (!temp.ContainsKey((uint)array[i - 1]))
                            {
                                temp.Add((uint)array[i - 1], new List<uint>());
                            }
                            temp[(uint)array[i - 1]].Add((uint)array[i]);
                            temp[(uint)array[i - 1]].Add((uint)array[i - 2]);

                            if (!temp.ContainsKey((uint)array[i - 2]))
                            {
                                temp.Add((uint)array[i - 2], new List<uint>());
                            }
                            temp[(uint)array[i - 2]].Add((uint)array[i]);
                            temp[(uint)array[i - 2]].Add((uint)array[i - 1]);

                        }
                        outputModel.Adjacency  = temp;
                        //TODO: zgrywaæ normalne.
                        outputModel.NormalList = null;
                    }
                    else
                    {
                        outputModel.Adjacency  = null;
                        outputModel.VertexList = null;
                        outputModel.NormalList = null;
                    }

                    geometryGrabbed = true;
                    return;
                }
            }

            foreach (NodeContent child in node.Children)
            {
                ProcessNode(child);
            }
        }
        /****************************************************************************/


    }
    /********************************************************************************/

}
/************************************************************************************/