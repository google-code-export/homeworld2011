using Microsoft.Xna.Framework.Audio;

namespace PlagueEngine.Audio
{
    public struct SoundCue
    {
        public float Volume;
        public float Pitch;
        public float Pan;
        public bool IsLooped;
        public bool AllowMultiInstancing;
        public SoundEffect SoundEffect;


        public SoundCue(float volume, float pitch, float pan, SoundEffect soundEffect)
        {
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            SoundEffect = soundEffect;
            AllowMultiInstancing = false;
            IsLooped = false;
            SoundEffect.DistanceScale = 1000f;
        }

        public SoundCue(float volume, float pitch, float pan, SoundEffect soundEffect, bool allowMultiInstancing)
        {
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            SoundEffect = soundEffect;
            AllowMultiInstancing = allowMultiInstancing;
            IsLooped = false;
            SoundEffect.DistanceScale = 1000f;
        }

        public SoundCue(float volume, float pitch, float pan, SoundEffect soundEffect, bool allowMultiInstancing, float distaceScale,bool isLooped)
        {
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            SoundEffect = soundEffect;
            AllowMultiInstancing = allowMultiInstancing;
            IsLooped = isLooped;
            SoundEffect.DistanceScale = distaceScale;
        }
    }
}
