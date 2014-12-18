using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class Bar : Sprite
	{
		#region Declarations
		// Màu của thanh. (Ví dụ: Thanh máu màu đỏ, thanh năng lượng màu xanh)
		Color color;

		/* Thanh gồm có 2 phần: */

		// Phần khung bao ở rìa ngoài thanh.
		Rectangle edgeSourceRectangle;

		// Phần màu trắng bên trong có thể được nhuộm bằng color.
		Rectangle fillerSourceRectangle;
		#endregion

		#region Constructors
		public Bar
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, 
			Rectangle edgeSourceRectangle, Rectangle fillerSourceRectangle,
			Vector2 position, Color color
			)
			: base
			(
			gameplayScreen, textureImage, position, 0, new Point(1, 1), new Point(1, 1)
			)
		{
			this.drawLayer = 0.9f;
			this.color = color;
			this.edgeSourceRectangle = edgeSourceRectangle;
			this.fillerSourceRectangle = fillerSourceRectangle;
			this.origin = Vector2.Zero;
		}
		#endregion

		#region Methods
		public void Draw(int CurrentValue, int MaxValue, SpriteBatch spriteBatch)
		{
			// Vẽ phần bên trong thanh.
			spriteBatch.Draw
				(
				textureImage,
				new Rectangle
					((int)CurrentWorldPosition.X + 36, (int)CurrentWorldPosition.Y, 
					(int)(textureImage.Width * ((double)CurrentValue / MaxValue)), 42),
				fillerSourceRectangle, color, RotationAngle, 
				origin, SpriteEffects.None, drawLayer - 0.05f
				);

			// Vẽ khung ngoài rìa bao quanh.
			spriteBatch.Draw
				(
				textureImage,
				new Rectangle
					((int)CurrentWorldPosition.X, (int)CurrentWorldPosition.Y, 
					textureImage.Width, 42),
				edgeSourceRectangle, Color.White * 0.8f, RotationAngle, 
				origin, SpriteEffects.None, drawLayer
				);
		}
		#endregion
	}
}