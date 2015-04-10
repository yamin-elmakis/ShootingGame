using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        readonly Microsoft.Xna.Framework.Game _game;
        protected SpriteBatch SpriteBatch;
        private readonly TextureCenter _textureCenter;

        public List<ParticleData> ParticleList; 
        
        public Explosion(Microsoft.Xna.Framework.Game game) : base(game)
        {
            this._game = game;
            SpriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            _textureCenter = (TextureCenter)game.Services.GetService(typeof(TextureCenter));
            ParticleList = new List<ParticleData>();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            for (var i = ParticleList.Count - 1; i >= 0; i--)
            {
                var particle = ParticleList[i];
                if (!UpdateParticle(gameTime, particle))
                {
                    ParticleList.RemoveAt(i);
                }
            }
            base.Update(gameTime);
        }

        private Boolean UpdateParticle(GameTime gameTime, ParticleData particle)
        {
            var now = (float) gameTime.TotalGameTime.TotalMilliseconds;
            var timeAlive = now - particle.BirthTime;

            if (timeAlive > particle.MaxAge)
                return false;
            
            var relAge = timeAlive/particle.MaxAge;
            particle.Position = 0.5f*particle.Accelaration*relAge*relAge + particle.Direction*relAge + particle.OrginalPosition;

            var invAge = 1.0f - relAge;
            particle.ModColor = new Color(new Vector4(invAge, invAge, invAge, invAge));

            var positionFromCenter = particle.Position - particle.OrginalPosition;
            var distance = positionFromCenter.Length();
            particle.Scaling = (50.0f + distance)/200.0f;
            return true;
        }

        public void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
        {
            for (var i = 0; i < numberOfParticles; i++)
            {
                var particle = new ParticleData(explosionPos, size, maxAge, i, gameTime);
                ParticleList.Add(particle);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Game1.globalTransformation);
            foreach (var particle in ParticleList)
            {
                SpriteBatch.Draw(_textureCenter.ExplosionTexture, particle.Position, null, particle.ModColor, particle.Rotation, new Vector2(256, 256), particle.Scaling, SpriteEffects.None, 1);
            }
            SpriteBatch.End();
            //Debug.WriteLine("particleList: " + ParticleList.Count);
            
            base.Draw(gameTime);
        }
    }

    public class ParticleData
    {
        public float BirthTime, MaxAge,Scaling;
        public Vector2 OrginalPosition, Accelaration, Direction, Position;
        public Color ModColor;
        public int Rotation = 1;
        private readonly Random _randomizer = new Random();

        public ParticleData(Vector2 explosionPos, float explosionSize, float maxAge, int rotation, GameTime gameTime)
        {
            Scaling = 0.25f;
            ModColor = Color.White;
            this.Rotation = rotation;
            BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            MaxAge = maxAge;
            OrginalPosition = explosionPos;
            Position = explosionPos;

            var particleDistance = (float)_randomizer.NextDouble() * explosionSize;
            var displacement = new Vector2(particleDistance, 0);
            var angle = MathHelper.ToRadians(_randomizer.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            Direction = displacement * 2.0f;
            Accelaration = -Direction;
        }
    }
}
