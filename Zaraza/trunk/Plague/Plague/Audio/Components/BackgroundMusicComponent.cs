using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace PlagueEngine.Audio.Components
{
    /// <summary>
    /// BackgroundMusicComponent jest komponentem odpowiedzialnym za przełączanie podkładu muzycznego.
    /// Stanowi tak jakby przejściówkę w komunikacji z <see cref="PlagueEngine.Audio.AudioManager"/>.
    /// </summary>
    class BackgroundMusicComponent
    {
        private readonly AudioManager _audioManager;
        private TimeSpan _changeTimer;
        private bool _changeAllowed = true;
        private bool _automaticMode = true;
        private Random _random;
        private string _defaultGroupName;
        private SongCue _lastSongCue;
        public BackgroundMusicComponent()
        {
            ChangeTime = TimeSpan.FromSeconds(5);
            Enabled = true;
            _audioManager = AudioManager.GetInstance;
            Songs = new Dictionary<string, Dictionary<string, SongCue>>();
            DefaultGroupName = "default";
        }

        /// <summary>
        /// Słownik przechowujący Piosenki ułożone w grupy tematyczne.
        /// Standardowo zawiera grupę o nazwie podanej w parametrze _defaultGropuName.
        /// </summary>
        public Dictionary<string, Dictionary<string, SongCue>> Songs { get; private set; }

        public void LoadFolder(string folderName, float volume)
        {
            LoadFolder(folderName, volume, DefaultGroupName);
        }

        public void LoadFolder(string folderName, float volume, string groupName)
        {
            var dir = new DirectoryInfo("Content\\" + _audioManager.ContentFolder + "\\" + folderName);
            if (!dir.Exists) return;
            var xbnFiles = dir.GetFiles("*.xnb");

            foreach (var songName in xbnFiles.Select(file => file.Name.Substring(0, file.Name.Length - file.Extension.Length)))
            {
                LoadSong(songName, folderName + "\\" + songName, groupName, volume);
            }
#if DEBUG
            if (xbnFiles.Length == 0)
                Diagnostics.PushLog("W folderze " + folderName + " nie ma plików .xba");
#endif
        }

        public void LoadFolderTree(string folderName, float volume, int maxDepth)
        {
            if (maxDepth < 0) return;
            var dir = new DirectoryInfo("Content\\" + _audioManager.ContentFolder + "\\" + folderName);
            if (!dir.Exists) return;
            var dirFiles = dir.GetDirectories();
            var xbnFiles = dir.GetFiles("*.xnb");
            foreach (var directoryInfo in dirFiles)
            {
                LoadFolderTree(folderName + "\\" + directoryInfo.Name, volume, maxDepth - 1);
            }
            foreach (var songName in xbnFiles.Select(file => file.Name.Substring(0, file.Name.Length - file.Extension.Length)))
            {
                LoadSong(songName, folderName + "\\" + songName, DefaultGroupName, volume);
            }
#if DEBUG
            if (xbnFiles.Length == 0)
                Diagnostics.PushLog("W folderze " + folderName + " nie ma plików .xba");
#endif
        }

        /// <summary>
        /// Ładuje piosenkę do <see cref="PlagueEngine.Audio.AudioManager"/> i przyspisuje ją do standardowej grupy w <see cref="BackgroundMusicComponent"/> .
        /// </summary>
        /// <param name="songName">Nazwa piosenki do załadowania</param>
        public void LoadSong(string songName)
        {
            LoadSong(songName, songName, DefaultGroupName, 1.0f);
        }

        public void LoadSong(string songCueName, string songName, string groupName, float volume)
        {
            groupName = groupName.Trim();
            if (!Songs.ContainsKey(groupName)) Songs.Add(groupName, new Dictionary<string, SongCue>());

            if (Songs[groupName].ContainsKey(songName))
            {
#if DEBUG
                Diagnostics.PushLog("Piosenka " + songName + " została już załadowana");
#endif
                return;
            }
            var song = _audioManager.LoadSong(songName);
            if (song != null)
            {
                Songs[groupName].Add(songCueName, new SongCue(song, volume));
            }
        }

        public void PlaySong(string groupName, string songName)
        {
            PlaySong(groupName, songName, false);
        }

        public void PlaySong(string groupName, string songName, bool loop)
        {
            groupName = groupName.Trim();
            if (!Songs.ContainsKey(groupName))
            {
#if DEBUG
                Diagnostics.PushLog("Grupa piosenk o nazwie " + groupName + " nie występuje w tym komponencie");
#endif
                return;
            }
            if (Songs[groupName].ContainsKey(songName))
            {
#if DEBUG
                Diagnostics.PushLog("Piosenka " + songName + " nie występuje w tej grupie:" + groupName);
#endif
                return;
            }
            if (_audioManager != null) { _audioManager.PlaySong(Songs[groupName][songName], loop); }

        }

        public void Update(TimeSpan elapsedGameTime)
        {
            //Sprawdzamy czy komponent jest aktywy i czy jest w trybie automatycznym jeśli nie to kończymy pętle.
            if ((_audioManager == null || !_audioManager.Enabled) || !Enabled || !AutomaticMode)
            {
                return;
            }
            //Sprawdzamy czy obecna grupa do odtwarzania nie jest pustym stringiem
            if (string.IsNullOrWhiteSpace(CurrentGroup))
            {
                //przypisujemy standardową grupę
                CurrentGroup = DefaultGroupName;
            }
            //Sprawdzamy czy słownik zawiera obecną grupę, jesli nie to kończymy pętlę.
            if (!Songs.ContainsKey(CurrentGroup)) return;

            //Spawdzamy czy można dokonać zmiany piosenki
            //jeżeli czas przekroczy wartość ChangeTime to można zmienić piosenkę.
            if (_changeTimer >= ChangeTime || ForceChange)
            {
                _changeAllowed = true;
                _changeTimer = new TimeSpan();
            }
            if (!_changeAllowed && _changeTimer < ChangeTime)
            {
                //jeśli nie to zwiększamy czas
                _changeTimer.Add(elapsedGameTime);
                return;
            }

            if ((!ForceChange && _audioManager.IsPlaying) || Songs[CurrentGroup].Values.Count == 0) return;
            if (ForceChange)
            {
                ForceChange = false;
            }
            if (Songs[CurrentGroup].Values.Count == 1)
            {
                _lastSongCue = Songs[CurrentGroup].Values.ElementAt(0);
            }
            else
            {
                int songId;
                do
                {
                    songId = _random.Next(0, Songs[CurrentGroup].Values.Count - 1);
                } while (Equals(_lastSongCue, Songs[CurrentGroup].Values.ElementAt(songId)));

                _lastSongCue = Songs[CurrentGroup].Values.ElementAt(songId);
            }
            _audioManager.PlaySong(_lastSongCue);
#if DEBUG
            Diagnostics.PushLog("Start nowej piosenki:" + _lastSongCue.Song.Name + " z grupy " + CurrentGroup);
#endif
            _changeAllowed = false;
        }

        /// <summary>
        /// Zmienna określa czy <see cref="BackgroundMusicComponent"/> jest aktywny czy nie.
        /// UWAGA!! Dezaktywowanie <see cref="BackgroundMusicComponent"/> spowoduje jedynie,
        /// że nie będzie on dalej wybierał następnych piosenek do odegrania. Nie wyciszy do obecnie granej muzyki.
        /// </summary>
        /// <value>
        ///   <c>true</c> jeśli aktywny; inaczej, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Określa obecnie wybraną grupę podkładów muzycznych do odtwarzania
        /// </summary>
        /// <value>
        /// Obecna grupa podkładów muzycznych do odtwarzania
        /// </value>
        public string CurrentGroup { get; set; }

        public string DefaultGroupName
        {
            get { return _defaultGroupName; }
            set
            {
                value = value.Trim();
                if (!string.IsNullOrWhiteSpace(_defaultGroupName) && Songs.ContainsKey(_defaultGroupName))
                {
                    if (_defaultGroupName.Equals(value)) return;
                    var temp = Songs[_defaultGroupName];
                    Songs.Remove(_defaultGroupName);
                    _defaultGroupName = value;
                    Songs.Add(value, temp);
                }
                else
                {
                    Songs.Add(value, new Dictionary<string, SongCue>());
                }
                _defaultGroupName = value;
            }
        }

        public TimeSpan ChangeTime { get; set; }

        public bool ForceChange { get; set; }

        public bool AutomaticMode
        {
            get { return _automaticMode; }
            set
            {
                _automaticMode = value;
                if (value && _random == null)
                {
                    //Tworzymy nowy random który nam się może przydać
                    _random = new Random();
                }
                else
                {
                    //Usuwamy random jeśli nie jest nam już potrzebny.
                    _random = null;
                }
            }
        }

    }
}

