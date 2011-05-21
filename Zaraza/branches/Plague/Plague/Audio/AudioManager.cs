using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace PlagueEngine.Audio
{
    public class AudioManager : GameComponent
    {
        static private AudioManager _audioManager = null;


        #region Prywatne pola

        private ContentManager _content;
        private String _contentFolder;

        private Dictionary<string, Song> _songs = new Dictionary<string, Song>();
        private Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();

        private Song _currentSong = null;
        private SoundEffectInstance[] _playingSounds = new SoundEffectInstance[MaxSounds];
        private AudioListener _listener = new AudioListener();
        private bool _isMusicPaused = false;

        private bool _isFading = false;
        private MusicFadeEffect _fadeEffect;
        #endregion

        // Maksymalna ilość dźwięków która będzie odtwarzana jednocześnie.
        private const int MaxSounds = 32;

        public static AudioManager Instance
        {
            get
            {
                if (_audioManager == null)
                {
                    throw new NullReferenceException("AudioMenager nie zosatał odpowiednio zainicjalizowany.");
                }
                return _audioManager;
            }
        }

        /// <summary>
        /// Pobiera bądź ustawia obecnie graną piosenkę
        /// </summary>
        public string CurrentSong { get; private set; }

        /// <summary>
        /// Gets or sets the volume to play songs. 1.0f is max volume.
        /// </summary>
        public float MusicVolume
        {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = value; }
        }

        /// <summary>
        /// Gets or sets the master volume for all sounds. 1.0f is max volume.
        /// </summary>
        public float SoundVolume
        {
            get { return SoundEffect.MasterVolume; }
            set { SoundEffect.MasterVolume = value; }
        }

        /// <summary>
        /// Gets whether a song is playing or paused (i.e. not stopped).
        /// </summary>
        public bool IsSongActive { get { return _currentSong != null && MediaPlayer.State != MediaState.Stopped; } }

        /// <summary>
        /// Gets whether the current song is paused.
        /// </summary>
        public bool IsSongPaused { get { return _currentSong != null && _isMusicPaused; } }


        static public void SetInstance(Game game)
        {
            _audioManager = new AudioManager(game);
        }
        static public void SetInstance(Game game, string contentFolder)
        {
            _audioManager = new AudioManager(game, contentFolder);
        }
        /// <summary>
        /// Creates a new Audio Manager. Add this to the Components collection of your Game.
        /// </summary>
        /// <param name="game">The Game</param>
        private AudioManager(Game game)
            : base(game)
        {
            _content = game.ContentManager;
        }

        /// <summary>
        /// Creates a new Audio Manager. Add this to the Components collection of your Game.
        /// </summary>
        /// <param name="game">The Game</param>
        /// <param name="contentFolder">Root folder to load audio content from</param>
        private AudioManager(Game game, string contentFolder)
            : base(game)
        {
            _contentFolder = contentFolder + "\\";
            _content = game.ContentManager;
        }

        /// <summary>
        /// Loads a Song into the AudioManager.
        /// </summary>
        /// <param name="songName">Name of the song to load</param>
        public void LoadSong(string songName)
        {
#if DEBUG
            try
            {
#endif
                LoadSong(songName, songName);
#if DEBUG
            }
            catch (InvalidOperationException e)
            {
                Diagnostics.PushLog(e.Message);
            }
#endif
        }

        /// <summary>
        /// Loads a Song into the AudioManager.
        /// </summary>
        /// <param name="songName">Name of the song to load</param>
        /// <param name="songPath">Path to the song asset file</param>
        public void LoadSong(string songName, string songPath)
        {
            if (_songs.ContainsKey(songName))
            {
#if DEBUG
                throw new InvalidOperationException(string.Format("Piosenka '{0}' została już załadowana", songName));
#else 
          return;
#endif
            }

            _songs.Add(songName, _content.Load<Song>(_contentFolder+songPath));
        }

        /// <summary>
        /// Loads a SoundEffect into the AudioManager.
        /// </summary>
        /// <param name="soundName">Name of the sound to load</param>
        public void LoadSound(string soundName)
        {
            LoadSound(soundName, soundName);
        }

        /// <summary>
        /// Loads a SoundEffect into the AudioManager.
        /// </summary>
        /// <param name="soundName">Name of the sound to load</param>
        /// <param name="soundPath">Path to the song asset file</param>
        public void LoadSound(string soundName, string soundPath)
        {
            if (_sounds.ContainsKey(soundName))
            {
#if DEBUG
                throw new InvalidOperationException(string.Format("Dźwięk '{0}' został już załadowany", soundName));
#else 
          return;
#endif

            }
            _sounds.Add(soundName, _content.Load<SoundEffect>(_contentFolder + soundPath));
        }

        /// <summary>
        /// Unloads all loaded songs and sounds.
        /// </summary>
        public void UnloadContent()
        {
            _content.Unload();
        }

        /// <summary>
        /// Starts playing the song with the given name. If it is already playing, this method
        /// does nothing. If another song is currently playing, it is stopped first.
        /// </summary>
        /// <param name="songName">Name of the song to play</param>
        public void PlaySong(string songName)
        {
            PlaySong(songName, false);
        }

        /// <summary>
        /// Starts playing the song with the given name. If it is already playing, this method
        /// does nothing. If another song is currently playing, it is stopped first.
        /// </summary>
        /// <param name="songName">Name of the song to play</param>
        /// <param name="loop">True if song should loop, false otherwise</param>
        public void PlaySong(string songName, bool loop)
        {
            if (CurrentSong != songName)
            {
                if (_currentSong != null)
                {
                    MediaPlayer.Stop();
                }
                if (!_songs.ContainsKey(songName))
                {
                        LoadSong(songName);
                       
                }
                if (!_songs.TryGetValue(songName, out _currentSong))
                {
#if DEBUG
                    throw new ArgumentException(string.Format("Piosenka '{0}' nie istnieje", songName));
#else 
          return;
#endif
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
        }

        /// <summary>
        /// Pauses the currently playing song. This is a no-op if the song is already paused,
        /// or if no song is currently playing.
        /// </summary>
        public void PauseSong()
        {
            if (_currentSong != null && !_isMusicPaused)
            {
                if (Enabled) MediaPlayer.Pause();
                _isMusicPaused = true;
            }
        }

        /// <summary>
        /// Resumes the currently paused song. This is a no-op if the song is not paused,
        /// or if no song is currently playing.
        /// </summary>
        public void ResumeSong()
        {
            if (_currentSong != null && _isMusicPaused)
            {
                if (Enabled) MediaPlayer.Resume();
                _isMusicPaused = false;
            }
        }

        /// <summary>
        /// Stops the currently playing song. This is a no-op if no song is currently playing.
        /// </summary>
        public void StopSong()
        {
            if (_currentSong != null && MediaPlayer.State != MediaState.Stopped)
            {
                MediaPlayer.Stop();
                _isMusicPaused = false;
            }
        }

        /// <summary>
        /// Smoothly transition between two volumes.
        /// </summary>
        /// <param name="targetVolume">Target volume, 0.0f to 1.0f</param>
        /// <param name="duration">Length of volume transition</param>
        public void FadeSong(float targetVolume, TimeSpan duration)
        {
            if (duration <= TimeSpan.Zero)
            {

#if DEBUG
                throw new ArgumentException("Czas trwania powinien być liczbą dodatnią");
#else 
          return;
#endif
            }

            _fadeEffect = new MusicFadeEffect(MediaPlayer.Volume, targetVolume, duration);
            _isFading = true;
        }

        /// <summary>
        /// Stop the current fade.
        /// </summary>
        /// <param name="option">Options for setting the music volume</param>
        public void CancelFade(FadeCancelOptions option)
        {
            if (_isFading)
            {
                switch (option)
                {
                    case FadeCancelOptions.Source: MediaPlayer.Volume = _fadeEffect.SourceVolume; break;
                    case FadeCancelOptions.Target: MediaPlayer.Volume = _fadeEffect.TargetVolume; break;
                }

                _isFading = false;
            }
        }

        /// <summary>
        /// Plays the sound of the given name.
        /// </summary>
        /// <param name="soundName">Name of the sound</param>
        public void PlaySound(string soundName)
        {
            PlaySound(soundName, 1.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Plays the sound of the given name at the given volume.
        /// </summary>
        /// <param name="soundName">Name of the sound</param>
        /// <param name="volume">Volume, 0.0f to 1.0f</param>
        public void PlaySound(string soundName, float volume)
        {
            PlaySound(soundName, volume, 0.0f, 0.0f);
        }

        /// <summary>
        /// Plays the sound of the given name with the given parameters.
        /// </summary>
        /// <param name="soundName">Name of the sound</param>
        /// <param name="volume">Volume, 0.0f to 1.0f</param>
        /// <param name="pitch">Pitch, -1.0f (down one octave) to 1.0f (up one octave)</param>
        /// <param name="pan">Pan, -1.0f (full left) to 1.0f (full right)</param>
        public void PlaySound(string soundName, float volume, float pitch, float pan)
        {
            SoundEffect sound;

            if (!_sounds.TryGetValue(soundName, out sound))
            {
#if DEBUG
                throw new ArgumentException(string.Format("Dźwięk '{0}' nie istnieje", soundName));
#else 
          return;
#endif
            }

            int index = GetAvailableSoundIndex();

            if (index != -1)
            {
                _playingSounds[index] = sound.CreateInstance();
                _playingSounds[index].Volume = volume;
                _playingSounds[index].Pitch = pitch;
                _playingSounds[index].Pan = pan;
                _playingSounds[index].Play();

                if (!Enabled)
                {
                    _playingSounds[index].Pause();
                }
            }
        }

        /// <summary>
        /// Stops all currently playing sounds.
        /// </summary>
        public void StopAllSounds()
        {
            for (int i = 0; i < _playingSounds.Length; ++i)
            {
                if (_playingSounds[i] != null)
                {
                    _playingSounds[i].Stop();
                    _playingSounds[i].Dispose();
                    _playingSounds[i] = null;
                }
            }
        }

        /// <summary>
        /// Called per loop unless Enabled is set to false.
        /// </summary>
        /// <param name="gameTime">Time elapsed since last frame</param>
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < _playingSounds.Length; ++i)
            {
                if (_playingSounds[i] != null && _playingSounds[i].State == SoundState.Stopped)
                {
                    _playingSounds[i].Dispose();
                    _playingSounds[i] = null;
                }
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

            base.Update(gameTime);
        }

        // Pauses all music and sound if disabled, resumes if enabled.
        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (Enabled)
            {
                for (int i = 0; i < _playingSounds.Length; ++i)
                {
                    if (_playingSounds[i] != null && _playingSounds[i].State == SoundState.Paused)
                    {
                        _playingSounds[i].Resume();
                    }
                }

                if (!_isMusicPaused)
                {
                    MediaPlayer.Resume();
                }
            }
            else
            {
                for (int i = 0; i < _playingSounds.Length; ++i)
                {
                    if (_playingSounds[i] != null && _playingSounds[i].State == SoundState.Playing)
                    {
                        _playingSounds[i].Pause();
                    }
                }

                MediaPlayer.Pause();
            }

            base.OnEnabledChanged(sender, args);
        }

        // Acquires an open sound slot.
        private int GetAvailableSoundIndex()
        {
            for (int i = 0; i < _playingSounds.Length; ++i)
            {
                if (_playingSounds[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        #region MusicFadeEffect
        private struct MusicFadeEffect
        {
            public float SourceVolume;
            public float TargetVolume;

            private TimeSpan _time;
            private TimeSpan _duration;

            public MusicFadeEffect(float sourceVolume, float targetVolume, TimeSpan duration)
            {
                SourceVolume = sourceVolume;
                TargetVolume = targetVolume;
                _time = TimeSpan.Zero;
                _duration = duration;
            }

            public bool Update(TimeSpan time)
            {
                _time += time;

                if (_time >= _duration)
                {
                    _time = _duration;
                    return true;
                }

                return false;
            }

            public float GetVolume()
            {
                return MathHelper.Lerp(SourceVolume, TargetVolume, (float)_time.Ticks / _duration.Ticks);
            }
        }
        #endregion
    }

    /// <summary>
    /// Options for AudioManager.CancelFade
    /// </summary>
    public enum FadeCancelOptions
    {
        /// <summary>
        /// Return to pre-fade volume
        /// </summary>
        Source,
        /// <summary>
        /// Snap to fade target volume
        /// </summary>
        Target,
        /// <summary>
        /// Keep current volume
        /// </summary>
        Current

    }
}
