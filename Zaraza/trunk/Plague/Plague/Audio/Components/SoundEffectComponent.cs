using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PlagueEngine.Audio.Components
{
    class SoundEffectComponent
    {
        private readonly Dictionary<string, SoundCue> _sounds = new Dictionary<string, SoundCue>();
        private Dictionary<string, SoundEffectInstance> _playingSounds;
        private readonly AudioEmitter _emitter;
        private readonly AudioManager _audioManager;
        public SoundEffectComponent()
        {
            _audioManager = AudioManager.GetInstance;
            _emitter = new AudioEmitter();
            _playingSounds = new Dictionary<string, SoundEffectInstance>();
        }

        public void CreateNewSound(string soundEffectName, string soundName, float volume, float pitch, float pan)
        {
            var sf = _audioManager.LoadSound(soundName);
            if (sf != null)
            {
                _sounds.Add(soundEffectName, new SoundCue(volume, pitch, pan, sf));
            }
        }

        public void CreateNewSoundFromFolder(string folderName, float volume, float pitch, float pan)
        {
            var dir = new DirectoryInfo("Content\\" + _audioManager.ContentFolder + "\\" + folderName);
            if (!dir.Exists) return;
            var xbnFiles = dir.GetFiles("*.xnb");
            foreach (var soundName in xbnFiles.Select(file => file.Name.Substring(0, file.Name.Length - file.Extension.Length)))
            {
                CreateNewSound(soundName, folderName + "\\" + soundName, volume, pitch, pan);
            }

        }
        /// <summary>
        /// Plays the sound of the given name.
        /// </summary>
        /// <param name="soundName">Name of the sound</param>
        public void PlaySound(string soundName)
        {
            SoundCue sound;

            if (!_sounds.TryGetValue(soundName, out sound))
            {
#if DEBUG
                Diagnostics.PushLog("Dźwięk " + soundName + " nie istnieje");
#endif
                return;
            }
            if (!sound.AllowMultiInstancing && _playingSounds.ContainsKey(soundName))
            {
                var playedSound = _playingSounds[soundName];
                if (playedSound!=null)
                {
                    if (playedSound.IsDisposed)
                    {
                        _playingSounds.Remove(soundName);
                    }
                    else
                    {
#if DEBUG
                        Diagnostics.PushLog("Dźwięk " + soundName + " nie pozwala na tworzenie jego wielu instacji.");
#endif
                        return;
                    }
                }
            }
            var sei = _audioManager.PlaySound(sound, _emitter);
            if (!sound.AllowMultiInstancing && sei != null)
            {
                _playingSounds.Add(soundName, sei);
            }
        }

        public void SetPosiotion(Vector3 position)
        {
            _emitter.Position = position;
        }
        /// <summary>
        /// Stops all currently playing sounds.
        /// </summary>
        public void StopAllSounds()
        {
            foreach (var soundEffectInstance in _playingSounds.Values)
            {
                soundEffectInstance.Stop();
                soundEffectInstance.Dispose();
            }
            _playingSounds.Clear();
        }
    }
}
