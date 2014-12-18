using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class Effect : Sprite
	{
		/// <summary>
		/// Nếu ko truyền vào tham số millisecondsPerFrame thì sử dụng 
		/// defaultMillisecondsPerFrame = 16 (mặc định)
		/// </summary>
		public Effect
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

		/// <summary>
		/// Ngược lại, sử dụng millisecondsPerFrame từ Constructor.
		/// </summary>
		public Effect
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
			this.drawLayer = 0.5f;
		}

		// Override để tránh vẽ bóng cho hiệu ứng
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
					Color.White, RotationAngle, origin, scale,
					SpriteEffects.None, drawLayer
					);
			}
		}
	}
}
