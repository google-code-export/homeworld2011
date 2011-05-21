using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngineSkinning;

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
        internal static Renderer renderer = null;
        private Techniques technique;
       
        private List<String> subscribedAnimations = new List<String>();
        private Vector3[] corners = new Vector3[8];
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
            TimeRatio      = timeRatio;

            BlendTime       = blendTime;
            BlendDuration   = blendDuration;
            Pause           = pause;
            Blend           = blend;

            currentAnimation = new AnimationPlayer(model.SkinningData, currentClip, currentTime, currentKeyframe, OnAnimationEnd);
            blendAnimation   = new AnimationPlayer(model.SkinningData, blendClip, blendClipTime, blendKeyframe, OnAnimationEnd);
            
            renderer.batchedSkinnedMeshes.AddSkinnedMeshComponent(technique, this);

            AnimationControl = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {            
            renderer.batchedSkinnedMeshes.RemoveSkinnedMeshComponent(technique, this);
            base.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Start Clip
        /****************************************************************************/
        public void StartClip(String name)
        {
            currentAnimation.Start(name);

            Pause = false;
            Blend = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Current Clip
        /****************************************************************************/
        public String CurrentClip
        {
            get
            {
                if (Blend)
                {
                    if (blendAnimation.Clip != null) return blendAnimation.Clip.Name;
                }

                if (currentAnimation.Clip != null) return currentAnimation.Clip.Name;

                return String.Empty;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Blend 
        /****************************************************************************/
        public void BlendTo(String animation, TimeSpan duration)
        {
            if (Blend)
            {
                AnimationPlayer tmp = currentAnimation;
                currentAnimation = blendAnimation;
                blendAnimation = tmp;
            }

            Blend = true;

            BlendDuration = duration;
            BlendTime = TimeSpan.Zero;

            blendAnimation.Start(animation);
        }   
        /****************************************************************************/


        /****************************************************************************/
        /// Play Clip
        /****************************************************************************/
        public void PlayClip(String name)
        {
            if (currentAnimation.Clip != null)
            {
                if (currentAnimation.Clip.Name.Equals(name))
                {
                    Pause = false;
                    return;
                }
            }

            Pause = false;
            Blend = false;

            currentAnimation.Start(name);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pause
        /****************************************************************************/
        public void PauseAnimation()
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
            currentAnimation.Stop();
            Blend = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Reset
        /****************************************************************************/
        public void Reset()
        {
            if (currentAnimation.Clip == null) return;
            currentAnimation.Start(currentAnimation.Clip.Name);

            Blend = false;
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
        /// On Animation End
        /****************************************************************************/
        public void OnAnimationEnd(String animation)
        {
            if (subscribedAnimations.Contains(animation))
            {
                gameObject.SendEvent(new AnimationEndEvent(animation),
                                     EventsSystem.Priority.Normal,
                                     gameObject);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update(TimeSpan time, Matrix rootTransform)
        {
            if (!AnimationControl) return;

            TimeSpan deltaTime;

            if (Pause) deltaTime = TimeSpan.Zero;
            else       deltaTime = TimeSpan.FromTicks((TimeRatio >= 1 ? time.Ticks * (long)TimeRatio : time.Ticks / (long)(1 / TimeRatio)));

            currentAnimation.Update(deltaTime, rootTransform);

            if (Blend)
            {            
                BlendTime += time;

                if (BlendTime > BlendDuration)
                {
                    Blend = false;
                    
                    AnimationPlayer tmp = currentAnimation;
                    
                    currentAnimation = blendAnimation;                    
                    blendAnimation   = tmp;
                    
                    blendAnimation.Stop();                    

                    return;
                }

                blendAnimation.Update(deltaTime,rootTransform);

                float BlendRatio = (float)BlendTime.TotalSeconds / (float)BlendDuration.TotalSeconds;

                for (int bone = 0; bone < currentAnimation.SkinTransforms.Length; bone++)
                {
                    currentAnimation.WorldTransforms[bone] = Matrix.Lerp(currentAnimation.WorldTransforms[bone], blendAnimation.WorldTransforms[bone], BlendRatio);
                    currentAnimation.SkinTransforms [bone] = Matrix.Lerp(currentAnimation.SkinTransforms [bone], blendAnimation.SkinTransforms [bone], BlendRatio);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update Bone Transforms
        /****************************************************************************/
        public void UpdateBoneTransforms(TimeSpan time)
        {
            if (!AnimationControl) return;

            TimeSpan deltaTime;

            if (Pause) deltaTime = TimeSpan.Zero;
            else       deltaTime = TimeSpan.FromTicks((TimeRatio >= 1 ? time.Ticks * (long)TimeRatio : time.Ticks / (long)(1 / TimeRatio)));

            currentAnimation.UpdateBoneTransforms(deltaTime);

            if(Blend) blendAnimation.UpdateBoneTransforms(deltaTime);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update Bone Transforms
        /****************************************************************************/
        public void UpdateBoneTransforms(Matrix[] WorldTransforms)
        {
            currentAnimation.WorldTransforms = WorldTransforms;
            currentAnimation.UpdateSkinTransforms();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Bounding Box
        /****************************************************************************/
        public BoundingBox BoundingBox
        {
            get
            {
                Vector3.Transform(Model.BoundingBox.GetCorners().ToArray(), ref gameObject.World, corners);
                return BoundingBox.CreateFromPoints(corners);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Matrix[] SkinTransforms  { get { return currentAnimation.SkinTransforms;  } }
        public Matrix[] WorldTransforms { get { return currentAnimation.WorldTransforms; } }

        public PlagueEngineSkinnedModel Model           { get; private set; }
        public TexturesPack             Textures        { get; private set; }        
        
        public bool                     Pause           { get; private set; }
        public bool                     Blend           { get; private set; }
        public TimeSpan                 BlendDuration   { get; private set; }
        public TimeSpan                 BlendTime       { get; private set; }

        public AnimationPlayer currentAnimation { get; private set; }
        public AnimationPlayer blendAnimation   { get; private set; }

        public float TimeRatio { get; set; }
        
        public List<int>                SkeletonHierarchy { get { return Model.SkinningData.SkeletonHierarchy; } }
        public Dictionary<String, int>  BoneMap           { get { return Model.SkinningData.BoneMap;           } }
        
        public bool AnimationControl { get; set; }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/