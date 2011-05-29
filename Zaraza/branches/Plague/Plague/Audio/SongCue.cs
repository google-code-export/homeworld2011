using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace PlagueEngine.Audio
{
    public struct SongCue
    {
        public float Volume;
        public Song Song;


        public SongCue(float volume, Song song)
        {
            Volume =  MathHelper.Clamp(volume,1.0f,0.0f);
            Song = song;
        }

        public SongCue(Song song)
        {
            Volume = 1.0f;
            Song = song;
        }
    }
}
