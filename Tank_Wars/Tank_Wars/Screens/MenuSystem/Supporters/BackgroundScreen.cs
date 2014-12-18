using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	class BackgroundScreen : GameScreen
	{
		#region Declarations
		ContentManager content;
		Texture2D menuBackgroundTexture;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public BackgroundScreen()
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}
		#endregion

		#region Methods
		public override void LoadContent()
		{
			if (content == null)
				content = new ContentManager(TankWars.Services, "Content");

			menuBackgroundTexture = content.Load<Texture2D>(@"Texture\MenuBackground");
		}

		public override void UnloadContent()
		{
			content.Unload();
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus,
													   bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);
		}

		public override void Draw(GameTime gameTime)
		{
			SpriteBatch spriteBatch = TankWars.SpriteBatch;
			Viewport viewport = TankWars.GraphicsDevice.Viewport;
			Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

			spriteBatch.Begin();

			spriteBatch.Draw(menuBackgroundTexture, fullscreen,
							 new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

			spriteBatch.End();
		}
		#endregion
	}
}