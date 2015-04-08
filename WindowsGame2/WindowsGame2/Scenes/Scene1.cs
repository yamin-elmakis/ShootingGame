using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame2;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using YaminGame.Sprites;
using YaminGame.Utilities;
using Game.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes
{
    public class Scene1 : Scene
    {
        protected Microsoft.Xna.Framework.Game game;
        protected SpriteBatch spriteBatch;
        protected SoundCenter soundCenter;
        protected SpriteFont font;
        public Rectangle goal1Rect, goal2Rect;
        public int P1Score = 0, P2Score = 0;
        private bool endScene;
        private string endText = "";
        private float timeElapsed, timeToUpdate = 1f;
        private Vector2 textSize;

        public override int State { get; set; }

        /// <summary>
        /// new
        /// </summary>
        private Rocket _rocket;
        private Carriage[] players;
        private Explosion _explosion;
        GraphicsDevice device;
        Texture2D groundTexture;
        
        private float playerExplosionSize = 80.0f, playerExplosionMaxAge = 2000.0f, terrainExplosionSize = 30.0f, terrainExplosionMaxAge = 1000.0f;
        private int playerExplosionParticles = 10, terrainExplosionParticles = 4;
        Texture2D backgroundTexture;
        Texture2D foregroundTexture;
        Color[,] foregroundColorArray;
        int numberOfPlayers = 4;
        int currentPlayer = 0;
        int[] terrainContour;
        Random randomizer = new Random();
        
        Vector2 baseScreenSize = new Vector2(800, 600);

        public Scene1(Microsoft.Xna.Framework.Game game, GraphicsDevice device): base(game)
        {
            this.game = game;
            this.device = device;
            backgroundTexture = game.Content.Load<Texture2D>("background");
            groundTexture = game.Content.Load<Texture2D>("ground");
            
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            soundCenter = (SoundCenter)game.Services.GetService(typeof(SoundCenter));
            font = (SpriteFont)game.Services.GetService(typeof(SpriteFont));
            _rocket = new Rocket(game);
            _explosion = new Explosion(game);
            SceneComponents.Add(_explosion);
            SceneComponents.Add(_rocket);
            foreach (var carriage in players)
            {
                SceneComponents.Add(carriage);   
            }
            GenerateTerrainContour();
            SetUpPlayers();
            FlattenTerrainBelowPlayers();
            CreateForeground();
        }

        private void GenerateTerrainContour()
        {
            terrainContour = new int[Game1.screenWidth];

            double rand1 = randomizer.NextDouble() + 1;
            double rand2 = randomizer.NextDouble() + 2;
            double rand3 = randomizer.NextDouble() + 3;

            float offset = Game1.screenHeight / 2;
            float peakheight = 100;
            float flatness = 70;

            for (int x = 0; x < Game1.screenWidth; x++)
            {
                double height = peakheight / rand1 * Math.Sin((float)x / flatness * rand1 + rand1);
                height += peakheight / rand2 * Math.Sin((float)x / flatness * rand2 + rand2);
                height += peakheight / rand3 * Math.Sin((float)x / flatness * rand3 + rand3);
                height += offset;
                terrainContour[x] = (int)height;
            }
        }

        private void SetUpPlayers()
        {
            players = new Carriage[numberOfPlayers];
            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i] = new Carriage(game, i);
                players[i].Position = new Vector2();
                players[i].Position.X = Game1.screenWidth / (numberOfPlayers + 1) * (i + 1);
                players[i].Position.Y = terrainContour[(int)players[i].Position.X];
            }
        }

        private void FlattenTerrainBelowPlayers()
        {
            foreach (Carriage player in players)
                if (player.IsAlive)
                    for (int x = 0; x < 40; x++)
                        terrainContour[(int)player.Position.X + x] = terrainContour[(int)player.Position.X];
        }

        private void CreateForeground()
        {
            Color[,] groundColors = Utils.TextureTo2DArray(groundTexture);
            Color[] foregroundColors = new Color[Game1.screenWidth * Game1.screenHeight];

            for (int x = 0; x < Game1.screenWidth; x++)
            {
                for (int y = 0; y < Game1.screenHeight; y++)
                {
                    if (y > terrainContour[x])
                        foregroundColors[x + y * Game1.screenWidth] = groundColors[x % groundTexture.Width, y % groundTexture.Height];
                    else
                        foregroundColors[x + y * Game1.screenWidth] = Color.Transparent;
                }
            }

            foregroundTexture = new Texture2D(device, Game1.screenWidth, Game1.screenHeight, false, SurfaceFormat.Color);
            foregroundTexture.SetData(foregroundColors);

            foregroundColorArray = Utils.TextureTo2DArray(foregroundTexture);
        }

        public override void Update(GameTime gameTime)
        {
            // for managing the scenes
            if (endScene)
            {
                timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timeElapsed > timeToUpdate)
                    State = 1;
                return;
            }

            // check if a player can aim
            if ((!Rocket.rocketFlying) && (_explosion.particleList.Count == 0))
                ProcessKeyboard();

            if (Rocket.rocketFlying)
            {
                Vector2 terrainCollisionPoint = _rocket.CheckTerrainCollision(gameTime, foregroundColorArray);
                if (terrainCollisionPoint.X > -1)
                {
                    for (int i = 0; i < players.Length; i++)
                        players[i].Position.Y = terrainContour[(int)players[i].Position.X];

                    float rotation = (float)randomizer.Next(10);
                    Matrix mat = Matrix.CreateTranslation(-explosionTexture.Width / 2, -explosionTexture.Height / 2, 0) * Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(terrainExplosionSize / (float)explosionTexture.Width * 2.0f) * Matrix.CreateTranslation(terrainCollisionPoint.X, terrainCollisionPoint.Y, 0);
                    AddCrater(explosionColorArray, mat);
                    FlattenTerrainBelowPlayers();
                    CreateForeground();
                    soundCenter.HitTerrain.Play();
                    NextPlayer();
                }

                if (_rocket.CheckOutOfScreen())
                {
                    NextPlayer();
                }
            }
            
            base.Update(gameTime);
        }

        private void AddCrater(Color[,] tex, Matrix mat)
        {
            int width = tex.GetLength(0);
            int height = tex.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tex[x, y].R > 10)
                    {
                        Vector2 imagePos = new Vector2(x, y);
                        Vector2 screenPos = Vector2.Transform(imagePos, mat);

                        int screenX = (int)screenPos.X;
                        int screenY = (int)screenPos.Y;

                        if ((screenX) > 0 && (screenX < Game1.screenWidth))
                            if (terrainContour[screenX] < screenY)
                                terrainContour[screenX] = screenY;
                    }
                }
            }
        }

        private void ProcessKeyboard()
        {
            KeyboardState keybState = Keyboard.GetState();
            if (keybState.IsKeyDown(Keys.Left))
                players[currentPlayer].Angle -= 0.01f;
            if (keybState.IsKeyDown(Keys.Right))
                players[currentPlayer].Angle += 0.01f;

            if (players[currentPlayer].Angle > MathHelper.PiOver2)
                players[currentPlayer].Angle = -MathHelper.PiOver2;
            if (players[currentPlayer].Angle < -MathHelper.PiOver2)
                players[currentPlayer].Angle = MathHelper.PiOver2;

            if (keybState.IsKeyDown(Keys.Down))
                players[currentPlayer].Power -= 1;
            if (keybState.IsKeyDown(Keys.Up))
                players[currentPlayer].Power += 1;
            if (keybState.IsKeyDown(Keys.PageDown))
                players[currentPlayer].Power -= 20;
            if (keybState.IsKeyDown(Keys.PageUp))
                players[currentPlayer].Power += 20;

            if (players[currentPlayer].Power > 1000)
                players[currentPlayer].Power = 1000;
            if (players[currentPlayer].Power < 0)
                players[currentPlayer].Power = 0;

            if (keybState.IsKeyDown(Keys.Enter) || keybState.IsKeyDown(Keys.Space))
            {
                Rocket.rocketFlying = true;
                soundCenter.Launch.Play();

                _rocket.rocketPosition = players[currentPlayer].Position;
                _rocket.rocketPosition.X += 20;
                _rocket.rocketPosition.Y -= 10;
                _rocket.rocketAngle = players[currentPlayer].Angle;
                Vector2 up = new Vector2(0, -1);
                Matrix rotMatrix = Matrix.CreateRotationZ(_rocket.rocketAngle);
                _rocket.rocketDirection = Vector2.Transform(up, rotMatrix);
                _rocket.rocketDirection *= players[currentPlayer].Power / 50.0f;
            }
        }

        private void NextPlayer()
        {
            // + set the color of the rocket
            currentPlayer = currentPlayer + 1;
            currentPlayer = currentPlayer % numberOfPlayers;
            while (!players[currentPlayer].IsAlive)
                currentPlayer = ++currentPlayer % numberOfPlayers;
        }

        private void _showEndScene()
        {
            endScene = true;
            if (P1Score == P2Score)
                endText = "Tie";
            else endText = (P1Score > P2Score) ? "Carriage 1 Wins" : "Carriage 2 Wins";
            textSize = font.MeasureString(endText)/2;
        }

        public override void Initialize()
        {
            goal1Rect = new Rectangle(125, Consts.GoalYline, 200, 1);
            goal2Rect = new Rectangle(125, Game1.screenHeight - Consts.GoalYline - 5, 200, 1);
            State = 0;
            P1Score = 0;
            P2Score = 0;
            endScene = false;
        }

        public override void Draw(GameTime gameTime)
        {
            var screenRectangle = new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            spriteBatch.DrawString(font, "P1 Score:" + P1Score + "   P2 Score:" + P2Score, new Vector2(5, 5), Color.Red);   
            base.Draw(gameTime);
            if (endScene)
            {
                spriteBatch.DrawString(font, endText, new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2), Color.Blue, 0, textSize, 2.0f, SpriteEffects.None, 0.5f);
            }
        }

    }
}
