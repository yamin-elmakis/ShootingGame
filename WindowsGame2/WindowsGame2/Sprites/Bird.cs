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
        private SpriteEffects SpriteEffect;

        private Vector2 _birdPosition, Origin, _birdSpeed;
        protected int FrameIndex = 0, Width, Height, Index;
        private const int Frames = 3, Peakheight = 100, MaxSpeed = 60;
        protected Rectangle[] Rectangles;
        private float _timeElapsed, _timeToUpdate = 0.05f;
        public Rectangle birdRect;
        private readonly Color _color;
        private float Rotation = 0f, Scale = 1f;
        
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
            this.Index = index;
            Width = _textureCenter.BirdSprite.Width / Frames;
            Height = _textureCenter.BirdSprite.Height;
            for (var i = 0; i < Frames; i++)
                Rectangles[i] = new Rectangle(i * Width, 0, Width, Height);
            FramesPerSecond = 10;
            _color = Color.White;

            _birdSpeed = new Vector2(MaxSpeed, 0);
            _birdPosition.X = Game1.screenWidth - Width;
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

        public override void Update(GameTime gameTime)
        {
            ChangeSpeed();
            Debug.WriteLine("_birdSpeed.X "+Index+": " + _birdSpeed.X);
            _birdPosition.X -= _birdSpeed.X*(float) gameTime.ElapsedGameTime.TotalSeconds;

            if (_birdPosition.X < -2 * Width)
                _birdPosition.X = Game1.screenWidth + Width;

            _timeElapsed += (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeElapsed > _timeToUpdate)
            {
                _timeElapsed -= _timeToUpdate;
                FrameIndex = (FrameIndex + 1)%Frames;
            }

            birdRect = new Rectangle((int) _birdPosition.X, (int) _birdPosition.Y,
                Width, Convert.ToInt32(_textureCenter.BirdSprite.Height));
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBacth.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.globalTransformation);
            _spriteBacth.Draw(_textureCenter.BirdSprite, _birdPosition, Rectangles[FrameIndex],
                _color, Rotation, Origin, Scale, SpriteEffect, 0f);
            _spriteBacth.End();
            base.Draw(gameTime);
        }
    }
}
