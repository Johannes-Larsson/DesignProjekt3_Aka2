using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesignKoncept2
{
	class FloatingText
	{
		int lifeTime;
		int life;
		string text;
		Color color;
		Vector2 position;

		public bool Destroy { get { if(life >= lifeTime) return true; else return false; } }

		public FloatingText(int lifeTime, string text, Color color, Vector2 position)
		{
			this.lifeTime = lifeTime;
			this.text = text;
			this.color = color;
			this.position = position;
		}

		public void Draw(SpriteBatch batch)
		{
			life++;
			position.Y += 1;
			batch.DrawString(Game1.font, text, position, color); 
		}
	}
}
