using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PlagueEngine.Audio.Components
{
    class SoundEffectComponent
    {
        private readonly Dictionary<string, Dictionary<string, SoundCue>> _sounds = new Dictionary<string, Dictionary<string, SoundCue>>();
        private readonly Dictionary<string, SoundEffectInstance> _playingSounds;
        private readonly AudioEmitter _emitter;
        private readonly AudioManager _audioManager;
        private readonly Random _random;

        public SoundEffectComponent()
        {
            _random = new Random();
            _audioManager = AudioManager.GetInstance;
            _emitter = new AudioEmitter();
            _playingSounds = new Dictionary<string, SoundEffectInstance>();
        }

        public void LoadSound(string soundEffectGroup, string soundEffectName, string soundName, float volume, float pitch, float pan)
        {
            var sf = _audioManager.LoadSound(soundName);
            if (sf == null) return;
            if (!_sounds.ContainsKey(soundEffectGroup))
            {
                _sounds.Add(soundEffectGroup, new Dictionary<string, SoundCue>());
            }
            _sounds[soundEffectGroup].Add(soundEffectName, new SoundCue(volume, pitch, pan, sf)); ;
        }

        public void LoadFolder(string folderName, float volume, float pitch, float pan)
        {
            var dir = new DirectoryInfo("Content\\" + _audioManager.ContentFolder + "\\" + folderName);
            if (!dir.Exists) return;
            var xbnFiles = dir.GetFiles("*.xnb");
            foreach (var soundName in xbnFiles.Select(file => file.Name.Substring(0, file.Name.Length - file.Extension.Length)))
            {
                LoadSound(dir.Name, soundName, folderName + "\\" + soundName, volume, pitch, pan);
            }
        }

        public void LoadFolderTree(string folderName, float volume, float pitch, float pan, int maxDepth)
        {
            if (maxDepth < 0) return;
            var dir = new DirectoryInfo("Content\\" + _audioManager.ContentFolder + "\\" + folderName);
            if (!dir.Exists) return;
            var dirFiles = dir.GetDirectories();
            var xbnFiles = dir.GetFiles("*.xnb");
            foreach (var directoryInfo in dirFiles)
            {
                LoadFolderTree(folderName + "\\" + directoryInfo.Name, volume, pitch, pan, maxDepth - 1);
            }
            foreach (var soundName in xbnFiles.Select(file => file.Name.Substring(0, file.Name.Length - file.Extension.Length)))
            {
                LoadSound(dir.Name, soundName, folderName + "\\" + soundName, volume, pitch, pan);
            }
#if DEBUG
            if (xbnFiles.Length == 0)
                Diagnostics.PushLog("W folderze " + folderName + " nie ma plików .xba");
#endif
        }

        /// <summary>
        /// Odtwarza dźwięk o podanej nazwie
        /// </summary>
        /// <param name="soundName">Nazwa dźwięku</param>
        public void PlaySound(string soundGroup, string soundName)
        {
            SoundCue sound;
            if (!_sounds.ContainsKey(soundGroup) || !_sounds[soundGroup].TryGetValue(soundName, out sound))
            {
#if DEBUG
                Diagnostics.PushLog("Dźwięk " + soundName + " lub grupa "+soundGroup+" nie istnieje.");
#endif
                return;
            }
            PlaySound(sound,soundName);
        }

        private void PlaySound(SoundCue sound, string soundName)
        {
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

        public void PlayRandomSound(string soundGroup)
        {
            if (!_sounds.ContainsKey(soundGroup) || _sounds[soundGroup].Values.Count==0)
            {
#if DEBUG
                Diagnostics.PushLog("Grupa " + soundGroup + " nie istnieje lub nie zawiera dźwięków");
#endif
                return;
            }
            var soundId = _random.Next(_sounds[soundGroup].Values.Count - 1);
            var sound = _sounds[soundGroup].Values.ElementAt(soundId);
            if(sound.SoundEffect!=null)
            {
                PlaySound(sound, _sounds[soundGroup].Keys.ElementAt(soundId));
            }
            

        }
        public void SetPosiotion(Vector3 position)
        {
            _emitter.Position = position;
        }

        public void StopSound(string soundName)
        {
            SoundEffectInstance soundEffectInstance;
            if (!_playingSounds.TryGetValue(soundName,out soundEffectInstance))
            {
#if DEBUG
                Diagnostics.PushLog("Obecnie dźwięk " + soundName + " nie jest odtwarzany");
#endif
                return;
            }
            soundEffectInstance.Stop();
            soundEffectInstance.Dispose();
        }

        public bool IsPlaying()
        {
            var index = _playingSounds.Values.Count(soundEffectInstance => soundEffectInstance != null && !soundEffectInstance.IsDisposed);
            return index > 0;
        }
        /// <summary>
        /// Zatrzymuje wszystkie obecne dźwięki
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
