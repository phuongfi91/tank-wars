using System;

namespace Tank_Wars
{
#if WINDOWS
	static class Program
	{
		/// <summary>
		/// Cổng vào của Game.
		/// </summary>
		static void Main(string[] args)
		{
			using (TankWarsGame tankWarsGame = new TankWarsGame())
			{
				tankWarsGame.Run();
			}
		}
	}
#endif
}