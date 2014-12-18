using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	// Các Power của Tank
	public enum Power
	{
		None, Armor, Speed, Ghost, Invisible, Laser 
	}

	public abstract class Tank : Sprite
	{
		#region Declarations
		// ID của loại Tank.
		protected int typeID;
		public int TypeID
		{
			get { return typeID; }
		}

		// Index của Tank trong list quản lý nó.
		protected int typeIndex;
		public int TypeIndex
		{
			get { return typeIndex; }
			set { typeIndex = value; }
		}

		// fader được dùng để chuyển đổi dần dần các thông số màu sắc, 
		// độ trong suốt,... của tank sau khi chuyển Power.
		protected float fader;

		// Trạng thái Power của Tank
		protected Power powerState;

		// Màu sắc hiện tại của Tank.
		protected Color currentColor;

		// Màu sắc mặc định của Tank.
		protected readonly Color defaultColor;

		// Độ trong suốt của Tank.
		protected float visibility;

		/* Bộ phận Top của Tank gồm các thông số tương tự như của Bot: */
		// Texture.
		protected Texture2D textureImageTop;
		protected Point frameSizeTop;
		protected Point sheetSizeTop;
		protected Vector2 originTop;
		protected float rotationAngleTop;
		protected float scaleTop;
		protected float drawLayerTop;

		// hitPoints: (hay Máu) sức chịu đựng của đối tượng trước khi bị
		// phá hủy.
		protected int hitPoints;

		// MaxHitPoints (Hằng): hitPoints cực đại.
		public readonly int MaxHitPoints;

		// hitPointsRegenerateRate (Hằng): Tốc độ hồi máu.
		protected readonly int hitPointsRegenerateRate;

		// Năng lượng có thể được Tank sử dụng cho nhiều mục đích khác
		// nhau (các loại Power).
		// Đặc biệt, năng lượng có khả năng tự phục hồi sau một khoảng
		// thời gian xác định và nếu năng lượng đầy, hitPoints của Tank
		// cũng sẽ tự phục hồi.
		protected int energyPoints;

		// MaxEnergyPoints (Hằng): energyPoints cực đại.
		public readonly int MaxEnergyPoints;

		// energyPointsRegenerateRate (Hằng): Tốc độ năng lượng.
		protected readonly int energyPointsRegenerateRate;

		// Cooldown của năng lượng. Dùng cho cơ chế tự phục hồi năng lượng, 
		// chỉ phục hồi sau một thời gian xác định. (sau khi bị trúng đạn)
		protected int energyCoolDown;
		protected readonly int energyCoolDownTime;

		// Cooldown này được dùng để khống chế Repeat Rate của súng.
		protected int mainGunCoolDown;
		protected readonly int mainGunCoolDownTime;
		protected int secondaryGunCoolDown;
		protected readonly int secondaryGunCoolDownTime;
		
		// Tốc độ tối đa khi di chuyển và tốc độ đang di chuyển:
		// Tốc độ tối đa có thể di chuyển maximumSpeed được lập trình viên
		// truyền vào thông qua Constructor.
		// Tốc độ đang di chuyển movingSpeed đc cập nhật liên tục trong game
		// cho các Tanks (cả của AI và của người chơi)
		// Lưu ý: .X là tốc độ đi tới và .Y là tốc độ đi lùi, kết hợp với
		// rotationAngle tạo nên khả năng di chuyển di chuyển 360 độ. 
		// Vector ở đây ko nên hiểu theo nghĩa truyền thống, mà chỉ đơn 
		// thuần là một biến có khả năng lưu trữ 2 giá trị kiểu Float (một
		// cho tốc độ đi tới và một cho tốc độ đi lùi)
		protected readonly Vector2 maximumSpeed;
		protected Vector2 movingSpeed;

		// movingOffset: hướng di chuyển của đối tượng. Ví dụ, người chơi 
		// đang ở tọa độ (5,6), sau khi nhận đc input từ bàn phím và được 
		// xử lý, movingOffset nhận giá trị (2,3), thì sau đó tọa độ của 
		// người chơi sẽ đc cộng với movingOffset, kết quả là tọa độ mới (7,9).
		protected Vector2 movingOffset;
		#endregion

		#region Properties
		#region Public
		/// <summary>
		/// Trạng thái tồn tại của Tank.
		/// </summary>
		public virtual bool IsAlive
		{
			get { return hitPoints > 0; }
		}

		/// <summary>
		/// Truy xuất và thay đổi hitPoints của Tank.
		/// </summary>
		public virtual int HitPoints
		{
			get { return hitPoints; }
			set
			{
				if (value < hitPoints)
				{
					EnergyIsSteady = false;
				}
				hitPoints = (int)MathHelper.Clamp(value, 0, MaxHitPoints);
			}
		}

		/// <summary>
		/// Truy xuất và thay đổi energyPoints của Tank.
		/// </summary>
		public int EnergyPoints
		{
			get { return energyPoints; }
			set
			{
				if (value < energyPoints)
				{
					EnergyIsSteady = false;
				}
				energyPoints = (int)MathHelper.Clamp(value, 0, MaxEnergyPoints);
			}
		}

		/// <summary>
		/// Trạng thái năng lượng: đc cập nhật và kiểm soát liên tục nhằm 
		/// phục vụ khả năng tự phục hồi khi cần thiết.
		/// </summary>
		public bool EnergyIsSteady
		{
			get
			{
				return EnergyCoolDown == energyCoolDownTime;
			}
			set
			{
				if (value == false) EnergyCoolDown = 0;
			}
		}

		/// <summary>
		/// Trạng thái Power của người chơi
		/// </summary>
		public Power PowerState
		{
			get { return powerState; }
			set
			{
				if (powerState != value
					&&
					(value == Power.Armor || EnergyPoints > 0))
				{
					GameplayScreen.Content.Audio.PowerSwitch.Play(0.8f, 0, 0);
					powerState = value;

					// Sau khi thay đổi Power, fader sẽ tự trả về giá trị 0
					// nhằm phục vụ việc đổi màu.
					fader = 0;
				}
			}
		}

		/// <summary>
		/// Override Property CurrentWorldPosition của base để thực hiện việc
		/// kiểm soát vị trí Tank nếu xảy ra va chạm với nhau hoặc với tường.
		/// </summary>
		public override Vector2 CurrentWorldPosition
		{
			get { return position; }
			set
			{
				Vector2 backupCurrentWorldPosition = position;

				position.X = MathHelper.Clamp
					(value.X, frameSize.X / 2,
					Camera.WholeWorldRectangle.Width - frameSize.X / 2);
				if (CollisionCheck())
				{
					position.X = backupCurrentWorldPosition.X;
				}

				position.Y = MathHelper.Clamp
					(value.Y, frameSize.Y / 2,
					Camera.WholeWorldRectangle.Height - frameSize.Y / 2);
				if (CollisionCheck())
				{
					position.Y = backupCurrentWorldPosition.Y;
				}
			}
		}

		/// <summary>
		/// Override Property RotationAngle của base nhằm thực hiện việc
		/// kiểm soát góc quay nếu xảy ra va chạm.
		/// </summary>
		public override float RotationAngle
		{
			get { return rotationAngle; }
			set
			{
				float backupRotationAngle = rotationAngle;
				Vector2 backupCurrentWorldPosition = CurrentWorldPosition;
				rotationAngle = MathHelper.WrapAngle(value);
				float[] checkOrder = { -MathHelper.PiOver2, MathHelper.PiOver2, 0, MathHelper.Pi };
				int i = -1;
				while (CollisionCheck())
				{
					i++;
					if (i > checkOrder.Length - 1)
					{
						CurrentWorldPosition = backupCurrentWorldPosition;
						rotationAngle = backupRotationAngle;
						break;
					}
					float tempAngle = rotationAngle + checkOrder[i];
					movingOffset.X = (float)
						(
						maximumSpeed.X * Math.Cos(tempAngle)
						);

					movingOffset.Y = (float)
						(
						maximumSpeed.Y * Math.Sin(tempAngle)
						);
					CurrentWorldPosition += movingOffset;
				}
			}
		}

		/// <summary>
		/// Truy xuất và thay đổi góc quay của Top.
		/// </summary>
		public float RotationAngleTop
		{
			get { return rotationAngleTop; }
			set { rotationAngleTop = MathHelper.WrapAngle(value); }
		}

		/// <summary>
		/// Truy xuất và thay đổi movingSpeed.
		/// </summary>
		public Vector2 MovingSpeed
		{
			get { return movingSpeed; }
			set
			{
				float slowFactor = 1;
				if (Map.IsWaterTile(CurrentMapPosition))
				{
					slowFactor = 2;
				}
				else if (Map.IsRoadTile(CurrentMapPosition))
				{
					slowFactor = 0.75f;
				}
				if (PowerState != Power.Speed)
					movingSpeed = Vector2.Clamp(value, Vector2.Zero, maximumSpeed / slowFactor);
				else
					movingSpeed = Vector2.Clamp(value, Vector2.Zero, maximumSpeed * 2 / slowFactor);
			}
		}

		/// <summary>
		/// Truy xuất movingOffset hiện tại.
		/// </summary>
		public Vector2 MovingOffset
		{
			get
			{
				movingOffset.X = (float)
					(
					MovingSpeed.X * Math.Cos(RotationAngle)
					-
					MovingSpeed.Y * Math.Cos(RotationAngle)
					);

				movingOffset.Y = (float)
					(
					MovingSpeed.X * Math.Sin(RotationAngle)
					-
					MovingSpeed.Y * Math.Sin(RotationAngle)
					);
				return movingOffset;
			}
		}
		#endregion

		#region Private
		/// <summary>
		/// Truy xuất và thay đổi Cooldown của năng lượng.
		/// </summary>
		protected int EnergyCoolDown
		{
			get { return energyCoolDown; }
			set
			{
				energyCoolDown =
					(int)MathHelper.Clamp(value, 0, energyCoolDownTime);
			}
		}

		/// <summary>
		/// Truy xuất và thay đổi Cooldown của súng chính.
		/// </summary>
		protected int MainGunCoolDown
		{
			get { return mainGunCoolDown; }
			set
			{
				mainGunCoolDown =
					(int)MathHelper.Clamp(value, 0, mainGunCoolDownTime);
			}
		}

		/// <summary>
		/// Truy xuất và thay đổi Cooldown của súng phụ.
		/// </summary>
		protected int SecondaryGunCoolDown
		{
			get { return secondaryGunCoolDown; }
			set
			{
				secondaryGunCoolDown =
					(int)MathHelper.Clamp(value, 0, secondaryGunCoolDownTime);
			}
		}

		/// <summary>
		/// Trạng thái sẵn sàng của súng chính.
		/// </summary>
		protected virtual bool MainGunIsShootable
		{
			get
			{
				return mainGunCoolDown == mainGunCoolDownTime;
			}
			set
			{
				if (value == false)
				{
					mainGunCoolDown = 0;

					// Nếu Tank khai hỏa trong khi đang tàng hình thì sẽ mất 
					// trạng thái tàng hình.
					if (PowerState == Power.Invisible)
					{
						PowerState = Power.Armor;
					}
				}
			}
		}

		/// <summary>
		/// Trạng thái sẵn sàng của súng phụ.
		/// </summary>
		protected virtual bool SecondaryGunIsShootable
		{
			get
			{
				return secondaryGunCoolDown == secondaryGunCoolDownTime;
			}
			set
			{
				if (value == false)
				{
					secondaryGunCoolDown = 0;

					// Nếu Tank khai hỏa trong khi đang tàng hình thì sẽ mất 
					// trạng thái tàng hình.
					if (PowerState == Power.Invisible)
					{
						PowerState = Power.Armor;
					}
				}
			}
		}

		/// <summary>
		/// Lấy vị trí của nòng súng, tùy thuộc vào góc quay Top của người chơi.
		/// Nhờ có property này mà đạn bắn ra luôn có góc quay và vị trí phù
		/// hợp với bộ phận Top.
		/// </summary>
		protected Vector2 GunPosition
		{
			get
			{
				Vector2 gunOffset;

				gunOffset.X = frameSize.X * (float)Math.Cos(RotationAngleTop);
				gunOffset.Y = frameSize.Y * (float)Math.Sin(RotationAngleTop);

				return CurrentWorldPosition + gunOffset;
			}
		}
		#endregion
		#endregion

		#region Constructors
		/// <summary>
		/// Nếu ko truyền vào tham số millisecondsPerFrame thì sử dụng 
		/// defaultMillisecondsPerFrame = 16 (mặc định)
		/// Ngoài ra, Constructors của Tank sẽ có thêm topTexture, maximumSpeed,
		/// color, hitPoints so với base.
		/// </summary>
		public Tank
			(
			GameplayScreen gameplayScreen, Texture2D textureBot, Texture2D textureTop, Color color,
			Vector2 position, float rotationAngle, Vector2 maximumSpeed,
			int hitPoints, Point frameSizeBot, Point sheetSizeBot,
			Point frameSizeTop, Point sheetSizeTop
			)
			: this
			(
			gameplayScreen, textureBot, textureTop, color, position, rotationAngle, maximumSpeed,
			hitPoints, frameSizeBot, sheetSizeBot, frameSizeTop, sheetSizeTop,
			defaultMillisecondsPerFrame
			)
		{

		}

		/// <summary>
		/// Ngược lại, sử dụng millisecondsPerFrame từ Constructor.
		/// </summary>
		public Tank
			(
			GameplayScreen gameplayScreen, Texture2D textureBot, Texture2D textureTop, Color color,
			Vector2 position, float rotationAngle, Vector2 maximumSpeed,
			int hitPoints, Point frameSizeBot, Point sheetSizeBot, 
			Point frameSizeTop, Point sheetSizeTop, int millisecondsPerFrame
			)
			: base
			(
			gameplayScreen, textureBot, position, rotationAngle, frameSizeBot, sheetSizeBot, 
			millisecondsPerFrame
			)
		{
			this.drawLayer = 0.2f;
			this.drawLayerTop = drawLayer + 0.1f;
			this.textureImageTop = textureTop;
			this.frameSizeTop = frameSizeTop;
			this.sheetSizeTop = sheetSizeTop;
			this.originTop = new Vector2(frameSizeTop.X / 2, frameSizeTop.Y / 2);
			this.scaleTop = 1.0f;

			// Mặc định, Tank không có Power.
			this.powerState = Power.None;
			this.visibility = 1.0f;
			this.currentColor = this.defaultColor = color;
			this.hitPoints = this.MaxHitPoints = hitPoints;
			this.hitPointsRegenerateRate = 20;
			// energyPoints sẽ có giá trị bằng hitPoints
			this.energyPoints = this.MaxEnergyPoints = hitPoints;
			this.energyPointsRegenerateRate = 40;
			this.energyCoolDownTime = 300;

			this.mainGunCoolDownTime = 60;
			this.secondaryGunCoolDownTime = 5;
			this.maximumSpeed = maximumSpeed;

			this.isCollidable = true;

			this.textureImageData = Texture.GetTextureData
				(textureImage, new Rectangle(0, 0, frameSize.X, frameSize.Y));
		}
		#endregion

		#region Methods
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			MovingSpeed -= new Vector2(0.1f, 0.1f);

			// Update cooldown của súng
			MainGunCoolDown++;
			SecondaryGunCoolDown++;

			if (!IsAlive)
			{
				Map.MapSquares[(int)CurrentMapPosition.X,
					(int)CurrentMapPosition.Y][CurrentMapPositionIndex, 0] =
				Map.MapSquares[(int)CurrentMapPosition.X,
					(int)CurrentMapPosition.Y][CurrentMapPositionIndex, 1] = 0;
				Explode();
			}

			// Cập nhật vị trí trên bản đồ.
			UpdateMapPosition();
		}

		/// <summary>
		/// Do Tank có thêm bộ phận Top nên ta cần override hàm Draw của
		/// base để vẽ thêm bộ phận này
		/// </summary>
		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Chỉ vẽ nếu đối tượng đang hiện hữu trong Viewport
			if (Camera.ObjectIsVisible(BoundingWorldRectangle))
			{
				// Lấy vị trí bóng.
				Vector2 shadowPosition = new Vector2
					(CurrentScreenPosition.X + 5.0f, CurrentScreenPosition.Y + 5.0f);

				// Vẽ Bot.
				spriteBatch.Draw
					(
					textureImage, CurrentScreenPosition, new Rectangle
						(
						currentFrame.X * frameSize.X,
						currentFrame.Y * frameSize.Y,
						frameSize.X, frameSize.Y
						),
					currentColor * visibility, RotationAngle, origin, scale,
					SpriteEffects.None, drawLayer
					);

				// Vẽ bóng Bot.
				spriteBatch.Draw
					(
					textureImage, shadowPosition, new Rectangle
						(
						currentFrame.X * frameSize.X,
						currentFrame.Y * frameSize.Y,
						frameSize.X, frameSize.Y
						),
					Color.Black * 0.6f * visibility, RotationAngle, origin, scale,
					SpriteEffects.None, drawLayer - 0.05f
					);

				// Lấy vị trí bóng.
				shadowPosition = new Vector2
					(CurrentScreenPosition.X + 3.0f, CurrentScreenPosition.Y + 3.0f);

				// Vẽ Top.
				spriteBatch.Draw
					(
					textureImageTop, CurrentScreenPosition, new Rectangle
						(0, 0, frameSizeTop.X, frameSizeTop.Y),
					currentColor * visibility, RotationAngleTop, originTop, 
					scaleTop, SpriteEffects.None, drawLayerTop
					);

				// Vẽ bóng của Top.
				spriteBatch.Draw
					(
					textureImageTop, shadowPosition, new Rectangle
						(0, 0, frameSizeTop.X, frameSizeTop.Y),
					Color.Black * 0.8f * visibility, RotationAngleTop, originTop, 
					scaleTop, SpriteEffects.None, drawLayerTop - 0.05f
					);
			}
		}

		#region Actions
		// Khai hỏa Cannon.
		protected virtual void FireCannon(int energyCost)
		{
			if (MainGunIsShootable)
			{
				EnergyPoints -= energyCost;
				GameplayScreen.AmmunitionList.Add
					(
						new Missile
							(
							this.GameplayScreen, GameplayScreen.Content.Texture.Missile, GunPosition,
							RotationAngleTop, 8, 100, new Point(24, 12),
							new Point(8, 5)
							)
					);
				Audio.Play
					(GameplayScreen.Content.Audio.MissileFiring, this.CurrentWorldPosition,
					GameplayScreen.Player.CurrentWorldPosition, 5, 0.0f, 0.5f);
				MainGunIsShootable = false;
			}
		}

		// Khai hỏa súng máy.
		protected virtual void FireMachineGun(int energyCost)
		{
			if (SecondaryGunIsShootable)
			{
				EnergyPoints -= energyCost;
				GameplayScreen.AmmunitionList.Add
					(
						new Bullet
							(
							this.GameplayScreen, GameplayScreen.Content.Texture.Bullet, GunPosition,
							RotationAngleTop, 20, 10, new Point(12, 6),
							new Point(1, 1)
							)
					);
				Audio.Play
					(GameplayScreen.Content.Audio.BulletFiring, this.CurrentWorldPosition,
					GameplayScreen.Player.CurrentWorldPosition, 5, 0.0f, 0.5f);
				SecondaryGunIsShootable = false;
			}
		}

		// Khai hỏa Power Laser.
		protected virtual void FirePowerLaser(int energyCost)
		{
			if (MainGunIsShootable)
			{
				EnergyPoints -= energyCost;
				GameplayScreen.AmmunitionList.Add
					(
						new BigLaserRay
							(
							this.GameplayScreen, GameplayScreen.Content.Texture.BigLaserRay, GunPosition,
							RotationAngleTop, 16, 400, new Point(32, 12),
							new Point(1, 1)
							)
					);
				Audio.Play
					(GameplayScreen.Content.Audio.BigLaserRayFiring, this.CurrentWorldPosition,
					GameplayScreen.Player.CurrentWorldPosition, 5, 0.0f, 0.5f);
				MainGunIsShootable = false;
			}
		}

		// Khai hỏa tiểu liên Laser.
		protected virtual void FireGatlingLaser(int energyCost)
		{
			if (SecondaryGunIsShootable)
			{
				EnergyPoints -= energyCost;
				GameplayScreen.AmmunitionList.Add
					(
						new SmallLaserRay
							(
							this.GameplayScreen, GameplayScreen.Content.Texture.SmallLaserRay, GunPosition,
							RotationAngleTop, 40, 40, new Point(32, 8),
							new Point(1, 1)
							)
					);
				Audio.Play
					(GameplayScreen.Content.Audio.SmallLaserRayFiring, this.CurrentWorldPosition,
					GameplayScreen.Player.CurrentWorldPosition, 5, 0.0f, 0.5f);
				SecondaryGunIsShootable = false;
			}
		}

		// Nổ.
		protected void Explode()
		{
			GameplayScreen.EffectList.Add
				(
					new CollisionEffect
						(
						this.GameplayScreen, GameplayScreen.Content.Texture.ExplosionSheet, CurrentWorldPosition, 0,
						new Point(128, 128), new Point(8, 5)
						)
				);
			Audio.Play
				(GameplayScreen.Content.Audio.Explosion, this.CurrentWorldPosition,
				GameplayScreen.Player.CurrentWorldPosition, 500, 0.0f, 1.0f);
		}
		#endregion

		#region Helpers
		/************************************************************************/
		/*   [Trợ giúp] Các hàm xoay và lấy góc giữa 2 điểm (hoặc 2 đối tượng)  */
		/************************************************************************/
		protected float Rotate
			(float originAngle, float targetAngle, float rotatingSpeed)
		{
			if (targetAngle.Equals(float.NaN))
			{
				return originAngle;
			}
			float difference = MathHelper.WrapAngle(targetAngle - originAngle);
			difference = MathHelper.Clamp(difference, -rotatingSpeed, rotatingSpeed);
			return MathHelper.WrapAngle(originAngle + difference);
		}

		protected float Rotate
			(float originAngle, Sprite targetSprite, float rotatingSpeed)
		{
			float targetAngle = (float)GetAngle(this, targetSprite);
			return Rotate(originAngle, targetAngle, rotatingSpeed);
		}

		protected double GetAngle
			(Vector2 originCoordinate, Vector2 targetCoordinate)
		{
			return GetAngle
				(originCoordinate.X, originCoordinate.Y,
				targetCoordinate.X, targetCoordinate.Y);
		}

		protected double GetAngle(Sprite originTank, Sprite targetTank)
		{
			return GetAngle
				(originTank.CurrentWorldPosition, targetTank.CurrentWorldPosition);
		}

		protected double GetAngle
			(double originX, double originY, double targetX, double targetY)
		{
			double temp = Math.Sqrt
				((targetX - originX) * (targetX - originX)
				+
				(targetY - originY) * (targetY - originY));
			temp = MathHelper.Clamp
				((float)((targetX - originX) / temp), -1.0f, 1.0f);
			if (targetY > originY)
				return Math.Acos(temp);
			else
				return -Math.Acos(temp);
		}

		public override void UpdateMapPosition()
		{
			Map.MapSquares
				[CurrentMapPosition.X, CurrentMapPosition.Y]
				[CurrentMapPositionIndex, 0] = typeID;
			Map.MapSquares
				[CurrentMapPosition.X, CurrentMapPosition.Y]
				[CurrentMapPositionIndex, 1] = typeIndex;

			if (CurrentMapPosition != Map.GetSquareAtPixel(CurrentWorldPosition))
			{
				PreviousMapPosition = CurrentMapPosition;
				CurrentMapPosition = Map.GetSquareAtPixel(CurrentWorldPosition);

				for (int i = 1; i < 4; i++)
				{
					if (Map.MapSquares
						[(int)CurrentMapPosition.X,
						(int)CurrentMapPosition.Y][i, 0] == 0)
					{
						Map.MapSquares
							[(int)CurrentMapPosition.X,
							(int)CurrentMapPosition.Y][i, 0] = typeID;
						Map.MapSquares
							[(int)CurrentMapPosition.X,
							(int)CurrentMapPosition.Y][i, 1] = typeIndex;
						PreviousMapPositionIndex = CurrentMapPositionIndex;
						CurrentMapPositionIndex = i;
						break;
					}
				}

				Map.MapSquares
					[(int)PreviousMapPosition.X, (int)PreviousMapPosition.Y]
					[PreviousMapPositionIndex, 0] =
				Map.MapSquares
					[(int)PreviousMapPosition.X, (int)PreviousMapPosition.Y]
					[PreviousMapPositionIndex, 1] = 0;
			}
		}
		#endregion
		#endregion
	}
}