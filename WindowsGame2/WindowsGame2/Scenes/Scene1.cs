using System;
using System.Diagnostics;
using WindowsGame2;
using Game.Scenes;
using Game.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YaminGame.Sprites;
using YaminGame.Utilities;

namespace YaminGame.Scenes
{
    public class Scene1 : Scene
    {
        public override int State { get; set; }
        public int CurrentPlayer = 0;
        
        protected SpriteBatch SpriteBatch;
        protected SoundCenter SoundCenter;
        protected SpriteFont Font;
        protected bool NewRound = true;
        protected Rocket _rocket;
        protected Carriage[] _carriages;
        protected GraphicsDevice Device;

        private readonly Microsoft.Xna.Framework.Game _game;
        private bool _endScene;
        private string _endText = "END Scene1";
        private float _timeElapsed;
        private const float TimeToUpdate = 1.2f;
        private Vector2 _textSize;
        private Bird[] _birds;
        private readonly Explosion _explosion;
        private float playerExplosionSize = 80.0f, playerExplosionMaxAge = 2000.0f;
        private float terrainExplosionSize = 30.0f, terrainExplosionMaxAge = 1000.0f;
        private float birdExplosionSize = 20.0f, birdExplosionMaxAge = 900.0f;
        private int playerExplosionParticles = 10, terrainExplosionParticles = 5, birdExplosionParticles = 4;
        private readonly TextureCenter _textureCenter;
        private double _snowHeight;
        private int NumberOfPlayers = 3;
        private int[] _terrainContour;
        private readonly Random _randomizer = new Random();

        public Scene1(Microsoft.Xna.Framework.Game game, GraphicsDevice device): base(game)
        {
            _game = game;
            Device = device;
            
            _textureCenter = (TextureCenter)game.Services.GetService(typeof(TextureCenter));
            SpriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            SoundCenter = (SoundCenter)game.Services.GetService(typeof(SoundCenter));
            Font = (SpriteFont)game.Services.GetService(typeof(SpriteFont));
            _rocket = new Rocket(game);
            _explosion = new Explosion(game);
            _birds = new Bird[3];
            SceneComponents.Add(_explosion);
            SceneComponents.Add(_rocket);
            for (var index = 0; index < _birds.Length; index++)
            {
                _birds[index] = new Bird(game, index);
                SceneComponents.Add(_birds[index]);    
            }
            GenerateTerrainContour();
            SetUpPlayers();
            _rocket.Color = _carriages[0].Color;
            foreach (var carriage in _carriages)
            {
                SceneComponents.Add(carriage);   
            }
            FlattenTerrainBelowPlayers();
            CreateForeground();
            Debug.WriteLine("Scene1 constarcot end!");
        }

        private void GenerateTerrainContour()
        {
            _terrainContour = new int[Game1.ScreenWidth];

            var rand1 = _randomizer.NextDouble() + 1;
            var rand2 = _randomizer.NextDouble() + 2;
            var rand3 = _randomizer.NextDouble() + 3;

            float offset = Game1.ScreenHeight / 2;
            const float peakheight = 100;
            const float flatness = 70;
            double minHeight = Game1.ScreenHeight;

            for (var x = 0; x < Game1.ScreenWidth; x++)
            {
                var height = peakheight / rand1 * Math.Sin((float)x / flatness * rand1 + rand1);
                height += peakheight / rand2 * Math.Sin((float)x / flatness * rand2 + rand2);
                height += peakheight / rand3 * Math.Sin((float)x / flatness * rand3 + rand3);
                height += offset;
                _terrainContour[x] = (int)height;
                
                // find the highest point in the mountens
                // the lowest number in the array
                if (height < minHeight)
                {
                    minHeight = height;
                }
            }
            // set the snow level
            _snowHeight = 1.5 * minHeight;
        }

        private void SetUpPlayers()
        {
            _carriages = new Carriage[NumberOfPlayers];
            for (var i = 0; i < NumberOfPlayers; i++)
            {
                _carriages[i] = new Carriage(_game, i)
                {
                    Position = new Vector2 {X = Game1.ScreenWidth/(NumberOfPlayers + 1)*(i + 1)}
                };
                _carriages[i].Position.Y = _terrainContour[(int)_carriages[i].Position.X];
            }
        }

        private void FlattenTerrainBelowPlayers()
        {
            foreach (var player in _carriages)
                if (player.IsAlive)
                    for (var x = 0; x < 40; x++)
                        _terrainContour[(int)player.Position.X + x] = _terrainContour[(int)player.Position.X];
        }

        private void CreateForeground()
        {
            var groundColors = Utils.TextureTo2DArray(_textureCenter.GroundTexture);
            var foregroundColors = new Color[Game1.ScreenWidth * Game1.ScreenHeight];

            for (var x = 0; x < Game1.ScreenWidth; x++)
            {
                for (var y = 0; y < Game1.ScreenHeight; y++)
                {
                    if (y > _terrainContour[x])
                        // add snow at the top of the mountain
                        if (y < _snowHeight)
                        {
                            foregroundColors[x + y * Game1.ScreenWidth] = _textureCenter.SnowColors[x % _textureCenter.SnowTexture.Width, y % _textureCenter.SnowTexture.Height];
                            //Debug.WriteLine("CreateForeground: y:" + y + " snowHeight: " + snowHeight);
                        }
                        else foregroundColors[x + y * Game1.ScreenWidth] = groundColors[x % _textureCenter.GroundTexture.Width, y % _textureCenter.GroundTexture.Height]; 
                            
                    else
                        foregroundColors[x + y * Game1.ScreenWidth] = Color.Transparent;
                }
            }

            _textureCenter.ForegroundTexture = new Texture2D(Device, Game1.ScreenWidth, Game1.ScreenHeight, false, SurfaceFormat.Color);
            _textureCenter.ForegroundTexture.SetData(foregroundColors);

            _textureCenter.ForegroundColorArray = Utils.TextureTo2DArray(_textureCenter.ForegroundTexture);
        }

        public override void Update(GameTime gameTime)
        {
            // for managing the scenes
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.J))
            {
                _showEndScene();
            }
            if (_endScene)
            {
                _timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timeElapsed > TimeToUpdate)
                    State = 1;
                return;
            }

            // check if a player can aim
            if ((!Rocket.RocketFlying) && (_explosion.ParticleList.Count == 0))
                ProcessKeyboard();

            if (Rocket.RocketFlying)
            {
                var terrainCollisionPoint = _rocket.CheckTerrainCollision(gameTime, _textureCenter.ForegroundColorArray);
                var playerCollisionPoint = CheckPlayersCollision();
                CheckBirdsCollision(gameTime);

                if (terrainCollisionPoint.X > -1)
                {
                    Debug.WriteLine("Rocket hit terrain");
                    AddExplosion(terrainCollisionPoint, terrainExplosionParticles, terrainExplosionSize,
                        terrainExplosionMaxAge, gameTime);
                    SoundCenter.HitTerrain.Play();
                    NextPlayer();
                }
                else if (playerCollisionPoint.X > -1)
                {
                    Debug.WriteLine("Rocket hit player");
                    AddExplosion(playerCollisionPoint, playerExplosionParticles, playerExplosionSize,
                        playerExplosionMaxAge, gameTime);
                    SoundCenter.HitCannon.Play();
                    NextPlayer();
                }
                else if (_rocket.CheckOutOfScreen())
                    NextPlayer();
            }

            base.Update(gameTime);
        }

        private void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
        {
            _explosion.AddExplosion(explosionPos, numberOfParticles, size, maxAge, gameTime);

            var rotation = (float)_randomizer.Next(1,10);
            var mat =
                Matrix.CreateTranslation(-_textureCenter.ExplosionTexture.Width / 2,
                    -_textureCenter.ExplosionTexture.Height / 2, 0) * Matrix.CreateRotationZ(rotation) *
                Matrix.CreateScale(terrainExplosionSize / (float)_textureCenter.ExplosionTexture.Width * 2.0f) *
                Matrix.CreateTranslation(explosionPos.X, explosionPos.Y, 0);
            AddCrater(_textureCenter.ExplosionColorArray, mat);

            // set the new place of the players
            foreach (var carriage in _carriages)
                carriage.Position.Y = _terrainContour[(int)carriage.Position.X];

            FlattenTerrainBelowPlayers();
            CreateForeground();
        }

        private void CheckBirdsCollision(GameTime gameTime)
        {
            foreach (var bird in _birds)
            {
                var birdCollisionPoint = bird.CheckBirdCollision(_rocket);
                if (!(birdCollisionPoint.X > -1)) continue;
                SoundCenter.HitBird.Play();
                _explosion.AddExplosion(birdCollisionPoint, birdExplosionParticles, birdExplosionSize, birdExplosionMaxAge, gameTime);
            }
        }

        private Vector2 CheckPlayersCollision()
        {
            var playerCollisionPoint = new Vector2(-1, -1);
            for (var index = 0; index < _carriages.Length; index++)
            {
                var carriage = _carriages[index];
                if (carriage.IsAlive && index != CurrentPlayer)
                {
                    playerCollisionPoint = _rocket.CheckPlayersCollision(carriage);

                    if (playerCollisionPoint.X > -1)
                    {
                        carriage.IsAlive = false;
                        break;
                    }  
                }
            }
            return playerCollisionPoint;
        }

        private void AddCrater(Color[,] tex, Matrix mat)
        {
            var width = tex.GetLength(0);
            var height = tex.GetLength(1);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if (tex[x, y].R <= 10) 
                        continue;
                    
                    var imagePos = new Vector2(x, y);
                    var screenPos = Vector2.Transform(imagePos, mat);

                    var screenX = (int)screenPos.X;
                    var screenY = (int)screenPos.Y;

                    if ((screenX) <= 0 || (screenX >= Game1.ScreenWidth)) 
                        continue;
                        
                    if (_terrainContour[screenX] < screenY)
                        _terrainContour[screenX] = screenY;
                }
            }
        }

        private void ProcessKeyboard()
        {
            KeyboardState keybState = Keyboard.GetState();
            if (keybState.IsKeyDown(Keys.Left))
                _carriages[CurrentPlayer].Angle -= 0.01f;
            if (keybState.IsKeyDown(Keys.Right))
                _carriages[CurrentPlayer].Angle += 0.01f;

            if (_carriages[CurrentPlayer].Angle > MathHelper.PiOver2)
                _carriages[CurrentPlayer].Angle = -MathHelper.PiOver2;
            if (_carriages[CurrentPlayer].Angle < -MathHelper.PiOver2)
                _carriages[CurrentPlayer].Angle = MathHelper.PiOver2;

            if (keybState.IsKeyDown(Keys.Down))
                _carriages[CurrentPlayer].Power -= 1;
            if (keybState.IsKeyDown(Keys.Up))
                _carriages[CurrentPlayer].Power += 1;
            if (keybState.IsKeyDown(Keys.PageDown))
                _carriages[CurrentPlayer].Power -= 20;
            if (keybState.IsKeyDown(Keys.PageUp))
                _carriages[CurrentPlayer].Power += 20;

            if (_carriages[CurrentPlayer].Power > 1000)
                _carriages[CurrentPlayer].Power = 1000;
            if (_carriages[CurrentPlayer].Power < 0)
                _carriages[CurrentPlayer].Power = 0;

            if (!keybState.IsKeyDown(Keys.Enter) && !keybState.IsKeyDown(Keys.Space)) return;
            Rocket.RocketFlying = true;
            SoundCenter.Launch.Play();

            _rocket.RocketPosition = _carriages[CurrentPlayer].Position;
            _rocket.RocketPosition.X += 20;
            _rocket.RocketPosition.Y -= 10;
            _rocket.RocketAngle = _carriages[CurrentPlayer].Angle;
            var up = new Vector2(0, -1);
            var rotMatrix = Matrix.CreateRotationZ(_rocket.RocketAngle);
            _rocket.RocketDirection = Vector2.Transform(up, rotMatrix);
            _rocket.RocketDirection *= _carriages[CurrentPlayer].Power / 50.0f;
        }

        private void NextPlayer()
        {
            NewRound = true;
            CurrentPlayer = CurrentPlayer + 1;
            CurrentPlayer = CurrentPlayer % NumberOfPlayers;
            while (!_carriages[CurrentPlayer].IsAlive)
                CurrentPlayer = ++CurrentPlayer % NumberOfPlayers;
            _rocket.Color = _carriages[CurrentPlayer].Color;
            //Debug.WriteLine("NextPlayer: " + CurrentPlayer);
            var count = 0;
            for (var index = 0; index < _carriages.Length; index++)
            {
                var carriage = _carriages[index];
                
                // set the victury text in case this player won the game
                if (!carriage.IsAlive) continue;
                count++;
                _endText = "The " + carriage.ColorName+ " Player number" + (index + 1) + " won the game";
            }
            if (count < 2)
            {
                _showEndScene();
            }
        }

        private void _showEndScene()
        {
            _endScene = true;
            _textSize = Font.MeasureString(_endText)/2;
        }

        public override void Initialize()
        {
            State = 0;
            _endScene = false;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Game1.GlobalTransformation);
            var screenRectangle = new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight);
            // Draw Scenery
            SpriteBatch.Draw(_textureCenter.BackgroundTexture, screenRectangle, Color.White);
            SpriteBatch.Draw(_textureCenter.ForegroundTexture, screenRectangle, Color.White);
            DrawText();

            if (_endScene)
            {
                SpriteBatch.DrawString(Font, _endText,
                    new Vector2(Game1.ScreenWidth/2 , Game1.ScreenHeight/2 - _textSize.Y),
                    _carriages[CurrentPlayer].Color, 0, _textSize, 1.5f, SpriteEffects.None, 0.5f);
            }
            SpriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawText()
        {
            var player = _carriages[CurrentPlayer];
            var currentAngle = (int)MathHelper.ToDegrees(player.Angle);
            SpriteBatch.DrawString(Font, "Cannon angle: " + currentAngle, new Vector2(20, 20), player.Color);
            SpriteBatch.DrawString(Font, "Cannon power: " + player.Power, new Vector2(20, 45), player.Color);
        }
    }
}
