using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DesignKoncept2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum GameState { Menu, About, PreGame, Game, Over } ;

        public static SpriteFont font;
        public static MouseState ms, oms;

        Menu gameOverMenuSucces, gameOverMenuFail, startMenu, aboutMenu;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameState gameState;

        Gem selectedGem1, selectedGem2;

        public static Point ScreenSize { get { return new Point(Board.BoardSizePx.X, Board.BoardSizePx.Y + 100); } } 

        public static Texture2D gemTextures;

		int time;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = ScreenSize.X;
            graphics.PreferredBackBufferHeight = ScreenSize.Y;
            IsMouseVisible = true;        
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Board.Initialize();
            oms = Mouse.GetState();
            gameState = GameState.Menu;
            base.Initialize();
        }

		void StartGame()
		{
			gameState = GameState.PreGame;
			time = 10 * 60; // n * 60 where n is seconds, ie time is in frames
            Board.InitializeLevel();
		}

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gemTextures = Content.Load<Texture2D>("gems");
            font = Content.Load<SpriteFont>("font");

			gameOverMenuSucces = new Menu(new string[] { "", "play next level", "view highscores", "quit game" }, new Vector2(ScreenSize.X / 2, ScreenSize.Y / 2 - 100));
			gameOverMenuFail = new Menu(new string[] { "use time given to you", "try again", "view highscores", "quit game" }, new Vector2(ScreenSize.X / 2, ScreenSize.Y / 2 - 100));
            startMenu = new Menu(new string[] { "play game", "about the game", "quit" }, new Vector2(ScreenSize.X, ScreenSize.Y) / 2);
            aboutMenu = new Menu(new string[] { "back" }, new Vector2(ScreenSize.X, ScreenSize.Y * 1.6f) / 2);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ms = Mouse.GetState();

            switch (gameState)
            {
                case GameState.Game:
					time -= 1;
					if (time < 0) time = 0;
                    if (Board.CanMakeMove())
                    {
						if (time > 0 && !Board.LevelCompleted)
						{
							Board.Combo = 0;
							if (ms.LeftButton == ButtonState.Pressed && Board.PointIsOnBoard(ms.X, ms.Y))
							{
								Gem clickedGem = Board.Gems[ms.X / Board.GemSize, ms.Y / Board.GemSize];
								if (selectedGem1 == null) selectedGem1 = clickedGem;
								else if (selectedGem1 != clickedGem && selectedGem2 == null)
								{
									selectedGem2 = clickedGem;
									Color tmp = selectedGem2.Color;
									selectedGem2.Color = selectedGem1.Color;
									selectedGem1.Color = tmp;
									selectedGem1.StartDestroy();
									selectedGem2.StartDestroy();
								}
							}
							else
							{
								selectedGem2 = null;
								selectedGem1 = null;
							}
						}
						else gameState = GameState.Over;
                    }

                    Board.Update();
                    break;

                case GameState.PreGame:
                    if (ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Released) gameState = GameState.Game;
                    break;

                case GameState.About:
                    if (aboutMenu.Buttons[0].IsClicked) gameState = GameState.Menu;
                    break;

                case GameState.Menu:
                    if (startMenu.Buttons[0].IsClicked) StartGame();
                    if (startMenu.Buttons[1].IsClicked) gameState = GameState.About;
                    if (startMenu.Buttons[2].IsClicked) Exit();
                    break;

                case GameState.Over:
                    if (Board.LevelCompleted)
                    {
                        gameOverMenuSucces.Buttons[0].Text = "give " + time / 60 + "s to a friend";
                        if (gameOverMenuSucces.Buttons[1].IsClicked) StartGame();
                        else if (gameOverMenuSucces.Buttons[3].IsClicked) Exit();
                    }
                    else 
                    {
                        if (gameOverMenuFail.Buttons[1].IsClicked) StartGame();
                        else if (gameOverMenuFail.Buttons[3].IsClicked) Exit();
                    }
                    break;
            }

            oms = ms;

            base.Update(gameTime);
        }

        bool PointIsOnScreen(Point position) //should check if on board instead, also be moved to board class
        {
            return position.X > 0 && position.X < ScreenSize.X && position.Y > 0 && position.Y < Game1.ScreenSize.Y;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Color backgroundColor = Color.DarkGray;
            if (gameState == GameState.Game && !Board.CanMakeMove()) backgroundColor = Color.OrangeRed;
            GraphicsDevice.Clear(backgroundColor);
            spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.Game:
                    Board.Draw(spriteBatch);
                    spriteBatch.DrawString(font, "Gem goal: " + Board.TileGoal.ToString(), new Vector2(0, ScreenSize.Y - 40), Color.White);
                    string dt = "Destroyed Gems: " + Board.DestroyedTiles.ToString();
                    spriteBatch.DrawString(font, dt, new Vector2(ScreenSize.X - font.MeasureString(dt).X, ScreenSize.Y - 40), Color.White);

                    string score = "score: " + Board.Score.ToString();
                    spriteBatch.DrawString(font, score, new Vector2(ScreenSize.X - font.MeasureString(score).X, ScreenSize.Y - 90), Color.White);

					string timeString  = (time/ 60f).ToString("0.0");
					Color c = (time < 3 * 60 && time % 60 > 30) ? Color.Red : Color.White;
                    spriteBatch.DrawString(font, timeString, new Vector2(ScreenSize.X / 2 - font.MeasureString(timeString).X / 2, ScreenSize.Y - 90), c);

                    spriteBatch.DrawString(font, "Level: " + Board.Level, new Vector2(0, ScreenSize.Y - 90), Color.White);
                    break;

                case GameState.PreGame:
                    DrawCenteredString("destroy " + Board.TileGoal + " tiles", new Vector2(0, -.5f));
                    DrawCenteredString("click anywhere to start", new Vector2(0, .5f));
                    break;
                    
                case GameState.Menu:
                    startMenu.Draw(spriteBatch);
                    break;

                case GameState.About:
                    aboutMenu.Draw(spriteBatch);
                    DrawCenteredStrings(new string[] {"drag two tiles to", "make them switch place", "chunks of the same color", "will be destroyed", "destroy a set amount", "to complete the level", "send left over time to friends", "or use time sent to you", "to complete the level if you fail", "(some features don't work", "since this is a prototype)" } );
                    break;
                    
                case GameState.Over:
                    if (Board.LevelCompleted)
                    {
                        DrawCenteredString("you scored " + Board.Score + " on level " + (Board.Level + 1), new Vector2(0, -5));
                        gameOverMenuSucces.Draw(spriteBatch);
                    }
                    else
                    {
                        DrawCenteredString("you need " + (Board.TileGoal - Board.DestroyedTiles).ToString() + " more tiles", new Vector2(0, -5f));
                        gameOverMenuFail.Draw(spriteBatch);
                    }
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

		void DrawCenteredString(string s)
		{
            DrawCenteredString(s, Vector2.Zero);
		}

		void DrawCenteredString(string s, Vector2 offset)
		{
			spriteBatch.DrawString(font, s, (new Vector2(ScreenSize.X, ScreenSize.Y) - font.MeasureString(s)) / 2 + offset * font.MeasureString(s), Color.LightGray);
		}

        void DrawCenteredStrings(string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                DrawCenteredString(strings[i], new Vector2(0, i - (strings.Length - 1)/ 2));
            }
        }
    }
}