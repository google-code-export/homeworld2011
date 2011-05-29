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
            SoundEffect.DistanceScale = 100f;
        }

        public SoundCue(float volume, float pitch, float pan, SoundEffect soundEffect, float distaceScale)
        {
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            SoundEffect = soundEffect;
            SoundEffect.DistanceScale = distaceScale;
        }
    }
}
