using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using PlagueEngine.Audio.Components;

namespace PlagueEngine.Audio
{
    public class AudioManager
    {
        static private AudioManager _audioManager;


        #region Prywatne pola

        private readonly ContentManager _contentManager;
        private readonly String _contentFolder;

        private readonly Dictionary<string, Song> _songs = new Dictionary<string, Song>();
        private readonly Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();

        private BackgroundMusicComponent _activeBMC;
        private readonly SoundEffectInstance[] _playingSounds = new SoundEffectInstance[MaxSounds];
        private Song _currentSong;
        private bool _isMusicPaused;
        private bool _isFading;
        private bool _enabled = true;
        private MusicFadeEffect _fadeEffect;
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

        public string CurrentSong { get; private set; }

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

        public AudioListener Listener { get; set; }

        private AudioManager(Game game)
        {
            Listener = new AudioListener();
            _contentManager = game.ContentManager;
        }
        private AudioManager(Game game, string contentFolder)
        {
            Listener = new AudioListener();
            _contentFolder = contentFolder + "\\";
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
                song = _contentManager.Load<Song>(_contentFolder + songPath);
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
                soundEffect = _contentManager.Load<SoundEffect>(_contentFolder + soundPath);
                _sounds.Add(soundName, soundEffect);
            }

            return soundEffect;
        }

        public void UnloadContent()
        {
            _contentManager.Unload();
        }

        public void PlaySong(string songName)
        {
            PlaySong(songName, false);
        }

        public void PlaySong(string songName, bool loop)
        {
            if (CurrentSong == songName) return;

            if (!_songs.ContainsKey(songName))
            {
                LoadSong(songName);

            }
            if (!_songs.TryGetValue(songName, out _currentSong))
            {
#if DEBUG
                Diagnostics.PushLog("Piosenka " + songName + " nie istnieje");
#else 
          return;
#endif
            }
            if (_currentSong != null)
            {
                MediaPlayer.Stop();
            }
            CurrentSong = songName;

            _isMusicPaused = false;
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Play(_currentSong);

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
            if (_currentSong == null || _isMusicPaused) return;

            if (Enabled) MediaPlayer.Pause();
            _isMusicPaused = true;
        }

        /// <summary>
        /// Wznawianie odtwarzanie zatrzymanej piosenki
        /// </summary>
        public void ResumeSong()
        {
            if (_currentSong == null || !_isMusicPaused) return;
            if (Enabled) MediaPlayer.Resume();
            _isMusicPaused = false;
        }

        /// <summary>
        /// Zatrzymuje odtwarzanie obecnej piosenki
        /// </summary>
        public void StopSong()
        {
            if (_currentSong == null || MediaPlayer.State == MediaState.Stopped) return;
            MediaPlayer.Stop();
            _isMusicPaused = false;
        }

        /// <summary>
        /// Odtwarza dźwięk
        /// </summary>
        /// <param name="soundCue">Obiekt klasy SoundCue</param>
        /// <param name="emitter">Obiekt klasy AudioEmitter wskazujący na źródło dźwięku</param>
        public void PlaySound(SoundCue soundCue, AudioEmitter emitter)
        {
            var index = GetAvailableSoundIndex();

            if (index == -1) return;
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
            for (var i = 0; i < _playingSounds.Length; ++i)
            {
                if (_playingSounds[i] == null || _playingSounds[i].State != SoundState.Stopped) continue;
                _playingSounds[i].Dispose();
                _playingSounds[i] = null;
            }

            if (_currentSong != null && MediaPlayer.State == MediaState.Stopped)
            {
                _currentSong = null;
                CurrentSong = null;
                _isMusicPaused = false;
            }

            if (_isFading && !_isMusicPaused)
            {
                if (_currentSong != null && MediaPlayer.State == MediaState.Playing)
                {
                    if (_fadeEffect.Update(gameTime.ElapsedGameTime))
                    {
                        _isFading = false;
                    }

                    MediaPlayer.Volume = _fadeEffect.GetVolume();
                }
                else
                {
                    _isFading = false;
                }
            }
        }

        /// <summary>
        /// Wywoływana przy zmainie stanu Enable.
        /// </summary>
        protected void OnEnabledChanged()
        {
            if (Enabled)
            {
                foreach (var t in _playingSounds)
                {
                    if (t != null && t.State == SoundState.Paused)
                    {
                        t.Resume();
                    }
                }

                if (!_isMusicPaused)
                {
                    MediaPlayer.Resume();
                }
            }
            else
            {
                foreach (var t in _playingSounds)
                {
                    if (t != null && t.State == SoundState.Playing)
                    {
                        t.Pause();
                    }
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
