using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesignKoncept2
{
	class Menu
	{
		public List<Button> Buttons { get; private set; }

		public Menu(string[] buttonTexts, Vector2 startPos)
		{
			Buttons = new List<Button>();
			for(int i = 0; i < buttonTexts.Length; i++) Buttons.Add(new Button(startPos + new Vector2(0, i * Game1.font.MeasureString("0").Y), buttonTexts[i]));
		}

		public void Draw(SpriteBatch batch)
		{
			foreach (Button b in Buttons) b.Draw(batch);
		}
	}
}
