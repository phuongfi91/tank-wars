using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class EnvironmentDirt : Effect
	{
		#region Constructors
		// Nếu ko truyền vào tham số millisecondsPerFrame
		// Sử dụng defaultMillisecondsPerFrame = 16 (mặc định)
		public EnvironmentDirt
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, float drawLayer, 
			Point frameSize, Point sheetSize
			)
			: this
			(
			gameplayScreen, textureImage, position, drawLayer, frameSize, sheetSize,
			defaultMillisecondsPerFrame
			)
		{

		}

		// Ngược lại, sử dụng custom millisecondsPerFrame
		public EnvironmentDirt
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, float drawLayer, 
			Point frameSize, Point sheetSize, int millisecondsPerFrame
			)
			: base
			(
			gameplayScreen, textureImage, position, 0, frameSize, sheetSize, 
			millisecondsPerFrame
			)
		{
			Random rand = new Random((int)(position.X * position.Y - position.X + position.Y));
			this.scale = MathHelper.Clamp((float)rand.NextDouble(), 0.4f, 1.0f);
			this.rotationAngle = (float)(rand.NextDouble() * MathHelper.TwoPi);
			this.drawLayer = drawLayer;
		}
		#endregion
	}
}
