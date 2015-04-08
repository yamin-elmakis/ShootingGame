using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YaminGame.Utilities;

namespace YaminGame.Sprites
{
    class Explosion : DrawableGameComponent
    {
        Microsoft.Xna.Framework.Game game;
        protected SpriteBatch spriteBatch;
        protected SoundCenter soundCenter;
        Texture2D explosionTexture;
        Color[,] explosionColorArray;
        public List<Particle> particleList = new List<Particle>();
        Random randomizer = new Random();

        public Explosion(Microsoft.Xna.Framework.Game game) : base(game)
        {
            this.game = game;
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            soundCenter = (SoundCenter)game.Services.GetService(typeof(SoundCenter));
            explosionTexture = game.Content.Load<Texture2D>("explosion");
            explosionColorArray = Utils.TextureTo2DArray(explosionTexture);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        private void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
        {
            for (int i = 0; i < numberOfParticles; i++)
                AddExplosionParticle(explosionPos, size, maxAge, gameTime);

        }

        private void AddExplosionParticle(Vector2 explosionPos, float explosionSize, float maxAge, GameTime gameTime)
        {
            Particle particle = new Particle(game);

            particle.OrginalPosition = explosionPos;
            particle.Position = particle.OrginalPosition;

            particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            particle.MaxAge = maxAge;
            particle.Scaling = 0.25f;
            particle.ModColor = Color.White;

            float particleDistance = (float)randomizer.NextDouble() * explosionSize;
            Vector2 displacement = new Vector2(particleDistance, 0);
            float angle = MathHelper.ToRadians(randomizer.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            particle.Direction = displacement * 2.0f;
            particle.Accelaration = -particle.Direction;

            particleList.Add(particle);

        }

        public override void Draw(GameTime gameTime)
        {
            //spriteBacth.Draw(Particle, ballPosition, Rectangles[FrameIndex],
            //    ballColor, Rotation, Origin, Scale, SpriteEffect, 0f);
            foreach (var particle in particleList)
            {
                particle.Draw(gameTime);
            }
            base.Draw(gameTime);
        }


    }
}
