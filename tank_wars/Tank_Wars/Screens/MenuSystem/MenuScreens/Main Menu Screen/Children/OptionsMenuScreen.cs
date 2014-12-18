using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Tank_Wars
{
	class OptionsMenuScreen : MenuScreen
	{
		#region Declarations
		MenuEntry fullScreen;
		MenuEntry sound;
		MenuEntry music;

		static bool isFullScreen = true;
		static bool isSoundOn = true;
		static bool isMusicOn = true;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public OptionsMenuScreen()
			: base("Options")
		{
			fullScreen = new MenuEntry(string.Empty);
			sound = new MenuEntry(string.Empty);
			music = new MenuEntry(string.Empty);
			SetMenuEntryText();

			MenuEntry back = new MenuEntry("Back");

			fullScreen.Selected += FullScreenMenuEntrySelected;
			sound.Selected += SoundMenuEntrySelected;
			music.Selected += MusicMenuEntrySelected;
			back.Selected += OnCancel;

			MenuEntries.Add(fullScreen);
			MenuEntries.Add(sound);
			MenuEntries.Add(music);
			MenuEntries.Add(back);
		}
		#endregion

		#region Method
		private void SetMenuEntryText()
		{
			fullScreen.Text = "Full Screen: " + (isFullScreen ? "On" : "Off");
			sound.Text = "Sound: " + (isSoundOn ? "On" : "Off");
			music.Text = "Music: " + (isMusicOn ? "On" : "Off");
		}
		#endregion

		#region Events
		void FullScreenMenuEntrySelected(object sender, EventArgs e)
		{
			isFullScreen = !isFullScreen;
			TankWars.Graphics.IsFullScreen = isFullScreen;
			if (isFullScreen)
			{
				TankWars.Graphics.PreferredBackBufferWidth
					= TankWars.Graphics.GraphicsDevice.DisplayMode.Width;
				TankWars.Graphics.PreferredBackBufferHeight
					= TankWars.Graphics.GraphicsDevice.DisplayMode.Height;
				TankWars.Graphics.ApplyChanges();
			}
			else
			{
				TankWars.Graphics.PreferredBackBufferWidth = 1024;
				TankWars.Graphics.PreferredBackBufferHeight = 768;
				TankWars.Graphics.ApplyChanges();
			}

			SetMenuEntryText();
		}

		void SoundMenuEntrySelected(object sender, EventArgs e)
		{
			isSoundOn = !isSoundOn;

			if (isSoundOn)
			{
				SoundEffect.MasterVolume = 1.0f;
			}
			else SoundEffect.MasterVolume = 0.0f;

			SetMenuEntryText();
		}

		void MusicMenuEntrySelected(object sender, EventArgs e)
		{
			isMusicOn = !isMusicOn;

			if (isMusicOn)
			{
				MediaPlayer.IsMuted = false;
			}
			else MediaPlayer.IsMuted = true;

			SetMenuEntryText();
		}
		#endregion
	}
}