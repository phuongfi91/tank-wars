using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class EnvironmentFlame : Effect
	{
		#region Constructors
		// Nếu ko truyền vào tham số millisecondsPerFrame
		// Sử dụng defaultMillisecondsPerFrame = 16 (mặc định)
		public EnvironmentFlame
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, 
			Point frameSize, Point sheetSize
			)
			: this
			(
			gameplayScreen, textureImage, position, frameSize, sheetSize, 
			defaultMillisecondsPerFrame
			)
		{

		}

		// Ngược lại, sử dụng custom millisecondsPerFrame
		public EnvironmentFlame
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, 
			Point frameSize, Point sheetSize, int millisecondsPerFrame
			)
			: base
			(
			gameplayScreen, textureImage, position, 0, frameSize, sheetSize, 
			millisecondsPerFrame
			)
		{
			Random rand = new Random
				((int)(position.X * position.Y - position.X + position.Y));
			currentFrame.X = rand.Next(1, sheetSize.X);
			currentFrame.Y = rand.Next(1, sheetSize.Y);
		}
		#endregion

		#region Methods
		// Override để nhuộm màu cam cho hiệu ứng lửa.
		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (Camera.ObjectIsVisible(BoundingWorldRectangle))
			{
				spriteBatch.Draw
					(
					textureImage, CurrentScreenPosition, new Rectangle
						(
						currentFrame.X * frameSize.X,
						currentFrame.Y * frameSize.Y,
						frameSize.X, frameSize.Y
						),
					Color.Orange, RotationAngle, origin, scale,
					SpriteEffects.None, drawLayer
					);
			}
		}
		#endregion
	}
}
