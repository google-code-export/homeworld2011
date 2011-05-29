using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PlagueEngine.Audio
{
    public class AudioManager : GameComponent
    {
        #region Singleton

        private static AudioManager _audioManager = null;

        #endregion

        #region Audio Data

        private AudioEngine _audioEngine;  // The audio engine used to play all cues
        private SoundBank _soundBank;      // The soundbank that contains all cues

        public AudioEngine AudioEngine
        {
            get { return _audioEngine; }
            set { _audioEngine = value; }
        }

        public SoundBank SoundBank
        {
            get { return _soundBank; }
            set { _soundBank = value; }
        }

        public WaveBank SfxWaveBank
        {
            get { return _sfxWaveBank; }
            set { _sfxWaveBank = value; }
        }

        public WaveBank BgmWaveBank
        {
            get { return _bgmWaveBank; }
            set { _bgmWaveBank = value; }
        }

        public AudioCategory BgmCategory
        {
            get { return _bgmCategory; }
            set { _bgmCategory = value; }
        }

        public AudioCategory SfxCategory
        {
            get { return _sfxCategory; }
            set { _sfxCategory = value; }
        }

        public float BgmVolume
        {
            get { return _bgmVolume; }
            set { _bgmVolume = value; }
        }

        public float SfxVolume
        {
            get { return _sfxVolume; }
            set { _sfxVolume = value; }
        }

        public Cue SfxMusicCue
        {
            get { return _sfxMusicCue; }
            set { _sfxMusicCue = value; }
        }

        public Cue BgmMusicCue
        {
            get { return _bgmMusicCue; }
            set { _bgmMusicCue = value; }
        }

        private WaveBank _sfxWaveBank;     // sound effect (sfx) bank
        private WaveBank _bgmWaveBank;     // background music (bgm) bank

        private AudioCategory _bgmCategory;// bgm category
        private AudioCategory _sfxCategory;// sfx category

        private float _bgmVolume = 1.0f;   // bgm volume
        private float _sfxVolume = 1.0f;   // sfx volume

        private Cue _sfxMusicCue;          // current cue for sfx
        private Cue _bgmMusicCue;          // current cue for bgm

        #endregion

        #region Initialization

        private AudioManager(Game game,
            string settingsFile,
            string seWaveBankFile,
            string bgmWaveBankFile,
            string soundBankFile)
            : base(game)
        {
            try
            {
                _audioEngine = new AudioEngine(settingsFile);
                _sfxWaveBank = new WaveBank(_audioEngine, seWaveBankFile);
                _bgmWaveBank = new WaveBank(_audioEngine, bgmWaveBankFile, 0, 16);
                _soundBank = new SoundBank(_audioEngine, soundBankFile);
            }
            catch (NoAudioHardwareException)
            {
                // silently fall back to silence
                _audioEngine = null;
                _sfxWaveBank = null;
                _bgmWaveBank = null;
                _soundBank = null;
            }
        }

        //public static void Initialize(Game game, string settingsFile,
        //    string waveBankFile, string soundBankFile)
        //{
        //    audioManager = new AudioManager(game, settingsFile, waveBankFile, soundBankFile);

        //    if (game != null)
        //        game.Components.Add(audioManager);

        //}

        public static void Initialize(Game game)
        {
            const string settingsFile = @"Content\Gtg.xgs";
            const string seWaveBankFile = @"Content\SeWaveBank.xwb";
            const string bgmWaveFile = @"Content\BgmWaveBank.xwb";
            const string soundBankFile = @"Content\SoundBank.xsb";

            _audioManager = new AudioManager(game,
                settingsFile,
                seWaveBankFile,
                bgmWaveFile,
                soundBankFile);

            if (game != null)
                game.Components.Add(_audioManager);

            _audioManager._sfxCategory = _audioManager._audioEngine.GetCategory("Default");
            _audioManager._bgmCategory = _audioManager._audioEngine.GetCategory("Music");

            _audioManager._sfxCategory.SetVolume(_audioManager._sfxVolume);
            _audioManager._bgmCategory.SetVolume(_audioManager._bgmVolume);

        }

        #endregion

        #region Cue Methods

        public static Cue GetSfxCue(string cueName)
        {
            if ((_audioManager == null) || (_audioManager._audioEngine == null) ||
                (_audioManager._soundBank == null) || (_audioManager._sfxWaveBank == null))
                return null;

            return _audioManager._soundBank.GetCue(cueName);
        }

        public static void PlaySfxCue(string cueName)
        {
            if ((_audioManager != null) && (_audioManager._audioEngine != null) &&
                (_audioManager._soundBank != null) && (_audioManager._sfxWaveBank != null))
                _audioManager._soundBank.PlayCue(cueName);

        }

        public static void PlaySfxMusic(string musicCueName)
        {
            if ((_audioManager == null) || (_audioManager._audioEngine == null) ||
                (_audioManager._soundBank == null) || (_audioManager._sfxWaveBank == null))
                return;

            if (_audioManager._sfxMusicCue != null)
                _audioManager._sfxMusicCue.Stop(AudioStopOptions.AsAuthored);

            _audioManager._sfxMusicCue = GetSfxCue(musicCueName);

            if (_audioManager._sfxMusicCue != null)
                _audioManager._sfxMusicCue.Play();

        }

        public static Cue GetBgmCue(string cueName)
        {
            if ((_audioManager == null) || (_audioManager._audioEngine == null) ||
                (_audioManager._soundBank == null) || (_audioManager._bgmWaveBank == null))
                return null;

            return _audioManager._soundBank.GetCue(cueName);
        }

        public static void PlayBgmCue(string cueName)
        {
            if ((_audioManager != null) && (_audioManager._audioEngine != null) &&
                (_audioManager._soundBank != null) && (_audioManager._bgmWaveBank != null))
                _audioManager._soundBank.PlayCue(cueName);
        }

        public static void PlayBgmMusic(string musicCueName)
        {
            if ((_audioManager == null) || (_audioManager._audioEngine == null) ||
                (_audioManager._soundBank == null) || (_audioManager._bgmWaveBank == null))
                return;

            if (_audioManager._bgmMusicCue != null)
                _audioManager._bgmMusicCue.Stop(AudioStopOptions.AsAuthored);

            _audioManager._bgmMusicCue = GetBgmCue(musicCueName);

            if (_audioManager._bgmMusicCue != null)
                _audioManager._bgmMusicCue.Play();

        }

        #endregion

        #region Updating Methods

        public override void Update(GameTime gameTime)
        {
            // update the audio engine
            if (_audioEngine != null)
                _audioEngine.Update();

            base.Update(gameTime);
        }

        public void SetBgmVolume(float value)
        {
            _audioManager._bgmVolume = value;

            if (value < 0.1f)
                _audioManager._bgmVolume = 0.1f;
            else if (value > 10.0f)
                _audioManager._bgmVolume = 10.0f;
        }

        public void SetSfxVolume(float value)
        {
            _audioManager._sfxVolume = value;

            if (value < 0.1f)
                _audioManager._sfxVolume = 0.1f;
            else if (value > 10.0f)
                _audioManager._sfxVolume = 10.0f;
        }

        #endregion

        #region Instance Disposal Methods

        /// <summary>
        /// Clean up the component when it is disposing
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_soundBank != null)
                    {
                        _soundBank.Dispose();
                        _soundBank = null;
                    }

                    if (_sfxWaveBank != null)
                    {
                        _sfxWaveBank.Dispose();
                        _sfxWaveBank = null;
                    }

                    if (_bgmWaveBank != null)
                    {
                        _bgmWaveBank.Dispose();
                        _bgmWaveBank = null;
                    }

                    if (_audioEngine != null)
                    {
                        _audioEngine.Dispose();
                        _audioEngine = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion
    }
}
