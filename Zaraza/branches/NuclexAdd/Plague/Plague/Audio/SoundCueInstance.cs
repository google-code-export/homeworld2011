using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using PlagueEngine.Audio.Components;
using Microsoft.Xna.Framework;

namespace PlagueEngine.Audio
{
    class SoundCueInstance
    {
        public SoundEffectInstance Instance;
        public SoundEffectComponent SoundEffectComponent;
        public float MaxDistance;
        private float _instanceVolume;
        public bool IsDisposed;
        public SoundCueInstance(SoundEffectComponent soundEffectComponent, SoundEffectInstance instance, float maxDistance)
        {
            MaxDistance = maxDistance;
            _instanceVolume = instance.Volume;
            SoundEffectComponent = soundEffectComponent;
            Instance = instance;
        }
        public void Update(AudioListener[] listeners)
        {
            if(SoundEffectComponent!= null && SoundEffectComponent.Emitter!=null);
            float distance;
            Vector3 lp = listeners[0].Position;
            Vector3 ep = SoundEffectComponent.Emitter.Position; 
            Vector3.Distance(ref lp, ref ep, out distance);
            if (MaxDistance > 0 && distance >= MaxDistance)
            {
                Instance.Volume = 0.0f;
            }
            else if (Instance.Volume != _instanceVolume)
            {
                Instance.Volume = _instanceVolume;
            }
            Instance.Apply3D(listeners, SoundEffectComponent.Emitter);
        }
        public bool IsActive
        {

            get { return !IsDisposed && !Instance.IsDisposed && Instance.State != SoundState.Stopped; }
        }
        public void Dispose()
        {
            SoundEffectComponent = null;
            Instance.Dispose();
            Instance = null;
            IsDisposed = true;
        }
    }
}
