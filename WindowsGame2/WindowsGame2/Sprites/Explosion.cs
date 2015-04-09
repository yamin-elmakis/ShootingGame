using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame2;
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
        
        public List<Particle> particleList = new List<Particle>();
        
        public Explosion(Microsoft.Xna.Framework.Game game) : base(game)
        {
            this.game = game;
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
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
            {
                Particle particle = new Particle(game, explosionPos, size, maxAge, i, gameTime);
                particleList.Add(particle);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Game1.globalTransformation);
            foreach (var particle in particleList)
            {
                particle.Draw(gameTime);
            }
            spriteBatch.End();
            
            base.Draw(gameTime);
        }


    }
}
