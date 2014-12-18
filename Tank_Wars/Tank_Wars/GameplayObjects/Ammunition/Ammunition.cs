using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class Ammunition : Sprite
	{
		#region Declarations
		// Sát thương.
		int damage;

		// Tốc độ di chuyển.
		float speed;

		// Offset di chuyển.
		Vector2 movingOffset;
		#endregion

		#region Properties
		/// <summary>
		/// Truy xuất và thay đổi CurrentWorldPosition: Tọa độ hiện tại 
		/// trong toàn màn chơi.
		/// </summary>
		public override Vector2 CurrentWorldPosition
		{
			get { return position; }
			set { position = value; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Nếu ko truyền vào tham số millisecondsPerFrame thì sử dụng 
		/// defaultMillisecondsPerFrame = 16 (mặc định)
		/// </summary>
		public Ammunition
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, float rotationAngle, 
			float speed, int damage, Point frameSize, Point sheetSize
			)
			: this
			(
			gameplayScreen, textureImage, position, rotationAngle, speed, damage, 
			frameSize, sheetSize, defaultMillisecondsPerFrame
			)
		{

		}

		/// <summary>
		/// Ngược lại, sử dụng millisecondsPerFrame từ Constructor.
		/// </summary>
		public Ammunition
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, float rotationAngle,
			float speed, int damage, Point frameSize, Point sheetSize, 
			int millisecondsPerFrame
			)
			: base
			(
			gameplayScreen, textureImage, position, rotationAngle, frameSize, sheetSize, 
			millisecondsPerFrame
			)
		{
			this.drawLayer = 0.3f;
			this.speed = speed;
			this.damage = damage;
			this.isCollidable = true;

			this.textureImageData = Texture.GetTextureData
				(textureImage, new Rectangle(0, 0, frameSize.X, frameSize.Y));

			// Tính toán hướng bay của đạn với tốc độ bay cho trước và hướng
			// quay hiện tại của nòng súng
			this.movingOffset.X = (float)(speed * Math.Cos(rotationAngle));
			this.movingOffset.Y = (float)(speed * Math.Sin(rotationAngle));
		}
		#endregion

		#region Methods
		public override void Update(GameTime gameTime)
		{
			// Update vị trí dựa theo direction
			CurrentWorldPosition += movingOffset;

			// Loại bỏ các Ammunition đã nằm quá xa tầm nhìn của người chơi
			// ra khỏi List nhằm giảm tải xử lý.
			if  (
				CurrentWorldPosition.X < Camera.Viewport.Left - 1000
				||
				CurrentWorldPosition.X > Camera.Viewport.Right + 1000
				||
				CurrentWorldPosition.Y < Camera.Viewport.Top - 1000
				||
				CurrentWorldPosition.Y > Camera.Viewport.Bottom + 1000
				||
				CurrentWorldPosition.X < 0
				||
				CurrentWorldPosition.X >= Camera.WholeWorldRectangle.Width
				||
				CurrentWorldPosition.Y < 0
				||
				CurrentWorldPosition.Y >= Camera.WholeWorldRectangle.Height
				)
			{
				GameplayScreen.AmmunitionList.Remove(this);
				GameplayScreen.CurrentListItem--;
				return;
			}

			UpdateMapPosition();

			base.Update(gameTime);
		}

		/// <summary>
		/// Xử lý va chạm.
		/// </summary>
		protected override void HandleCollision(Sprite possibleCollisionTarget)
		{
			// Tạo Impact do va chạm giữa đạn với đối tượng gây ra.
			CreateImpact();

			Type targetType = possibleCollisionTarget.GetType();

			if (targetType.IsSubclassOf(typeof(Tank_Wars.Tank)))
			{
				Tank tank = (Tank)possibleCollisionTarget;

				// Nếu Tank đang bật chế độ Power Armor và còn Energy thì
				// trừ damage vào EnergyPoints của Tank thay cho HitPoints.
				if (tank.PowerState == Power.Armor && tank.EnergyPoints > 0)
				{
					CreateEnergyImpact();
					tank.EnergyPoints -= damage;
				}
				else tank.HitPoints -= damage;
			}
		}

		// Tạo hiệu ứng va chạm với đối tượng tùy theo loại vũ khí.
		protected virtual void CreateImpact()
		{
			GameplayScreen.AmmunitionList.Remove(this);
			GameplayScreen.CurrentListItem--;
		}

		/// <summary>
		/// Tạo hiệu ứng va chạm với giáp năng lượng của đối tượng.
		/// </summary>
		protected void CreateEnergyImpact()
		{
			GameplayScreen.EffectList.Add
				(
				new EnergyCollisionEffect
					(
					this.GameplayScreen, GameplayScreen.Content.Texture.EnergyImpactSheet, GameplayScreen.Player, 
					RotationAngle + MathHelper.Pi, new Point(96, 96), new Point(7, 5), 1
					)
				);
			Audio.Play
				(GameplayScreen.Content.Audio.SmallLaserRayImpact, this.CurrentWorldPosition,
				GameplayScreen.Player.CurrentWorldPosition, 50, 0.0f, 0.3f);
		}
		#endregion
	}
}
