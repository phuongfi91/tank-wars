using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class PowerIcon : Sprite
	{
		#region Declarations
		// Index của icon này trong powerIconList. (hay còn gọi là số thứ tự
		// bắt đầu từ 0)
		int powerIconListIndex;

		// Loại Power mà Icon này đại diện.
		public readonly Power Power;

		// Tên gọi đầy đủ của Power dưới dạng String.
		String powerName;

		// Trọng tâm của String.
		Vector2 powerNameOrigin;

		// Màu hiện tại của Icon.
		Color currentColor;

		// Bởi vì Texture truyền vào là một Sheet, ta cần biết được phải cắt
		// mảnh nào trong đó để vẽ, sourceRectangle phục vụ mục đích này.
		Rectangle sourceRectangle;
		#endregion

		#region Constructors
		public PowerIcon
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Rectangle sourceRectangle,
			Power power, String powerName, Point frameSize, Point sheetSize
			)
			: base
			(
			gameplayScreen, textureImage, Vector2.Zero, 0, frameSize, sheetSize
			)
		{
			this.drawLayer = 0.9f;
			this.powerIconListIndex = GameplayScreen.PowerIconList.Count;
			this.Power = power;
			this.powerName = powerName;
			this.origin = new Vector2(frameSize.X / 2, 0);
			this.powerNameOrigin = new Vector2
				(gameplayScreen.Content.Font.HUDFont.MeasureString(powerName).X / 2, 0);
			this.sourceRectangle = sourceRectangle;
			this.currentColor = Color.White;
		}
		#endregion

		#region Methods
		public override void Update(GameTime gameTime)
		{
			position = new Vector2
				(Camera.ViewportWidth / 2 - 
				(GameplayScreen.PowerIconList.Count - 1) * 32 + 
				powerIconListIndex * 64, 8);

			// Nếu Power của người chơi không trùng với Power mà Icon này 
			// đại diện thì vẽ nó một cách bình thường.
			if (GameplayScreen.Player.PowerState != Power)
			{
				scale = MathHelper.Clamp(scale -= 0.01f, 1.0f, 1.1f);
				currentColor = Color.White;
			}
			// Ngược lại phóng to nó và đổi màu.
			else
			{
				scale = MathHelper.Clamp(scale += 0.01f, 1.0f, 1.1f);
				currentColor = Color.DeepSkyBlue;
			}
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Nếu Power của người chơi trùng với Power mà Icon này đại diện
			// thì vẽ thêm 1 String ghi tên của Power bên dưới Icon này.
			if (GameplayScreen.Player.PowerState == Power)
			{
				spriteBatch.DrawString(GameplayScreen.Content.Font.HUDFont, powerName,
					new Vector2(CurrentWorldPosition.X, CurrentWorldPosition.Y + 62),
					currentColor * 0.6f, 0, powerNameOrigin, 1.0f, 
					SpriteEffects.None, drawLayer);
				spriteBatch.DrawString(GameplayScreen.Content.Font.HUDFont, powerName,
					new Vector2(CurrentWorldPosition.X + 2, CurrentWorldPosition.Y + 64),
					Color.Black * 0.6f, 0, powerNameOrigin, 1.0f, 
					SpriteEffects.None, drawLayer - 0.05f);
			}

			// Vẽ Power Icon.
			spriteBatch.Draw
				(
				textureImage, CurrentWorldPosition, sourceRectangle, currentColor * 0.8f,
				RotationAngle, origin, scale, SpriteEffects.None, drawLayer
				);
		}
		#endregion
	}
}