using Microsoft.Xna.Framework;

namespace Tank_Wars
{
	class AboutMenuScreen : MenuScreen
	{
		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public AboutMenuScreen() : base("About")
		{
			MenuEntry line1 = new MenuEntry("Nguyen Dang Chau");
			MenuEntry line2 = new MenuEntry("Nguyen Dung Phuong");
			MenuEntry line3 = new MenuEntry("Bui Tan Phat");
			MenuEntry back = new MenuEntry("Back");

			back.Selected += OnCancel;

			MenuEntries.Add(line1);
			MenuEntries.Add(line2);
			MenuEntries.Add(line3);
			MenuEntries.Add(back);
		}
		#endregion
	}
}