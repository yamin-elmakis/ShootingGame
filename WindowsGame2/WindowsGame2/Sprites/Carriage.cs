using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame2;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YaminGame.Utilities;

namespace Game.Sprites
{
    public class Carriage : DrawableGameComponent
    {
        Microsoft.Xna.Framework.Game game;
        SpriteBatch spriteBatch;
        private Vector2 playerPosition;
        public Rectangle playerRect;

        /// <summary>
        /// new
        /// </summary>
        public Vector2 Position;
        public bool IsAlive;
        public Color Color;
        public float Angle;
        public float Power;

        public float playerScaling;
        private readonly TextureCenter _textureCenter;

        private static Color[] playerColors;
        private static int shiftColor;

        static Carriage ()
        {
            var _randomizer = new Random();
            shiftColor = _randomizer.Next(8);
            playerColors = new Color[10];
            playerColors[0] = Color.Red;
            playerColors[1] = Color.Green;
            playerColors[2] = Color.Blue;
            playerColors[3] = Color.Purple;
            playerColors[4] = Color.Orange;
            playerColors[5] = Color.Indigo;
            playerColors[6] = Color.Yellow;
            playerColors[7] = Color.SaddleBrown;
            playerColors[8] = Color.Tomato;
            playerColors[9] = Color.Turquoise;
        }

        public Carriage(Microsoft.Xna.Framework.Game game, int i): base(game)
        {
            this.game = game;
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            _textureCenter = (TextureCenter)game.Services.GetService(typeof (TextureCenter));

            playerScaling = 40.0f / (float)_textureCenter.CarriageTexture.Width;
            InitCarriage(i);

            game.IsMouseVisible = true;
        }

        private void InitCarriage(int i)
        {
            IsAlive = true;
            Color = playerColors[(i + shiftColor)%10];
            Angle = MathHelper.ToRadians(90);
            Power = 100;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.globalTransformation);
            if (IsAlive)
            {
                var xPos = (int)Position.X;
                var yPos = (int)Position.Y;
                var cannonOrigin = new Vector2(11, 50);

                spriteBatch.Draw(_textureCenter.CannonTexture, new Vector2(xPos + 20, yPos - 10), null, Color, Angle, cannonOrigin, playerScaling, SpriteEffects.None, 1);
                spriteBatch.Draw(_textureCenter.CarriageTexture, Position, null, Color, 0, new Vector2(0, _textureCenter.CarriageTexture.Height), playerScaling, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
