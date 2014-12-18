using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	class MissionStartScreen : GameScreen
	{
		#region Declarations
		float pauseAlpha;
		ContentManager content;
		TimeSpan lifeSpan;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public MissionStartScreen(GameMode gameMode)
		{
			lifeSpan = TimeSpan.FromSeconds(5);
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}
		#endregion

		#region Methods
		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		public override void LoadContent()
		{
			if (content == null)
			{
				content = new ContentManager(TankWars.Services, "Content");
			}

			// once the load has finished, we use ResetElapsedTime to tell the game's
			// timing mechanism that we have just finished a very long frame, and that
			// it should not try to catch up.
			TankWars.ResetElapsedTime();
		}
		
		/// <summary>
		/// Unload graphics content used by the game.
		/// </summary>
		public override void UnloadContent()
		{
			content.Unload();
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update
			(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);

			// Gradually fade in or out depending on whether we are covered by the pause screen.
			if (coveredByOtherScreen)
				pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
			else
				pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

			if (IsActive)
			{

			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Draw(GameTime gameTime)
		{
			// Màu nền
			TankWars.GraphicsDevice.Clear(Color.Black);

			// Dùng để vẽ các đối tượng
			SpriteBatch spriteBatch = TankWars.SpriteBatch;

			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend
				/*, null, null, null, null, Camera.get_transformation(Game.GraphicsDevice)*/);



			spriteBatch.End();

			// If the game is transitioning on or off, fade it out to black.
			if (TransitionPosition > 0 || pauseAlpha > 0)
			{
				float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

				TankWars.FadeBackBufferToBlack(alpha);
			}
		}

		// Xử lý trường hợp người chơi nhấn Esc
		public override void HandleInput(InputState input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (input.IsPauseGame)
			{
				
			}
		}
		#endregion
	}
}