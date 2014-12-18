using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tank_Wars
{
	public class Player : Tank
	{
		#region Properties
		#region Public
		/// <summary>
		/// Override IsAlive của base vì IsAlive của base sẽ gây ra một lỗi
		/// nhỏ trong cơ chế update cho Player.
		/// </summary>
		public bool IsDead { get; set; }

		/// <summary>
		/// Override của base vì khi Player dùng thành công một số Power nhất
		/// định trong khi di chuyển thì Player phải bị trừ EnergyPoints.
		/// </summary>
		public override Vector2 CurrentWorldPosition
		{
			get { return position; }
			set
			{
				Vector2 tempWorldPosition = position;
				base.CurrentWorldPosition = value;
				if (tempWorldPosition != position)
				{
					GameplayScreen.Content.Audio.TankMoving.Play();
					switch (PowerState)
					{
						case Power.Speed:
							EnergyPoints -= 2;
							break;
						case Power.Ghost:
							EnergyPoints -= 10;
							break;
						case Power.Invisible:
							EnergyPoints -= 1;
							break;
					}
				}
			}
		}
		#endregion
		#endregion

		#region Constructors
		// Nếu ko truyền vào tham số millisecondsPerFrame
		// Sử dụng defaultMillisecondsPerFrame = 16 (mặc định)
		public Player
			(
			GameplayScreen gameplayScreen, Texture2D botTexture, Texture2D topTexture, Color color,
			Vector2 position, float rotationAngle, Vector2 maximumSpeed,
			int hitPoints, Point frameSize, Point sheetSize, Point frameSizeTop,
			Point sheetSizeTop
			)
			: this
			(
			gameplayScreen, botTexture, topTexture, color, position, rotationAngle, maximumSpeed,
			hitPoints, frameSize, sheetSize, frameSizeTop, sheetSizeTop, 
			defaultMillisecondsPerFrame
			)
		{
			
		}

		// Ngược lại, sử dụng custom millisecondsPerFrame
		public Player
			(
			GameplayScreen gameplayScreen, Texture2D botTexture, Texture2D topTexture, Color color,
			Vector2 position, float rotationAngle, Vector2 maximumSpeed,
			int hitPoints, Point frameSize, Point sheetSize, Point frameSizeTop,
			Point sheetSizeTop, int millisecondsPerFrame
			)
			: base
			(
			gameplayScreen, botTexture, topTexture, color, position, rotationAngle, maximumSpeed,
			hitPoints, frameSize, sheetSize, frameSizeTop, sheetSizeTop, 
			millisecondsPerFrame
			)
		{
			this.typeID = 88;
			this.typeIndex = -1;
			this.powerState = Power.Armor;

			CurrentMapPosition = PreviousMapPosition =
				Map.GetSquareAtPixel(CurrentWorldPosition);
			CurrentMapPositionIndex = PreviousMapPositionIndex = 1;
			Map.MapSquares
				[(int)CurrentMapPosition.X, (int)CurrentMapPosition.Y]
				[CurrentMapPositionIndex, 0] = typeID;
			Map.MapSquares
				[(int)CurrentMapPosition.X, (int)CurrentMapPosition.Y]
				[CurrentMapPositionIndex, 1] = typeIndex;
		}
		#endregion

		#region Methods
		public override void Update(GameTime gameTime)
		{
			// Regenerate energyPoints
			if (EnergyIsSteady)
			{
				if (EnergyPoints < MaxEnergyPoints)
				{
					GameplayScreen.Content.Audio.Regenerating.Play();
				}
				EnergyPoints += energyPointsRegenerateRate;

				// Regenerate hitPoints
				if (EnergyPoints == MaxEnergyPoints)
				{
					if (HitPoints < MaxHitPoints)
					{
						GameplayScreen.Content.Audio.Regenerating.Play();
					}
					HitPoints += hitPointsRegenerateRate;
				}
			}

			// Nếu sử dụng hết năng lượng thì switch về Power Armor
			if (EnergyPoints == 0)
			{
				PowerState = Power.Armor;
			}

			// energyPoints Cooldown Timer
			EnergyCoolDown++;

			// Auto Scroll Viewport theo sự di chuyển của người chơi.
			ScrolllingControl();

			if (!base.IsAlive)
			{
				Map.MapSquares[(int)CurrentMapPosition.X,
					(int)CurrentMapPosition.Y][CurrentMapPositionIndex, 0] =
				Map.MapSquares[(int)CurrentMapPosition.X,
					(int)CurrentMapPosition.Y][CurrentMapPositionIndex, 1] = 0;
				Explode();
				this.IsDead = true;
				return;
			}

			// Đảm bảo rằng trạng thái hiện tại của tank là isCollidable, trừ
			// khi tank chuyển sang Power Ghost.
			this.isCollidable = true;
			switch (PowerState)
			{
				case Power.Speed:
					fader = ((float)Math.Sin
						(gameTime.TotalGameTime.TotalSeconds * 6)) * 0.25f + 0.25f;
					currentColor = Color.Lerp(defaultColor, Color.Red, fader);
					visibility = MathHelper.Clamp(visibility + 0.02f, 0, 1.0f);
					break;
				case Power.Invisible:
					fader = MathHelper.Clamp(fader + 0.02f, 0, 1.0f);
					currentColor = Color.Lerp(currentColor, defaultColor, fader);
					visibility = MathHelper.Lerp(visibility, 0.4f, fader);
					EnergyPoints -= 1;
					break;
				case Power.Ghost:
					fader = MathHelper.Clamp(fader + 0.02f, 0, 1.0f);
					currentColor = Color.Lerp(currentColor, Color.White, fader);
					visibility = MathHelper.Lerp(visibility, 0.4f, fader);
					this.isCollidable = false;
					break;
				default:
					fader = MathHelper.Clamp(fader + 0.02f, 0, 1.0f);
					currentColor = Color.Lerp(currentColor, defaultColor, fader);
					visibility = MathHelper.Clamp(visibility + 0.02f, 0, 1.0f);
					break;
			}

			base.Update(gameTime);
		}

		
		// Xử lý Input của người chơi
		public void HandleInput(InputState input)
		{
			#region Mouse Input
			RotationAngleTop = Rotate(RotationAngleTop, GameplayScreen.Cursor, 1.0f);

			// Input là Fire Main Gun:
			if (input.IsFireMainGun)
			{
				switch(PowerState)
				{
					case Power.Laser:
						FirePowerLaser(200);
						break;
					default:
						FireCannon(0);
						break;
				}
			}

			// Input là Fire Secondary Gun:
			if (input.IsFireSecondaryGun)
			{
				switch (PowerState)
				{
					case Power.Laser:
						FireGatlingLaser(20);
						break;
					default:
						FireMachineGun(0);
						break;
				}
			}
			#endregion

			#region Keyboard Input
			// Xử lý trạng thái Power, tùy theo Input:
			if (input.IsSelectPowerNo1)
			{
				PowerState = GameplayScreen.PowerIconList[0].Power;
			}
			else if (input.IsSelectPowerNo2)
			{
				PowerState = GameplayScreen.PowerIconList[1].Power;
			}
			else if (input.IsSelectPowerNo3)
			{
				PowerState = GameplayScreen.PowerIconList[2].Power;
			}
			else if (input.IsSelectPowerNo4)
			{
				PowerState = GameplayScreen.PowerIconList[3].Power;
			}
			else if (input.IsSelectPowerNo5)
			{
				PowerState = GameplayScreen.PowerIconList[4].Power;
			}

			// Xử lý sự xoay chuyển của bộ phận Bot
			if (input.IsRotateLeft)
				RotationAngle -= 0.05f;
			if (input.IsRotateRight)
				RotationAngle += 0.05f;

			// Xử lý sự di chuyển tới lui của Tank.
			if (input.IsMoveForward)
			{
				MovingSpeed += new Vector2(0.5f,0.0f);
			}
			if (input.IsMoveBackward)
			{
				MovingSpeed += new Vector2(0.0f,0.5f);
			}
			CurrentWorldPosition += MovingOffset;
			#endregion
		}

		// Auto Scroll Viewport theo sự di chuyển của người chơi
		private void ScrolllingControl()
		{
			Rectangle scrollArea = new Rectangle
							(
								(Camera.Viewport.Width - frameSize.X) / 2,
								(Camera.Viewport.Height - frameSize.Y) / 2,
								frameSize.X,
								frameSize.Y
							);

			if  (
				(BoundingScreenRectangle.X < scrollArea.X)
				&&
				(MovingOffset.X < 0)
				)
			{
				Camera.Move(new Vector2(MovingOffset.X, 0));
			}

			if (
				(BoundingScreenRectangle.Right > scrollArea.Right)
				&&
				(MovingOffset.X > 0)
				)
			{
				Camera.Move(new Vector2(MovingOffset.X, 0));
			}

			if (
				(BoundingScreenRectangle.Y < scrollArea.Y)
				&&
				(MovingOffset.Y < 0)
				)
			{
				Camera.Move(new Vector2(0, MovingOffset.Y));
			}

			if (
				(BoundingScreenRectangle.Bottom > scrollArea.Bottom)
				&&
				(MovingOffset.Y > 0)
				)
			{
				Camera.Move(new Vector2(0, MovingOffset.Y));
			}
		}
		#endregion
	}
}