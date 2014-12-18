using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class Direction : Effect
	{
		#region Declarations
		Tank director;
		#endregion

		#region Constructors
		// Nếu ko truyền vào tham số millisecondsPerFrame
		// Sử dụng defaultMillisecondsPerFrame = 16 (mặc định)
		public Direction
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Tank director, float rotationAngle,
			Point frameSize, Point sheetSize
			)
			: this
			(
			gameplayScreen, textureImage, director, rotationAngle, frameSize, sheetSize,
			defaultMillisecondsPerFrame
			)
		{

		}

		// Ngược lại, sử dụng custom millisecondsPerFrame
		public Direction
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Tank director, float rotationAngle,
			Point frameSize, Point sheetSize, int millisecondsPerFrame
			)
			: base
			(
			gameplayScreen, textureImage, director.CurrentWorldPosition, rotationAngle, frameSize, sheetSize,
			millisecondsPerFrame
			)
		{
			this.drawLayer = 0.1f;
			this.isLooped = true;
			this.director = director;
			this.scale = 1.5f;
			this.origin = new Vector2(- frameSize.X / 16, frameSize.Y / 2);
		}
		#endregion

		#region Methods
		public override void Update(GameTime gameTime)
		{
			this.CurrentWorldPosition = director.CurrentWorldPosition;
			this.RotationAngle = director.RotationAngle;

			base.Update(gameTime);
		}
		#endregion
	}
}
