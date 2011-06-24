using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PlagueEngineSkinning;


/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// AnimationPlayer
    /********************************************************************************/
    class AnimationPlayer
    {

        /****************************************************************************/
        /// Delegates
        /****************************************************************************/
        public delegate void OnAnimationEndDelegate(String animation);
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public AnimationClip Clip        { get; private set; }
        public TimeSpan      ClipTime    { get; private set; }
        public int           Keyframe    { get; private set; }

        public Matrix[] BoneTransforms   { get; private set; }
        public Matrix[] WorldTransforms  { get; set; }
        public Matrix[] SkinTransforms   { get; set; }

        public SkinningData skinningData { get; private set; }

        public OnAnimationEndDelegate OnAnimationEnd { get; set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public AnimationPlayer(SkinningData           skinningData,
                               String                 clip,
                               TimeSpan               clipTime,
                               int                    keyframe,
                               OnAnimationEndDelegate onAnimationEnd)
        {
            this.skinningData = skinningData;

            BoneTransforms  = new Matrix[skinningData.BindPose.Count];
            WorldTransforms = new Matrix[skinningData.BindPose.Count];
            SkinTransforms  = new Matrix[skinningData.BindPose.Count];

            ClipTime       = clipTime;
            Keyframe       = keyframe;
            OnAnimationEnd = onAnimationEnd;

            if (!String.IsNullOrEmpty(clip)) Clip = skinningData.AnimationClips[clip];
            else Clip = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Start
        /****************************************************************************/
        public void Start(String name)
        {
            Clip     = skinningData.AnimationClips[name];
            ClipTime = TimeSpan.Zero;
            Keyframe = 0;
            
            skinningData.BindPose.CopyTo(BoneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Stop 
        /****************************************************************************/
        public void Stop()
        {
            Clip     = null;
            ClipTime = TimeSpan.Zero;
            Keyframe = 0;

            skinningData.BindPose.CopyTo(BoneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update(TimeSpan time, Matrix rootTransform)
        {
            UpdateBoneTransforms(time);
            UpdateWorldTransforms(rootTransform);
            UpdateSkinTransforms();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update Bone Transforms
        /****************************************************************************/
        public void UpdateBoneTransforms(TimeSpan time)
        {
            if (Clip == null) return;

            ClipTime += time;

            /***************/
            // Looping
            /***************/
            if (ClipTime >= Clip.Duration)
            {
                OnAnimationEnd(Clip.Name);

                if (Clip.Loop)
                {
                    while (ClipTime >= Clip.Duration) ClipTime -= Clip.Duration;                    
                    Keyframe = 0;
                    skinningData.BindPose.CopyTo(BoneTransforms);
                }
                else
                {                    
                    IList<Keyframe> keyframesu = Clip.Keyframes;
                    
                    for (int i = Keyframe; i < keyframesu.Count; i++)
                    {
                        Keyframe keyframe = keyframesu[i];
                        BoneTransforms[keyframe.Bone] = keyframe.Transform;
                    }

                    Keyframe = 0;
                    Clip = null;
                    ClipTime = TimeSpan.Zero;
                    return;
                }
            }
            /***************/

            IList<Keyframe> keyframes = Clip.Keyframes;

            while (Keyframe < keyframes.Count)
            {
                Keyframe keyframe = keyframes[Keyframe];

                if (keyframe.Time > ClipTime) break;

                BoneTransforms[keyframe.Bone] = keyframe.Transform;
                Keyframe++;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update World Transforms
        /****************************************************************************/
        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            WorldTransforms[0] = BoneTransforms[0] * rootTransform;

            for (int bone = 1; bone < WorldTransforms.Length; bone++)
            {
                int parentBone = skinningData.SkeletonHierarchy[bone];

                WorldTransforms[bone] = BoneTransforms[bone] * WorldTransforms[parentBone];
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update Skin Transforms
        /****************************************************************************/
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < SkinTransforms.Length; bone++)
            {
                SkinTransforms[bone] = skinningData.InverseBindPose[bone] * WorldTransforms[bone];
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/