﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DesignKoncept2
{
    static class Board
    {
        public static Gem[,] Gems { get; private set; }
        public static Point BoardSizeTiles { get { return new Point(6, 8); } }
        public static Point BoardSizePx { get { return new Point(BoardSizeTiles.X * GemSize, BoardSizeTiles.Y * GemSize); } }
        public static int GemSize { get { return 64; } }
        public static int Combo { get; set; }
        public static int DestroyedTiles { get; set; }
		public static int Score { get; set; }
        public static List<FloatingText> FloatingTexts { get; set; }
        public static bool LevelCompleted
        {
            get
            {
                return (DestroyedTiles > TileGoal);
            }
        }
        public static int Time { get; set; }
        public static int TileGoal { get; set; }
        public static int Level { get; set; }

        public static void Initialize()
        {
            Gems = new Gem[BoardSizeTiles.X, BoardSizeTiles.Y];
			FloatingTexts = new List<FloatingText>();
            Random r = new Random();
            for (int x = 0; x < BoardSizeTiles.X; x++)
            {
                for (int y = 0; y < BoardSizeTiles.Y; y++)
                {
                    Gems[x, y] = new Gem(new Vector2(x * GemSize, y * GemSize), r);
                }
            }
        }

        public static void InitializeLevel()
        {
            if(LevelCompleted) Level++;
            TileGoal = (int)(30 * Math.Pow(1.15f, Level));
            DestroyedTiles = 0;
        }

		public static void AddScore(int value)
		{
			Score += value; 
			FloatingTexts.Add(new FloatingText(20, value.ToString(), Color.White, new Vector2(Game1.ScreenSize.X / 2, Game1.ScreenSize.Y - 90))); 
		}

        public static List<Gem> AdjacentGems(Gem g)
        {
            List<Gem> toReturn = new List<Gem>();
            Point gemIndex = new Point((int)g.Position.X / GemSize, (int)g.Position.Y / GemSize);

            if (gemIndex.X < BoardSizeTiles.X - 1) toReturn.Add(Gems[gemIndex.X + 1, gemIndex.Y]);
            if (gemIndex.X > 0) toReturn.Add(Gems[gemIndex.X - 1, gemIndex.Y]);
            if (gemIndex.Y < BoardSizeTiles.Y - 1) toReturn.Add(Gems[gemIndex.X, gemIndex.Y + 1]);
            if (gemIndex.Y > 0) toReturn.Add(Gems[gemIndex.X, gemIndex.Y - 1]);

            for (int i = toReturn.Count - 1; i >= 0; i--)
            {
                if (toReturn[i].Color != g.Color) toReturn.RemoveAt(i);
            }

            return toReturn;
        }

        public static bool CanMakeMove()
        {
            foreach (Gem g in Gems) if (g.Destroy) return false;
            return true;
        }

        public static bool PointIsOnBoard(int x, int y)
        {
            return (x >= 0 && x < BoardSizePx.X && y >= 0 && y < BoardSizePx.Y);
        }

        public static void Update()
        {
            Random r = new Random();
            foreach (Gem g in Gems) g.Update(r);
			for (int i = FloatingTexts.Count - 1; i >= 0; i--)
			{
				if (FloatingTexts[i].Destroy) FloatingTexts.RemoveAt(i);
			}
        }

        public static void Draw(SpriteBatch batch)
        {
            foreach (Gem g in Gems) g.Draw(batch);
			foreach (FloatingText t in FloatingTexts) t.Draw(batch);
        }
    }
}