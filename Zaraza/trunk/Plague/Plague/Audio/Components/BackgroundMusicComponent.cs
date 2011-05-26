using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;

namespace PlagueEngine.Audio.Components
{
    class BackgroundMusicComponent
    {

        private Dictionary<string, Song> _songs = new Dictionary<string, Song>();
        private bool _enabled = true;
        private string _currentGroup;
        private bool _isMusicPaused;
        private Song _currentSong;
        private bool _isFading;

        /// <summary>
        /// Pobiera bądź ustawia obecnie graną piosenkę
        /// </summary>
        public string CurrentSong { get; private set; }

        /// <summary>
        /// Słownik przechowujący Piosenki ułożone w grupy tematyczne.
        /// Standardowo zawiera grupę default.
        /// </summary>
        public Dictionary<string, Song> Songs
        {
            get { return _songs; }
            set { _songs = value; }
        }

        /// <summary>
        /// Ładuje piosenkę do AudioManager i przyspisuje ją do standardowej grupy w BackgroundMusicComponent.
        /// </summary>
        /// <param name="songName">Nazwa piosenki do załadowania</param>
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
            if (Songs.ContainsKey(songName))
            {
#if DEBUG
                throw new InvalidOperationException("Piosenka " + songName + " została już załadowana");
#else 
          return;
#endif
            }
            //_songs.Add(songName, _contentManager.Load<Song>(_contentFolder + songPath));
        }

        /// <summary>
        /// Gets whether a song is playing or paused (i.e. not stopped).
        /// </summary>
        public bool IsSongActive { get { return _currentSong != null && MediaPlayer.State != MediaState.Stopped; } }

        /// <summary>
        /// Gets whether the current song is paused.
        /// </summary>
        public bool IsSongPaused { get { return _currentSong != null && _isMusicPaused; } }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;

            }
        }

    }
}

