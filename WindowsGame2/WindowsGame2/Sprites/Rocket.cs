using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame2;
using Game.Scenes;
using Game.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YaminGame.Utilities;

namespace YaminGame.Sprites
{
    internal class Rocket : DrawableGameComponent
    {
        private Microsoft.Xna.Framework.Game game;
        private SpriteBatch spriteBacth;
        private Texture2D wallSprite;
        private Vector2 wallPosition;
        private SoundCenter soundCenter;
        public Vector2 wallSpeed;
        public Rectangle wallRect;
        public bool IsIntersects;

        /// <summary>
        /// new
        /// </summary>
        Texture2D rocketTexture;
        Texture2D smokeTexture;
        Texture2D explosionTexture;
        static bool rocketFlying = false;
        Vector2 rocketPosition;
        Vector2 rocketDirection;
        float rocketAngle;
        float rocketScaling = 0.1f;
        List<Vector2> smokeList = new List<Vector2>(); Random randomizer = new Random();
        Color[,] rocketColorArray;

        Color[,] explosionColorArray;
        List<Particle> particleList = new List<Particle>();

        public Rocket(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
            this.game = game;
            rocketTexture = game.Content.Load<Texture2D>("rocket");
            smokeTexture = game.Content.Load<Texture2D>("smoke");
            explosionTexture = game.Content.Load<Texture2D>("explosion");
            rocketColorArray = Scene1.TextureTo2DArray(rocketTexture);
            spriteBacth = (SpriteBatch) game.Services.GetService(typeof (SpriteBatch));
            soundCenter = (SoundCenter) game.Services.GetService(typeof (SoundCenter));
            //wallSpeed = new Vector2(100,0);
            //wallPosition = new Vector2(0, game.GraphicsDevice.Viewport.Height/2 - wallSprite.Height/2);
            //IsIntersects = false;
        }

        protected override void Dispose(bool disposing)
        {
            wallSprite.Dispose();
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            wallPosition += wallSpeed*(float) gameTime.ElapsedGameTime.TotalSeconds;
            var maxX = game.GraphicsDevice.Viewport.Width - wallSprite.Width;

            if (wallPosition.X > maxX || wallPosition.X < 0) wallSpeed.X *= -1;
            
            wallRect = new Rectangle((int) wallPosition.X, (int) wallPosition.Y, wallSprite.Width, wallSprite.Height);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBacth.Draw(wallSprite, wallPosition, Color.White);
            base.Draw(gameTime);
        }



    }
}
