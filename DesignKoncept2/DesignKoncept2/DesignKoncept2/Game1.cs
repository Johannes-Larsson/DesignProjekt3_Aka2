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
        enum GameState { Menu, Game, Over } ;

        public static SpriteFont font;
        public static MouseState ms, oms;

		Menu gameOverMenuSucces, gameOverMenuFail;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameState gameState;
		bool completedLevel;

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
			gameState = GameState.Game;
			time = 8* 60; // n * 60 where n is seconds, ie time is in frames
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

			gameOverMenuSucces = new Menu(new string[] { "give your left over time to a friend", "play next level", "view highscores", "quit game" }, new Vector2(ScreenSize.X / 2, ScreenSize.Y / 2 - 100));
			gameOverMenuFail = new Menu(new string[] { "try again", "view highscores", "quit game" }, new Vector2(ScreenSize.X / 2, ScreenSize.Y / 2 - 100)); 
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
						if (time > 0)
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

                case GameState.Menu:
					if (ms.LeftButton == ButtonState.Pressed) StartGame();
                    break;

                case GameState.Over:
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
            GraphicsDevice.Clear(Color.DarkGray);
            spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.Game:
                    Board.Draw(spriteBatch);
                    spriteBatch.DrawString(font, Board.Combo.ToString(), new Vector2(0, ScreenSize.Y - 90), Color.White);
                    spriteBatch.DrawString(font, Board.DestroyedTiles.ToString(), new Vector2(ScreenSize.X - font.MeasureString(Board.DestroyedTiles.ToString()).X, ScreenSize.Y - 90), Color.White);
                    spriteBatch.DrawString(font, Board.Score.ToString(), new Vector2(ScreenSize.X / 2 - font.MeasureString(Board.Score.ToString()).X / 2, ScreenSize.Y - 90), Color.White);
					string timeString  = (time/ 60f).ToString("0.0");
					Color c = (time < 3 * 60 && time % 60 > 30) ? Color.Red : Color.White;
					spriteBatch.DrawString(font, timeString, new Vector2(ScreenSize.X / 2, ScreenSize.Y - 40) - font.MeasureString(timeString) / 2, c);
                    break;
                    
                case GameState.Menu:
					DrawCenteredString("click to start");
                    break;
                    
                case GameState.Over:
					if (completedLevel) gameOverMenuSucces.Draw(spriteBatch);
					else gameOverMenuFail.Draw(spriteBatch);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

		void DrawCenteredString(string s)
		{
			spriteBatch.DrawString(font, s, (new Vector2(ScreenSize.X, ScreenSize.Y) - font.MeasureString(s)) / 2, Color.White); 
		}
		void DrawCenteredString(string s, Vector2 offset)
		{
			spriteBatch.DrawString(font, s, (new Vector2(ScreenSize.X, ScreenSize.Y) - font.MeasureString(s)) / 2 + offset * font.MeasureString(s), Color.White);
		}
    }
}
