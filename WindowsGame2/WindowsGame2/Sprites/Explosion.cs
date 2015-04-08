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
        public Texture2D explosionTexture;
        public Color[,] explosionColorArray;
        public List<Particle> particleList = new List<Particle>();
        
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
            for (int i = particleList.Count - 1; i >= 0; i--)
            {
                Particle particle = particleList[i];
                if(!particle.IsAlive)
                    particleList.RemoveAt(i);
            }
            base.Update(gameTime);
        }

        public void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
        {
            for (int i = 0; i < numberOfParticles; i++) 
                AddExplosionParticle(explosionPos, size, maxAge, gameTime);
        }

        private void AddExplosionParticle(Vector2 explosionPos, float explosionSize, float maxAge, GameTime gameTime)
        {
            Particle particle = new Particle(game, explosionPos, explosionSize, maxAge, gameTime);
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
