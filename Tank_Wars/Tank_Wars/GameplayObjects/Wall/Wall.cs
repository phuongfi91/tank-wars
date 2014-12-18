using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	/// <summary>
	/// Class Wall. Chủ yếu dùng để xử lý va chạm với các đối tượng khác
	/// trong game như đạn, tank,...
	/// </summary>
	public class Wall : Sprite
	{
		#region Constructor
		public Wall
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Rectangle sourceRectangle, Vector2 position,
			float rotationAngle, Point frameSize, Point sheetSize
			)
			: base
			(
			gameplayScreen, textureImage, position, rotationAngle, frameSize, sheetSize
			)
		{
			// Do Texture truyền vào là một Sheet, ta cần xác định xem cần phải
			// cắt mảnh nào để lấy dữ liệu màu, đây là lý do cần phải truyền vào
			// sourceRectangle.
			this.textureImageData = Texture.GetTextureData
				(textureImage, sourceRectangle);
		}
		#endregion
	}
}
