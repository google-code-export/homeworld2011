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
        private Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>> noInstDiff         = null;
        private Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>> noInstDiffSpec     = null;
        private Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>> noInstDiffNorm     = null;
        private Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>> noInstDiffSpecNorm = null;

        private Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>> instDiff         = null;
        private Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>> instDiffSpec     = null;
        private Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>> instDiffNorm     = null;
        private Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>> instDiffSpecNorm = null;

        private ContentManager content  = null;
        private Renderer       renderer = null;

        private Effect instancingEffect   = null;
        private Effect noInstancingEffect = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Instance Vertex Declaration
        /****************************************************************************/
        private VertexDeclaration InstanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public BatchedMeshes(ContentManager content, Renderer renderer)
        {
            this.content  = content;
            this.renderer = renderer;

            noInstDiff          = new Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>>();
            noInstDiffSpec      = new Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>>();
            noInstDiffNorm      = new Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>>();
            noInstDiffSpecNorm  = new Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>>();

            instDiff            = new Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>>();
            instDiffSpec        = new Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>>();
            instDiffNorm        = new Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>>();
            instDiffSpecNorm    = new Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>>();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Model (0)
        /****************************************************************************/
        public PlagueEngineModel PickModel(InstancingModes instancingMode,Techniques technique, String modelName)
        {

            if (instancingMode == InstancingModes.NoInstancing)
            {
                switch (technique)
                {
                    case Techniques.Diffuse:                return PickModel(modelName, noInstDiff);
                    case Techniques.DiffuseSpecular:        return PickModel(modelName, noInstDiffSpec);
                    case Techniques.DiffuseNormal:          return PickModel(modelName, noInstDiffNorm);
                    case Techniques.DiffuseSpecularNormal:  return PickModel(modelName, noInstDiffSpecNorm);
                }
            }
            else
            {
                switch (technique)
                {
                    case Techniques.Diffuse:                return PickModel(modelName, instDiff);
                    case Techniques.DiffuseSpecular:        return PickModel(modelName, instDiffSpec);
                    case Techniques.DiffuseNormal:          return PickModel(modelName, instDiffNorm);
                    case Techniques.DiffuseSpecularNormal:  return PickModel(modelName, instDiffSpecNorm);                
                }            
            }
            
            return null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Model (1)
        /****************************************************************************/
        private PlagueEngineModel PickModel(String modelName, Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>> container)
        {
            foreach (PlagueEngineModel model in container.Keys)
            {
                if (model.Name.Equals(modelName)) return model;
            }

            PlagueEngineModel result = content.LoadModel(modelName);
            container.Add(result, new Dictionary<TexturesPack, List<MeshComponent>>());
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Model (2)
        /****************************************************************************/
        private PlagueEngineModel PickModel(String modelName, Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>> container)
        {
            foreach (PlagueEngineModel model in container.Keys)
            {
                if (model.Name.Equals(modelName)) return model;
            }

            PlagueEngineModel result = content.LoadModel(modelName);
            container.Add(result, new Dictionary<TexturesPack, InstancesData>());
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Textures Pack (0)
        /****************************************************************************/
        public TexturesPack PickTexturesPack(InstancingModes   instancingMode,
                                             Techniques        technique,
                                             PlagueEngineModel model, 
                                             String[]          textures)
        {

            if (instancingMode == InstancingModes.NoInstancing)
            {
                switch (technique)
                {
                    case Techniques.Diffuse:                return PickTexturesPack(model,textures, noInstDiff);
                    case Techniques.DiffuseSpecular:        return PickTexturesPack(model,textures, noInstDiffSpec);
                    case Techniques.DiffuseNormal:          return PickTexturesPack(model,textures, noInstDiffNorm);
                    case Techniques.DiffuseSpecularNormal:  return PickTexturesPack(model,textures, noInstDiffSpecNorm);
                }
            }
            else
            {
                switch (technique)
                {
                    case Techniques.Diffuse:                return PickTexturesPack(model, textures, instDiff);
                    case Techniques.DiffuseSpecular:        return PickTexturesPack(model, textures, instDiffSpec);
                    case Techniques.DiffuseNormal:          return PickTexturesPack(model, textures, instDiffNorm);
                    case Techniques.DiffuseSpecularNormal:  return PickTexturesPack(model, textures, instDiffSpecNorm);
                }
            }

            return null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Textures Pack (1)
        /****************************************************************************/
        private TexturesPack PickTexturesPack(PlagueEngineModel model, String[] textures, Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>> container)
        {
            foreach (TexturesPack texturesPack in container[model].Keys)
            {
                if (texturesPack.Equals(textures)) return texturesPack;
            }

            TexturesPack result = new TexturesPack(content.LoadTexture2D(textures[0]),
                                                   content.LoadTexture2D(textures[1]),
                                                   content.LoadTexture2D(textures[2]));

            container[model].Add(result, new List<MeshComponent>());
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick Textures Pack (2)
        /****************************************************************************/
        private TexturesPack PickTexturesPack(PlagueEngineModel model, String[] textures, Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>> container)
        {
            foreach (TexturesPack texturesPack in container[model].Keys)
            {
                if (texturesPack.Equals(textures)) return texturesPack;
            }

            TexturesPack result = new TexturesPack(content.LoadTexture2D(textures[0]),
                                                   content.LoadTexture2D(textures[1]),
                                                   content.LoadTexture2D(textures[2]));

            container[model].Add(result, new InstancesData());
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Add Mesh Component
        /****************************************************************************/
        public void AddMeshComponent(InstancingModes instancingMode,Techniques technique,MeshComponent mesh)
        {
            switch(instancingMode)
            {
                case InstancingModes.NoInstancing:
                    {
                        switch (technique)
                        {
                            case Techniques.Diffuse:                noInstDiff        [mesh.Model][mesh.Textures].Add(mesh); break;
                            case Techniques.DiffuseSpecular:        noInstDiffSpec    [mesh.Model][mesh.Textures].Add(mesh); break;
                            case Techniques.DiffuseNormal:          noInstDiffNorm    [mesh.Model][mesh.Textures].Add(mesh); break;
                            case Techniques.DiffuseSpecularNormal:  noInstDiffSpecNorm[mesh.Model][mesh.Textures].Add(mesh); break;
                        }
                    }
                    break;

                case InstancingModes.StaticInstancing:
                    {
                        switch (technique)
                        {
                            case Techniques.Diffuse:               instDiff        [mesh.Model][mesh.Textures].StaticMeshes.Add(mesh); break;
                            case Techniques.DiffuseSpecular:       instDiffSpec    [mesh.Model][mesh.Textures].StaticMeshes.Add(mesh); break;
                            case Techniques.DiffuseNormal:         instDiffNorm    [mesh.Model][mesh.Textures].StaticMeshes.Add(mesh); break;
                            case Techniques.DiffuseSpecularNormal: instDiffSpecNorm[mesh.Model][mesh.Textures].StaticMeshes.Add(mesh); break;
                        }                                       
                    }
                    break;

                case InstancingModes.DynamicInstancing:
                    {
                        switch (technique)
                        {
                            case Techniques.Diffuse:               instDiff        [mesh.Model][mesh.Textures].DynamicMeshes.Add(mesh); break;
                            case Techniques.DiffuseSpecular:       instDiffSpec    [mesh.Model][mesh.Textures].DynamicMeshes.Add(mesh); break;
                            case Techniques.DiffuseNormal:         instDiffNorm    [mesh.Model][mesh.Textures].DynamicMeshes.Add(mesh); break;
                            case Techniques.DiffuseSpecularNormal: instDiffSpecNorm[mesh.Model][mesh.Textures].DynamicMeshes.Add(mesh); break;
                        }                                                           
                    }
                    break;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Mesh Component (0)
        /****************************************************************************/
        public void RemoveMeshComponent(InstancingModes instancingMode, Techniques technique, MeshComponent mesh)
        {
            if (instancingMode == InstancingModes.NoInstancing)
            {
                switch (technique)
                {
                    case Techniques.Diffuse:                RemoveMeshComponent(mesh, noInstDiff);          break;
                    case Techniques.DiffuseSpecular:        RemoveMeshComponent(mesh, noInstDiffSpec);      break;
                    case Techniques.DiffuseNormal:          RemoveMeshComponent(mesh, noInstDiffNorm);      break;
                    case Techniques.DiffuseSpecularNormal:  RemoveMeshComponent(mesh, noInstDiffSpecNorm);  break;
                }
            }
            else
            {
                switch (technique)
                {
                    case Techniques.Diffuse:                RemoveMeshComponent(mesh, instancingMode, instDiff);         break;
                    case Techniques.DiffuseSpecular:        RemoveMeshComponent(mesh, instancingMode, instDiffSpec);     break;
                    case Techniques.DiffuseNormal:          RemoveMeshComponent(mesh, instancingMode, instDiffNorm);     break;
                    case Techniques.DiffuseSpecularNormal:  RemoveMeshComponent(mesh, instancingMode, instDiffSpecNorm); break;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Mesh Component (1)
        /****************************************************************************/
        private void RemoveMeshComponent(MeshComponent mesh, Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>> container)
        {
            container[mesh.Model][mesh.Textures].Remove(mesh);

            if (container[mesh.Model][mesh.Textures].Count == 0)
            {
                container[mesh.Model].Remove(mesh.Textures);

                if (container[mesh.Model].Keys.Count == 0)
                {
                    container.Remove(mesh.Model);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Mesh Component (2)
        /****************************************************************************/
        private void RemoveMeshComponent(MeshComponent mesh, InstancingModes instancingMode, Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>> container)
        {
            if (instancingMode == InstancingModes.StaticInstancing)
            {
                container[mesh.Model][mesh.Textures].StaticMeshes.Remove(mesh);
            }
            else
            {
                container[mesh.Model][mesh.Textures].DynamicMeshes.Remove(mesh);
            }

            if (container[mesh.Model][mesh.Textures].DynamicMeshes.Count == 0 && 
                container[mesh.Model][mesh.Textures].StaticMeshes.Count  == 0)
            {
                if(container[mesh.Model][mesh.Textures].StaticInstances != null)
                {
                    container[mesh.Model][mesh.Textures].StaticInstances.Dispose();
                }

                if (container[mesh.Model][mesh.Textures].DynamicInstances != null)
                {
                    container[mesh.Model][mesh.Textures].DynamicInstances.Dispose();
                }

                container[mesh.Model].Remove(mesh.Textures);

                if (container[mesh.Model].Keys.Count == 0)
                {
                    container.Remove(mesh.Model);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public void Draw()
        {
            instancingEffect.CurrentTechnique = instancingEffect.Techniques["DiffuseTechnique"];
            Draw(instDiff);
            instancingEffect.CurrentTechnique = instancingEffect.Techniques["DiffuseSpecularTechnique"];
            Draw(instDiffSpec);
            instancingEffect.CurrentTechnique = instancingEffect.Techniques["DiffuseNormalTechnique"];
            Draw(instDiffNorm);
            instancingEffect.CurrentTechnique = instancingEffect.Techniques["DiffuseSpecularNormalTechnique"];
            Draw(instDiffSpecNorm);
            
            noInstancingEffect.CurrentTechnique = noInstancingEffect.Techniques["DiffuseTechnique"];
            Draw(noInstDiff);
            noInstancingEffect.CurrentTechnique = noInstancingEffect.Techniques["DiffuseSpecularTechnique"];
            Draw(noInstDiffSpec);
            noInstancingEffect.CurrentTechnique = noInstancingEffect.Techniques["DiffuseNormalTechnique"];
            Draw(noInstDiffNorm);
            noInstancingEffect.CurrentTechnique = noInstancingEffect.Techniques["DiffuseSpecularNormalTechnique"];
            Draw(noInstDiffSpecNorm);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw (1)
        /****************************************************************************/
        private void Draw(Dictionary<PlagueEngineModel, Dictionary<TexturesPack, List<MeshComponent>>> container)
        {
            foreach (PlagueEngineModel model in container.Keys)
            {
                renderer.Device.Indices = model.IndexBuffer;
                renderer.Device.SetVertexBuffer(model.VertexBuffer);

                foreach (TexturesPack texturesPack in container[model].Keys)
                {
                    noInstancingEffect.Parameters["DiffuseMap" ].SetValue(texturesPack.Diffuse);
                    noInstancingEffect.Parameters["SpecularMap"].SetValue(texturesPack.Specular);
                    noInstancingEffect.Parameters["NormalsMap" ].SetValue(texturesPack.Normals);

                    foreach (MeshComponent mesh in container[model][texturesPack])
                    {
                        noInstancingEffect.Parameters["World"].SetValue(mesh.GameObject.World);
                        noInstancingEffect.CurrentTechnique.Passes[0].Apply();

                        renderer.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.VertexCount, 0, model.TriangleCount);
                    }
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw (2)
        /****************************************************************************/
        private void Draw(Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>> container)
        {
            foreach (PlagueEngineModel model in container.Keys)
            {
                renderer.Device.Indices = model.IndexBuffer;
                VertexBufferBinding vertexBufferBinding = new VertexBufferBinding(model.VertexBuffer);

                foreach (TexturesPack texturesPack in container[model].Keys)
                {
                    instancingEffect.Parameters["DiffuseMap"].SetValue(texturesPack.Diffuse);
                    instancingEffect.Parameters["SpecularMap"].SetValue(texturesPack.Specular);
                    instancingEffect.Parameters["NormalsMap"].SetValue(texturesPack.Normals);

                    instancingEffect.CurrentTechnique.Passes[0].Apply();

                    InstancesData instancesData = container[model][texturesPack];
                    
                    /*************************/
                    // Static Instances
                    /*************************/
                    if (instancesData.StaticInstances != null && instancesData.StaticInstanceCount != 0)
                    {
                        renderer.Device.SetVertexBuffers(vertexBufferBinding,
                                                         new VertexBufferBinding(instancesData.StaticInstances, 0, 1));

                        renderer.Device.DrawInstancedPrimitives(PrimitiveType.TriangleList,
                                                                0,
                                                                0,
                                                                model.VertexCount,
                                                                0,
                                                                model.TriangleCount,
                                                                instancesData.StaticInstanceCount);
                    }
                    /*************************/


                    /*************************/
                    // Dynamic Instances
                    /*************************/
                    if (instancesData.DynamicMeshes.Count != 0)
                    {
                        if (instancesData.DynamicInstances == null)
                        {
                            instancesData.DynamicInstances = new DynamicVertexBuffer(renderer.Device,
                                                                                     InstanceVertexDeclaration,
                                                                                     instancesData.DynamicMeshes.Count,
                                                                                     BufferUsage.WriteOnly);
                        }
                        else if (instancesData.DynamicInstances.VertexCount < instancesData.DynamicMeshes.Count)
                        {
                            instancesData.DynamicInstances.Dispose();

                            instancesData.DynamicInstances = new DynamicVertexBuffer(renderer.Device,
                                                                                     InstanceVertexDeclaration,
                                                                                     instancesData.DynamicMeshes.Count,
                                                                                     BufferUsage.WriteOnly);
                        }


                        Matrix[] transforms = new Matrix[instancesData.DynamicMeshes.Count];
                        int i = 0;
                        foreach (MeshComponent mesh in instancesData.DynamicMeshes)
                        {
                            transforms[i++] = mesh.GameObject.World;
                        }

                        instancesData.DynamicInstances.SetData(transforms, 0, instancesData.DynamicMeshes.Count, SetDataOptions.Discard);

                        renderer.Device.SetVertexBuffers(vertexBufferBinding,
                                                         new VertexBufferBinding(instancesData.DynamicInstances, 0, 1));

                        renderer.Device.DrawInstancedPrimitives(PrimitiveType.TriangleList,
                                                                0,
                                                                0,
                                                                model.VertexCount,
                                                                0,
                                                                model.TriangleCount,
                                                                instancesData.DynamicMeshes.Count);
                    }
                    /*************************/

                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Commit Mesh Transforms (0)
        /****************************************************************************/
        public void CommitMeshTransforms()
        {
            CommitMeshTransforms(instDiff);
            CommitMeshTransforms(instDiffSpec);
            CommitMeshTransforms(instDiffNorm);
            CommitMeshTransforms(instDiffSpecNorm);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Commit Mesh Transforms (1)
        /****************************************************************************/
        private void CommitMeshTransforms(Dictionary<PlagueEngineModel, Dictionary<TexturesPack, InstancesData>> container)
        {
            foreach (PlagueEngineModel model in container.Keys)
            {
                foreach (TexturesPack texturesPack in container[model].Keys)
                {
                    InstancesData instancesData = container[model][texturesPack];

                    Matrix[] transforms = new Matrix[instancesData.StaticMeshes.Count];

                    if (instancesData.StaticMeshes.Count == 0) continue;

                    int instances = 0;
                    foreach (MeshComponent meshComponent in instancesData.StaticMeshes)
                    {
                        transforms[instances++] = meshComponent.GameObject.World;
                    }

                    if (instancesData.StaticInstances != null) instancesData.StaticInstances.Dispose();

                    instancesData.StaticInstanceCount = instances;

                    instancesData.StaticInstances = new VertexBuffer(renderer.Device, InstanceVertexDeclaration, instances, BufferUsage.WriteOnly);
                    instancesData.StaticInstances.SetData(transforms, 0, instances);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Effects
        /****************************************************************************/
        public void LoadEffects()
        {
            instancingEffect   = content.LoadEffect("InstancedMeshEffect");
            noInstancingEffect = content.LoadEffect("MeshEffect");
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Set Effect Parameter
        /****************************************************************************/
        public void SetEffectParameter(String name, bool value)
        {
            instancingEffect.Parameters[name].SetValue(value);
            noInstancingEffect.Parameters[name].SetValue(value);
        }

        public void SetEffectParameter(String name, int value)
        {
            instancingEffect.Parameters[name].SetValue(value);
            noInstancingEffect.Parameters[name].SetValue(value);
        }

        public void SetEffectParameter(String name, float value)
        {
            instancingEffect.Parameters[name].SetValue(value);
            noInstancingEffect.Parameters[name].SetValue(value);
        }

        public void SetEffectParameter(String name, Matrix value)
        {
            instancingEffect.Parameters[name].SetValue(value);
            noInstancingEffect.Parameters[name].SetValue(value);
        }

        public void SetEffectParameter(String name, Texture value)
        {
            instancingEffect.Parameters[name].SetValue(value);
            noInstancingEffect.Parameters[name].SetValue(value);
        }

        public void SetEffectParameter(String name, Vector2 value)
        {
            instancingEffect.Parameters[name].SetValue(value);
            noInstancingEffect.Parameters[name].SetValue(value);
        }

        public void SetEffectParameter(String name, Vector3 value)
        {
            instancingEffect.Parameters[name].SetValue(value);
            noInstancingEffect.Parameters[name].SetValue(value);
        }

        public void SetEffectParameter(String name, Vector4 value)
        {
            instancingEffect.Parameters[name].SetValue(value);
            noInstancingEffect.Parameters[name].SetValue(value);
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
            public List<MeshComponent> StaticMeshes = new List<MeshComponent>();
            public List<MeshComponent> DynamicMeshes = new List<MeshComponent>();
            public VertexBuffer StaticInstances = null;
            public DynamicVertexBuffer DynamicInstances = null;
            public int StaticInstanceCount = 0;
            /************************************************************************/

        }
        /****************************************************************************/
       
    }
    /********************************************************************************/


    /********************************************************************************/
    /// Instancing Modes
    /********************************************************************************/
    public enum InstancingModes
    {
        NoInstancing,
        StaticInstancing,
        DynamicInstancing
    }
    /********************************************************************************/


    /********************************************************************************/
    /// PlagueEngineTechniques
    /********************************************************************************/
    public enum Techniques
    { 
        Diffuse,
        DiffuseSpecular,
        DiffuseNormal,
        DiffuseSpecularNormal
    }
    /********************************************************************************/


}
/************************************************************************************/