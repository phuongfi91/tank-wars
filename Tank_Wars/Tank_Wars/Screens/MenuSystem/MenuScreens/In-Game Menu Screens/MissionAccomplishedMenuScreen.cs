using System;
using Microsoft.Xna.Framework;

namespace Tank_Wars
{
	class MissionAccomplishedMenuScreen : MenuScreen
	{
		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public MissionAccomplishedMenuScreen()
			: base("Mission Accomplished")
		{
			
			MenuEntry continueMenuEntry = new MenuEntry("Continue");
			continueMenuEntry.Selected += ContinueMenuEntrySelected;
			MenuEntries.Add(continueMenuEntry);

			MenuEntry restartMissionMenuEntry = new MenuEntry("Restart Mission");
			MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");

			restartMissionMenuEntry.Selected += RestartMissionMenuEntrySelected;
			quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

			MenuEntries.Add(restartMissionMenuEntry);
			MenuEntries.Add(quitGameMenuEntry);
		}
		#endregion

		#region Events
		void ContinueMenuEntrySelected(object sender, EventArgs e)
		{
			GameplayScreen gameplayScreen =
				(GameplayScreen)TankWars.GetScreen(typeof(GameplayScreen));
			if (gameplayScreen != null)
			{
				gameplayScreen.CurrentMission++;
				gameplayScreen.Initialize();
				ExitScreen();
			}
		}

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