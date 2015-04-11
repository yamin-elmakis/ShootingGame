using System;
using System.Diagnostics;
using WindowsGame2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YaminGame.Scenes;
using YaminGame.Sprites;

namespace Game.Scenes
{
    internal class Scene2 : Scene1
    {
        private float timeElapsed, timeToUpdate = 0.2f;
        private readonly Random _randomizer = new Random();
        private float wind;
        private bool setWind = false;

        public Scene2(Microsoft.Xna.Framework.Game game, GraphicsDevice device): base(game, device)
        {
            timeElapsed = 0;
            State = 0;
            _rocket.MaxX = Game1.ScreenWidth*2;
            _rocket.MinX = -Game1.ScreenWidth;
            Debug.WriteLine("Scene2 constarcot end!");
        }

        private void SetWind()
        {
            wind = (float)_randomizer.NextDouble();
            wind -= 0.9f;
            _rocket.Gravity.X = wind;
        }

        public override void Update(GameTime gameTime)
        {
            timeElapsed += (float)
                gameTime.ElapsedGameTime.TotalSeconds;
            //Debug.WriteLine("time: " + timeElapsed);
            if (NewRound)
            {
                NewRound = false;
                SetWind();
            }
            base.Update(gameTime);
        }

        private void DrawWindText()
        {
            var player = _carriages[CurrentPlayer];
            var windText = "Wind: ";
            if (wind.Equals(0.0f))
                windText += "0";
            else
                windText += wind > 0 ? " --> " + wind.ToString("0.00") : " <-- " + (-wind).ToString("0.00");
            SpriteBatch.DrawString(Font, windText, new Vector2(20, 70), player.Color);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.GlobalTransformation);
            DrawWindText();
            SpriteBatch.End();
            
        }
    }
}
