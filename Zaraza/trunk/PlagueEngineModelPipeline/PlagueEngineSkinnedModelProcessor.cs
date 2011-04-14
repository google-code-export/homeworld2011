using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using PlagueEngine.Rendering;

/************************************************************************************/
/// Plague Engine Model Pipeline
/************************************************************************************/
namespace PlagueEngineModelPipeline
{

    /********************************************************************************/
    /// Plague Engine Skinned Model Processor
    /********************************************************************************/
    [ContentProcessor(DisplayName = "Plague Engine Skinned Model Processor")]
    public class PlagueEngineSkinnedModelProcessor : ContentProcessor<NodeContent, PlagueEngineSkinnedModelContent>
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        ContentProcessorContext         context;
        PlagueEngineSkinnedModelContent outputModel;
        BoneContent                     skeleton;
        bool                            geometryGrabbed = false;
        Dictionary<String, int>         boneMap;

        const int                       MaxBones        = 59;
        /****************************************************************************/


        /****************************************************************************/
        /// Process
        /****************************************************************************/
        public override PlagueEngineSkinnedModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            this.context     = context;
            outputModel      = new PlagueEngineSkinnedModelContent();
            boneMap          = new Dictionary<String, int>();
                        
            ValidateMesh(input, null);
            
            skeleton = MeshHelper.FindSkeleton(input);
            
            if (skeleton == null) 
                throw new InvalidContentException("Input skeleton not found.");

            //FlattenTransforms(skeleton);
            
            IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);
            
            if (bones.Count > MaxBones) 
                throw new InvalidContentException(string.Format("Skeleton has {0} bones, but the maximum supported is {1}.",bones.Count, MaxBones));
            
            List<Matrix> bindPose          = new List<Matrix>();
            List<Matrix> inverseBindPose   = new List<Matrix>();
            List<int>    skeletonHierarchy = new List<int>();

            foreach (BoneContent bone in bones)
            {
                bindPose.Add(bone.Transform);
                inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
            }
            
            Dictionary<string, AnimationClip> animationClips = ProcessAnimations(skeleton.Animations, bones);
            
            outputModel.SkinningData = new  SkinningData(animationClips,
                                                         bindPose,
                                                         inverseBindPose,
                                                         skeletonHierarchy);
            
            ProcessNode(input);
            
            return outputModel;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Process Animations
        /****************************************************************************/
        private Dictionary<String, AnimationClip> ProcessAnimations(AnimationContentDictionary animations,
                                                                    IList<BoneContent> bones)
        {            
            for (int i = 0; i < bones.Count; i++)
            {
                String boneName = bones[i].Name;

                if (!String.IsNullOrEmpty(boneName))
                {
                    boneMap.Add(boneName, i);
                }
            }

            Dictionary<String, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();

            foreach (KeyValuePair<String, AnimationContent> animation in animations)
            {
                AnimationClip processed = ProcessAnimation(animation.Value, boneMap);
                animationClips.Add(animation.Key, processed);
            }

            if (animationClips.Count == 0)
                throw new InvalidContentException("Input file does not contain any animations.");
            
            return animationClips;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Process Animation
        /****************************************************************************/
        private AnimationClip ProcessAnimation(AnimationContent animation,Dictionary<String, int> boneMap)
        {
            List<Keyframe> keyframes = new List<Keyframe>();

            if(animation.Duration <= TimeSpan.Zero)
                throw new InvalidContentException("Animation has a zero duration.");

            foreach(KeyValuePair<String, AnimationChannel> channel in animation.Channels)
            {
                int boneIndex;

                if(!boneMap.TryGetValue(channel.Key, out boneIndex))
                    throw new InvalidContentException(string.Format(
                        "Found animation for bone '{0}', " +
                        "which is not part of the skeleton.", channel.Key));

                foreach(AnimationKeyframe keyframe in channel.Value)
                {
                    keyframes.Add(new Keyframe(boneIndex,keyframe.Time,keyframe.Transform));
                }                
            }

            if (keyframes.Count == 0)
                throw new InvalidContentException("Animation has no keyframes.");

            keyframes.Sort(CompareKeyFrameTimes);

            return new AnimationClip(animation.Duration, keyframes);
        }                                               
        /****************************************************************************/


        /****************************************************************************/
        /// Compare Keyframe Times
        /****************************************************************************/
        private static int CompareKeyFrameTimes(Keyframe a, Keyframe b)
        {
            return a.Time.CompareTo(b.Time);
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

                    outputModel.Name                = geometry.Name;                    
                    outputModel.TriangleCount       = geometry.Indices.Count / 3;
                    outputModel.VertexCount         = geometry.Vertices.VertexCount;                    
                    outputModel.IndexCollection     = geometry.Indices;

                    List<Vector4> boneIndicesChannelData = new List<Vector4>();
                    List<Vector4> boneWeightsChannelData = new List<Vector4>();

                    foreach (BoneWeightCollection boneWeightCollection in geometry.Vertices.Channels["Weights0"])
                    {
                        if (boneWeightCollection.Count > 4)
                            throw new InvalidContentException("More than 4 weights attached to single vertex");

                        if (boneWeightCollection.Count == 0)
                            throw new InvalidContentException("No weights attached to vertex");

                        Vector4 boneIndices = new Vector4(0);
                        Vector4 boneWeights = new Vector4(0);

                        for (int i = 0; i < boneWeightCollection.Count; i++)
                        { 
                            switch(i)
                            {
                                case 0:
                                    boneIndices.X = (float)boneMap[boneWeightCollection[i].BoneName];
                                    boneWeights.X = boneWeightCollection[i].Weight;
                                    break;
                                case 1:
                                    boneIndices.Y = (float)boneMap[boneWeightCollection[i].BoneName];
                                    boneWeights.Y = boneWeightCollection[i].Weight;
                                    break;
                                case 2:
                                    boneIndices.Z = (float)boneMap[boneWeightCollection[i].BoneName];
                                    boneWeights.Z = boneWeightCollection[i].Weight;
                                    break;
                                case 3:
                                    boneIndices.W = (float)boneMap[boneWeightCollection[i].BoneName];
                                    boneWeights.W = boneWeightCollection[i].Weight;
                                    break;
                            }                            
                        }

                        boneIndicesChannelData.Add(boneIndices);
                        boneWeightsChannelData.Add(boneWeights);
                    }

                    geometry.Vertices.Channels.Remove("Weights0");
                    geometry.Vertices.Channels.Add<Vector4>("BlendIndices0", boneIndicesChannelData);
                    geometry.Vertices.Channels.Add<Vector4>("BlendWeight0",boneWeightsChannelData);
                    
                    outputModel.VertexBufferContent = geometry.Vertices.CreateVertexBuffer();                    

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


        /****************************************************************************/
        /// Validate Mesh
        /****************************************************************************/
        private void ValidateMesh(NodeContent node, String parentBoneName)
        {
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                if (parentBoneName != null)
                { 
                    context.Logger.LogWarning(null,null,
                        "Mesh {0} is a child of bone {1}. SkinnedModelProcessor " +
                        "does not correctly handle meshes that are children of bones.",
                        mesh.Name, parentBoneName);
                }

                if (!MeshHasSkinning(mesh))
                { 
                    context.Logger.LogWarning(null,null,
                        "Mesh {0} has no skinning information, so it has been deleted.",
                        mesh.Name);

                    mesh.Parent.Children.Remove(mesh);
                    return;
                }
                
            }
            else if (node is BoneContent)
            {
                parentBoneName = node.Name;
            }

            foreach (NodeContent child in new List<NodeContent>(node.Children))
            {
                ValidateMesh(child, parentBoneName);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Mesh Has Skinning
        /****************************************************************************/
        private bool MeshHasSkinning(MeshContent mesh)
        {
            foreach (GeometryContent geometry in mesh.Geometry)
            {
                if (!geometry.Vertices.Channels.Contains(VertexChannelNames.Weights())) return false;
            }

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Flatten Transforms
        /****************************************************************************/
        private void FlattenTransforms(NodeContent node)
        {
            foreach (NodeContent child in node.Children)
            {
                if (child == skeleton) continue;

                MeshHelper.TransformScene(child, child.Transform);

                child.Transform = Matrix.Identity;

                FlattenTransforms(child);
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/