using System;
using Microsoft.Xna.Framework;

namespace Tank_Wars
{
	class SinglePlayerMenuScreen : MenuScreen
	{
		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public SinglePlayerMenuScreen()
			: base("Single Player")
		{
			MenuEntry startCampaign = new MenuEntry("Start Campaign");
			//MenuEntry faceOff = new MenuEntry("Face Off");
			//MenuEntry tankRacing = new MenuEntry("Tank Racing");
			MenuEntry laserTorture = new MenuEntry("Laser Torture");
			//MenuEntry loadGame = new MenuEntry("Load Game");

			MenuEntry back = new MenuEntry("Back");

			startCampaign.Selected += StartCampaignMenuEntrySelected;
			//faceOff.Selected += FaceOffMenuEntrySelected;
			//tankRacing.Selected += TankRacingMenuEntrySelected;
			laserTorture.Selected += LaserTortureMenuEntrySelected;
			//loadGame.Selected += LoadGameMenuEntrySelected;
			back.Selected += OnCancel;

			MenuEntries.Add(startCampaign);
			//MenuEntries.Add(faceOff);
			//MenuEntries.Add(tankRacing);
			MenuEntries.Add(laserTorture);
			//MenuEntries.Add(loadGame);
			MenuEntries.Add(back);
		}
		#endregion

		#region Events
		void StartCampaignMenuEntrySelected(object sender, EventArgs e)
		{
			LoadingScreen.Load
				(TankWars, true, new GameplayScreen(GameMode.Campaign));
		}

		void FaceOffMenuEntrySelected(object sender, EventArgs e)
		{
			LoadingScreen.Load
				(TankWars, true, new GameplayScreen(GameMode.FaceOff));
		}

		void TankRacingMenuEntrySelected(object sender, EventArgs e)
		{
			LoadingScreen.Load
				(TankWars, true, new GameplayScreen(GameMode.TankRacing));
		}

		void LaserTortureMenuEntrySelected(object sender, EventArgs e)
		{
			LoadingScreen.Load
				(TankWars, true, new GameplayScreen(GameMode.LaserTorture));
		}

		void LoadGameMenuEntrySelected(object sender, EventArgs e)
		{

		}
		#endregion
	}
}