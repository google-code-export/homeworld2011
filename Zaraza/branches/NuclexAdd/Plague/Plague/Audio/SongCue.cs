using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace PlagueEngine.Audio
{
    public struct SongCue
    {
        public float Volume;
        public Song Song;


        public SongCue(Song song, float volume)
        {
            Volume = MathHelper.Clamp(volume, 0.0f, 1.0f);
#if DEBUG
            Diagnostics.Debug("New SongCue " + song.Name + ", volume " + volume);
            if (!Equals(Volume, volume))
            {
                Diagnostics.Info("Volum of SongCue " + song.Name + " was claped to " + Volume);
            }
#endif
            Song = song;
        }

        public SongCue(Song song):this(song,1.0f){}
    }
}
