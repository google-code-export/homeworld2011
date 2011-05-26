using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace PlagueEngine.Audio
{
    public struct SoundCue
    {
        public float Volume;
        public float Pitch;
        public float Pan;
        public SoundEffect SoundEffect;


        public SoundCue(float volume, float pitch, float pan, SoundEffect soundEffect)
        {
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            SoundEffect = soundEffect;
        }
    }
}
