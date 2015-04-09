using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YaminGame.Utilities
{
    class TextureCenter
    {
        Microsoft.Xna.Framework.Game game;
        public Texture2D carriageTexture { get; private set; }
        public Texture2D cannonTexture { get; private set; }
        public Color[,] carriageColorArray { get; private set; }
        public Color[,] cannonColorArray { get; private set; }
        public Texture2D explosionTexture { get; private set; }
        public Color[,] explosionColorArray { get; private set; }
        public Texture2D rocketTexture { get; private set; }
        public Texture2D smokeTexture { get; private set; }
        public Color[,] rocketColorArray { get; private set; }
        public Texture2D backgroundTexture { get; private set; }
        public Texture2D foregroundTexture { get; set; }
        public Color[,] foregroundColorArray { get; set; }
        public Texture2D groundTexture { get; private set; }

        public TextureCenter(Microsoft.Xna.Framework.Game game)
        {
            this.game = game;
            carriageTexture = game.Content.Load<Texture2D>("carriage");
            cannonTexture = game.Content.Load<Texture2D>("cannon");
            carriageColorArray = Utils.TextureTo2DArray(carriageTexture);
            cannonColorArray = Utils.TextureTo2DArray(cannonTexture);
            explosionTexture = game.Content.Load<Texture2D>("explosion");
            explosionColorArray = Utils.TextureTo2DArray(explosionTexture);
            rocketTexture = game.Content.Load<Texture2D>("rocket");
            smokeTexture = game.Content.Load<Texture2D>("smoke");
            rocketColorArray = Utils.TextureTo2DArray(rocketTexture);
            backgroundTexture = game.Content.Load<Texture2D>("background");
            groundTexture = game.Content.Load<Texture2D>("ground");

        }
    }
}
