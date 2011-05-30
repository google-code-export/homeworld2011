using System;
using Microsoft.Xna.Framework;

namespace PlagueEngine.Audio
{
    /// <summary>
    /// Struktura odpowiadająca za efekt wyciaszania dźwięku, muzyki
    /// </summary>
    public struct MusicFadeEffect
    {
        public float SourceVolume;
        public float TargetVolume;

        private TimeSpan _time;
        private TimeSpan _duration;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="MusicFadeEffect"/>.
        /// </summary>
        /// <param name="sourceVolume">Głośność początkowa</param>
        /// <param name="targetVolume">Głośność końcowa</param>
        /// <param name="duration">Czas trwania</param>
        public MusicFadeEffect(float sourceVolume, float targetVolume, TimeSpan duration)
        {
            SourceVolume = sourceVolume;
            TargetVolume = targetVolume;
            _time = TimeSpan.Zero;
            _duration = duration;
        }

        /// <summary>
        /// Funkcja aktualizująca efekt.
        /// </summary>
        /// <param name="time">Czas petli gry</param>
        /// <returns>True jeśli efekt się nie zakończył i False jeśli już dobiegł końca</returns>
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

        /// <summary>
        /// Pobiera aktualną głośność
        /// </summary>
        /// <returns>Głośność</returns>
        public float GetVolume()
        {
            return MathHelper.Lerp(SourceVolume, TargetVolume, (float)_time.Ticks / _duration.Ticks);
        }
    }

    /// <summary>
    /// Opcje dla przewania wyciszania
    /// </summary>
    public enum FadeCancelOptions
    {
        /// <summary>
        /// Ustaw głośność przed wyciszeniem
        /// </summary>
        Source,
        /// <summary>
        /// Ustaw głośność docelową wyciszenia
        /// </summary>
        Target,
        /// <summary>
        /// Pozastaw bierzącą głośność
        /// </summary>
        Current

    }
}
