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
        private AnimationClip   currentClip     = null;
        private TimeSpan        currentTime     = TimeSpan.Zero;
        private int             currentKeyframe = 0;
        private bool            pause           = false;

        internal static Renderer renderer = null;

        private Techniques technique;

        private Matrix[] boneTransforms;
        private Matrix[] worldTransforms;
        private Matrix[] skinTransforms;

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
            pause = false;

            skinningData.BindPose.CopyTo(boneTransforms);
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
            skinningData.BindPose.CopyTo(boneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Reset
        /****************************************************************************/
        public void Reset()
        {
            if (currentClip == null) return;

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