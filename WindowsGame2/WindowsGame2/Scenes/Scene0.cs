using WindowsGame2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YaminGame.Utilities;

namespace YaminGame.Scenes
{
    class Scene0 : Scene
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;

        public override int State { get; set; }

        public Scene0(Microsoft.Xna.Framework.Game game) : base(game)
        {
            _spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            _font = (SpriteFont)game.Services.GetService(typeof(SpriteFont)); 
            _initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            
            if (keyState.IsKeyDown(Keys.S))
            {
                State = 1;
            }
            if (keyState.IsKeyDown(Keys.J))
            {
                State = 1;
            }
            base.Update(gameTime);
        }

        private void _initialize()
        {
            State = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.GlobalTransformation);
            _spriteBatch.DrawString(_font, Consts.GameHelp, new Vector2(5, 5), Color.Red);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
