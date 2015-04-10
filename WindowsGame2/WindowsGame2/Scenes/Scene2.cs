﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WindowsGame2;
using Game.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using YaminGame.Scenes;
using YaminGame.Sprites;
using YaminGame.Utilities;

namespace Game.Scenes
{
    internal class Scene2 : Scene1
    {
        private Rocket _rocket;
        private float timeElapsed, timeToUpdate = 0.2f;

        public Scene2(Microsoft.Xna.Framework.Game game, GraphicsDevice device)
            : base(game, device)
        {
            //this.game = game;
            //this.screenHeight = screenHeight;
            //this.screenWidth = screenWidth;
            //back = game.Content.Load<Texture2D>("hockey_ice");
            //spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            //soundCenter = (SoundCenter)game.Services.GetService(typeof(SoundCenter));
            //font = (SpriteFont)game.Services.GetService(typeof(SpriteFont));
            //Particle = new Particle(game);
            _rocket = new Rocket(game);
            SceneComponents.Add(_rocket);
            _initialize();
        }

        public override void Update(GameTime gameTime)
        {
            timeElapsed += (float)
                gameTime.ElapsedGameTime.TotalSeconds;
            Debug.WriteLine("time: " + timeElapsed);
            
            base.Update(gameTime);
        }

        private void _initialize()
        {
            timeElapsed = 0;
            State = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.globalTransformation);
            //var screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            //spriteBatch.Draw(back, screenRectangle, Color.White);
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
