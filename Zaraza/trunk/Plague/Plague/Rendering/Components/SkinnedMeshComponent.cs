using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PlagueEngine.LowLevelGameFlow;


/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// Skinned MeshC omponent
    /********************************************************************************/
    class SkinnedMeshComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private SkinningData    skinningData    = null;
        private bool            pause           = false;
        private bool            blend           = false;
        private TimeSpan        blendDuration   = TimeSpan.Zero;
        private TimeSpan        blendTime       = TimeSpan.Zero;

        internal static Renderer renderer = null;

        private Techniques technique;

        private Matrix[]      boneTransforms;
        private Matrix[]      worldTransforms;
        private Matrix[]      skinTransforms;
        private AnimationClip currentClip     = null;
        private TimeSpan      currentTime     = TimeSpan.Zero;
        private int           currentKeyframe = 0;
        
        private Matrix[]      boneBlendTransforms;
        private Matrix[]      worldBlendTransforms;
        private Matrix[]      skinBlendTransforms;
        private AnimationClip blendClip     = null;
        private TimeSpan      blendClipTime = TimeSpan.Zero;
        private int           blendKeyframe = 0;
        
        private List<String> subscribedAnimations = new List<String>();
        /****************************************************************************/
        

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SkinnedMeshComponent(GameObjectInstance gameObject,
                                    PlagueEngineSkinnedModel model,
                                    TexturesPack textures,
                                    Techniques technique)
            : base(gameObject)
        {
            Model          = model;
            Textures       = textures;
            this.technique = technique;
            skinningData   = model.SkinningData;

            boneTransforms  = new Matrix[skinningData.BindPose.Count];
            worldTransforms = new Matrix[skinningData.BindPose.Count];
            skinTransforms  = new Matrix[skinningData.BindPose.Count];

            boneBlendTransforms  = new Matrix[skinningData.BindPose.Count];
            worldBlendTransforms = new Matrix[skinningData.BindPose.Count];
            skinBlendTransforms  = new Matrix[skinningData.BindPose.Count];

            TimeRatio = 1.0f;

            skinningData.BindPose.CopyTo(boneTransforms);   

            renderer.batchedSkinnedMeshes.AddSkinnedMeshComponent(technique, this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.batchedSkinnedMeshes.RemoveSkinnedMeshComponent(technique, this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Start Clip
        /****************************************************************************/
        public void StartClip(String name)
        {
            currentClip = skinningData.AnimationClips[name];
            currentTime = TimeSpan.Zero;
            currentKeyframe = 0;
            blend = false;
            pause = false;

            skinningData.BindPose.CopyTo(boneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Blend 
        /****************************************************************************/
        public void Blend(String animation, TimeSpan duration)
        {
            blend = true;
            blendDuration   = duration;
            blendClip       = skinningData.AnimationClips[animation];
            blendClipTime   = TimeSpan.Zero;
            blendTime       = TimeSpan.Zero;
            blendKeyframe   = 0;
            skinningData.BindPose.CopyTo(boneBlendTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Play Clip
        /****************************************************************************/
        public void PlayClip(String name)
        {
            if (currentClip != null)
            {
                pause = false;

                if (currentClip.Name.Equals(name)) return;
            }

            currentClip     = skinningData.AnimationClips[name];
            currentTime     = TimeSpan.Zero;
            currentKeyframe = 0;
            skinningData.BindPose.CopyTo(boneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pause
        /****************************************************************************/
        public void Pause()
        {
            pause = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Is Paused
        /****************************************************************************/
        public bool IsPaused()
        {
            return pause;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Resume 
        /****************************************************************************/
        public void Resume()
        {
            pause = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Stop
        /****************************************************************************/
        public void Stop()
        {
            currentClip = null;
            blend       = false;

            skinningData.BindPose.CopyTo(boneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Reset
        /****************************************************************************/
        public void Reset()
        {
            if (currentClip == null) return;

            blend           = false;

            pause           = false;
            currentTime     = TimeSpan.Zero;
            currentKeyframe = 0;

            skinningData.BindPose.CopyTo(boneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Animations End
        /****************************************************************************/
        public void SubscribeAnimationsEnd(params String[] animations)
        {
            subscribedAnimations.AddRange(animations);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Animations End Subscription
        /****************************************************************************/
        public void CancelAnimationsEndSubscription(params String[] animations)
        {
            foreach (String animation in animations)
            {
                subscribedAnimations.Remove(animation);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update(TimeSpan time, Matrix rootTransform)
        {
            if (pause) return;

            UpdateBoneTransforms(time);
            UpdateWorldTransforms(rootTransform);
            UpdateSkinTransforms();

            if (blend)
            {
                blendTime += TimeSpan.FromTicks((TimeRatio >= 1 ? time.Ticks * (long)TimeRatio : time.Ticks / (long)(1 / TimeRatio))); ;
                
                if (blendTime > blendDuration)
                {
                    blend           = false;
                    currentClip     = blendClip;
                    currentTime     = blendTime;
                    currentKeyframe = blendKeyframe;
                    
                    return;
                }

                UpdateBoneBlendTransforms(time);
                UpdateWorldBlendTransforms(rootTransform);
                UpdateSkinBlendTransforms();

                float BlendRatio = (float)(blendTime.TotalSeconds / blendDuration.TotalSeconds);

                for (int bone = 0; bone < skinBlendTransforms.Length; bone++)
                {
                    skinTransforms[bone] = Matrix.Lerp(skinTransforms[bone],skinBlendTransforms[bone],BlendRatio);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update Bone Transforms
        /****************************************************************************/
        public void UpdateBoneTransforms(TimeSpan time)
        {
            if (currentClip == null) return;

            currentTime += TimeSpan.FromTicks((TimeRatio >= 1 ? time.Ticks * (long)TimeRatio : time.Ticks / (long)(1 / TimeRatio)));

            /***************/
            // Looping
            /***************/
            if (currentTime >= currentClip.Duration)
            {
                if (subscribedAnimations.Contains(currentClip.Name))
                {
                    gameObject.SendEvent(new AnimationEndEvent(currentClip.Name), 
                                         EventsSystem.Priority.Normal, 
                                         gameObject);
                }

                if (currentClip.Loop)
                {
                    while (currentTime >= currentClip.Duration)
                    {
                        currentTime -= currentClip.Duration;
                    }

                    currentKeyframe = 0;
                }
                else
                {
                    currentKeyframe = 0;
                    currentClip = null;
                    currentTime = TimeSpan.Zero;
                    return;
                }
            }
            /***************/

            IList<Keyframe> keyframes = currentClip.Keyframes;

            while (currentKeyframe < keyframes.Count)
            {
                Keyframe keyframe = keyframes[currentKeyframe];

                if (keyframe.Time > currentTime) break;

                boneTransforms[keyframe.Bone] = keyframe.Transform;

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
        /// Update Bone Blend Transforms
        /****************************************************************************/
        public void UpdateBoneBlendTransforms(TimeSpan time)
        {
            if (blendClip == null) return;

            blendClipTime += TimeSpan.FromTicks((TimeRatio >= 1 ? time.Ticks * (long)TimeRatio : time.Ticks / (long)(1 / TimeRatio)));

            /***************/
            // Looping
            /***************/
            if (blendClipTime >= blendClip.Duration)
            {
                if (subscribedAnimations.Contains(blendClip.Name))
                {
                    gameObject.SendEvent(new AnimationEndEvent(blendClip.Name),
                                         EventsSystem.Priority.Normal,
                                         gameObject);
                }

                if (blendClip.Loop)
                {
                    while (blendClipTime >= blendClip.Duration)
                    {
                        blendClipTime -= blendClip.Duration;
                    }

                    blendKeyframe = 0;
                }
                else
                {
                    blendKeyframe = 0;
                    blendClip = null;
                    blendClipTime = TimeSpan.Zero;
                    return;
                }
            }
            /***************/

            IList<Keyframe> keyframes = blendClip.Keyframes;

            while (blendKeyframe < keyframes.Count)
            {
                Keyframe keyframe = keyframes[blendKeyframe];

                if (keyframe.Time > blendClipTime) break;

                boneBlendTransforms[keyframe.Bone] = keyframe.Transform;

                blendKeyframe++;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update World Transforms
        /****************************************************************************/
        public void UpdateWorldBlendTransforms(Matrix rootTransform)
        {
            worldBlendTransforms[0] = boneBlendTransforms[0] * rootTransform;

            for (int bone = 1; bone < worldBlendTransforms.Length; bone++)
            {
                int parentBone = skinningData.SkeletonHierarchy[bone];

                worldBlendTransforms[bone] = boneBlendTransforms[bone] * worldBlendTransforms[parentBone];
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update Skin Transforms
        /****************************************************************************/
        public void UpdateSkinBlendTransforms()
        {
            for (int bone = 0; bone < skinBlendTransforms.Length; bone++)
            {
                skinBlendTransforms[bone] = skinningData.InverseBindPose[bone] * worldBlendTransforms[bone];
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

        public PlagueEngineSkinnedModel Model           { get; private set; }
        public TexturesPack             Textures        { get; private set; }
        public float                    TimeRatio       { get; set; }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/