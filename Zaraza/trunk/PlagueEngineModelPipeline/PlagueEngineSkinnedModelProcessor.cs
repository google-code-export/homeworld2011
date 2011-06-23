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
        private AnimationClipSettings[]         clips = new AnimationClipSettings[64];
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

            ValidateMesh(input, null);
            
            skeleton = MeshHelper.FindSkeleton(input);
            
            if (skeleton == null) 
                throw new InvalidContentException("Input skeleton not found.");

            //FlattenTransforms(skeleton);
            
            IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);

            foreach (var x in bones)
            {
                context.Logger.LogWarning(null, null, x.Name);                
            }

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

            foreach (KeyValuePair<String, AnimationContent> animation in skeleton.Animations)
            {
                AnimationClip clip = ProcessAnimation(animation.Value);
                animationClips.Add(clip.Name, clip);
            }
            
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
            String animationName = animation.Name;
            
            bool loop = false;
            if(animationName[0] == 'L' && animationName[1] == '_')
            {                
                loop = true;
                animationName = animationName.Substring(2);                
            }

            TimeSpan Duration = animation.Duration;
            foreach (AnimationClipSettings settings in clips)
            {
                if(String.IsNullOrEmpty(settings.Name)) continue;
                
                if (settings.Name.Equals(animationName))
                {
                    Duration = TimeSpan.FromSeconds(settings.Duration);
                    break;
                }
            }
            
            AnimationClip animationClip = new AnimationClip(Duration,new List<Keyframe>(),loop,animationName);


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
                    animationClip.Keyframes.Add(new Keyframe(boneIndex,
                                                             keyframe.Time,
                                                             keyframe.Transform));
                }                
            }

            animationClip.Keyframes.Sort(CompareKeyFrameTimes);

            return animationClip;
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
        /// Animation Clip 1
        /****************************************************************************/
        [DisplayName("1.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 1.")]
        public String AnimationClip1 { get { return clips[0].Name; } set { clips[0].Name = value; } }

        [DisplayName("1.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 1.")]
        public double AnimationClip1Duration { get { return clips[0].Duration; } set { clips[0].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 2
        /****************************************************************************/
        [DisplayName("2.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 2.")]
        public String AnimationClip2 { get { return clips[1].Name; } set { clips[1].Name = value; } }

        [DisplayName("2.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 2.")]
        public double AnimationClip2Duration { get { return clips[1].Duration; } set { clips[1].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 3
        /****************************************************************************/
        [DisplayName("3.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 3.")]
        public String AnimationClip3 { get { return clips[2].Name; } set { clips[2].Name = value; } }

        [DisplayName("3.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 3.")]
        public double AnimationClip3Duration { get { return clips[2].Duration; } set { clips[2].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 4
        /****************************************************************************/
        [DisplayName("4.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 4.")]
        public String AnimationClip4 { get { return clips[3].Name; } set { clips[3].Name = value; } }

        [DisplayName("4.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 4.")]
        public double AnimationClip4Duration { get { return clips[3].Duration; } set { clips[3].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 5
        /****************************************************************************/
        [DisplayName("5.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 5.")]
        public String AnimationClip5 { get { return clips[4].Name; } set { clips[4].Name = value; } }

        [DisplayName("5.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 5.")]
        public double AnimationClip5Duration { get { return clips[4].Duration; } set { clips[4].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 6
        /****************************************************************************/
        [DisplayName("6.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 6.")]
        public String AnimationClip6 { get { return clips[5].Name; } set { clips[5].Name = value; } }

        [DisplayName("6.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 6.")]
        public double AnimationClip6Duration { get { return clips[5].Duration; } set { clips[5].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 7
        /****************************************************************************/
        [DisplayName("7.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 7.")]
        public String AnimationClip7 { get { return clips[6].Name; } set { clips[6].Name = value; } }

        [DisplayName("7.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 7.")]
        public double AnimationClip7Duration { get { return clips[6].Duration; } set { clips[6].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 8
        /****************************************************************************/
        [DisplayName("8.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 8.")]
        public String AnimationClip8 { get { return clips[7].Name; } set { clips[7].Name = value; } }

        [DisplayName("8.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 8.")]
        public double AnimationClip8Duration { get { return clips[7].Duration; } set { clips[7].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 9
        /****************************************************************************/
        [DisplayName("9.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 9.")]
        public String AnimationClip9 { get { return clips[8].Name; } set { clips[8].Name = value; } }

        [DisplayName("9.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 9.")]
        public double AnimationClip9Duration { get { return clips[8].Duration; } set { clips[8].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 10
        /****************************************************************************/
        [DisplayName("10.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 10.")]
        public String AnimationClip10 { get { return clips[9].Name; } set { clips[9].Name = value; } }

        [DisplayName("10.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 10.")]
        public double AnimationClip10Duration { get { return clips[9].Duration; } set { clips[9].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 11
        /****************************************************************************/
        [DisplayName("11.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 11.")]
        public String AnimationClip11 { get { return clips[10].Name; } set { clips[10].Name = value; } }

        [DisplayName("11.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 11.")]
        public double AnimationClip11Duration { get { return clips[10].Duration; } set { clips[10].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 12
        /****************************************************************************/
        [DisplayName("12.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 12.")]
        public String AnimationClip12 { get { return clips[11].Name; } set { clips[11].Name = value; } }

        [DisplayName("12.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 12.")]
        public double AnimationClip12Duration { get { return clips[11].Duration; } set { clips[11].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 13
        /****************************************************************************/
        [DisplayName("13.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 13.")]
        public String AnimationClip13 { get { return clips[12].Name; } set { clips[12].Name = value; } }

        [DisplayName("13.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 13.")]
        public double AnimationClip13Duration { get { return clips[12].Duration; } set { clips[12].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 14
        /****************************************************************************/
        [DisplayName("14.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 14.")]
        public String AnimationClip14 { get { return clips[13].Name; } set { clips[13].Name = value; } }

        [DisplayName("14.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 14.")]
        public double AnimationClip14Duration { get { return clips[13].Duration; } set { clips[13].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 15
        /****************************************************************************/
        [DisplayName("15.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 15.")]
        public String AnimationClip15 { get { return clips[14].Name; } set { clips[14].Name = value; } }

        [DisplayName("15.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 15.")]
        public double AnimationClip15Duration { get { return clips[14].Duration; } set { clips[14].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 16
        /****************************************************************************/
        [DisplayName("16.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 16.")]
        public String AnimationClip16 { get { return clips[15].Name; } set { clips[15].Name = value; } }

        [DisplayName("16.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 16.")]
        public double AnimationClip16Duration { get { return clips[15].Duration; } set { clips[15].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 17
        /****************************************************************************/
        [DisplayName("17.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 17.")]
        public String AnimationClip17 { get { return clips[16].Name; } set { clips[16].Name = value; } }

        [DisplayName("17.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 17.")]
        public double AnimationClip17Duration { get { return clips[16].Duration; } set { clips[16].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 18
        /****************************************************************************/
        [DisplayName("18.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 18.")]
        public String AnimationClip18 { get { return clips[17].Name; } set { clips[17].Name = value; } }

        [DisplayName("18.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 18.")]
        public double AnimationClip18Duration { get { return clips[17].Duration; } set { clips[17].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 19
        /****************************************************************************/
        [DisplayName("19.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 19.")]
        public String AnimationClip19 { get { return clips[18].Name; } set { clips[18].Name = value; } }

        [DisplayName("19.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 19.")]
        public double AnimationClip19Duration { get { return clips[18].Duration; } set { clips[18].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 20
        /****************************************************************************/
        [DisplayName("20.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 20.")]
        public String AnimationClip20 { get { return clips[19].Name; } set { clips[19].Name = value; } }

        [DisplayName("20.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 20.")]
        public double AnimationClip20Duration { get { return clips[19].Duration; } set { clips[19].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 21
        /****************************************************************************/
        [DisplayName("21.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 21.")]
        public String AnimationClip21 { get { return clips[20].Name; } set { clips[20].Name = value; } }

        [DisplayName("21.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 21.")]
        public double AnimationClip21Duration { get { return clips[20].Duration; } set { clips[20].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 22
        /****************************************************************************/
        [DisplayName("22.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 22.")]
        public String AnimationClip22 { get { return clips[21].Name; } set { clips[21].Name = value; } }

        [DisplayName("22.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 22.")]
        public double AnimationClip22Duration { get { return clips[21].Duration; } set { clips[21].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 23
        /****************************************************************************/
        [DisplayName("23.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 23.")]
        public String AnimationClip23 { get { return clips[22].Name; } set { clips[22].Name = value; } }

        [DisplayName("23.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 23.")]
        public double AnimationClip23Duration { get { return clips[22].Duration; } set { clips[22].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 24
        /****************************************************************************/
        [DisplayName("24.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 24.")]
        public String AnimationClip24 { get { return clips[23].Name; } set { clips[23].Name = value; } }

        [DisplayName("24.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 24.")]
        public double AnimationClip24Duration { get { return clips[23].Duration; } set { clips[23].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 25
        /****************************************************************************/
        [DisplayName("25.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 25.")]
        public String AnimationClip25 { get { return clips[24].Name; } set { clips[24].Name = value; } }

        [DisplayName("25.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 25.")]
        public double AnimationClip25Duration { get { return clips[24].Duration; } set { clips[24].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 26
        /****************************************************************************/
        [DisplayName("26.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 26.")]
        public String AnimationClip26 { get { return clips[25].Name; } set { clips[25].Name = value; } }

        [DisplayName("26.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 26.")]
        public double AnimationClip26Duration { get { return clips[25].Duration; } set { clips[25].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 27
        /****************************************************************************/
        [DisplayName("27.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 27.")]
        public String AnimationClip27 { get { return clips[26].Name; } set { clips[26].Name = value; } }

        [DisplayName("27.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 27.")]
        public double AnimationClip27Duration { get { return clips[26].Duration; } set { clips[26].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 28
        /****************************************************************************/
        [DisplayName("28.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 28.")]
        public String AnimationClip28 { get { return clips[27].Name; } set { clips[27].Name = value; } }

        [DisplayName("28.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 28.")]
        public double AnimationClip28Duration { get { return clips[27].Duration; } set { clips[27].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 29
        /****************************************************************************/
        [DisplayName("29.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 29.")]
        public String AnimationClip29 { get { return clips[28].Name; } set { clips[28].Name = value; } }

        [DisplayName("29.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 29.")]
        public double AnimationClip29Duration { get { return clips[28].Duration; } set { clips[28].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 30
        /****************************************************************************/
        [DisplayName("30.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 30.")]
        public String AnimationClip30 { get { return clips[29].Name; } set { clips[29].Name = value; } }

        [DisplayName("30.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 30.")]
        public double AnimationClip30Duration { get { return clips[29].Duration; } set { clips[29].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 31
        /****************************************************************************/
        [DisplayName("31.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 31.")]
        public String AnimationClip31 { get { return clips[30].Name; } set { clips[30].Name = value; } }

        [DisplayName("31.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 31.")]
        public double AnimationClip31Duration { get { return clips[30].Duration; } set { clips[30].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 32
        /****************************************************************************/
        [DisplayName("32.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 32.")]
        public String AnimationClip32 { get { return clips[31].Name; } set { clips[31].Name = value; } }

        [DisplayName("32.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 32.")]
        public double AnimationClip32Duration { get { return clips[31].Duration; } set { clips[31].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 33
        /****************************************************************************/
        [DisplayName("33.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 33.")]
        public String AnimationClip33 { get { return clips[32].Name; } set { clips[32].Name = value; } }

        [DisplayName("33.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 33.")]
        public double AnimationClip33Duration { get { return clips[32].Duration; } set { clips[32].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 34
        /****************************************************************************/
        [DisplayName("34.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 34.")]
        public String AnimationClip34 { get { return clips[33].Name; } set { clips[33].Name = value; } }

        [DisplayName("34.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 34.")]
        public double AnimationClip34Duration { get { return clips[33].Duration; } set { clips[33].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 35
        /****************************************************************************/
        [DisplayName("35.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 35.")]
        public String AnimationClip35 { get { return clips[34].Name; } set { clips[34].Name = value; } }

        [DisplayName("35.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 35.")]
        public double AnimationClip35Duration { get { return clips[34].Duration; } set { clips[34].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 36
        /****************************************************************************/
        [DisplayName("36.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 36.")]
        public String AnimationClip36 { get { return clips[35].Name; } set { clips[35].Name = value; } }

        [DisplayName("36.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 36.")]
        public double AnimationClip36Duration { get { return clips[35].Duration; } set { clips[35].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 37
        /****************************************************************************/
        [DisplayName("37.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 37.")]
        public String AnimationClip37 { get { return clips[36].Name; } set { clips[36].Name = value; } }

        [DisplayName("37.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 37.")]
        public double AnimationClip37Duration { get { return clips[36].Duration; } set { clips[36].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 38
        /****************************************************************************/
        [DisplayName("38.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 38.")]
        public String AnimationClip38 { get { return clips[37].Name; } set { clips[37].Name = value; } }

        [DisplayName("38.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 38.")]
        public double AnimationClip38Duration { get { return clips[37].Duration; } set { clips[37].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 39
        /****************************************************************************/
        [DisplayName("39.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 39.")]
        public String AnimationClip39 { get { return clips[38].Name; } set { clips[38].Name = value; } }

        [DisplayName("39.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 39.")]
        public double AnimationClip39Duration { get { return clips[38].Duration; } set { clips[38].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 40
        /****************************************************************************/
        [DisplayName("40.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 40.")]
        public String AnimationClip40 { get { return clips[39].Name; } set { clips[39].Name = value; } }

        [DisplayName("40.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 40.")]
        public double AnimationClip40Duration { get { return clips[39].Duration; } set { clips[39].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 41
        /****************************************************************************/
        [DisplayName("41.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 41.")]
        public String AnimationClip41 { get { return clips[40].Name; } set { clips[40].Name = value; } }

        [DisplayName("41.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 41.")]
        public double AnimationClip41Duration { get { return clips[40].Duration; } set { clips[40].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 42
        /****************************************************************************/
        [DisplayName("42.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 42.")]
        public String AnimationClip42 { get { return clips[41].Name; } set { clips[41].Name = value; } }

        [DisplayName("42.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 42.")]
        public double AnimationClip42Duration { get { return clips[41].Duration; } set { clips[41].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 43
        /****************************************************************************/
        [DisplayName("43.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 43.")]
        public String AnimationClip43 { get { return clips[42].Name; } set { clips[42].Name = value; } }

        [DisplayName("43.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 43.")]
        public double AnimationClip43Duration { get { return clips[42].Duration; } set { clips[42].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 44
        /****************************************************************************/
        [DisplayName("44.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 44.")]
        public String AnimationClip44 { get { return clips[43].Name; } set { clips[43].Name = value; } }

        [DisplayName("44.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 44.")]
        public double AnimationClip44Duration { get { return clips[43].Duration; } set { clips[43].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 45
        /****************************************************************************/
        [DisplayName("45.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 45.")]
        public String AnimationClip45 { get { return clips[44].Name; } set { clips[44].Name = value; } }

        [DisplayName("45.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 45.")]
        public double AnimationClip45Duration { get { return clips[44].Duration; } set { clips[44].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 46
        /****************************************************************************/
        [DisplayName("46.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 46.")]
        public String AnimationClip46 { get { return clips[45].Name; } set { clips[45].Name = value; } }

        [DisplayName("46.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 46.")]
        public double AnimationClip46Duration { get { return clips[45].Duration; } set { clips[45].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 47
        /****************************************************************************/
        [DisplayName("47.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 47.")]
        public String AnimationClip47 { get { return clips[46].Name; } set { clips[46].Name = value; } }

        [DisplayName("47.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 47.")]
        public double AnimationClip47Duration { get { return clips[46].Duration; } set { clips[46].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 48
        /****************************************************************************/
        [DisplayName("48.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 48.")]
        public String AnimationClip48 { get { return clips[47].Name; } set { clips[47].Name = value; } }

        [DisplayName("48.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 48.")]
        public double AnimationClip48Duration { get { return clips[47].Duration; } set { clips[47].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 49
        /****************************************************************************/
        [DisplayName("49.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 49.")]
        public String AnimationClip49 { get { return clips[48].Name; } set { clips[48].Name = value; } }

        [DisplayName("49.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 49.")]
        public double AnimationClip49Duration { get { return clips[48].Duration; } set { clips[48].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 50
        /****************************************************************************/
        [DisplayName("50.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 50.")]
        public String AnimationClip50 { get { return clips[49].Name; } set { clips[49].Name = value; } }

        [DisplayName("50.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 50.")]
        public double AnimationClip50Duration { get { return clips[49].Duration; } set { clips[49].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 51
        /****************************************************************************/
        [DisplayName("51.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 51.")]
        public String AnimationClip51 { get { return clips[50].Name; } set { clips[50].Name = value; } }

        [DisplayName("51.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 51.")]
        public double AnimationClip51Duration { get { return clips[50].Duration; } set { clips[50].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 52
        /****************************************************************************/
        [DisplayName("52.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 52.")]
        public String AnimationClip52 { get { return clips[51].Name; } set { clips[51].Name = value; } }

        [DisplayName("52.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 52.")]
        public double AnimationClip52Duration { get { return clips[51].Duration; } set { clips[51].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 53
        /****************************************************************************/
        [DisplayName("53.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 53.")]
        public String AnimationClip53 { get { return clips[52].Name; } set { clips[52].Name = value; } }

        [DisplayName("53.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 53.")]
        public double AnimationClip53Duration { get { return clips[52].Duration; } set { clips[52].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 54
        /****************************************************************************/
        [DisplayName("54.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 54.")]
        public String AnimationClip54 { get { return clips[53].Name; } set { clips[53].Name = value; } }

        [DisplayName("54.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 54.")]
        public double AnimationClip54Duration { get { return clips[53].Duration; } set { clips[53].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 55
        /****************************************************************************/
        [DisplayName("55.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 55.")]
        public String AnimationClip55 { get { return clips[54].Name; } set { clips[54].Name = value; } }

        [DisplayName("55.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 55.")]
        public double AnimationClip55Duration { get { return clips[54].Duration; } set { clips[54].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 56
        /****************************************************************************/
        [DisplayName("56.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 56.")]
        public String AnimationClip56 { get { return clips[55].Name; } set { clips[55].Name = value; } }

        [DisplayName("56.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 56.")]
        public double AnimationClip56Duration { get { return clips[55].Duration; } set { clips[55].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 57
        /****************************************************************************/
        [DisplayName("57.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 57.")]
        public String AnimationClip57 { get { return clips[56].Name; } set { clips[56].Name = value; } }

        [DisplayName("57.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 57.")]
        public double AnimationClip57Duration { get { return clips[56].Duration; } set { clips[56].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 58
        /****************************************************************************/
        [DisplayName("58.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 58.")]
        public String AnimationClip58 { get { return clips[57].Name; } set { clips[57].Name = value; } }

        [DisplayName("58.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 58.")]
        public double AnimationClip58Duration { get { return clips[57].Duration; } set { clips[57].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 59
        /****************************************************************************/
        [DisplayName("59.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 59.")]
        public String AnimationClip59 { get { return clips[58].Name; } set { clips[58].Name = value; } }

        [DisplayName("59.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 59.")]
        public double AnimationClip59Duration { get { return clips[58].Duration; } set { clips[58].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 60
        /****************************************************************************/
        [DisplayName("60.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 60.")]
        public String AnimationClip60 { get { return clips[59].Name; } set { clips[59].Name = value; } }

        [DisplayName("60.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 60.")]
        public double AnimationClip60Duration { get { return clips[59].Duration; } set { clips[59].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 61
        /****************************************************************************/
        [DisplayName("61.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 61.")]
        public String AnimationClip61 { get { return clips[60].Name; } set { clips[60].Name = value; } }

        [DisplayName("61.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 61.")]
        public double AnimationClip61Duration { get { return clips[60].Duration; } set { clips[60].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 62
        /****************************************************************************/
        [DisplayName("62.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 62.")]
        public String AnimationClip62 { get { return clips[61].Name; } set { clips[61].Name = value; } }

        [DisplayName("62.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 62.")]
        public double AnimationClip62Duration { get { return clips[61].Duration; } set { clips[61].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 63
        /****************************************************************************/
        [DisplayName("63.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 63.")]
        public String AnimationClip63 { get { return clips[62].Name; } set { clips[62].Name = value; } }

        [DisplayName("63.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 63.")]
        public double AnimationClip63Duration { get { return clips[62].Duration; } set { clips[62].Duration = value; } }
        /****************************************************************************/


        /****************************************************************************/
        /// Animation Clip 64
        /****************************************************************************/
        [DisplayName("64.1 Animation Clip")]
        [DefaultValue("")]
        [Description("Name of Animation Clip 64.")]
        public String AnimationClip64 { get { return clips[63].Name; } set { clips[63].Name = value; } }

        [DisplayName("64.2 Animation Clip: Duration")]
        [DefaultValue(0)]
        [Description("Duration of Animation Clip 64.")]
        public double AnimationClip64Duration { get { return clips[63].Duration; } set { clips[63].Duration = value; } }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// AnimationClipSettings
    /********************************************************************************/
    struct AnimationClipSettings
    {
        public String Name; 
        public double Duration;
    }
    /********************************************************************************/

}
/************************************************************************************/