using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using PlagueEngineSkinning;

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
        private ContentProcessorContext         context;
        private PlagueEngineSkinnedModelContent outputModel;
        private BoneContent                     skeleton;
        private bool                            geometryGrabbed = false;
        private Dictionary<String, int>         boneMap;

        private float                           frameDuration   = 0;
        private AnimationClipSettings[]         clips           = new AnimationClipSettings[32];
        Dictionary<String, AnimationClip>       animationClips  = new Dictionary<string, AnimationClip>();

        private const int                       MaxBones        = 59;
        /****************************************************************************/


        /****************************************************************************/
        /// Process
        /****************************************************************************/
        public override PlagueEngineSkinnedModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            this.context     = context;
            outputModel      = new PlagueEngineSkinnedModelContent();
            boneMap          = new Dictionary<String, int>();

            if(!ValidateAnimations())
                throw new InvalidContentException("Animations not set.");

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

            MapBones(bones);

            ProcessAnimation(skeleton.Animations.ElementAt(0).Value);
            
            outputModel.SkinningData = new  SkinningData(animationClips,
                                                         bindPose,
                                                         inverseBindPose,
                                                         skeletonHierarchy,
                                                         boneMap);
            
            ProcessNode(input);
            
            return outputModel;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Map Bones 
        /****************************************************************************/
        private void MapBones(IList<BoneContent> bones)
        {
            for (int i = 0; i < bones.Count; i++)
            {
                String boneName = bones[i].Name;

                if (!String.IsNullOrEmpty(boneName))
                {
                    boneMap.Add(boneName, i);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Process Animation
        /****************************************************************************/
        private AnimationClip ProcessAnimation(AnimationContent animation)
        {          
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
                    foreach (KeyValuePair<String, AnimationClip> animationClip in animationClips)
                    {
                        if (keyframe.Time >= animationClip.Value.StartTime &&
                            keyframe.Time <= animationClip.Value.EndTime)
                        {
                            animationClip.Value.Keyframes.Add(new Keyframe(boneIndex,
                                                                           keyframe.Time - animationClip.Value.StartTime,
                                                                           keyframe.Transform));
                        }
                    }                    
                }                
            }

            foreach (KeyValuePair<String, AnimationClip> animationClip in animationClips)
            {
                animationClip.Value.Keyframes.Sort(CompareKeyFrameTimes);
            }            

            return null;
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

                outputModel.Name = mesh.Name;                

                MeshHelper.OptimizeForCache(mesh);
                if (mesh.Geometry.Count != 0)
                {
                    MeshHelper.CalculateNormals(mesh, false);

                    MeshHelper.CalculateTangentFrames(mesh, VertexChannelNames.TextureCoordinate(0), VertexChannelNames.Tangent(0), VertexChannelNames.Binormal(0));
                    
                    GeometryContent geometry = mesh.Geometry[0];
                                        
                    outputModel.TriangleCount       = geometry.Indices.Count / 3;
                    outputModel.VertexCount         = geometry.Vertices.VertexCount;                    
                    outputModel.IndexCollection     = geometry.Indices;

                    List<Vector4> boneIndicesChannelData = new List<Vector4>();
                    List<Vector4> boneWeightsChannelData = new List<Vector4>();


                    int vertextID = 0;
                    foreach (BoneWeightCollection boneWeightCollection in geometry.Vertices.Channels["Weights0"])
                    {
                        if (boneWeightCollection.Count > 4)
                        {
                            String boneNames = String.Empty;
                            foreach (BoneWeight weight in boneWeightCollection)
                            {
                                boneNames += weight.BoneName + ": " + weight.Weight.ToString() + "\n";
                            }

                            throw new InvalidContentException("More than 4 weights attached to single vertex. " +
                                                               "#" + vertextID + ".\n" +
                                                               boneNames);                        
                        }


                        if (boneWeightCollection.Count == 0)
                            throw new InvalidContentException("No weights attached to vertex. #" + vertextID + ".");

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
                        vertextID++;
                    }

                    geometry.Vertices.Channels.Remove("Weights0");
                    geometry.Vertices.Channels.Add<Vector4>("BlendIndices0", boneIndicesChannelData);
                    geometry.Vertices.Channels.Add<Vector4>("BlendWeight0",boneWeightsChannelData);
                    
                    outputModel.VertexBufferContent = geometry.Vertices.CreateVertexBuffer();
                    outputModel.BoundingBox = BoundingBox.CreateFromPoints(geometry.Vertices.Positions);

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


        /****************************************************************************/
        /// Validate Animations
        /****************************************************************************/
        private bool ValidateAnimations()
        {
            bool result = false;

            foreach (AnimationClipSettings clip in clips)
            {
                if (!String.IsNullOrEmpty(clip.Name))
                {
                    result = true;

                    animationClips.Add(clip.Name, new AnimationClip(TimeSpan.FromSeconds(clip.StartFrame * frameDuration),
                                                                    TimeSpan.FromSeconds(clip.EndFrame   * frameDuration),
                                                                    new List<Keyframe>(),
                                                                    clip.Loop,
                                                                    clip.Name));
                }
            }

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        [DisplayName("00: Framerate")]
        [DefaultValue(25)]
        [Description("Sets animation framerate.")]        
        public uint Framerate 
        {
            get
            {
                return (uint)(1 / frameDuration);
            }

            set
            {
                frameDuration = 1.0f / (float)value; 
            }
        }


        /****************************************************************************/
        /// Animation Clip 1
        /****************************************************************************/
        [DisplayName("01.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 1.")]
        public String AnimationClip1 { get { return clips[0].Name; } set { clips[0].Name = value; } }

        [DisplayName("01.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 1.")]
        public uint AnimationClip1Start { get { return clips[0].StartFrame; } set { clips[0].StartFrame = value; } }

        [DisplayName("01.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 1.")]
        public uint AnimationClip1End { get { return clips[0].EndFrame; } set { clips[0].EndFrame = value; } }

        [DisplayName("01.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 1.")]
        public bool AnimationClip1Loop { get { return clips[0].Loop; } set { clips[0].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 2
        /****************************************************************************/
        [DisplayName("02.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 2.")]
        public String AnimationClip2 { get { return clips[1].Name; } set { clips[1].Name = value; } }

        [DisplayName("02.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 2.")]
        public uint AnimationClip2Start { get { return clips[1].StartFrame; } set { clips[1].StartFrame = value; } }

        [DisplayName("02.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 2.")]
        public uint AnimationClip2End { get { return clips[1].EndFrame; } set { clips[1].EndFrame = value; } }

        [DisplayName("02.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 2.")]
        public bool AnimationClip2Loop { get { return clips[1].Loop; } set { clips[1].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 3
        /****************************************************************************/
        [DisplayName("03.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 3.")]
        public String AnimationClip3 { get { return clips[2].Name; } set { clips[2].Name = value; } }

        [DisplayName("03.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 3.")]
        public uint AnimationClip3Start { get { return clips[2].StartFrame; } set { clips[2].StartFrame = value; } }

        [DisplayName("03.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 3.")]
        public uint AnimationClip3End { get { return clips[2].EndFrame; } set { clips[2].EndFrame = value; } }

        [DisplayName("03.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 3.")]
        public bool AnimationClip3Loop { get { return clips[2].Loop; } set { clips[2].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 4
        /****************************************************************************/
        [DisplayName("04.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 4.")]
        public String AnimationClip4 { get { return clips[3].Name; } set { clips[3].Name = value; } }

        [DisplayName("04.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 4.")]
        public uint AnimationClip4Start { get { return clips[3].StartFrame; } set { clips[3].StartFrame = value; } }

        [DisplayName("04.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 4.")]
        public uint AnimationClip4End { get { return clips[3].EndFrame; } set { clips[3].EndFrame = value; } }

        [DisplayName("04.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 4.")]
        public bool AnimationClip4Loop { get { return clips[3].Loop; } set { clips[3].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 5
        /****************************************************************************/
        [DisplayName("05.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 5.")]
        public String AnimationClip5 { get { return clips[4].Name; } set { clips[4].Name = value; } }

        [DisplayName("05.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 5.")]
        public uint AnimationClip5Start { get { return clips[4].StartFrame; } set { clips[4].StartFrame = value; } }

        [DisplayName("05.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 5.")]
        public uint AnimationClip5End { get { return clips[4].EndFrame; } set { clips[4].EndFrame = value; } }

        [DisplayName("05.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 5.")]
        public bool AnimationClip5Loop { get { return clips[4].Loop; } set { clips[4].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 6
        /****************************************************************************/
        [DisplayName("06.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 6.")]
        public String AnimationClip6 { get { return clips[5].Name; } set { clips[5].Name = value; } }

        [DisplayName("06.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 6.")]
        public uint AnimationClip6Start { get { return clips[5].StartFrame; } set { clips[5].StartFrame = value; } }

        [DisplayName("06.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 6.")]
        public uint AnimationClip6End { get { return clips[5].EndFrame; } set { clips[5].EndFrame = value; } }

        [DisplayName("06.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 6.")]
        public bool AnimationClip6Loop { get { return clips[5].Loop; } set { clips[5].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 7
        /****************************************************************************/
        [DisplayName("07.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 7.")]
        public String AnimationClip7 { get { return clips[6].Name; } set { clips[6].Name = value; } }

        [DisplayName("07.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 7.")]
        public uint AnimationClip7Start { get { return clips[6].StartFrame; } set { clips[6].StartFrame = value; } }

        [DisplayName("07.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 7.")]
        public uint AnimationClip7End { get { return clips[6].EndFrame; } set { clips[6].EndFrame = value; } }

        [DisplayName("07.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 7.")]
        public bool AnimationClip7Loop { get { return clips[6].Loop; } set { clips[6].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 8
        /****************************************************************************/
        [DisplayName("08.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 8.")]
        public String AnimationClip8 { get { return clips[7].Name; } set { clips[7].Name = value; } }

        [DisplayName("08.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 8.")]
        public uint AnimationClip8Start { get { return clips[7].StartFrame; } set { clips[7].StartFrame = value; } }

        [DisplayName("08.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 8.")]
        public uint AnimationClip8End { get { return clips[7].EndFrame; } set { clips[7].EndFrame = value; } }

        [DisplayName("08.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 8.")]
        public bool AnimationClip8Loop { get { return clips[7].Loop; } set { clips[7].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 9
        /****************************************************************************/
        [DisplayName("09.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 9.")]
        public String AnimationClip9 { get { return clips[8].Name; } set { clips[8].Name = value; } }

        [DisplayName("09.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 9.")]
        public uint AnimationClip9Start { get { return clips[8].StartFrame; } set { clips[8].StartFrame = value; } }

        [DisplayName("09.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 9.")]
        public uint AnimationClip9End { get { return clips[8].EndFrame; } set { clips[8].EndFrame = value; } }

        [DisplayName("09.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 9.")]
        public bool AnimationClip9Loop { get { return clips[8].Loop; } set { clips[8].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 10
        /****************************************************************************/
        [DisplayName("10.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 10.")]
        public String AnimationClip10 { get { return clips[9].Name; } set { clips[9].Name = value; } }

        [DisplayName("10.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 10.")]
        public uint AnimationClip10Start { get { return clips[9].StartFrame; } set { clips[9].StartFrame = value; } }

        [DisplayName("10.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 10.")]
        public uint AnimationClip10End { get { return clips[9].EndFrame; } set { clips[9].EndFrame = value; } }

        [DisplayName("10.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 10.")]
        public bool AnimationClip10Loop { get { return clips[9].Loop; } set { clips[9].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 11
        /****************************************************************************/
        [DisplayName("11.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 11.")]
        public String AnimationClip11 { get { return clips[10].Name; } set { clips[10].Name = value; } }

        [DisplayName("11.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 11.")]
        public uint AnimationClip11Start { get { return clips[10].StartFrame; } set { clips[10].StartFrame = value; } }

        [DisplayName("11.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 11.")]
        public uint AnimationClip11End { get { return clips[10].EndFrame; } set { clips[10].EndFrame = value; } }

        [DisplayName("11.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 11.")]
        public bool AnimationClip11Loop { get { return clips[10].Loop; } set { clips[10].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 12
        /****************************************************************************/
        [DisplayName("12.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 12.")]
        public String AnimationClip12 { get { return clips[11].Name; } set { clips[11].Name = value; } }

        [DisplayName("12.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 12.")]
        public uint AnimationClip12Start { get { return clips[11].StartFrame; } set { clips[11].StartFrame = value; } }

        [DisplayName("12.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 12.")]
        public uint AnimationClip12End { get { return clips[11].EndFrame; } set { clips[11].EndFrame = value; } }

        [DisplayName("12.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 12.")]
        public bool AnimationClip12Loop { get { return clips[11].Loop; } set { clips[11].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 13
        /****************************************************************************/
        [DisplayName("13.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 13.")]
        public String AnimationClip13 { get { return clips[12].Name; } set { clips[12].Name = value; } }

        [DisplayName("13.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 13.")]
        public uint AnimationClip13Start { get { return clips[12].StartFrame; } set { clips[12].StartFrame = value; } }

        [DisplayName("13.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 13.")]
        public uint AnimationClip13End { get { return clips[12].EndFrame; } set { clips[12].EndFrame = value; } }

        [DisplayName("13.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 13.")]
        public bool AnimationClip13Loop { get { return clips[12].Loop; } set { clips[12].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 14
        /****************************************************************************/
        [DisplayName("14.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 14.")]
        public String AnimationClip14 { get { return clips[13].Name; } set { clips[13].Name = value; } }

        [DisplayName("14.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 14.")]
        public uint AnimationClip14Start { get { return clips[13].StartFrame; } set { clips[13].StartFrame = value; } }

        [DisplayName("14.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 14.")]
        public uint AnimationClip14End { get { return clips[13].EndFrame; } set { clips[13].EndFrame = value; } }

        [DisplayName("14.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 14.")]
        public bool AnimationClip14Loop { get { return clips[13].Loop; } set { clips[13].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 15
        /****************************************************************************/
        [DisplayName("15.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 15.")]
        public String AnimationClip15 { get { return clips[14].Name; } set { clips[14].Name = value; } }

        [DisplayName("15.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 15.")]
        public uint AnimationClip15Start { get { return clips[14].StartFrame; } set { clips[14].StartFrame = value; } }

        [DisplayName("15.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 15.")]
        public uint AnimationClip15End { get { return clips[14].EndFrame; } set { clips[14].EndFrame = value; } }

        [DisplayName("15.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 15.")]
        public bool AnimationClip15Loop { get { return clips[14].Loop; } set { clips[14].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 16
        /****************************************************************************/
        [DisplayName("16.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 16.")]
        public String AnimationClip16 { get { return clips[15].Name; } set { clips[15].Name = value; } }

        [DisplayName("16.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 16.")]
        public uint AnimationClip16Start { get { return clips[15].StartFrame; } set { clips[15].StartFrame = value; } }

        [DisplayName("16.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 16.")]
        public uint AnimationClip16End { get { return clips[15].EndFrame; } set { clips[15].EndFrame = value; } }

        [DisplayName("16.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 16.")]
        public bool AnimationClip16Loop { get { return clips[15].Loop; } set { clips[15].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 17
        /****************************************************************************/
        [DisplayName("17.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 17.")]
        public String AnimationClip17 { get { return clips[16].Name; } set { clips[16].Name = value; } }

        [DisplayName("17.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 17.")]
        public uint AnimationClip17Start { get { return clips[16].StartFrame; } set { clips[16].StartFrame = value; } }

        [DisplayName("17.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 17.")]
        public uint AnimationClip17End { get { return clips[16].EndFrame; } set { clips[16].EndFrame = value; } }

        [DisplayName("17.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 17.")]
        public bool AnimationClip17Loop { get { return clips[16].Loop; } set { clips[16].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 18
        /****************************************************************************/
        [DisplayName("18.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 18.")]
        public String AnimationClip18 { get { return clips[17].Name; } set { clips[17].Name = value; } }

        [DisplayName("18.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 18.")]
        public uint AnimationClip18Start { get { return clips[17].StartFrame; } set { clips[17].StartFrame = value; } }

        [DisplayName("18.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 18.")]
        public uint AnimationClip18End { get { return clips[17].EndFrame; } set { clips[17].EndFrame = value; } }

        [DisplayName("18.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 18.")]
        public bool AnimationClip18Loop { get { return clips[17].Loop; } set { clips[17].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 19
        /****************************************************************************/
        [DisplayName("19.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 19.")]
        public String AnimationClip19 { get { return clips[18].Name; } set { clips[18].Name = value; } }

        [DisplayName("19.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 19.")]
        public uint AnimationClip19Start { get { return clips[18].StartFrame; } set { clips[18].StartFrame = value; } }

        [DisplayName("19.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 19.")]
        public uint AnimationClip19End { get { return clips[18].EndFrame; } set { clips[18].EndFrame = value; } }

        [DisplayName("19.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 19.")]
        public bool AnimationClip19Loop { get { return clips[18].Loop; } set { clips[18].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 20
        /****************************************************************************/
        [DisplayName("20.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 20.")]
        public String AnimationClip20 { get { return clips[19].Name; } set { clips[19].Name = value; } }

        [DisplayName("20.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 20.")]
        public uint AnimationClip20Start { get { return clips[19].StartFrame; } set { clips[19].StartFrame = value; } }

        [DisplayName("20.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 20.")]
        public uint AnimationClip20End { get { return clips[19].EndFrame; } set { clips[19].EndFrame = value; } }

        [DisplayName("20.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 20.")]
        public bool AnimationClip20Loop { get { return clips[19].Loop; } set { clips[19].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 21
        /****************************************************************************/
        [DisplayName("21.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 21.")]
        public String AnimationClip21 { get { return clips[20].Name; } set { clips[20].Name = value; } }

        [DisplayName("21.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 21.")]
        public uint AnimationClip21Start { get { return clips[20].StartFrame; } set { clips[20].StartFrame = value; } }

        [DisplayName("21.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 21.")]
        public uint AnimationClip21End { get { return clips[20].EndFrame; } set { clips[20].EndFrame = value; } }

        [DisplayName("21.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 21.")]
        public bool AnimationClip21Loop { get { return clips[20].Loop; } set { clips[20].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 22
        /****************************************************************************/
        [DisplayName("22.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 22.")]
        public String AnimationClip22 { get { return clips[21].Name; } set { clips[21].Name = value; } }

        [DisplayName("22.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 22.")]
        public uint AnimationClip22Start { get { return clips[21].StartFrame; } set { clips[21].StartFrame = value; } }

        [DisplayName("22.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 22.")]
        public uint AnimationClip22End { get { return clips[21].EndFrame; } set { clips[21].EndFrame = value; } }

        [DisplayName("22.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 22.")]
        public bool AnimationClip22Loop { get { return clips[21].Loop; } set { clips[21].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 23
        /****************************************************************************/
        [DisplayName("23.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 23.")]
        public String AnimationClip23 { get { return clips[22].Name; } set { clips[22].Name = value; } }

        [DisplayName("23.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 23.")]
        public uint AnimationClip23Start { get { return clips[22].StartFrame; } set { clips[22].StartFrame = value; } }

        [DisplayName("23.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 23.")]
        public uint AnimationClip23End { get { return clips[22].EndFrame; } set { clips[22].EndFrame = value; } }

        [DisplayName("23.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 23.")]
        public bool AnimationClip23Loop { get { return clips[22].Loop; } set { clips[22].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 24
        /****************************************************************************/
        [DisplayName("24.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 24.")]
        public String AnimationClip24 { get { return clips[23].Name; } set { clips[23].Name = value; } }

        [DisplayName("24.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 24.")]
        public uint AnimationClip24Start { get { return clips[23].StartFrame; } set { clips[23].StartFrame = value; } }

        [DisplayName("24.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 24.")]
        public uint AnimationClip24End { get { return clips[23].EndFrame; } set { clips[23].EndFrame = value; } }

        [DisplayName("24.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 24.")]
        public bool AnimationClip24Loop { get { return clips[23].Loop; } set { clips[23].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 25
        /****************************************************************************/
        [DisplayName("25.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 25.")]
        public String AnimationClip25 { get { return clips[24].Name; } set { clips[24].Name = value; } }

        [DisplayName("25.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 25.")]
        public uint AnimationClip25Start { get { return clips[24].StartFrame; } set { clips[24].StartFrame = value; } }

        [DisplayName("25.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 25.")]
        public uint AnimationClip25End { get { return clips[24].EndFrame; } set { clips[24].EndFrame = value; } }

        [DisplayName("25.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 25.")]
        public bool AnimationClip25Loop { get { return clips[24].Loop; } set { clips[24].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 26
        /****************************************************************************/
        [DisplayName("26.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 26.")]
        public String AnimationClip26 { get { return clips[25].Name; } set { clips[25].Name = value; } }

        [DisplayName("26.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 26.")]
        public uint AnimationClip26Start { get { return clips[25].StartFrame; } set { clips[25].StartFrame = value; } }

        [DisplayName("26.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 26.")]
        public uint AnimationClip26End { get { return clips[25].EndFrame; } set { clips[25].EndFrame = value; } }

        [DisplayName("26.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 26.")]
        public bool AnimationClip26Loop { get { return clips[25].Loop; } set { clips[25].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 27
        /****************************************************************************/
        [DisplayName("27.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 27.")]
        public String AnimationClip27 { get { return clips[26].Name; } set { clips[26].Name = value; } }

        [DisplayName("27.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 27.")]
        public uint AnimationClip27Start { get { return clips[26].StartFrame; } set { clips[26].StartFrame = value; } }

        [DisplayName("27.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 27.")]
        public uint AnimationClip27End { get { return clips[26].EndFrame; } set { clips[26].EndFrame = value; } }

        [DisplayName("27.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 27.")]
        public bool AnimationClip27Loop { get { return clips[26].Loop; } set { clips[26].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 28
        /****************************************************************************/
        [DisplayName("28.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 28.")]
        public String AnimationClip28 { get { return clips[27].Name; } set { clips[27].Name = value; } }

        [DisplayName("28.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 28.")]
        public uint AnimationClip28Start { get { return clips[27].StartFrame; } set { clips[27].StartFrame = value; } }

        [DisplayName("28.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 28.")]
        public uint AnimationClip28End { get { return clips[27].EndFrame; } set { clips[27].EndFrame = value; } }

        [DisplayName("28.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 28.")]
        public bool AnimationClip28Loop { get { return clips[27].Loop; } set { clips[27].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 29
        /****************************************************************************/
        [DisplayName("29.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 29.")]
        public String AnimationClip29 { get { return clips[28].Name; } set { clips[28].Name = value; } }

        [DisplayName("29.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 29.")]
        public uint AnimationClip29Start { get { return clips[28].StartFrame; } set { clips[28].StartFrame = value; } }

        [DisplayName("29.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 29.")]
        public uint AnimationClip29End { get { return clips[28].EndFrame; } set { clips[28].EndFrame = value; } }

        [DisplayName("29.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 29.")]
        public bool AnimationClip29Loop { get { return clips[28].Loop; } set { clips[28].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 30
        /****************************************************************************/
        [DisplayName("30.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 30.")]
        public String AnimationClip30 { get { return clips[29].Name; } set { clips[29].Name = value; } }

        [DisplayName("30.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 30.")]
        public uint AnimationClip30Start { get { return clips[29].StartFrame; } set { clips[29].StartFrame = value; } }

        [DisplayName("30.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 30.")]
        public uint AnimationClip30End { get { return clips[29].EndFrame; } set { clips[29].EndFrame = value; } }

        [DisplayName("30.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 30.")]
        public bool AnimationClip30Loop { get { return clips[29].Loop; } set { clips[29].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 31
        /****************************************************************************/
        [DisplayName("31.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 31.")]
        public String AnimationClip31 { get { return clips[30].Name; } set { clips[30].Name = value; } }

        [DisplayName("31.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 31.")]
        public uint AnimationClip31Start { get { return clips[30].StartFrame; } set { clips[30].StartFrame = value; } }

        [DisplayName("31.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 31.")]
        public uint AnimationClip31End { get { return clips[30].EndFrame; } set { clips[30].EndFrame = value; } }

        [DisplayName("31.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 31.")]
        public bool AnimationClip31Loop { get { return clips[30].Loop; } set { clips[30].Loop = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 32
        /****************************************************************************/
        [DisplayName("32.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 32.")]
        public String AnimationClip32 { get { return clips[31].Name; } set { clips[31].Name = value; } }

        [DisplayName("32.2 Animation Clip: Start Frame")]
        [DefaultValue(0)]
        [Description("Start frame of Animation Clip 32.")]
        public uint AnimationClip32Start { get { return clips[31].StartFrame; } set { clips[31].StartFrame = value; } }

        [DisplayName("32.3 Animation Clip: End Frame")]
        [DefaultValue(0)]
        [Description("End frame of Animation Clip 32.")]
        public uint AnimationClip32End { get { return clips[31].EndFrame; } set { clips[31].EndFrame = value; } }

        [DisplayName("32.4 Animation Clip: Loop")]
        [DefaultValue(false)]
        [Description("Looping of Animation Clip 32.")]
        public bool AnimationClip32Loop { get { return clips[31].Loop; } set { clips[31].Loop = value; } }
        /****************************************************************************/

        /****************************************************************************/
        

    }
    /********************************************************************************/


    /********************************************************************************/
    /// AnimationClipSettings
    /********************************************************************************/
    struct AnimationClipSettings
    {
        public String Name; 
        public uint   StartFrame;
        public uint   EndFrame;
        public bool   Loop;
    }
    /********************************************************************************/

}
/************************************************************************************/