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
        private bool PLAYER1 = true, PLAYER2 = false;
        private bool _player;
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
        private TextureCenter _textureCenter;

        private static Color[] playerColors;

        static Carriage ()
        {
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

            playerScaling = 40.0f / (float)_textureCenter.carriageTexture.Width;
            InitCarriage(i);

            game.IsMouseVisible = true;
        }

        private void InitCarriage(int i)
        {
            IsAlive = true;
            Color = playerColors[i];
            Angle = MathHelper.ToRadians(90);
            Power = 100;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            //playerRect = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, playerSprite.Width, playerSprite.Height);
            var keyState = Keyboard.GetState();
            if (_player.Equals(PLAYER1))
            {
                if (keyState.IsKeyDown(Keys.Right)) playerPosition.X += 5;
                if (keyState.IsKeyDown(Keys.Left)) playerPosition.X -= 5;    
            }
            else
            {
                if (keyState.IsKeyDown(Keys.D)) playerPosition.X += 5;
                if (keyState.IsKeyDown(Keys.A)) playerPosition.X -= 5; 
            }
            
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.globalTransformation);
            if (IsAlive)
            {
                int xPos = (int)Position.X;
                int yPos = (int)Position.Y;
                Vector2 cannonOrigin = new Vector2(11, 50);

                spriteBatch.Draw(_textureCenter.cannonTexture, new Vector2(xPos + 20, yPos - 10), null, Color, Angle, cannonOrigin, playerScaling, SpriteEffects.None, 1);
                spriteBatch.Draw(_textureCenter.carriageTexture, Position, null, Color, 0, new Vector2(0, _textureCenter.carriageTexture.Height), playerScaling, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
