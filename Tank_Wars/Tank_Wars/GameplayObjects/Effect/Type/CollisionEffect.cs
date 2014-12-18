using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class CollisionEffect : Effect
	{
		#region Constructors
		// Nếu ko truyền vào tham số millisecondsPerFrame
		// Sử dụng defaultMillisecondsPerFrame = 16 (mặc định)
		public CollisionEffect
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, float rotationAngle, 
			Point frameSize, Point sheetSize
			)
			: this
			(
			gameplayScreen, textureImage, position, rotationAngle, frameSize, sheetSize, 
			defaultMillisecondsPerFrame
			)
		{

		}

		// Ngược lại, sử dụng custom millisecondsPerFrame
		public CollisionEffect
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, float rotationAngle, 
			Point frameSize, Point sheetSize, int millisecondsPerFrame
			)
			: base
			(
			gameplayScreen, textureImage, position, rotationAngle, frameSize, sheetSize, 
			millisecondsPerFrame
			)
		{
			this.isLooped = false;
		}
		#endregion

		#region Methods
		public override void Update(GameTime gameTime)
		{
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
