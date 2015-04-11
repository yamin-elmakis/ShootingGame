using System;
using System.Collections.Generic;
using WindowsGame2;
using Game.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YaminGame.Utilities;

namespace YaminGame.Sprites
{
    public class Rocket : DrawableGameComponent
    {
        public static bool RocketFlying = false;
        public Vector2 Gravity, RocketPosition, RocketDirection;
        public float RocketAngle, RocketScaling = 0.1f;
        public List<Vector2> SmokeList = new List<Vector2>();
        public int MaxX { set; get; }
        public int MinX { set; get; }
        public Color Color { get; set; }

        private readonly SpriteBatch _spriteBatch;
        private readonly TextureCenter _textureCenter;
        private readonly Random _randomizer = new Random();
        
        public Rocket(Microsoft.Xna.Framework.Game game): base(game)
        {
            _textureCenter = (TextureCenter)game.Services.GetService(typeof(TextureCenter));
            _spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            Gravity = new Vector2(0, 1);
            MaxX = Game1.ScreenWidth;
            MinX = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (RocketFlying)
            {
                RocketDirection += Gravity/10.0f;
                RocketPosition += RocketDirection;
                RocketAngle = (float) Math.Atan2(RocketDirection.X, -RocketDirection.Y);

                for (var i = 0; i < 5; i++)
                {
                    var smokePos = RocketPosition;
                    smokePos.X += _randomizer.Next(10) - 5;
                    smokePos.Y += _randomizer.Next(10) - 5;
                    SmokeList.Add(smokePos);
                }
            }
            base.Update(gameTime);
        }

        public bool CheckOutOfScreen()
        {
            var rocketOutOfScreen = RocketPosition.Y > Game1.ScreenHeight;
            rocketOutOfScreen |= RocketPosition.X < MinX;
            rocketOutOfScreen |= RocketPosition.X > MaxX;
            //Debug.WriteLine("RocketPosition: " + RocketPosition + " bool: " + rocketOutOfScreen);
           
            if (!rocketOutOfScreen) return false;
           
            RocketFlying = false;
            SmokeList = new List<Vector2>();
            return true;
        }

        public Vector2 CheckTerrainCollision(GameTime gameTime, Color[,] foregroundColorArray)
        {
            var rocketMat = Matrix.CreateTranslation(-42, -240, 0) * Matrix.CreateRotationZ(RocketAngle) * Matrix.CreateScale(RocketScaling) * Matrix.CreateTranslation(RocketPosition.X, RocketPosition.Y, 0);
            var terrainMat = Matrix.Identity;
            var terrainCollisionPoint = Utils.TexturesCollide(_textureCenter.RocketColorArray, rocketMat, foregroundColorArray, terrainMat);
            if (terrainCollisionPoint.X > -1)            
                RocketHit();

            return terrainCollisionPoint;
        }

        public Vector2 CheckPlayersCollision(Carriage carriage)
        {
            var rocketMat = Matrix.CreateTranslation(-42, -240, 0) * Matrix.CreateRotationZ(RocketAngle) * Matrix.CreateScale(RocketScaling) * Matrix.CreateTranslation(RocketPosition.X, RocketPosition.Y, 0);
            var xPos = (int)carriage.Position.X;
            var yPos = (int)carriage.Position.Y;

            var carriageMat = Matrix.CreateTranslation(0, -_textureCenter.CarriageTexture.Height, 0) * Matrix.CreateScale(carriage.PlayerScaling) * Matrix.CreateTranslation(xPos, yPos, 0);
            var carriageCollisionPoint = Utils.TexturesCollide(_textureCenter.CarriageColorArray, carriageMat, _textureCenter.RocketColorArray, rocketMat);

            if (carriageCollisionPoint.X > -1)
            {
                RocketHit();
                return carriageCollisionPoint;
            }

            var cannonMat = Matrix.CreateTranslation(-11, -50, 0) * Matrix.CreateRotationZ(carriage.Angle) * Matrix.CreateScale(carriage.PlayerScaling) * Matrix.CreateTranslation(xPos + 20, yPos - 10, 0);
            var cannonCollisionPoint = Utils.TexturesCollide(_textureCenter.CannonColorArray, cannonMat, _textureCenter.RocketColorArray, rocketMat);
            
            if (!(cannonCollisionPoint.X > -1)) return new Vector2(-1, -1);
            
            RocketHit();
            carriage.IsAlive = false;
            return cannonCollisionPoint;
        }

        private void RocketHit()
        {
            RocketFlying = false;
            SmokeList = new List<Vector2>(); 
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.GlobalTransformation);
            if (RocketFlying)
                _spriteBatch.Draw(_textureCenter.RocketTexture, RocketPosition, null, Color, RocketAngle, new Vector2(42, 240), 0.1f, SpriteEffects.None, 1);
            foreach (var smokePos in SmokeList)
                _spriteBatch.Draw(_textureCenter.SmokeTexture, smokePos, null, Color.White, 0, new Vector2(40, 35), 0.2f, SpriteEffects.None, 1);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
