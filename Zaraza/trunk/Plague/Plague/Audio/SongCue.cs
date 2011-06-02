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
#if DEBUG
            Diagnostics.PushLog("Nowy SongCue " + song.Name + ", głośność "+ volume);
#endif
            Volume = MathHelper.Clamp(volume, 0.0f, 1.0f);
            Song = song;
        }

        public SongCue(Song song):this(song,1.0f){}
    }
}
