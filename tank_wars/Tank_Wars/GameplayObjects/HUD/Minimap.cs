using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class Minimap : Sprite
	{
		#region Constructors
		public Minimap
			(
			GameplayScreen gameplayScreen, Texture2D textureImage
			)
			: base
			(
			gameplayScreen, textureImage, Vector2.Zero, 0, new Point(1, 1), new Point(1, 1)
			)
		{
			this.drawLayer = 0.8f;
			this.origin = Vector2.Zero;
		}
		#endregion

		#region Methods
		public override void Update(GameTime gameTime)
		{
			position =
				new Vector2(Camera.ViewportWidth - Map.MapWidth * 4 - 3, 0);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Vẽ Minimap.
			spriteBatch.Draw
				(
					textureImage, 
					new Rectangle
						((int)CurrentWorldPosition.X, (int)CurrentWorldPosition.Y, 
						Map.MapWidth * 4, Map.MapHeight * 4),
					new Rectangle(0, 0, textureImage.Width, textureImage.Height),
					new Color(255, 255, 255, 100), RotationAngle, origin, 
					SpriteEffects.None, drawLayer
				);

			// Vẽ bóng của Minimap.
			spriteBatch.Draw
				(
					textureImage, 
					new Rectangle
						((int)CurrentWorldPosition.X + 3, (int)CurrentWorldPosition.Y + 3, 
						Map.MapWidth * 4, Map.MapHeight * 4),
					new Rectangle(0, 0, textureImage.Width, textureImage.Height),
					new Color(0, 0, 0, 100), RotationAngle, origin, 
					SpriteEffects.None, drawLayer - 0.05f
				);

			// Vẽ hình chữ nhật đại diện cho Viewport trên Minimap.
			spriteBatch.Draw
				(
					GameplayScreen.Content.Texture.MiniMapRectangle, 
					new Rectangle
						((int)CurrentWorldPosition.X + Camera.Viewport.X / 16,
						(int)CurrentWorldPosition.Y + Camera.Viewport.Y / 16,
						Camera.ViewportWidth / 16, Camera.ViewportHeight / 16),
					new Rectangle
						(0, 0, GameplayScreen.Content.Texture.MiniMapRectangle.Width,
						GameplayScreen.Content.Texture.MiniMapRectangle.Height),
					Color.Black, RotationAngle, origin, 
					SpriteEffects.None, drawLayer + 0.1f
				);
		}

		/// <summary>
		/// Thể hiện đối tượng Tank lên Minimap.
		/// </summary>
		public void DrawDot
			(Tank tank, Color dotColor, SpriteBatch spriteBatch)
		{
			spriteBatch.Draw
				(
				GameplayScreen.Content.Texture.Blank, 
				new Rectangle
					(
					(int)(CurrentWorldPosition.X + tank.CurrentMapPosition.X * 4),
					(int)(CurrentWorldPosition.Y + tank.CurrentMapPosition.Y * 4), 
					4, 4
					),
				new Rectangle(0, 0, 4, 4), dotColor, RotationAngle, origin,
				SpriteEffects.None, drawLayer + 0.05f
				);
		}
		#endregion
	}
}