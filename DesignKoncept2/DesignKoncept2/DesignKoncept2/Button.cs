using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DesignKoncept2
{
	class Button
	{
        public string Text { get; set; }
		Vector2 position;
		Vector2 centeredPos { get { return position - Game1.font.MeasureString(Text) / 2; } }
		Rectangle rectangle { get { 
			Vector2 centering = Game1.font.MeasureString(Text) / 2;
			Vector2 absPos = position - centering;
			centering *= 2;
			return new Rectangle((int)absPos.X, (int)absPos.Y, (int)centering.X, (int)centering.Y);
		} }

		bool mouseIsOver
		{
			get
			{
				if (new Rectangle(Game1.ms.X, Game1.ms.Y, 1, 1).Intersects(rectangle)) return true;
				else return false;
			}
		}

		public bool IsClicked
		{
			get
			{
				if (mouseIsOver && Game1.ms.LeftButton == ButtonState.Pressed && Game1.oms.LeftButton != Game1.ms.LeftButton) return true;
				else return false;
			}
		}

		public Button(Vector2 position, string text)
		{
			this.position = position;
			this.Text = text;
		}

		public void Draw(SpriteBatch batch)
		{
			Color c = (mouseIsOver) ? Color.White : Color.LightGray; 
			batch.DrawString(Game1.font, Text, centeredPos, c);
		}
	}
}
