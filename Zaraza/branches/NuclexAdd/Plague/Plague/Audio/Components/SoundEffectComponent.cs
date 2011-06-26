using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PlagueEngine.TimeControlSystem;

namespace PlagueEngine.Audio.Components
{
    class SoundEffectComponent
    {
        private Dictionary<string, Dictionary<string, SoundCue>> _sounds = new Dictionary<string, Dictionary<string, SoundCue>>();
        private Dictionary<string, SoundEffectInstance> _playingSounds;
        private ExpireClock _timeLimiter;
        public AudioEmitter Emitter { get; private set; }

        private AudioManager _audioManager;
        private Random _random;

        public SoundEffectComponent()
        {
            _random = new Random();
            _audioManager = AudioManager.GetInstance;
            Emitter = new AudioEmitter();
            _playingSounds = new Dictionary<string, SoundEffectInstance>();
        }
        public void SetTimeLimit(double time)
        {
            _timeLimiter = ExpireClock.FromSeconds(time);
        }
        public void RemoveTimeLimit()
        {
            _timeLimiter = null;
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
            if (xbnFiles.Length == 0 && dirFiles.Count() == 0)
                Diagnostics.Warn("There was no files with .xba extension in folder: " + folderName);
#endif
        }

        /// <summary>
        /// Odtwarza dźwięk o podanej nazwie
        /// </summary>
        /// <param name="soundName">Nazwa dźwięku</param>
        public void PlaySound(string soundGroup, string soundName)
        {
            PlaySound(soundGroup, soundName, false,0);
        }

        public void PlaySound(string soundGroup, string soundName, bool isLooped)
        {

            PlaySound(soundGroup, soundName, isLooped, 0);
        }
        public void PlaySound(string soundGroup, string soundName, bool isLooped, float maxDistance)
        {
            SoundCue sound;
            if (!_sounds.ContainsKey(soundGroup) || !_sounds[soundGroup].TryGetValue(soundName, out sound))
            {
#if DEBUG
                Diagnostics.Warn("Sound " + soundName + " or group " + soundGroup + " does not exists.");
#endif
                return;
            }
            PlaySound(sound, soundName, isLooped, maxDistance);
        }

        private void PlaySound(SoundCue sound, string soundName, bool isLooped, float maxDistance = 0f)
        {
            if ((_timeLimiter == null || _timeLimiter.isExpired()))
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
                            Diagnostics.Info("Sound " + soundName + " won't be played. It has AllowMultiInstacing set to false.");
#endif
                            return;
                        }
                    }
                }
                var sei = _audioManager.PlaySound(this, sound, Emitter, isLooped, maxDistance);
                if (!sound.AllowMultiInstancing && sei != null)
                {
                    _playingSounds.Add(soundName, sei);
                }
            }
            else
#if DEBUG
                Diagnostics.Info("Sound " + soundName + " won't be played. Time limit for this component not expired yet.");
#endif
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
                Diagnostics.Warn("There is no group of song called " + soundGroup + " in this component.");
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
        public void SetPosition(Vector3 position)
        {
            Emitter.Position = position;
        }

        public void StopSound(string soundName)
        {
            SoundEffectInstance soundEffectInstance;
            if (!_playingSounds.TryGetValue(soundName,out soundEffectInstance))
            {
#if DEBUG
                Diagnostics.Info("Sound " + soundName + " could not be stoped. Song is not playing right now.");
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
                if (!soundEffectInstance.IsDisposed) { 
                    soundEffectInstance.Stop();
                    soundEffectInstance.Dispose();
                }
            }
            _playingSounds.Clear();
        }
        public void  ReleaseMe(){
            StopAllSounds();
            Emitter = null;
            _random = null;
            _playingSounds = null;
            _audioManager = null;
            _sounds.Clear();
            _sounds = null;
        }
    }
}
