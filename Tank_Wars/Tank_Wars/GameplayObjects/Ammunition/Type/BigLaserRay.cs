using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class BigLaserRay : Ammunition
	{
		#region Constructors
		/// <summary>
		/// Nếu ko truyền vào tham số millisecondsPerFrame thì sử dụng 
		/// defaultMillisecondsPerFrame = 16 (mặc định)
		/// </summary>
		public BigLaserRay
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, float rotationAngle,
			float speed, int damage, Point frameSize, Point sheetSize
			)
			: this
			(
			gameplayScreen, textureImage, position, rotationAngle, speed, damage, frameSize, 
			sheetSize, defaultMillisecondsPerFrame
			)
		{

		}

		/// <summary>
		/// Ngược lại, sử dụng millisecondsPerFrame từ Constructor.
		/// </summary>
		public BigLaserRay
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, float rotationAngle,
			float speed, int damage, Point frameSize, Point sheetSize, 
			int millisecondsPerFrame
			)
			: base
			(
			gameplayScreen, textureImage, position, rotationAngle, speed, damage, frameSize,
			sheetSize, millisecondsPerFrame
			)
		{

		}
		#endregion

		#region Methods
		protected override void CreateImpact()
		{
			base.CreateImpact();
			GameplayScreen.EffectList.Add
				(
				new CollisionEffect
					(
					this.GameplayScreen, GameplayScreen.Content.Texture.LaserImpactSheet, CurrentWorldPosition, 0,
					new Point(16, 16), new Point(11, 10), 1
					)
				);
			Audio.Play
				(GameplayScreen.Content.Audio.BigLaserRayImpact, this.CurrentWorldPosition,
				GameplayScreen.Player.CurrentWorldPosition, 50, 0.0f, 0.5f);
		}
		#endregion
	}
}
