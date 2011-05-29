using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PlagueEngine.Audio.Components
{
    class SoundEffectComponent
    {
        private readonly Dictionary<string, SoundCue> _sounds = new Dictionary<string, SoundCue>();
        private List<SoundEffectInstance> _playingSounds;
        private AudioEmitter _emitter;
        private readonly AudioManager _audioManager;
        public SoundEffectComponent()
        {
            _audioManager = AudioManager.GetInstance;
            _emitter = new AudioEmitter();
            _playingSounds = new List<SoundEffectInstance>();
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
            foreach (var file in xbnFiles)
            {
                var soundName = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                var sf = _audioManager.LoadSound(folderName + "\\" + soundName);
                if (sf != null)
                {
                    _sounds.Add(soundName, new SoundCue(volume, pitch, pan, sf));
                }
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

            _audioManager.PlaySound(sound, _emitter);
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

        }
    }
}
