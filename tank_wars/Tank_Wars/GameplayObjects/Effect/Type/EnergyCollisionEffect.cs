using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class EnergyCollisionEffect : Effect
	{
		#region Declarations
		Tank energyWielder;
		#endregion

		#region Constructors
		// Nếu ko truyền vào tham số millisecondsPerFrame
		// Sử dụng defaultMillisecondsPerFrame = 16 (mặc định)
		public EnergyCollisionEffect
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Tank energyWielder, float rotationAngle, 
			Point frameSize, Point sheetSize
			)
			: this
			(
			gameplayScreen, textureImage, energyWielder, rotationAngle, frameSize, sheetSize, 
			defaultMillisecondsPerFrame
			)
		{

		}

		// Ngược lại, sử dụng custom millisecondsPerFrame
		public EnergyCollisionEffect
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Tank energyWielder, float rotationAngle, 
			Point frameSize, Point sheetSize, int millisecondsPerFrame
			)
			: base
			(
			gameplayScreen, textureImage, energyWielder.CurrentWorldPosition, rotationAngle, frameSize, sheetSize, 
			millisecondsPerFrame
			)
		{
			this.isLooped = false;
			this.energyWielder = energyWielder;
		}
		#endregion

		#region Methods
		public override void Update(GameTime gameTime)
		{
			this.CurrentWorldPosition = energyWielder.CurrentWorldPosition;

			if (this.isFinishedAnimating)
			{
				GameplayScreen.EffectList.Remove(this);
				GameplayScreen.CurrentListItem--;
				return;
			}

			base.Update(gameTime);
		}
		#endregion
	}
}
