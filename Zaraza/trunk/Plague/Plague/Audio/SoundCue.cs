using Microsoft.Xna.Framework.Audio;

namespace PlagueEngine.Audio
{
    public struct SoundCue
    {
        public float Volume;
        public float Pitch;
        public float Pan;
        public bool AllowMultiInstancing;
        public SoundEffect SoundEffect;


        public SoundCue(float volume, float pitch, float pan, SoundEffect soundEffect)
        {
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            SoundEffect = soundEffect;
            AllowMultiInstancing = false;
            SoundEffect.DistanceScale = 100f;
        }

        public SoundCue(float volume, float pitch, float pan, SoundEffect soundEffect, bool allowMultiInstancing)
        {
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            SoundEffect = soundEffect;
            AllowMultiInstancing = allowMultiInstancing;
            SoundEffect.DistanceScale = 100f;
        }

        public SoundCue(float volume, float pitch, float pan, SoundEffect soundEffect, bool allowMultiInstancing, float distaceScale)
        {
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            SoundEffect = soundEffect;
            AllowMultiInstancing = allowMultiInstancing;
            SoundEffect.DistanceScale = distaceScale;
        }
    }
}
