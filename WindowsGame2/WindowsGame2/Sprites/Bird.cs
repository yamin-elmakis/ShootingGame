using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WindowsGame2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YaminGame.Utilities;

namespace YaminGame.Sprites
{
    class Bird : DrawableGameComponent
    {
        private readonly SpriteBatch _spriteBacth;
        private readonly TextureCenter _textureCenter;
        private Vector2 _birdPosition, _birdSpeed;
        protected int FrameIndex = 0, Width, Height, Index;
        private const int Frames = 3, Peakheight = 100, MaxSpeed = 60;
        protected Rectangle[] Rectangles;
        private float _timeElapsed, _timeToUpdate = 0.05f;
        private const float Rotation = 0f;
        private const float Scale = 1f;
        private Boolean IsAlive { get; set; }

        private readonly Random _randomizer = new Random();

        public int FramesPerSecond
        {
            set { _timeToUpdate = (1f / value); }
        }

        public Bird(Microsoft.Xna.Framework.Game game, int index): base(game)
        {
            _spriteBacth = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            _textureCenter = (TextureCenter)game.Services.GetService(typeof(TextureCenter));
            Rectangles = new Rectangle[Frames];
            Index = index;
            Width = _textureCenter.BirdSprite.Width / Frames;
            Height = _textureCenter.BirdSprite.Height;
            for (var i = 0; i < Frames; i++)
                Rectangles[i] = new Rectangle(i * Width, 0, Width, Height);
            FramesPerSecond = 10;
            IsAlive = true;
            _birdSpeed = new Vector2(MaxSpeed, 0);
            _birdPosition.X = Game1.ScreenWidth - Width;
            _birdPosition.Y = index*Height;
        }

        private void ChangeSpeed()
        {
            _birdSpeed.X += _randomizer.Next(-2*Index, 2*Index+1);
            if (_birdSpeed.X < 1)
                _birdSpeed.X = 10;
            if (_birdSpeed.X > MaxSpeed)
                _birdSpeed.X = MaxSpeed;
        }

        public Vector2 CheckBirdCollision(Rocket rocket)
        {
            if (!IsAlive) return new Vector2(-1, -1);
            
            var rocketMat = Matrix.CreateTranslation(-42, -240, 0) * Matrix.CreateRotationZ(rocket.RocketAngle) * Matrix.CreateScale(rocket.RocketScaling) * Matrix.CreateTranslation(rocket.RocketPosition.X, rocket.RocketPosition.Y, 0);
            var xPos = (int)_birdPosition.X;
            var yPos = (int)_birdPosition.Y;

            var birdMat = Matrix.CreateTranslation(0, -_textureCenter.BirdSprite.Height, 0) * Matrix.CreateScale(1) * Matrix.CreateTranslation(xPos, yPos, 0);
            var birdCollisionPoint = Utils.TexturesCollide(_textureCenter.BirdColorArray, birdMat, _textureCenter.RocketColorArray, rocketMat);

            if (!(birdCollisionPoint.X > -1)) return new Vector2(-1, -1);
           
            IsAlive = false;
            return birdCollisionPoint;
        }

        public override void Update(GameTime gameTime)
        {
            ChangeSpeed();
            //Debug.WriteLine("_birdSpeed.X "+Index+": " + _birdSpeed.X);
            _birdPosition.X -= _birdSpeed.X*(float) gameTime.ElapsedGameTime.TotalSeconds;

            if (_birdPosition.X < -2*Width)
            {
                _birdSpeed.X = MaxSpeed;
                IsAlive = true;
                _birdPosition.X = Game1.ScreenWidth + Width;
            }
            
            _timeElapsed += (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (!(_timeElapsed > _timeToUpdate)) return;

            _timeElapsed -= _timeToUpdate;
            FrameIndex = (FrameIndex + 1)%Frames;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsAlive) return;
            _spriteBacth.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.GlobalTransformation);
            _spriteBacth.Draw(_textureCenter.BirdSprite, _birdPosition, Rectangles[FrameIndex],
                Color.White, Rotation, new Vector2(), Scale, new SpriteEffects(), 0f);
            _spriteBacth.End();
            base.Draw(gameTime);
        }
    }
}
