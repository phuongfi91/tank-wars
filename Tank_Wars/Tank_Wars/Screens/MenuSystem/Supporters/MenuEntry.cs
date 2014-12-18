using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	class MenuEntry
	{
		#region Declarations
		string text;

		float selectionFade;

		Vector2 position;
		#endregion

		#region Properties
		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}
		#endregion

		#region Events
		public event EventHandler<EventArgs> Selected;

		protected internal virtual void OnSelectEntry()
		{
			if (Selected != null)
				Selected(this, new EventArgs());
		}
		#endregion

		#region Constructor
		public MenuEntry(string text)
		{
			this.text = text;
		}
		#endregion

		#region Methods
		public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
		{
			float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

			if (isSelected)
				selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
			else
				selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
		}

		public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
		{
			double time = gameTime.TotalGameTime.TotalSeconds;

			float pulsate = ((float)Math.Sin(time * 6)) * 0.25f + 0.75f;

			float scale = 1;

			Color color = isSelected ? Color.Yellow * pulsate : Color.Red;

			color *= screen.TransitionAlpha;

			TankWarsGame tankWars = screen.TankWars;
			SpriteBatch spriteBatch = tankWars.SpriteBatch;
			SpriteFont font = tankWars.Font;

			Vector2 origin = new Vector2(0, font.LineSpacing / 2);

			spriteBatch.DrawString(font, text, position, color, 0,
								   origin, scale, SpriteEffects.None, 0);
		}

		public virtual int GetHeight(MenuScreen screen)
		{
			return screen.TankWars.Font.LineSpacing;
		}

		public virtual int GetWidth(MenuScreen screen)
		{
			return (int)screen.TankWars.Font.MeasureString(Text).X;
		}
		#endregion
	}
}