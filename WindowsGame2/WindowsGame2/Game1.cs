using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YaminGame.Scenes;
using YaminGame.Utilities;

namespace WindowsGame2
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _device;
        private Scene0 _scene0;
        private Scene1 _scene1;
        private Scene2 _scene2;
        private SoundCenter _soundCenter;
        private TextureCenter _textureCenter;
        private SpriteFont _font;
        public static int ScreenWidth, ScreenHeight;
        public static Matrix GlobalTransformation = Matrix.CreateScale(new Vector3(1, 1, 1));

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferHeight = ScreenHeight = 500,
                PreferredBackBufferWidth = ScreenWidth = 500
            };
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            _graphics.ApplyChanges();
            Window.Title = "Yamin Elmakis Game";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _device = _graphics.GraphicsDevice;
            ScreenWidth = _device.PresentationParameters.BackBufferWidth;
            ScreenHeight = _device.PresentationParameters.BackBufferHeight;
            _soundCenter = new SoundCenter(this);
            _textureCenter = new TextureCenter(this);
            _font = Content.Load<SpriteFont>("MyFont");
            Services.AddService(typeof (SpriteBatch), _spriteBatch);
            Services.AddService(typeof (SoundCenter), _soundCenter);
            Services.AddService(typeof (SpriteFont), _font);
            Services.AddService(typeof(TextureCenter), _textureCenter);
            _scene0 = new Scene0(this);
            _scene1 = new Scene1(this, _device);
            _scene1.Hide();
            _scene0.Show();
            Components.Add(_scene0);
            Components.Add(_scene1);
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
           
            switch (_scene0.State + _scene1.State)
            {
                case 0:
                    _scene0.Show();
                    _scene1.Hide();
                    break;
                case 1:
                    _scene0.Hide();
                    _scene1.Show();
                    break;
                case 2:
                    _scene0.Hide();
                    _scene1.Hide();
                    if (_scene2 == null)
                    {
                        _scene2 = new Scene2(this, _device);
                        Components.Add(_scene2);
                    }
                    _scene2.Show();
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);            
        }
    }
}
