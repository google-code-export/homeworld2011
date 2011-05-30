using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using PlagueEngine.Audio.Components;
using PlagueEngine.Rendering.Components;

namespace PlagueEngine.Audio
{
    public class AudioManager
    {
        static private AudioManager _audioManager;


        #region Prywatne pola

        private readonly ContentManager _contentManager;
        public readonly String ContentFolder;

        private readonly Dictionary<string, Song> _songs = new Dictionary<string, Song>();
        private readonly Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();

        private readonly SoundEffectInstance[] _playingSounds = new SoundEffectInstance[MaxSounds];
        private bool _isFading;
        private bool _enabled = true;
        // Maksymalna ilość dźwięków która będzie odtwarzana jednocześnie.
        private const int MaxSounds = 64;

        #endregion


        public static void SetInstance(Game game)
        {
            _audioManager = new AudioManager(game);
        }
        public static void SetInstance(Game game, string contentFolder)
        {
            _audioManager = new AudioManager(game, contentFolder);
        }

        public static AudioManager GetInstance
        {
            get
            {
                if (_audioManager == null)
                {
#if DEBUG
                    Diagnostics.PushLog("AudioMenager nie został odpowiednio zainicjalizowany.");
#endif
                }
                return _audioManager;
            }
        }

        public float MusicVolume
        {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = value; }
        }

        public float SoundVolume
        {
            get { return SoundEffect.MasterVolume; }
            set { SoundEffect.MasterVolume = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                OnEnabledChanged();
            }
        }

        internal void SetListenerPosition(CameraComponent cameraComponent)
        {
            if (cameraComponent == null) return;
            Listener.Position = cameraComponent.Position;
            Listener.Forward = cameraComponent.Forward;
            Listener.Up = cameraComponent.Up;
        }
        public AudioListener Listener { get; private set; }

        public MusicFadeEffect FadeEffect { get; set; }

        public BackgroundMusicComponent BackgroundMusicComponent { get; set; }

        private AudioManager(Game game)
        {
            Listener = new AudioListener();
            _contentManager = game.ContentManager;
        }
        private AudioManager(Game game, string contentFolder)
        {
            Listener = new AudioListener();
            ContentFolder = contentFolder + "\\";
            _contentManager = game.ContentManager;
        }
        public Song LoadSong(string songName)
        {
            return LoadSong(songName, songName);
        }

        public Song LoadSong(string songName, string songPath)
        {
            Song song;
            if (_songs.ContainsKey(songName))
            {
#if DEBUG
                Diagnostics.PushLog("Piosenka " + songName + " została już załadowana. Zwrócono poprzednią instancje.");
#endif
                song = _songs[songName];
            }
            else
            {
                song = _contentManager.Load<Song>(ContentFolder + songPath);
                _songs.Add(songName, song);
            }

            return song;
        }
        public SoundEffect LoadSound(string soundName)
        {
            return LoadSound(soundName, soundName);
        }

        public SoundEffect LoadSound(string soundName, string soundPath)
        {
            SoundEffect soundEffect;
            if (_sounds.ContainsKey(soundName))
            {
#if DEBUG
                Diagnostics.PushLog("Dźwięk " + soundName + " został już załadowany. Zwrócono poprzednią instancje.");
#endif
                soundEffect = _sounds[soundName];
            }
            else
            {
                try
                {
                    soundEffect = _contentManager.Load<SoundEffect>(ContentFolder + soundPath);
                    _sounds.Add(soundName, soundEffect);
                }
                catch(Exception e)
                {
#if DEBUG
                    Diagnostics.PushLog("Problem z załadowaniem dźwięku. " + e.Message);
#endif
                    
                    soundEffect =null;

                }
                
            }

            return soundEffect;
        }


        public void PlaySong(SongCue songCue)
        {
            PlaySong(songCue, false);
        }

        public void PlaySong(SongCue songCue, bool loop)
        {
            MediaPlayer.Stop();
            MediaPlayer.Volume = songCue.Volume;
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Play(songCue.Song);
            if (!Enabled)
            {
                MediaPlayer.Pause();
            }
        }

        /// <summary>
        /// Pauza piosenki
        /// </summary>
        public void PauseSong()
        {
            if (MediaPlayer.State==MediaState.Paused) return;
            if (Enabled) MediaPlayer.Pause();
        }

        /// <summary>
        /// Wznawianie odtwarzanie zatrzymanej piosenki
        /// </summary>
        public void ResumeSong()
        {
            if (MediaPlayer.State != MediaState.Paused) return;
            if (Enabled) MediaPlayer.Resume();
        }

        /// <summary>
        /// Zatrzymuje odtwarzanie obecnej piosenki
        /// </summary>
        public void StopSong()
        {
            if (MediaPlayer.State == MediaState.Stopped) return;
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Odtwarza dźwięk
        /// </summary>
        /// <param name="soundCue">Obiekt klasy SoundCue</param>
        /// <param name="emitter">Obiekt klasy AudioEmitter wskazujący na źródło dźwięku</param>
        public SoundEffectInstance PlaySound(SoundCue soundCue, AudioEmitter emitter)
        {
            var index = GetAvailableSoundIndex();

            if (index == -1) return null;
            _playingSounds[index] = soundCue.SoundEffect.CreateInstance();
            _playingSounds[index].Volume = soundCue.Volume;
            _playingSounds[index].Pitch = soundCue.Pitch;
            _playingSounds[index].Pan = soundCue.Pan;
            if (emitter != null && Listener != null)
            {
                _playingSounds[index].Apply3D(Listener, emitter);
            }
            _playingSounds[index].Play();
            if (!Enabled)
            {
                _playingSounds[index].Pause();
            }
            return _playingSounds[index];
        }

        /// <summary>
        /// Zatrzymuje wszystkie dźwięki
        /// </summary>
        public void StopAllSounds()
        {
            for (var i = 0; i < _playingSounds.Length; ++i)
            {
                if (_playingSounds[i] == null) continue;
                _playingSounds[i].Stop();
                _playingSounds[i].Dispose();
                _playingSounds[i] = null;
            }
        }

        /// <summary>
        /// Wykonywana co pętle.
        /// </summary>
        /// <param name="gameTime">Czas jaki upłynał od ostatniej klatki</param>
        public void Update(GameTime gameTime)
        {
            if(BackgroundMusicComponent!=null)
            {
                BackgroundMusicComponent.Update(gameTime.ElapsedGameTime);
            }
            for (var i = 0; i < _playingSounds.Length; ++i)
            {
                if (_playingSounds[i] == null || (_playingSounds[i].State != SoundState.Stopped && !_playingSounds[i].IsDisposed)) continue;
                _playingSounds[i].Dispose();
                _playingSounds[i] = null;
            }

            if (!_isFading || MediaPlayer.State==MediaState.Paused) return;
            if (MediaPlayer.State == MediaState.Playing)
            {
                if (FadeEffect.Update(gameTime.ElapsedGameTime))
                {
                    _isFading = false;
                }

                MediaPlayer.Volume = FadeEffect.GetVolume();
            }
            else
            {
                _isFading = false;
            }
        }

        /// <summary>
        /// Wywoływana przy zmainie stanu Enable.
        /// </summary>
        protected void OnEnabledChanged()
        {
            if (Enabled)
            {
                foreach (var t in _playingSounds.Where(t => t != null && t.State == SoundState.Paused))
                {
                    t.Resume();
                }

                if (MediaPlayer.State != MediaState.Paused)
                {
                    MediaPlayer.Resume();
                }
            }
            else
            {
                foreach (var t in _playingSounds.Where(t => t != null && t.State == SoundState.Playing))
                {
                    t.Pause();
                }

                MediaPlayer.Pause();
            }
        }

        // Wyznacza pierwszy wolny indeks dla nowego SoundEffectu
        private int GetAvailableSoundIndex()
        {
            for (var i = 0; i < _playingSounds.Length; ++i)
            {
                if (_playingSounds[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

    }

}
