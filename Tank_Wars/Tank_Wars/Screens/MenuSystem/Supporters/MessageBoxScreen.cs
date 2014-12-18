using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	class MessageBoxScreen : GameScreen
	{
		#region Declarations
		Texture2D gradientTexture;
		string message;
		#endregion

		#region Events
		public event EventHandler<EventArgs> Accepted;
		public event EventHandler<EventArgs> Cancelled;
		#endregion

		#region Constructor
		public MessageBoxScreen(string message)
			: this(message, true)
		{

		}

		public MessageBoxScreen(string message, bool includeUsageText)
		{
			const string usageText = "\nSpace, Enter = OK" +
									 "\nEsc = Cancel";

			if (includeUsageText)
				this.message = message + usageText;
			else
				this.message = message;

			IsPopup = true;

			TransitionOnTime = TimeSpan.FromSeconds(0.2);
			TransitionOffTime = TimeSpan.FromSeconds(0.2);
		}
		#endregion

		#region Methods
		public override void LoadContent()
		{
			ContentManager content = TankWars.Content;

			gradientTexture = content.Load<Texture2D>(@"Texture\Gradient");
		}

		public override void HandleInput(InputState input)
		{
			if (input.IsMenuSelect)
			{
				if (Accepted != null)
					Accepted(this, new EventArgs());

				ExitScreen();
			}
			else if (input.IsMenuCancel)
			{
				if (Cancelled != null)
					Cancelled(this, new EventArgs());

				ExitScreen();
			}
		}

		public override void Draw(GameTime gameTime)
		{
			SpriteBatch spriteBatch = TankWars.SpriteBatch;
			SpriteFont font = TankWars.Font;

			TankWars.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

			Viewport viewport = TankWars.GraphicsDevice.Viewport;
			Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
			Vector2 textSize = font.MeasureString(message);
			Vector2 textPosition = (viewportSize - textSize) / 2;

			const int hPad = 32;
			const int vPad = 16;

			Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
														  (int)textPosition.Y - vPad,
														  (int)textSize.X + hPad * 2,
														  (int)textSize.Y + vPad * 2);

			Color color = Color.White * TransitionAlpha;

			spriteBatch.Begin();

			spriteBatch.Draw(gradientTexture, backgroundRectangle, Color.Orange);

			spriteBatch.DrawString(font, message, textPosition, color);

			spriteBatch.End();
		}
		#endregion
	}
}