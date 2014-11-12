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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        MouseState ms, oms;

        Gem selectedGem1, selectedGem2;

        public static Point ScreenSize { get { return new Point(Board.BoardSizePx.X, Board.BoardSizePx.Y + 100); } } 

        public static Texture2D gemTextures;

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
            base.Initialize();
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Initialize();
            ms = Mouse.GetState();
            if (Board.CanMakeMove())
            {
                Board.Combo = 0;
                if (ms.LeftButton == ButtonState.Pressed && Board.PointIsOnBoard(ms.X, ms.Y))
                {
                    Gem clickedGem = Board.Gems[ms.X / Board.GemSize, ms.Y / Board.GemSize];
                    if (oms.LeftButton == ButtonState.Released) selectedGem1 = clickedGem;
                    else if (selectedGem1 != clickedGem && selectedGem2 == null)
                    {
                        selectedGem2 = clickedGem;
                        Color tmp = selectedGem2.Color;
                        selectedGem2.Color = selectedGem1.Color;
                        selectedGem1.Color = tmp;
                        /*Debug.WriteLine("handling shape " + selectedGem2.Shape);
                        Board.HandleChunk(selectedGem2);
                        Debug.WriteLine("handling shape " + selectedGem1.Shape);
                        Board.HandleChunk(selectedGem1);*/
                        selectedGem1.StartDestroy();
                        selectedGem2.StartDestroy();
                    }
                    //foreach (Gem g in Board.GetSimilarGemChunk(clickedGem)) g.color = Color.Black;
                }
                else
                {
                    selectedGem2 = null;
                    selectedGem1 = null;
                }
            }

            Board.Update();

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

            Board.Draw(spriteBatch);
            spriteBatch.DrawString(font, Board.Combo.ToString(), new Vector2(0, ScreenSize.Y - 90), Color.White);
            spriteBatch.DrawString(font, Board.DestroyedTiles.ToString(), new Vector2(ScreenSize.X - font.MeasureString(Board.DestroyedTiles.ToString()).X, ScreenSize.Y - 90), Color.White);
            spriteBatch.DrawString(font, Board.Score.ToString(), new Vector2(ScreenSize.X / 2 - font.MeasureString(Board.Score.ToString()).X / 2, ScreenSize.Y - 90), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
