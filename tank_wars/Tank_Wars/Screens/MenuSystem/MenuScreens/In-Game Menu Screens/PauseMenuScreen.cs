using System;
using Microsoft.Xna.Framework;

namespace Tank_Wars
{
	class PauseMenuScreen : MenuScreen
	{
		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public PauseMenuScreen()
			: base("Paused")
		{
			MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
			MenuEntry restartMissionMenuEntry = new MenuEntry("Restart Mission");
			MenuEntry optionsMenuEntry = new MenuEntry("Options");
			MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");

			resumeGameMenuEntry.Selected += OnCancel;
			restartMissionMenuEntry.Selected += RestartMissionMenuEntrySelected;
			optionsMenuEntry.Selected += OptionsMenuEntrySelected;
			quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

			MenuEntries.Add(resumeGameMenuEntry);
			MenuEntries.Add(restartMissionMenuEntry);
			MenuEntries.Add(optionsMenuEntry);
			MenuEntries.Add(quitGameMenuEntry);
		}
		#endregion

		#region Events
		void RestartMissionMenuEntrySelected(object sender, EventArgs e)
		{
			GameplayScreen gameplayScreen =
				(GameplayScreen)TankWars.GetScreen(typeof(GameplayScreen));
			if (gameplayScreen != null)
			{
				gameplayScreen.Initialize();
				ExitScreen();
			}
		}

		void OptionsMenuEntrySelected(object sender, EventArgs e)
		{
			TankWars.AddScreen(new OptionsMenuScreen());
		}

		void QuitGameMenuEntrySelected(object sender, EventArgs e)
		{
			const string message = "Are you sure you want to quit current game?";

			MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

			confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

			TankWars.AddScreen(confirmQuitMessageBox);
		}

		void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
		{
			LoadingScreen.Load(TankWars, false, null, new BackgroundScreen(),
														   new MainMenuScreen());
		}
		#endregion
	}
}