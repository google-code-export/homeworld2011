using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// Skinning Data
    /********************************************************************************/
    public class SkinningData
    {

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SkinningData(Dictionary<string, AnimationClip> animationClips,
                            List<Matrix> bindPose,
                            List<Matrix> inverseBindPose,
                            List<int> skeletonHierarchy)
        {
            AnimationClips    = animationClips;
            BindPose          = bindPose;
            InverseBindPose   = inverseBindPose;
            SkeletonHierarchy = skeletonHierarchy;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor (for use only by the XNB deserializer)
        /****************************************************************************/
        private SkinningData()
        { 
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        [ContentSerializer]
        public Dictionary<String, AnimationClip> AnimationClips { get; private set; }
        [ContentSerializer]
        public List<Matrix> BindPose                            { get; private set; }
        [ContentSerializer]
        public List<Matrix> InverseBindPose                     { get; private set; }
        [ContentSerializer]
        public List<int> SkeletonHierarchy                      { get; private set; }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/