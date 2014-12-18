using System;
using Microsoft.Xna.Framework;

namespace Tank_Wars
{
	class MainMenuScreen : MenuScreen
	{
		#region Constructor
		/// <summary>
		/// Constructor fills in the menu contents.
		/// </summary>
		public MainMenuScreen()
			: base("Main Menu")
		{
			MenuEntry singlePlayerMenuEntry = new MenuEntry("Single Player");
			MenuEntry optionsMenuEntry = new MenuEntry("Options");
			MenuEntry aboutMenuEntry = new MenuEntry("About");
			MenuEntry exitMenuEntry = new MenuEntry("Exit");

			singlePlayerMenuEntry.Selected += SinglePlayerMenuEntrySelected;
			optionsMenuEntry.Selected += OptionsMenuEntrySelected;
			aboutMenuEntry.Selected += AboutMenuEntrySelected;
			exitMenuEntry.Selected += OnCancel;

			MenuEntries.Add(singlePlayerMenuEntry);
			MenuEntries.Add(optionsMenuEntry);
			MenuEntries.Add(aboutMenuEntry);
			MenuEntries.Add(exitMenuEntry);
		}
		#endregion

		#region Events
		void SinglePlayerMenuEntrySelected(object sender, EventArgs e)
		{
			TankWars.AddScreen(new SinglePlayerMenuScreen());
		}

		void OptionsMenuEntrySelected(object sender, EventArgs e)
		{
			TankWars.AddScreen(new OptionsMenuScreen());
		}

		void AboutMenuEntrySelected(object sender, EventArgs e)
		{
			TankWars.AddScreen(new AboutMenuScreen());
		}

		protected override void OnCancel()
		{
			const string message = "Are you sure you want to exit Tank Wars?";

			MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

			confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

			TankWars.AddScreen(confirmExitMessageBox);
		}

		void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
		{
			TankWars.Exit();
		}
		#endregion
	}
}