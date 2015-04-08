using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame2;
using YaminGame.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
//========================

//========================
namespace Game.Sprites
{
    public class Particle : DrawableGameComponent    {
        Microsoft.Xna.Framework.Game game;
        SpriteBatch spriteBacth;
        Texture2D ballSprite;
        Vector2 ballPosition;
        SoundCenter soundCenter;
        public Vector2 ballSpeed;
        public Rectangle ballRect;
        private readonly int _maxY, _maxX, _minY;

        private int frames = 5;
        protected Rectangle[] Rectangles;
        protected int FrameIndex = 0;
        private float timeToUpdate = 0.05f;

        /// <summary>
        /// new
        /// </summary>
        public float BirthTime;
        public float MaxAge;
        public Vector2 OrginalPosition;
        public Vector2 Accelaration;
        public Vector2 Direction;
        public Vector2 Position;
        public float Scaling;
        public Color ModColor;

        public Particle(Microsoft.Xna.Framework.Game game) : base(game)
        {
            this.game = game;
            ballSprite = game.Content.Load<Texture2D>("eballs5");
            spriteBacth = (SpriteBatch) game.Services.GetService(typeof (SpriteBatch));
            soundCenter = (SoundCenter) game.Services.GetService(typeof (SoundCenter));
            _minY = Consts.GoalYline; // the sides of the game board
            _maxX = game.GraphicsDevice.Viewport.Width - ballSprite.Width / frames;
            _maxY = game.GraphicsDevice.Viewport.Height - ballSprite.Height - Consts.GoalYline;
            var width = ballSprite.Width/frames;
            //Rectangles = new Rectangle[frames];
            //for (var i = 0; i < frames; i++)
            //    Rectangles[i] = new Rectangle(i*width, 0, width, ballSprite.Height);
            //FramesPerSecond = 10;
            //InitBallParam();
        }

        public void SetFrame(int frame)
        {
            //if (frame < Rectangles.Length)
            //    FrameIndex = frame;
        }

        protected override void Dispose(bool disposing)   {
            ballSprite.Dispose();
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)   {
           // ballPosition += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
;
            
        }

        public int FramesPerSecond
        {
            set { timeToUpdate = (1f / value); }
        }

        public override void Draw(GameTime gameTime)
        {
            //spriteBacth.Draw(Particle, ballPosition, Rectangles[FrameIndex],
            //    ballColor, Rotation, Origin, Scale, SpriteEffect, 0f);
            base.Draw(gameTime);
        }
    }
}
