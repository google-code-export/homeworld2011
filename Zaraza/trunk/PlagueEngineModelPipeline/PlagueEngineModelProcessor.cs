using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;


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