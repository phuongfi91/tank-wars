using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	abstract class MenuScreen : GameScreen
	{
		#region Declarations
		ContentManager content;
		SoundEffect menuEntryChangedSound;
		SoundEffect menuEntrySelectedSound;
		List<MenuEntry> menuEntries = new List<MenuEntry>();
		int selectedEntry = 0;
		string menuTitle;
		#endregion

		#region Properties
		protected IList<MenuEntry> MenuEntries
		{
			get { return menuEntries; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Constructor.
		/// </summary>
		public MenuScreen(string menuTitle)
		{
			this.menuTitle = menuTitle;

			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}

		public override void LoadContent()
		{
			if (content == null)
				content = new ContentManager(TankWars.Services, "Content");

			menuEntryChangedSound = content.Load<SoundEffect>(@"Audio\MenuEntryChanged");
			menuEntrySelectedSound = content.Load<SoundEffect>(@"Audio\MenuEntrySelected");
		}

		protected virtual void UpdateMenuEntryLocations()
		{
			float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

			Vector2 position = new Vector2(0f, 375f);

			for (int i = 0; i < menuEntries.Count; i++)
			{
				MenuEntry menuEntry = menuEntries[i];

				position.X = TankWars.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

				if (ScreenState == ScreenState.TransitionOn)
					position.X -= transitionOffset * 256;
				else
					position.X += transitionOffset * 512;

				menuEntry.Position = position;

				position.Y += menuEntry.GetHeight(this);
			}
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus,
													   bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

			for (int i = 0; i < menuEntries.Count; i++)
			{
				bool isSelected = IsActive && (i == selectedEntry);

				menuEntries[i].Update(this, isSelected, gameTime);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			UpdateMenuEntryLocations();

			GraphicsDevice graphics = TankWars.GraphicsDevice;
			SpriteBatch spriteBatch = TankWars.SpriteBatch;
			SpriteFont font = TankWars.Font;

			spriteBatch.Begin();

			for (int i = 0; i < menuEntries.Count; i++)
			{
				MenuEntry menuEntry = menuEntries[i];

				bool isSelected = IsActive && (i == selectedEntry);

				menuEntry.Draw(this, isSelected, gameTime);
			}

			float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

			Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 300);
			Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
			Color titleColor = Color.OrangeRed * TransitionAlpha;

			float titleScale = 1.25f;

			titlePosition.Y -= transitionOffset * 100;

			spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
								   titleOrigin, titleScale, SpriteEffects.None, 0);

			spriteBatch.End();
		}

		public override void HandleInput(InputState input)
		{
			if (input.IsMenuUp)
			{
				selectedEntry--;
				menuEntryChangedSound.Play();
				if (selectedEntry < 0)
					selectedEntry = menuEntries.Count - 1;
			}

			if (input.IsMenuDown)
			{
				selectedEntry++;
				menuEntryChangedSound.Play();
				if (selectedEntry >= menuEntries.Count)
					selectedEntry = 0;
			}

			if (input.IsMenuSelect)
			{
				menuEntrySelectedSound.Play();
				OnSelectEntry(selectedEntry);
			}
			else if (input.IsMenuCancel)
			{
				OnCancel();
			}
		}
		#endregion
		
		#region Events
		protected virtual void OnSelectEntry(int entryIndex)
		{
			menuEntries[entryIndex].OnSelectEntry();
		}

		protected virtual void OnCancel()
		{
			ExitScreen();
		}

		protected void OnCancel(object sender, EventArgs e)
		{
			OnCancel();
		}
		#endregion
	}
}