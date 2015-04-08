using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        Texture2D carriageTexture;
        Texture2D cannonTexture;
        Color[,] carriageColorArray;
        Color[,] cannonColorArray;
        float playerScaling;

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

            carriageTexture = game.Content.Load<Texture2D>("carriage");
            cannonTexture = game.Content.Load<Texture2D>("cannon");
            carriageColorArray = Scene1.TextureTo2DArray(carriageTexture);
            cannonColorArray = Scene1.TextureTo2DArray(cannonTexture);
            playerScaling = 40.0f / (float)carriageTexture.Width;
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
            //spriteBatch.Draw(playerSprite, playerPosition, Color.White);
            base.Draw(gameTime);
        }
    }
}
