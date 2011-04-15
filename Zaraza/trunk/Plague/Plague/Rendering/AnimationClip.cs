using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;


/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// Animation Clip
    /********************************************************************************/
    public class AnimationClip
    {

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public AnimationClip(TimeSpan start,TimeSpan end, List<Keyframe> keyframes,bool loop)
        {
            StartTime = start;
            EndTime   = end;
            Keyframes = keyframes;
            Loop      = loop;

            Duration = EndTime - StartTime;
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
        public TimeSpan StartTime       { get; private set; }
        [ContentSerializer]
        public TimeSpan EndTime         { get; private set; }
        [ContentSerializer]
        public List<Keyframe> Keyframes { get; private set; }
        [ContentSerializer]
        public bool Loop                { get; private set; }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/