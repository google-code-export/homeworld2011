using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering.Components;
using PlagueEngine.Resources;

/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// Static Instanced Meshes
    /********************************************************************************/
    class StaticInstancedMeshes
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>> data = null;
        
        private ContentManager content  = null;
        private Renderer       renderer = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Instance Vertex Declaration
        /****************************************************************************/
        private VertexDeclaration InstanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0,  VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public StaticInstancedMeshes(ContentManager content,Renderer renderer)
        {
            this.content  = content;
            this.renderer = renderer;

            data = new Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>>();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Model
        /****************************************************************************/
        public PlagueEngineModel PickModel(String modelName)
        {
            foreach (PlagueEngineModel model in data.Keys)
            {
                if (model.Name.Equals(modelName)) return model;
            }

            PlagueEngineModel result = content.LoadModel(modelName);
            data.Add(result, new Dictionary<TexturesPack, InstancesData>());
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Textures Pack
        /****************************************************************************/
        public TexturesPack PickTexturesPack(PlagueEngineModel model ,String[] textures)
        {
            foreach (TexturesPack texturesPack in data[model].Keys)
            {
                if (texturesPack.Equals(textures)) return texturesPack;
            }

            TexturesPack result = new TexturesPack(content.LoadTexture2D(textures[0]),
                                                   content.LoadTexture2D(textures[1]),
                                                   content.LoadTexture2D(textures[2]));

            data[model].Add(result, new InstancesData());
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Add Mesh Component
        /****************************************************************************/
        public void AddMeshComponent(MeshComponent mesh)
        {
            data[mesh.Model][mesh.Textures].MeshComponents.Add(mesh);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Mesh Component
        /****************************************************************************/
        public void RemoveMeshComponent(MeshComponent mesh)
        {
            data[mesh.Model][mesh.Textures].MeshComponents.Remove(mesh);

            if (data[mesh.Model][mesh.Textures].MeshComponents.Count == 0)
            {
                data[mesh.Model].Remove(mesh.Textures);

                if (data[mesh.Model].Keys.Count == 0)
                {
                    data.Remove(mesh.Model);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Commit Mesh Transforms
        /****************************************************************************/
        public void CommitMeshTransforms()
        {
            foreach (PlagueEngineModel model in data.Keys)
            {
                foreach (TexturesPack texturesPack in data[model].Keys)
                {
                    InstancesData instancesData = data[model][texturesPack];

                    Matrix[] transforms = new Matrix[instancesData.MeshComponents.Count];

                    int instances = 0;
                    foreach (MeshComponent meshComponent in instancesData.MeshComponents)
                    {
                        transforms[instances++] = meshComponent.GameObject.World;
                    }

                    if (instancesData.InstancesVertexBuffer != null) instancesData.InstancesVertexBuffer.Dispose();

                    instancesData.InstanceCount = instances;
                    
                    instancesData.InstancesVertexBuffer = new VertexBuffer(renderer.Device, InstanceVertexDeclaration, instances, BufferUsage.WriteOnly);
                    instancesData.InstancesVertexBuffer.SetData(transforms, 0, instances);                    
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public void Draw(Effect effect)
        {
            foreach (PlagueEngineModel model in data.Keys)
            {
                renderer.Device.Indices = model.IndexBuffer;
                VertexBufferBinding vertexBufferBinding = new VertexBufferBinding(model.VertexBuffer);

                foreach (TexturesPack texturesPack in data[model].Keys)
                {
                    if (data[model][texturesPack].InstancesVertexBuffer == null) continue;

                    renderer.Device.SetVertexBuffers(vertexBufferBinding, 
                                                     new VertexBufferBinding(data[model][texturesPack].InstancesVertexBuffer,0,1));
                                        
                    effect.Parameters["DiffuseMap" ].SetValue(texturesPack.Diffuse);
                    effect.Parameters["SpecularMap"].SetValue(texturesPack.Specular);
                    effect.Parameters["NormalsMap" ].SetValue(texturesPack.Normals);
                    
                    effect.CurrentTechnique.Passes[0].Apply();

                    renderer.Device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 
                                                            0, 
                                                            0, 
                                                            model.VertexCount, 
                                                            0, 
                                                            model.TriangleCount, 
                                                            data[model][texturesPack].InstanceCount);

                }
            }
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// InstancesData
        /****************************************************************************/
        private class InstancesData
        {

            /************************************************************************/
            /// Fields
            /************************************************************************/
            public List<MeshComponent>  MeshComponents        = new List<MeshComponent>();
            public VertexBuffer         InstancesVertexBuffer = null;
            public int                  InstanceCount         = 0;
            /************************************************************************/

        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/