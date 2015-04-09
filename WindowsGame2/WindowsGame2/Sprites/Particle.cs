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
        
        SpriteBatch spriteBatch;
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
        public bool IsAlive;
        protected int rotation = 1;
        Random randomizer = new Random();
        private TextureCenter _textureCenter;
        
        public Particle(Microsoft.Xna.Framework.Game game, Vector2 explosionPos, float explosionSize, float maxAge, int rotation, GameTime gameTime)
            : base(game)
        {
            this.game = game;
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            _textureCenter = (TextureCenter)game.Services.GetService(typeof(TextureCenter));

            Scaling = 0.25f;
            ModColor = Color.White;
            this.rotation = rotation;
            BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            MaxAge = maxAge;
            OrginalPosition = explosionPos;
            Position = explosionPos;

            float particleDistance = (float)randomizer.NextDouble() * explosionSize;
            Vector2 displacement = new Vector2(particleDistance, 0);
            float angle = MathHelper.ToRadians(randomizer.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            Direction = displacement * 2.0f;
            Accelaration = -Direction;
            IsAlive = true;
        }

        protected override void Dispose(bool disposing)   {
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)   {
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            float timeAlive = now - BirthTime;

            if (timeAlive > MaxAge)
                IsAlive = false;
            else
            {
                float relAge = timeAlive / MaxAge;
                Position = 0.5f * Accelaration * relAge * relAge + Direction * relAge + OrginalPosition;

                float invAge = 1.0f - relAge;
                ModColor = new Color(new Vector4(invAge, invAge, invAge, invAge));

                Vector2 positionFromCenter = Position - OrginalPosition;
                float distance = positionFromCenter.Length();
                Scaling = (50.0f + distance) / 200.0f; 
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(_textureCenter.explosionTexture, Position, null, ModColor, rotation, new Vector2(256, 256), Scaling, SpriteEffects.None, 1);
            //spriteBacth.Draw(Particle, ballPosition, Rectangles[FrameIndex],
            //    ballColor, Rotation, Origin, Scale, SpriteEffect, 0f);
            base.Draw(gameTime);
        }
    }
}
