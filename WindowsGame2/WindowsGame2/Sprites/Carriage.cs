using System;
using WindowsGame2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YaminGame.Utilities;

namespace Game.Sprites
{
    public class Carriage : DrawableGameComponent
    {
        public Vector2 Position;
        public bool IsAlive;
        public Color Color;
        public string ColorName;
        public float Angle, Power, PlayerScaling;

        private readonly SpriteBatch _spriteBatch;
        private readonly TextureCenter _textureCenter;
        private static readonly Color[] PlayerColors;
        private static readonly string[] ColorsNames;
        private static readonly int ShiftColor;

        static Carriage ()
        {
            var randomizer = new Random();
            ShiftColor = randomizer.Next(8);
            PlayerColors = new Color[10];
            ColorsNames = new[] { "Red", "Green", "Blue", "Purple", "Orange", "Indigo", "Yellow", "SaddleBrown", "Tomato", "Turquoise" };
            PlayerColors[0] = Color.Red;
            PlayerColors[1] = Color.Green;
            PlayerColors[2] = Color.Blue;
            PlayerColors[3] = Color.Purple;
            PlayerColors[4] = Color.Orange;
            PlayerColors[5] = Color.Indigo;
            PlayerColors[6] = Color.Yellow;
            PlayerColors[7] = Color.SaddleBrown;
            PlayerColors[8] = Color.Tomato;
            PlayerColors[9] = Color.Turquoise;
        }

        public Carriage(Microsoft.Xna.Framework.Game game, int i): base(game)
        {
            _spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            _textureCenter = (TextureCenter)game.Services.GetService(typeof (TextureCenter));

            PlayerScaling = 40.0f / _textureCenter.CarriageTexture.Width;
            InitCarriage(i);

            game.IsMouseVisible = true;
        }

        private void InitCarriage(int i)
        {
            IsAlive = true;
            Color = PlayerColors[(i + ShiftColor)%10];
            ColorName = ColorsNames[(i + ShiftColor) % 10];
            Angle = MathHelper.ToRadians(90);
            Power = 100;
        }
        
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.GlobalTransformation);
            if (IsAlive)
            {
                var xPos = (int)Position.X;
                var yPos = (int)Position.Y;
                var cannonOrigin = new Vector2(11, 50);

                _spriteBatch.Draw(_textureCenter.CannonTexture, new Vector2(xPos + 20, yPos - 10), null, Color, Angle, cannonOrigin, PlayerScaling, SpriteEffects.None, 1);
                _spriteBatch.Draw(_textureCenter.CarriageTexture, Position, null, Color, 0, new Vector2(0, _textureCenter.CarriageTexture.Height), PlayerScaling, SpriteEffects.None, 0);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
