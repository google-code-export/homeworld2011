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
    /// Batched Meshes
    /********************************************************************************/
    class BatchedMeshes
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>> data = null;
        
        private ContentManager content  = null;
        private Renderer       renderer = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public BatchedMeshes(ContentManager content, Renderer renderer)
        {
            this.content  = content;
            this.renderer = renderer;

            data = new Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>>();
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
            data.Add(result, new Dictionary<TexturesPack,  List<MeshComponent>>());
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

            data[model].Add(result, new List<MeshComponent>());
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Add Mesh Component
        /****************************************************************************/
        public void AddMeshComponent(MeshComponent mesh)
        {
            data[mesh.Model][mesh.Textures].Add(mesh);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Mesh Component
        /****************************************************************************/
        public void RemoveMeshComponent(MeshComponent mesh)
        {
            data[mesh.Model][mesh.Textures].Remove(mesh);

            if (data[mesh.Model][mesh.Textures].Count == 0)
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
        /// Draw
        /****************************************************************************/
        public void Draw(Effect effect)
        {
            foreach (PlagueEngineModel model in data.Keys)
            {
                renderer.Device.Indices = model.IndexBuffer;
                renderer.Device.SetVertexBuffer(model.VertexBuffer);

                foreach (TexturesPack texturesPack in data[model].Keys)
                {                                      
                    effect.Parameters["DiffuseMap" ].SetValue(texturesPack.Diffuse);
                    effect.Parameters["SpecularMap"].SetValue(texturesPack.Specular);
                    effect.Parameters["NormalsMap" ].SetValue(texturesPack.Normals);                                        

                    foreach (MeshComponent mesh in data[model][texturesPack])
                    {
                        effect.Parameters["World"].SetValue(mesh.GameObject.World);
                        effect.CurrentTechnique.Passes[0].Apply();

                        renderer.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.VertexCount, 0, model.TriangleCount);
                    }
                }
            }
        }
        /****************************************************************************/
       
    }
    /********************************************************************************/

}
/************************************************************************************/