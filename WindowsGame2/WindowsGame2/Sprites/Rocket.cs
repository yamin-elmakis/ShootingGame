using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using WindowsGame2;
using Game.Scenes;
using Game.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YaminGame.Utilities;

namespace YaminGame.Sprites
{
    internal class Rocket : DrawableGameComponent
    {
        private Microsoft.Xna.Framework.Game game;
        private SpriteBatch spriteBacth;
        private SoundCenter soundCenter;
        
        public Color Color { get; set; }

        Texture2D rocketTexture;
        Texture2D smokeTexture;
        
        public static bool rocketFlying = false;
        public Vector2 rocketPosition;
        public Vector2 rocketDirection;
        public float rocketAngle;
        float rocketScaling = 0.1f;
        public List<Vector2> smokeList = new List<Vector2>(); 
        Random randomizer = new Random();
        Color[,] rocketColorArray;
        

        public Rocket(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
            this.game = game;
            rocketTexture = game.Content.Load<Texture2D>("rocket");
            smokeTexture = game.Content.Load<Texture2D>("smoke");
            rocketColorArray = Utils.TextureTo2DArray(rocketTexture);
            spriteBacth = (SpriteBatch) game.Services.GetService(typeof (SpriteBatch));
            soundCenter = (SoundCenter) game.Services.GetService(typeof (SoundCenter));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            if (rocketFlying)
            {
                Vector2 gravity = new Vector2(0, 1);
                rocketDirection += gravity/10.0f;
                rocketPosition += rocketDirection;
                rocketAngle = (float) Math.Atan2(rocketDirection.X, -rocketDirection.Y);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 smokePos = rocketPosition;
                    smokePos.X += randomizer.Next(10) - 5;
                    smokePos.Y += randomizer.Next(10) - 5;
                    smokeList.Add(smokePos);
                }

                if (CheckOutOfScreen())
                {
                    rocketFlying = false;
                    smokeList = new List<Vector2>();
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (rocketFlying)
                spriteBacth.Draw(rocketTexture, rocketPosition, null, Color, rocketAngle, new Vector2(42, 240), 0.1f, SpriteEffects.None, 1);
            base.Draw(gameTime);
        }

        public bool CheckOutOfScreen()
        {
            bool rocketOutOfScreen = rocketPosition.Y > Game1.screenHeight;
            rocketOutOfScreen |= rocketPosition.X < 0;
            rocketOutOfScreen |= rocketPosition.X > Game1.screenWidth;

            return rocketOutOfScreen;
        }

        public Vector2 CheckTerrainCollision(GameTime gameTime, Color[,] foregroundColorArray)
        {
            Matrix rocketMat = Matrix.CreateTranslation(-42, -240, 0) * Matrix.CreateRotationZ(rocketAngle) * Matrix.CreateScale(rocketScaling) * Matrix.CreateTranslation(rocketPosition.X, rocketPosition.Y, 0);
            Matrix terrainMat = Matrix.Identity;
            Vector2 terrainCollisionPoint = Utils.TexturesCollide(rocketColorArray, rocketMat, foregroundColorArray, terrainMat);
            if (terrainCollisionPoint.X > -1)            
                rocketHit();

            return terrainCollisionPoint;
        }

        public Vector2 CheckPlayersCollision(Carriage carriage)
        {
            Matrix rocketMat = Matrix.CreateTranslation(-42, -240, 0) * Matrix.CreateRotationZ(rocketAngle) * Matrix.CreateScale(rocketScaling) * Matrix.CreateTranslation(rocketPosition.X, rocketPosition.Y, 0);
            int xPos = (int)carriage.Position.X;
            int yPos = (int)carriage.Position.Y;

            Matrix carriageMat = Matrix.CreateTranslation(0, -carriage.carriageTexture.Height, 0) * Matrix.CreateScale(carriage.playerScaling) * Matrix.CreateTranslation(xPos, yPos, 0);
            Vector2 carriageCollisionPoint = Utils.TexturesCollide(carriage.carriageColorArray, carriageMat, rocketColorArray, rocketMat);

            if (carriageCollisionPoint.X > -1)
            {
                rocketHit();
                carriage.IsAlive = false;
                return carriageCollisionPoint;
            }

            Matrix cannonMat = Matrix.CreateTranslation(-11, -50, 0) * Matrix.CreateRotationZ(carriage.Angle) * Matrix.CreateScale(carriage.playerScaling) * Matrix.CreateTranslation(xPos + 20, yPos - 10, 0);
            Vector2 cannonCollisionPoint = Utils.TexturesCollide(carriage.cannonColorArray, cannonMat, rocketColorArray, rocketMat);
            if (cannonCollisionPoint.X > -1)
            {
                rocketHit();
                carriage.IsAlive = false;
                return cannonCollisionPoint;
            }

            return new Vector2(-1, -1);
        }

        private void rocketHit()
        {
            rocketFlying = false;
            smokeList = new List<Vector2>(); 
        }


    }
}
