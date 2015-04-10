using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YaminGame.Utilities
{
    class TextureCenter
    {
        private Microsoft.Xna.Framework.Game _game;
        public Texture2D CarriageTexture { get; private set; }
        public Texture2D CannonTexture { get; private set; }
        public Color[,] CarriageColorArray { get; private set; }
        public Color[,] CannonColorArray { get; private set; }
        public Texture2D ExplosionTexture { get; private set; }
        public Color[,] ExplosionColorArray { get; private set; }
        public Texture2D RocketTexture { get; private set; }
        public Texture2D SmokeTexture { get; private set; }
        public Color[,] RocketColorArray { get; private set; }
        public Texture2D BackgroundTexture { get; private set; }
        public Texture2D ForegroundTexture { get; set; }
        public Color[,] ForegroundColorArray { get; set; }
        public Texture2D GroundTexture { get; private set; }
        public Texture2D BirdSprite { get; private set; }
        public Color[,] BirdColorArray { get; set; }

        public TextureCenter(Microsoft.Xna.Framework.Game game)
        {
            this._game = game;
            CarriageTexture = game.Content.Load<Texture2D>("carriage");
            CannonTexture = game.Content.Load<Texture2D>("cannon");
            CarriageColorArray = Utils.TextureTo2DArray(CarriageTexture);
            CannonColorArray = Utils.TextureTo2DArray(CannonTexture);
            ExplosionTexture = game.Content.Load<Texture2D>("explosion");
            ExplosionColorArray = Utils.TextureTo2DArray(ExplosionTexture);
            RocketTexture = game.Content.Load<Texture2D>("rocket");
            SmokeTexture = game.Content.Load<Texture2D>("smoke");
            RocketColorArray = Utils.TextureTo2DArray(RocketTexture);
            BackgroundTexture = game.Content.Load<Texture2D>("background");
            GroundTexture = game.Content.Load<Texture2D>("ground");
            BirdSprite = game.Content.Load<Texture2D>("dragon-ltr-sprite-sm");

        }
    }
}
