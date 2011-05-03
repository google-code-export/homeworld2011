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
    /// Batched Skinned Meshes
    /********************************************************************************/
    class BatchedSkinnedMeshes
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Dictionary<PlagueEngineSkinnedModel, Dictionary<TexturesPack, List<SkinnedMeshComponent>>> diff         = null;
        private Dictionary<PlagueEngineSkinnedModel, Dictionary<TexturesPack, List<SkinnedMeshComponent>>> diffSpec     = null;
        private Dictionary<PlagueEngineSkinnedModel, Dictionary<TexturesPack, List<SkinnedMeshComponent>>> diffNorm     = null;
        private Dictionary<PlagueEngineSkinnedModel, Dictionary<TexturesPack, List<SkinnedMeshComponent>>> diffSpecNorm = null;

        private ContentManager content  = null;
        private Renderer       renderer = null;

        private Effect skinningEffect = null;        
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public BatchedSkinnedMeshes(ContentManager content, Renderer renderer)
        {
            this.content  = content;
            this.renderer = renderer;

            diff         = new Dictionary<PlagueEngineSkinnedModel,Dictionary<TexturesPack,List<SkinnedMeshComponent>>>();
            diffSpec     = new Dictionary<PlagueEngineSkinnedModel,Dictionary<TexturesPack,List<SkinnedMeshComponent>>>();
            diffNorm     = new Dictionary<PlagueEngineSkinnedModel,Dictionary<TexturesPack,List<SkinnedMeshComponent>>>();
            diffSpecNorm = new Dictionary<PlagueEngineSkinnedModel,Dictionary<TexturesPack,List<SkinnedMeshComponent>>>();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Model (0)
        /****************************************************************************/
        public PlagueEngineSkinnedModel PickModel(Techniques technique, String modelName)
        {
            switch (technique)
            {
                case Techniques.Diffuse:               return PickModel(modelName, diff);
                case Techniques.DiffuseSpecular:       return PickModel(modelName, diffSpec);
                case Techniques.DiffuseNormal:         return PickModel(modelName, diffNorm);
                case Techniques.DiffuseSpecularNormal: return PickModel(modelName, diffSpecNorm);
            }
            
            return null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Model (1)
        /****************************************************************************/
        private PlagueEngineSkinnedModel PickModel(String modelName, Dictionary<PlagueEngineSkinnedModel, Dictionary<TexturesPack, List<SkinnedMeshComponent>>> container)
        {
            foreach (PlagueEngineSkinnedModel model in container.Keys)
            {
                if (model.Name.Equals(modelName)) return model;
            }

            PlagueEngineSkinnedModel result = content.LoadSkinnedModel(modelName);
            container.Add(result, new Dictionary<TexturesPack, List<SkinnedMeshComponent>>());
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Textures Pack (0)
        /****************************************************************************/
        public TexturesPack PickTexturesPack(Techniques technique, PlagueEngineSkinnedModel model, String[] textures)
        {
            switch (technique)
            {
                case Techniques.Diffuse:               return PickTexturesPack(model, textures, diff);
                case Techniques.DiffuseSpecular:       return PickTexturesPack(model, textures, diffSpec);
                case Techniques.DiffuseNormal:         return PickTexturesPack(model, textures, diffNorm);
                case Techniques.DiffuseSpecularNormal: return PickTexturesPack(model, textures, diffSpecNorm);
            }

            return null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Textures Pack (1)
        /****************************************************************************/
        private TexturesPack PickTexturesPack(PlagueEngineSkinnedModel model, String[] textures, Dictionary<PlagueEngineSkinnedModel, Dictionary<TexturesPack, List<SkinnedMeshComponent>>> container)
        {
            foreach (TexturesPack texturesPack in container[model].Keys)
            {
                if (texturesPack.Equals(textures)) return texturesPack;
            }

            TexturesPack result = new TexturesPack(content.LoadTexture2D(textures[0]),
                                                   content.LoadTexture2D(textures[1]),
                                                   content.LoadTexture2D(textures[2]));

            container[model].Add(result, new List<SkinnedMeshComponent>());
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Add Skinned Mesh Component
        /****************************************************************************/
        public void AddSkinnedMeshComponent(Techniques technique, SkinnedMeshComponent meshComponent)
        {
            switch (technique)
            { 
                case Techniques.Diffuse:
                    diff[meshComponent.Model][meshComponent.Textures].Add(meshComponent);
                    break;
                case Techniques.DiffuseSpecular:
                    diffSpec[meshComponent.Model][meshComponent.Textures].Add(meshComponent);
                    break;
                case Techniques.DiffuseNormal:
                    diffNorm[meshComponent.Model][meshComponent.Textures].Add(meshComponent);
                    break;
                case Techniques.DiffuseSpecularNormal:
                    diffSpecNorm[meshComponent.Model][meshComponent.Textures].Add(meshComponent);
                    break;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Skinned Mesh Component (0)
        /****************************************************************************/
        public void RemoveSkinnedMeshComponent(Techniques technique, SkinnedMeshComponent meshComponent)
        {
            switch (technique)
            { 
                case Techniques.Diffuse:
                    RemoveSkinnedMeshComponent(meshComponent, diff);
                    break;
                case Techniques.DiffuseSpecular:
                    RemoveSkinnedMeshComponent(meshComponent, diffSpec);
                    break;
                case Techniques.DiffuseNormal:
                    RemoveSkinnedMeshComponent(meshComponent, diffNorm);
                    break;
                case Techniques.DiffuseSpecularNormal:
                    RemoveSkinnedMeshComponent(meshComponent, diffSpecNorm);
                    break;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Skinned Mesh Component (1)
        /****************************************************************************/
        private void RemoveSkinnedMeshComponent(SkinnedMeshComponent meshComponent, Dictionary<PlagueEngineSkinnedModel, Dictionary<TexturesPack, List<SkinnedMeshComponent>>> container)
        {
            container[meshComponent.Model][meshComponent.Textures].Remove(meshComponent);

            if(container[meshComponent.Model][meshComponent.Textures].Count == 0)
            {
                container[meshComponent.Model].Remove(meshComponent.Textures);

                if (container[meshComponent.Model].Keys.Count == 0)
                {
                    container.Remove(meshComponent.Model);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw (0)
        /****************************************************************************/
        public void Draw(BoundingFrustum frustrum)
        {            
            skinningEffect.CurrentTechnique = skinningEffect.Techniques["DiffuseTechnique"];
            Draw(diff, frustrum);
            skinningEffect.CurrentTechnique = skinningEffect.Techniques["DiffuseSpecularTechnique"];
            Draw(diffSpec, frustrum);
            skinningEffect.CurrentTechnique = skinningEffect.Techniques["DiffuseNormalTechnique"];
            Draw(diffNorm, frustrum);
            skinningEffect.CurrentTechnique = skinningEffect.Techniques["DiffuseSpecularNormalTechnique"];
            Draw(diffSpecNorm, frustrum);

            DeltaTime = TimeSpan.Zero;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw (1)
        /****************************************************************************/
        private void Draw(Dictionary<PlagueEngineSkinnedModel, Dictionary<TexturesPack, List<SkinnedMeshComponent>>> container, BoundingFrustum frustrum)
        {
            foreach (PlagueEngineSkinnedModel model in container.Keys)
            {
                renderer.Device.Indices = model.IndexBuffer;
                renderer.Device.SetVertexBuffer(model.VertexBuffer);

                foreach (TexturesPack texturePack in container[model].Keys)
                {
                    skinningEffect.Parameters["DiffuseMap" ].SetValue(texturePack.Diffuse);
                    skinningEffect.Parameters["SpecularMap"].SetValue(texturePack.Specular);
                    skinningEffect.Parameters["NormalsMap" ].SetValue(texturePack.Normals);

                    foreach (SkinnedMeshComponent mesh in container[model][texturePack])
                    {
                        if (!frustrum.Intersects(mesh.BoundingBox))
                        {
                            if (mesh.Blend) mesh.UpdateBoneBlendTransforms(DeltaTime);
                            else mesh.UpdateBoneTransforms(DeltaTime);
                            continue;
                        }
                        else
                        {
                            mesh.Update(DeltaTime, mesh.GameObject.World);
                        }
                        
                        Matrix[] bones = mesh.SkinTransforms;

                        skinningEffect.Parameters["Bones"].SetValue(bones);
                        skinningEffect.CurrentTechnique.Passes[0].Apply();

                        renderer.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.VertexCount, 0, model.TriangleCount);
                    }
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Effect
        /****************************************************************************/
        public void LoadEffect()
        {
            skinningEffect = content.LoadEffect("SkinningEffect");
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Set Clip Plane
        /****************************************************************************/
        public void SetClipPlane(Vector4 plane)
        {
            skinningEffect.Parameters["ClipPlaneEnabled"].SetValue(true);
            skinningEffect.Parameters["ClipPlane"].SetValue(plane);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Disable Clip Plane
        /****************************************************************************/
        public void DisableClipPlane()
        {
            skinningEffect.Parameters["ClipPlaneEnabled"].SetValue(false);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Effect   Effect    { get { return skinningEffect; } }
        public TimeSpan DeltaTime { get; set; }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/