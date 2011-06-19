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
            LoadSound(soundEffectGroup, soundEffectName, soundName, volume, pitch, pan, false);
        }
        public void LoadSound(string soundEffectGroup, string soundEffectName, string soundName, float volume, float pitch, float pan, bool allowMultiInstancing)
        {
            var sf = _audioManager.LoadSound(soundName);
            if (sf == null) return;
            if (!_sounds.ContainsKey(soundEffectGroup))
            {
                _sounds.Add(soundEffectGroup, new Dictionary<string, SoundCue>());
            }
            _sounds[soundEffectGroup].Add(soundEffectName, new SoundCue(volume, pitch, pan, sf, allowMultiInstancing));
        }

        public void LoadFolder(string folderName, float volume, float pitch, float pan)
        {
            LoadFolder(folderName, volume, pitch, pan, false);
        }

        public void LoadFolder(string folderName, float volume, float pitch, float pan, bool allowMultiInstancing)
        {
            var dir = new DirectoryInfo("Content\\" + _audioManager.ContentFolder + "\\" + folderName);
            if (!dir.Exists) return;
            var xbnFiles = dir.GetFiles("*.xnb");
            foreach (var soundName in xbnFiles.Select(file => file.Name.Substring(0, file.Name.Length - file.Extension.Length)))
            {
                LoadSound(dir.Name, soundName, folderName + "\\" + soundName, volume, pitch, pan, allowMultiInstancing);
            }
        }

        public void LoadFolderTree(string folderName, float volume, float pitch, float pan, int maxDepth)
        {
            LoadFolderTree(folderName, volume, pitch, pan, maxDepth, false);
        }


        public void LoadFolderTree(string folderName, float volume, float pitch, float pan, int maxDepth, bool allowMultiInstancing)
        {
            if (maxDepth < 0) return;
            var dir = new DirectoryInfo("Content\\" + _audioManager.ContentFolder + "\\" + folderName);
            if (!dir.Exists) return;
            var dirFiles = dir.GetDirectories();
            var xbnFiles = dir.GetFiles("*.xnb");
            foreach (var directoryInfo in dirFiles)
            {
                LoadFolderTree(folderName + "\\" + directoryInfo.Name, volume, pitch, pan, maxDepth - 1, allowMultiInstancing);
            }
            foreach (var soundName in xbnFiles.Select(file => file.Name.Substring(0, file.Name.Length - file.Extension.Length)))
            {
                LoadSound(dir.Name, soundName, folderName + "\\" + soundName, volume, pitch, pan, allowMultiInstancing);
            }
#if DEBUG
            if (xbnFiles.Length == 0)
                Diagnostics.PushLog(LoggingLevel.WARN, "W folderze " + folderName + " nie ma plików .xba");
#endif
        }

        /// <summary>
        /// Odtwarza dźwięk o podanej nazwie
        /// </summary>
        /// <param name="soundName">Nazwa dźwięku</param>
        public void PlaySound(string soundGroup, string soundName)
        {
            PlaySound(soundGroup, soundName, false);
        }

        public void PlaySound(string soundGroup, string soundName, bool isLooped)
        {
            SoundCue sound;
            if (!_sounds.ContainsKey(soundGroup) || !_sounds[soundGroup].TryGetValue(soundName, out sound))
            {
#if DEBUG
                Diagnostics.PushLog(LoggingLevel.ERROR, "Dźwięk " + soundName + " lub grupa " + soundGroup + " nie istnieje.");
#endif
                return;
            }
            PlaySound(sound, soundName, isLooped);
        }
        private void PlaySound(SoundCue sound, string soundName)
        {
            PlaySound(sound, soundName, false);
        }
        private void PlaySound(SoundCue sound, string soundName, bool isLooped)
        {
            if (!sound.AllowMultiInstancing && _playingSounds.ContainsKey(soundName))
            {
                var playedSound = _playingSounds[soundName];
                if (playedSound != null)
                {
                    if (playedSound.IsDisposed)
                    {
                        _playingSounds.Remove(soundName);
                    }
                    else
                    {
#if DEBUG
                        Diagnostics.PushLog(LoggingLevel.INFO, "Dźwięk " + soundName + " nie pozwala na tworzenie jego wielu instacji.");
#endif
                        return;
                    }
                }
            }
            var sei = _audioManager.PlaySound(sound, _emitter, isLooped);
            if (!sound.AllowMultiInstancing && sei != null)
            {
                _playingSounds.Add(soundName, sei);
            }
        }

        public void PlayRandomSound(string soundGroup)
        {
            PlayRandomSound(soundGroup, false);
        }
        public void PlayRandomSound(string soundGroup, bool isLooped)
        {
            if (!_sounds.ContainsKey(soundGroup) || _sounds[soundGroup].Values.Count == 0)
            {
#if DEBUG
                Diagnostics.PushLog(LoggingLevel.ERROR, "Grupa " + soundGroup + " nie istnieje lub nie zawiera dźwięków");
#endif
                return;
            }
            var soundId = _random.Next(_sounds[soundGroup].Values.Count - 1);
            var sound = _sounds[soundGroup].Values.ElementAt(soundId);
            if (sound.SoundEffect != null)
            {
                PlaySound(sound, _sounds[soundGroup].Keys.ElementAt(soundId),isLooped);
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
                Diagnostics.PushLog(LoggingLevel.INFO,"Obecnie dźwięk " + soundName + " nie jest odtwarzany");
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
