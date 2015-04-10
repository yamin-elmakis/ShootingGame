using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace YaminGame.Utilities
{
  public class SoundCenter
    {
      Microsoft.Xna.Framework.Game game;

      public SoundCenter(Microsoft.Xna.Framework.Game game)
      {
          this.game = game;
          HitCannon = game.Content.Load<SoundEffect>("hitcannon");
          HitTerrain = game.Content.Load<SoundEffect>("hitterrain");
          Launch = game.Content.Load<SoundEffect>("launch");
      }

      public SoundEffect HitCannon { get; private set; }
      public SoundEffect HitTerrain { get; private set; }
      public SoundEffect Launch { get; private set; }
    }
}




