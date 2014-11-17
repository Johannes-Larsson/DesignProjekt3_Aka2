using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignKoncept2
{
    class Gem
    {
        Color[] colors = new Color[] { new Color(0x4E, 0x10, 0xAE), new Color(0xAA, 0x00, 0xA2), new Color(0xE4, 0x00, 0x45), new Color(0xFF, 0x6A, 0x00), new Color(0x00, 0xA0, 0x8A), new Color(0x67, 0xE3, 0x00) };

        bool destroy;
        public bool Destroy { get { return destroy; } set { destroy = value; destroyCounter = (value) ? 1 : 0; } }
        int destroyCounter;
        int maxDestroy;

        public enum GemShape { Blue = 0, Pink = 1, Green = 2, Brown = 3, White = 4}

        public Vector2 Position { get; set; }
        Vector2 velocity;
        public Color Color { get; set; }

        int blinkTime;

        public Gem(Vector2 position, Random r)
        {
            velocity = Vector2.Zero;
			Position = position;
            AssignNewShape(r, false);
            destroyCounter = 0;
            destroy = false;
            maxDestroy = 30;
        }

		public void AssignNewShape(Random r, bool allowSameShape)
		{
			Color oldShape = Color;
			do
			{
                Color = colors[r.Next(colors.Length)];
				if (allowSameShape || oldShape != Color) break;
			} while (true);
			blinkTime = 1;
		}

        public void StartDestroy()
        {
            if (Board.AdjacentGems(this).Count > 0)
            {
                Destroy = true;
                maxDestroy = 20 - (Board.Combo * 3);
                Board.FloatingTexts.Add(new FloatingText(30, maxDestroy.ToString(), Color.Black, Position));
                if (maxDestroy < 2) maxDestroy = 3;
            }
        }

        public void Update(Random r)
        {
            if (destroyCounter > 0) destroyCounter++;
            if (destroyCounter >= maxDestroy) //on destroy
            {
                Destroy = false;
				Board.FloatingTexts.Add(new FloatingText(30, (Board.Combo + 1).ToString() + "x", Color, Position));
                foreach (Gem g in Board.AdjacentGems(this)) if (g.Color == Color) g.StartDestroy();
                AssignNewShape(r, false);
                Board.Combo++;
                Board.DestroyedTiles++;
				Board.AddScore(Board.Combo);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Game1.gemTextures, Position + new Vector2(Board.GemSize / 2), null, Color, 0, new Vector2(Board.GemSize / 2), ((maxDestroy - destroyCounter) / (float) maxDestroy), SpriteEffects.None, 1);
        }
    }
}
