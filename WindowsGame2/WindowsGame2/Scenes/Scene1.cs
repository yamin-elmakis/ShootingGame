using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame2;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        protected Particle Particle;
        private Carriage[] players;
        protected SoundCenter soundCenter;
        protected SpriteFont font;
        protected int screenWidth, screenHeight;
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
        GraphicsDevice device;
        Texture2D groundTexture;
        
        Texture2D backgroundTexture;
        Texture2D foregroundTexture;
        Color[,] foregroundColorArray;
        int numberOfPlayers = 4;
        int currentPlayer = 0;
        int[] terrainContour;
        Random randomizer = new Random();
        
        Vector2 baseScreenSize = new Vector2(800, 600);

        public Scene1(Microsoft.Xna.Framework.Game game, int screenWidth, int screenHeight, GraphicsDevice device): base(game)
        {
            this.game = game;
            this.device = device;
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;
            backgroundTexture = game.Content.Load<Texture2D>("background");
            groundTexture = game.Content.Load<Texture2D>("ground");
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            soundCenter = (SoundCenter)game.Services.GetService(typeof(SoundCenter));
            font = (SpriteFont)game.Services.GetService(typeof(SpriteFont));
            Particle = new Particle(game);
            SceneComponents.Add(Particle);
            //SceneComponents.Add(_players);
            GenerateTerrainContour();
            SetUpPlayers();
            FlattenTerrainBelowPlayers();
            CreateForeground();
        }

        private void GenerateTerrainContour()
        {
            terrainContour = new int[screenWidth];

            double rand1 = randomizer.NextDouble() + 1;
            double rand2 = randomizer.NextDouble() + 2;
            double rand3 = randomizer.NextDouble() + 3;

            float offset = screenHeight / 2;
            float peakheight = 100;
            float flatness = 70;

            for (int x = 0; x < screenWidth; x++)
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
                players[i].Position.X = screenWidth / (numberOfPlayers + 1) * (i + 1);
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
            Color[,] groundColors = TextureTo2DArray(groundTexture);
            Color[] foregroundColors = new Color[screenWidth * screenHeight];

            for (int x = 0; x < screenWidth; x++)
            {
                for (int y = 0; y < screenHeight; y++)
                {
                    if (y > terrainContour[x])
                        foregroundColors[x + y * screenWidth] = groundColors[x % groundTexture.Width, y % groundTexture.Height];
                    else
                        foregroundColors[x + y * screenWidth] = Color.Transparent;
                }
            }

            foregroundTexture = new Texture2D(device, screenWidth, screenHeight, false, SurfaceFormat.Color);
            foregroundTexture.SetData(foregroundColors);

            foregroundColorArray = TextureTo2DArray(foregroundTexture);
        }

        public override void Update(GameTime gameTime)
        {
            if (endScene)
            {
                timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timeElapsed > timeToUpdate)
                    State = 1;
                return;
            }
           

            base.Update(gameTime);
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
            goal2Rect = new Rectangle(125, screenHeight - Consts.GoalYline - 5, 200, 1);
            State = 0;
            P1Score = 0;
            P2Score = 0;
            endScene = false;
        }

        public override void Draw(GameTime gameTime)
        {
            var screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            spriteBatch.DrawString(font, "P1 Score:" + P1Score + "   P2 Score:" + P2Score, new Vector2(5, 5), Color.Red);   
            base.Draw(gameTime);
            if (endScene)
            {
                spriteBatch.DrawString(font, endText, new Vector2(screenWidth / 2, screenHeight / 2), Color.Blue, 0, textSize, 2.0f, SpriteEffects.None, 0.5f);
            }
        }

        public static Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
    }
}
