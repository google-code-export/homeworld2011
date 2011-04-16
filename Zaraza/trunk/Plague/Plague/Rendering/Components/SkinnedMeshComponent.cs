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

        internal static Renderer renderer = null;

        private Techniques technique;

        private Matrix[]      boneTransforms;
        private Matrix[]      worldTransforms;
        private Matrix[]      skinTransforms;
        
        private Matrix[]      boneBlendTransforms;
        private Matrix[]      worldBlendTransforms;
        private Matrix[]      skinBlendTransforms;
        
        private List<String> subscribedAnimations = new List<String>();
        /****************************************************************************/
        

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SkinnedMeshComponent(GameObjectInstance       gameObject,
                                    PlagueEngineSkinnedModel model,
                                    TexturesPack             textures,
                                    Techniques               technique,
                                    float                    timeRatio,
                                    String                   currentClip,
                                    int                      currentKeyframe,
                                    TimeSpan                 currentTime,
                                    bool                     pause,
                                    bool                     blend,
                                    String                   blendClip,
                                    int                      blendKeyframe,
                                    TimeSpan                 blendClipTime,
                                    TimeSpan                 blendDuration,
                                    TimeSpan                 blendTime)
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

            TimeRatio = timeRatio;

            if (!String.IsNullOrEmpty(currentClip)) CurrentClip = skinningData.AnimationClips[currentClip];
            if (!String.IsNullOrEmpty(blendClip))   BlendClip   = skinningData.AnimationClips[blendClip];

            CurrentKeyframe = currentKeyframe;
            CurrentTime     = currentTime;
            BlendKeyframe   = blendKeyframe;
            BlendClipTime   = blendClipTime;
            BlendTime       = blendTime;
            BlendDuration   = blendDuration;
            Pause           = pause;
            Blend           = blend;

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
            CurrentClip     = skinningData.AnimationClips[name];
            CurrentTime     = TimeSpan.Zero;
            CurrentKeyframe = 0;
            Blend           = false;
            Pause           = false;

            skinningData.BindPose.CopyTo(boneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Blend 
        /****************************************************************************/
        public void BlendTo(String animation, TimeSpan duration)
        {
            Blend           = true;
            BlendDuration   = duration;
            BlendClip       = skinningData.AnimationClips[animation];
            BlendClipTime   = TimeSpan.Zero;
            BlendTime       = TimeSpan.Zero;
            BlendKeyframe   = 0;
            skinningData.BindPose.CopyTo(boneBlendTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Play Clip
        /****************************************************************************/
        public void PlayClip(String name)
        {
            if (CurrentClip != null)
            {
                Pause = false;

                if (CurrentClip.Name.Equals(name)) return;
            }

            CurrentClip     = skinningData.AnimationClips[name];
            CurrentTime     = TimeSpan.Zero;
            CurrentKeyframe = 0;
            skinningData.BindPose.CopyTo(boneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pause
        /****************************************************************************/
        public void PauseClip()
        {
            Pause = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Is Paused
        /****************************************************************************/
        public bool IsPaused()
        {
            return Pause;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Resume 
        /****************************************************************************/
        public void Resume()
        {
            Pause = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Stop
        /****************************************************************************/
        public void Stop()
        {
            CurrentClip = null;
            Blend       = false;

            skinningData.BindPose.CopyTo(boneTransforms);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Reset
        /****************************************************************************/
        public void Reset()
        {
            if (CurrentClip == null) return;

            Blend           = false;

            Pause           = false;
            CurrentTime     = TimeSpan.Zero;
            CurrentKeyframe = 0;

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
            if (Pause) return;

            UpdateBoneTransforms(time);
            UpdateWorldTransforms(rootTransform);
            UpdateSkinTransforms();

            if (Blend)
            {
                BlendTime += TimeSpan.FromTicks((TimeRatio >= 1 ? time.Ticks * (long)TimeRatio : time.Ticks / (long)(1 / TimeRatio))); ;
                
                if (BlendTime > BlendDuration)
                {
                    Blend           = false;
                    CurrentClip     = BlendClip;
                    CurrentTime     = BlendTime;
                    CurrentKeyframe = BlendKeyframe;
                    
                    return;
                }

                UpdateBoneBlendTransforms(time);
                UpdateWorldBlendTransforms(rootTransform);
                UpdateSkinBlendTransforms();

                float BlendRatio = (float)(BlendTime.TotalSeconds / BlendDuration.TotalSeconds);

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
            if (CurrentClip == null) return;

            CurrentTime += TimeSpan.FromTicks((TimeRatio >= 1 ? time.Ticks * (long)TimeRatio : time.Ticks / (long)(1 / TimeRatio)));

            /***************/
            // Looping
            /***************/
            if (CurrentTime >= CurrentClip.Duration)
            {
                if (subscribedAnimations.Contains(CurrentClip.Name))
                {
                    gameObject.SendEvent(new AnimationEndEvent(CurrentClip.Name), 
                                         EventsSystem.Priority.Normal, 
                                         gameObject);
                }

                if (CurrentClip.Loop)
                {
                    while (CurrentTime >= CurrentClip.Duration)
                    {
                        CurrentTime -= CurrentClip.Duration;
                    }

                    CurrentKeyframe = 0;
                }
                else
                {
                    CurrentKeyframe = 0;
                    CurrentClip = null;
                    CurrentTime = TimeSpan.Zero;
                    return;
                }
            }
            /***************/

            IList<Keyframe> keyframes = CurrentClip.Keyframes;

            while (CurrentKeyframe < keyframes.Count)
            {
                Keyframe keyframe = keyframes[CurrentKeyframe];

                if (keyframe.Time > CurrentTime) break;

                boneTransforms[keyframe.Bone] = keyframe.Transform;

                CurrentKeyframe++;
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
            if (BlendClip == null) return;

            BlendClipTime += TimeSpan.FromTicks((TimeRatio >= 1 ? time.Ticks * (long)TimeRatio : time.Ticks / (long)(1 / TimeRatio)));

            /***************/
            // Looping
            /***************/
            if (BlendClipTime >= BlendClip.Duration)
            {
                if (subscribedAnimations.Contains(BlendClip.Name))
                {
                    gameObject.SendEvent(new AnimationEndEvent(BlendClip.Name),
                                         EventsSystem.Priority.Normal,
                                         gameObject);
                }

                if (BlendClip.Loop)
                {
                    while (BlendClipTime >= BlendClip.Duration)
                    {
                        BlendClipTime -= BlendClip.Duration;
                    }

                    BlendKeyframe = 0;
                }
                else
                {
                    BlendKeyframe = 0;
                    BlendClip     = null;
                    BlendClipTime = TimeSpan.Zero;
                    return;
                }
            }
            /***************/

            IList<Keyframe> keyframes = BlendClip.Keyframes;

            while (BlendKeyframe < keyframes.Count)
            {
                Keyframe keyframe = keyframes[BlendKeyframe];

                if (keyframe.Time > BlendClipTime) break;

                boneBlendTransforms[keyframe.Bone] = keyframe.Transform;

                BlendKeyframe++;
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
        public Matrix[]      SkinTransforms  { get { return skinTransforms;  } }

        public PlagueEngineSkinnedModel Model           { get; private set; }
        public TexturesPack             Textures        { get; private set; }        
        public bool                     Pause           { get; private set; }
        public bool                     Blend           { get; private set; }
        public TimeSpan                 BlendDuration   { get; private set; }
        public TimeSpan                 BlendTime       { get; private set; }
        public AnimationClip            CurrentClip     { get; private set; }
        public TimeSpan                 CurrentTime     { get; private set; }
        public int                      CurrentKeyframe { get; private set; }
        public AnimationClip            BlendClip       { get; private set; }
        public TimeSpan                 BlendClipTime   { get; private set; }
        public int                      BlendKeyframe   { get; private set; }

        public float TimeRatio { get; set; }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/