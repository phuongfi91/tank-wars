using System;
using Microsoft.Xna.Framework;

namespace Tank_Wars
{
	class MissionFailedMenuScreen : MenuScreen
	{
		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public MissionFailedMenuScreen()
			: base("Mission Failed")
		{
			MenuEntry restartMissionMenuEntry = new MenuEntry("Restart Mission");
			MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");

			restartMissionMenuEntry.Selected += RestartMissionMenuEntrySelected;
			quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

			MenuEntries.Add(restartMissionMenuEntry);
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