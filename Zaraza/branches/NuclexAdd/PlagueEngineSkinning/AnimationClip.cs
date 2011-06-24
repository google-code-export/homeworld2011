using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;


/************************************************************************************/
/// PlagueEngineModelPipeline
/************************************************************************************/
namespace PlagueEngineSkinning
{

    /********************************************************************************/
    /// Animation Clip
    /********************************************************************************/
    public class AnimationClip
    {

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public AnimationClip(TimeSpan duration, List<Keyframe> keyframes,bool loop,String name)
        {
            Keyframes = keyframes;
            Loop      = loop;
            Name      = name;
            Duration  = duration;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor (for use only by the XNB deserializer)
        /****************************************************************************/
        private AnimationClip()
        { 
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        [ContentSerializer]
        public TimeSpan Duration        { get; private set; }
        [ContentSerializer]
        public List<Keyframe> Keyframes { get; private set; }
        [ContentSerializer]
        public bool Loop                { get; private set; }
        [ContentSerializer]
        public String Name              { get; private set; }        
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/