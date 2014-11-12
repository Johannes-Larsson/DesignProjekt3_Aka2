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

        public static void Initialize()
        {
            Gems = new Gem[BoardSizeTiles.X, BoardSizeTiles.Y];
            Random r = new Random();
            for (int x = 0; x < BoardSizeTiles.X; x++)
            {
                for (int y = 0; y < BoardSizeTiles.Y; y++)
                {
                    Gems[x, y] = new Gem(new Vector2(x * GemSize, y * GemSize), r);
                }
            }
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
            if (x >= 0 && x < BoardSizePx.X && y >= 0 && y < BoardSizePx.Y) return true;
            else return false;
        }

        public static void Update()
        {
            Random r = new Random();
            foreach (Gem g in Gems) g.Update(r);
        }

        public static void Draw(SpriteBatch batch)
        {
            foreach (Gem g in Gems)
            {
                g.Draw(batch);
            }
        }
    }
}