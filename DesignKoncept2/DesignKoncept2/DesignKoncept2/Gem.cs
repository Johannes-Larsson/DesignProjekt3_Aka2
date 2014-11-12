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
        const int maxDestroy = 20;

        public enum GemShape { Blue = 0, Pink = 1, Green = 2, Brown = 3, White = 4}

        public Vector2 Position { get; set; }
        Vector2 velocity;
        public Color Color { get; set; }

        public float alpha; //debug purposes

        int blinkTime;

        public Gem(Vector2 position, Random r)
        {
            velocity = Vector2.Zero;
			Position = position;
            AssignNewShape(r, false);
			alpha = 1;
            destroyCounter = 0;
            destroy = false;
        }

		public void AssignNewShape(Random r, bool allowSameShape)
		{
			Color oldShape = Color;
			do
			{
                Color = colors[r.Next(colors.Length)];
				if (allowSameShape || oldShape != Color) break;
			} while (true);

			alpha = .5f;
			blinkTime = 1;
		}

        public void StartDestroy()
        {
            if (Board.AdjacentGems(this).Count > 0) Destroy = true;
        }

        public void Update(Random r)
        {
            if (destroyCounter > 0) destroyCounter++;
            if (destroyCounter == maxDestroy) //on destroy
            {
                Destroy = false;
                foreach (Gem g in Board.AdjacentGems(this)) if(g.Color == Color) g.Destroy = true;
                AssignNewShape(r, false);
                Board.Combo++;
                Board.DestroyedTiles++;
                Board.Score += Board.Combo;
            }

            if (alpha < 1) blinkTime++;
            if (blinkTime == 30)
            {
                blinkTime = 0;
                alpha = 1;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Game1.gemTextures, Position + new Vector2(Board.GemSize / 2), null, Color * alpha, 0, new Vector2(Board.GemSize / 2), ((maxDestroy - destroyCounter) / (float) maxDestroy), SpriteEffects.None, 1);
        }
    }
}
