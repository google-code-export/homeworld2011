using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// Animation Player
    /********************************************************************************/
    class AnimationPlayer
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private SkinningData  skinningData    = null;
        private AnimationClip currentClip     = null;
        private TimeSpan      currentTime     = TimeSpan.Zero;
        private int           currentKeyframe = 0;

        private Matrix[] boneTransforms;
        private Matrix[] worldTransforms;
        private Matrix[] skinTransforms;        
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public AnimationPlayer(SkinningData skinningData)
        {
            this.skinningData = skinningData;

            boneTransforms  = new Matrix[skinningData.BindPose.Count];
            worldTransforms = new Matrix[skinningData.BindPose.Count];
            skinTransforms  = new Matrix[skinningData.BindPose.Count];

            skinningData.BindPose.CopyTo(boneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Start Clip
        /****************************************************************************/
        public void StartClip(AnimationClip clip)
        {
            currentClip     = clip;
            currentTime     = TimeSpan.Zero;
            currentKeyframe = 0;

            skinningData.BindPose.CopyTo(boneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Stop
        /****************************************************************************/
        public void Stop()
        {
            currentClip = null;
            skinningData.BindPose.CopyTo(boneTransforms);
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
            if (currentClip == null) return;

            currentTime += time;

            /***************/
            // Looping
            /***************/
            if (currentTime >= currentClip.Duration)
            {
                while (currentTime >= currentClip.Duration)
                {
                    currentTime -= currentClip.Duration;
                }

                currentKeyframe = 0;
                skinningData.BindPose.CopyTo(boneTransforms);
            }
            /***************/
            
            IList<Keyframe> keyframes = currentClip.Keyframes;

            while (currentKeyframe < keyframes.Count)
            {
                Keyframe keyframe = keyframes[currentKeyframe];

                if (keyframe.Time > currentTime) break;

                //if (keyframe.Bone == 2)
                {
                    boneTransforms[keyframe.Bone] = keyframe.Transform;
                }

                currentKeyframe++;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update World Transforms
        /****************************************************************************/
        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            worldTransforms[0] = boneTransforms[0] * rootTransform;

            for (int bone = 1; bone < worldTransforms.Length; bone++)
            {
                int parentBone = skinningData.SkeletonHierarchy[bone];

                worldTransforms[bone] = boneTransforms[bone] * worldTransforms[parentBone];
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update Skin Transforms
        /****************************************************************************/
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < skinTransforms.Length; bone++)
            {
                skinTransforms[bone] = skinningData.InverseBindPose[bone] * worldTransforms[bone];
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Matrix[]      BoneTransforms  { get { return boneTransforms;  } }
        public Matrix[]      WorldTransforms { get { return worldTransforms; } }
        public Matrix[]      SkinTransforms  { get { return skinTransforms;  } }
        public TimeSpan      CurrentTime     { get { return currentTime;     } }
        public AnimationClip CurrentClip     { get { return currentClip;     } }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/