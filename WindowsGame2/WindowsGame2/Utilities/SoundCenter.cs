using Microsoft.Xna.Framework.Audio;

namespace YaminGame.Utilities
{
    public class SoundCenter
    {
        public SoundEffect HitCannon { get; private set; }
        public SoundEffect HitTerrain { get; private set; }
        public SoundEffect HitBird { get; private set; }
        public SoundEffect Launch { get; private set; }

        public SoundCenter(Microsoft.Xna.Framework.Game game)
        {
            HitCannon = game.Content.Load<SoundEffect>("hitcannon");
            HitTerrain = game.Content.Load<SoundEffect>("hitterrain");
            Launch = game.Content.Load<SoundEffect>("launch");
            HitBird = game.Content.Load<SoundEffect>("explosion-05");
        }
    }
}




