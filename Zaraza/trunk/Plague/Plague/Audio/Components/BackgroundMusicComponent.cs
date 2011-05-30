using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlagueEngine.Audio;


namespace PlagueEngine.Audio.Components
{
    public class BackgroundMusicComponent
    {
        private bool _enabled = true;
        private string _currentGroup;
        private readonly AudioManager _audioManager;
        public TimeSpan ChangeTime = TimeSpan.FromSeconds(5);
        private TimeSpan _changeTimer;
        private bool _changeAllowed = true;
        private bool automaticMode = true;
        BackgroundMusicComponent()
        {
            _audioManager = AudioManager.GetInstance;
            Songs = new Dictionary<string, Dictionary<string, SongCue>>();
        }

        /// <summary>
        /// Słownik przechowujący Piosenki ułożone w grupy tematyczne.
        /// Standardowo zawiera grupę default.
        /// </summary>
        public Dictionary<string, Dictionary<string, SongCue>> Songs { get; set; }

        public void LoadFolder(string folderName, float volume)
        {
            LoadFolder(folderName, volume, "default");
        }

        public void LoadFolder(string folderName, float volume, string groupName)
        {
            var dir = new DirectoryInfo("Content\\" + _audioManager.ContentFolder + "\\" + folderName);
            if (!dir.Exists) return;
            var xbnFiles = dir.GetFiles("*.xnb");
            foreach (var songName in xbnFiles.Select(file => file.Name.Substring(0, file.Name.Length - file.Extension.Length)))
            {
                LoadSong(songName,folderName + "\\" + songName, groupName, volume);
            }
        }

        public void LoadFolderTree(string folderName, float volume)
        {
            var dir = new DirectoryInfo("Content\\" + _audioManager.ContentFolder + "\\" + folderName);
            if (!dir.Exists) return;
            var dirFiles = dir.GetDirectories();
            foreach (var directoryInfo in dirFiles)
            {
                LoadFolderTree(folderName + "\\" + directoryInfo.Name, volume, 1);
            }
            foreach (var songName in dir.GetFiles("*.xnb").Select(file => file.Name.Substring(0, file.Name.Length - file.Extension.Length)))
            {
                LoadSong(songName, folderName + "\\" + songName, "default", volume);
            }
        }

        public void LoadFolderTree(string folderName, float volume, int depth)
        {
            if (depth >= 5) return; 
            var dir = new DirectoryInfo("Content\\" + _audioManager.ContentFolder + "\\" + folderName);
            if (!dir.Exists) return;
            var dirFiles = dir.GetDirectories();
            foreach (var directoryInfo in dirFiles)
            {
                LoadFolderTree(folderName + "\\" + directoryInfo.Name, volume, depth+1);
            }
            foreach (var songName in dir.GetFiles("*.xnb").Select(file => file.Name.Substring(0, file.Name.Length - file.Extension.Length)))
            {
                LoadSong(songName, folderName + "\\" + songName, folderName.Replace("\\","_"), volume);
            }
        }
        /// <summary>
        /// Ładuje piosenkę do AudioManager i przyspisuje ją do standardowej grupy w BackgroundMusicComponent.
        /// </summary>
        /// <param name="songName">Nazwa piosenki do załadowania</param>
        public void LoadSong(string songName)
        {
            LoadSong(songName, songName, "default", 1.0f);
        }

        public void LoadSong(string songCueName, string songName, string groupName, float volume)
        {
            if (!Songs.ContainsKey(groupName)) Songs.Add("groupName", new Dictionary<string, SongCue>());

            if(Songs[groupName].ContainsKey(songName))
            {
#if DEBUG
                Diagnostics.PushLog("Piosenka " + songName + " została już załadowana");
#endif
                return;
            }
            var song = _audioManager.LoadSong(songName);
            if (song != null)
            {
                Songs[groupName].Add(songCueName, new SongCue(volume, song));
            }
        }

        public void PlaySong(string groupName, string songName)
        {
            PlaySong(groupName, songName, false);
        }

        public void PlaySong(string groupName, string songName, bool loop)
        {
            if (!Songs.ContainsKey(groupName))
            {
#if DEBUG
                Diagnostics.PushLog("Grupa piosenk o nazwie "+groupName+" nie występuje w tym komponencie");
#endif
                return;
            }
            if (Songs[groupName].ContainsKey(songName))
            {
#if DEBUG
                Diagnostics.PushLog("Piosenka "+songName+" nie występuje w tej grupie:"+groupName);
#endif
                return;
            }
            _audioManager.PlaySong(Songs[groupName][songName], loop);
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            if (!Enabled || !automaticMode) return;
          
        }

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

