using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class Cursor : Sprite
	{
		#region Declarations
		// Màu của con trỏ.
		Color color;
		#endregion

		#region Constructors
		public Cursor
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Color color, Point frameSize, Point sheetSize
			)
			: base
			(
			gameplayScreen, textureImage, Vector2.Zero, 0, frameSize, sheetSize
			)
		{
			this.drawLayer = 1.0f;
			this.color = color;

			// Khi va chạm với Enemy thì màu con trỏ sẽ đổi màu.
			this.isCollidable = true;

			this.textureImageData = Texture.GetTextureData
				(textureImage, new Rectangle(0, 0, frameSize.X, frameSize.Y));
		}
		#endregion

		#region Methods
		public override void Update(GameTime gameTime)
		{
			color = Color.LightSkyBlue;

			RotationAngle += 0.01f;

			UpdateMapPosition();

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Lấy vị trí của bóng
			Vector2 shadowPosition = new Vector2
				(CurrentScreenPosition.X + 3.0f, CurrentScreenPosition.Y + 3.0f);

			// Vẽ con trỏ.
			spriteBatch.Draw
				(
				GameplayScreen.Content.Texture.Cursor, CurrentScreenPosition,
				new Rectangle
					(
					currentFrame.X * frameSize.X,
					currentFrame.Y * frameSize.Y,
					frameSize.X, frameSize.Y
					),
				color, RotationAngle, origin, scale, 
				SpriteEffects.None, drawLayer
				);

			// Vẽ bóng của con trỏ.
			spriteBatch.Draw
				(
				GameplayScreen.Content.Texture.Cursor, shadowPosition,
				new Rectangle
					(
					currentFrame.X * frameSize.X,
					currentFrame.Y * frameSize.Y,
					frameSize.X, frameSize.Y
					),
				new Color(0, 0, 0, 100), RotationAngle, origin, scale, 
				SpriteEffects.None, drawLayer - 0.05f
				);
		}

		/// <summary>
		/// Vị trí của con trỏ sẽ luôn được lấy từ input của chuột.
		/// </summary>
		public void HandleInput(InputState input)
		{
			position = new Vector2
				(input.CurrentMouseState.X + Camera.Viewport.X,
				input.CurrentMouseState.Y + Camera.Viewport.Y);
		}

		/// <summary>
		/// Nếu va chạm với Enemy thì đổi màu.
		/// </summary>
		protected override void HandleCollision(Sprite possibleCollisionTarget)
		{
			Type targetType = possibleCollisionTarget.GetType();

			if (targetType == typeof(Tank_Wars.Enemy))
			{
				color = Color.Red;
			}
		}
		#endregion
	}
}