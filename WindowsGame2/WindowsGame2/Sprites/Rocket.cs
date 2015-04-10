using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Microsoft.Xna.Framework.Game _game;
        private readonly SpriteBatch _spriteBatch;
        private SoundCenter _soundCenter;
        private readonly TextureCenter _textureCenter;
        public static bool RocketFlying = false;
        public Vector2 RocketPosition, RocketDirection;
        public float RocketAngle;
        private const float RocketScaling = 0.1f;
        public List<Vector2> SmokeList = new List<Vector2>(); 
        private readonly Random _randomizer = new Random();
        
        public Color Color { get; set; }
        
        public Rocket(Microsoft.Xna.Framework.Game game): base(game)
        {
            this._game = game;
            _textureCenter = (TextureCenter)game.Services.GetService(typeof(TextureCenter));
            _spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            _soundCenter = (SoundCenter) game.Services.GetService(typeof (SoundCenter));
        }

        public override void Update(GameTime gameTime)
        {
            if (RocketFlying)
            {
                var gravity = new Vector2(0, 1);
                RocketDirection += gravity/10.0f;
                RocketPosition += RocketDirection;
                RocketAngle = (float) Math.Atan2(RocketDirection.X, -RocketDirection.Y);

                for (var i = 0; i < 5; i++)
                {
                    var smokePos = RocketPosition;
                    smokePos.X += _randomizer.Next(10) - 5;
                    smokePos.Y += _randomizer.Next(10) - 5;
                    SmokeList.Add(smokePos);
                }

                //if (CheckOutOfScreen())
                //{
                //    RocketFlying = false;
                //    SmokeList = new List<Vector2>();
                //}
            }
            base.Update(gameTime);
        }

        public bool CheckOutOfScreen()
        {
            var rocketOutOfScreen = RocketPosition.Y > Game1.screenHeight;
            rocketOutOfScreen |= RocketPosition.X < 0;
            rocketOutOfScreen |= RocketPosition.X > Game1.screenWidth;
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

            var carriageMat = Matrix.CreateTranslation(0, -_textureCenter.CarriageTexture.Height, 0) * Matrix.CreateScale(carriage.playerScaling) * Matrix.CreateTranslation(xPos, yPos, 0);
            var carriageCollisionPoint = Utils.TexturesCollide(_textureCenter.CarriageColorArray, carriageMat, _textureCenter.RocketColorArray, rocketMat);

            if (carriageCollisionPoint.X > -1)
            {
                RocketHit();
                return carriageCollisionPoint;
            }

            var cannonMat = Matrix.CreateTranslation(-11, -50, 0) * Matrix.CreateRotationZ(carriage.Angle) * Matrix.CreateScale(carriage.playerScaling) * Matrix.CreateTranslation(xPos + 20, yPos - 10, 0);
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
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.globalTransformation);
            if (RocketFlying)
                _spriteBatch.Draw(_textureCenter.RocketTexture, RocketPosition, null, Color, RocketAngle, new Vector2(42, 240), 0.1f, SpriteEffects.None, 1);
            foreach (var smokePos in SmokeList)
                _spriteBatch.Draw(_textureCenter.SmokeTexture, smokePos, null, Color.White, 0, new Vector2(40, 35), 0.2f, SpriteEffects.None, 1);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
