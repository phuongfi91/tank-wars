using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public enum GameMode
	{
		Campaign, FaceOff, TankRacing, LaserTorture
	}

	public class GameplayScreen : GameScreen
	{
		#region Declarations
		float pauseAlpha;
		ContentManager content;
		#endregion

		#region Properties
		#region Public
		public Content Content { get; set; }
		public int CurrentMission { get; set; }
		public String CurrentMissionName { get; private set; }
		public GameMode CurrentGameMode { get; private set; }

		public Player Player { get; set; }

		public Cursor Cursor { get; set; }

		public Camera Camera { get; set; }

		public Map Map { get; set; }

		// List các đối tượng do AI điều khiển
		public List<Tank> EnemyList { get; set; }

		// List các viên đạn do người chơi bắn ra
		public List<Ammunition> AmmunitionList { get; set; }

		// List các hiệu ứng nổ, va chạm
		public List<Sprite> EffectList { get; set; }

		// List các đối tượng Wall để check va chạm
		public List<Wall> WallList { get; set; }

		// List các Icon Power của người chơi
		public List<PowerIcon> PowerIconList { get; set; }

		// Dùng cho vòng lặp update các đối tượng trong list
		public int CurrentListItem { get; set; }
		#endregion

		#region Private
		private Direction Direction { get; set; }
		private Minimap Minimap { get; set; }
		private Bar HealthBar { get; set; }
		private Bar EnergyBar { get; set; }
		private int CampaignLenght { get; set; }
		private int MaximumMovableEnemies { get; set; }
		private int MovableEnemiesCount { get; set; }
		private int ImmovableEnemiesCount { get; set; }
		private TimeSpan MissionEndDelay { get; set; }
		private bool IsMissionAccomplished
		{
			get
			{
				return EnemyList.Count == 0 && MovableEnemiesCount == 0;
			}
		}
		private bool IsCampaignAccomplished
		{
			get
			{
				return
					IsMissionAccomplished
					&&
					CurrentMission == CampaignLenght;
			}
		}
		#endregion
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public GameplayScreen(GameMode gameMode)
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
			CurrentGameMode = gameMode;
			switch (CurrentGameMode)
			{
				case GameMode.Campaign:
					CurrentMission = 1;
					CampaignLenght = 8;
					break;
				case GameMode.LaserTorture:
					CurrentMission = 1;
					CampaignLenght = 10;
					break;
				default:
					break;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		public override void LoadContent()
		{
			if (content == null)
			{
				content = new ContentManager(TankWars.Services, "Content");
			}

			Content = new Content();
			Content.Font.HUDFont = content.Load<SpriteFont>(@"Fonts\HUDFont");

			// Load tất cả các Textures cần thiết
			Content.Texture.Cursor = content.Load<Texture2D>(@"Texture\Cursor");
			Content.Texture.Direction = content.Load<Texture2D>(@"Texture\Direction");
			Content.Texture.TankBot = content.Load<Texture2D>(@"Texture\TankBot");
			Content.Texture.TankTop = content.Load<Texture2D>(@"Texture\TankTop");
			Content.Texture.HumveeBot = content.Load<Texture2D>(@"Texture\HumveeBot");
			Content.Texture.HumveeTop = content.Load<Texture2D>(@"Texture\HumveeTop");
			Content.Texture.Bullet = content.Load<Texture2D>(@"Texture\Bullet");
			Content.Texture.Missile = content.Load<Texture2D>(@"Texture\Missile");
			Content.Texture.SmallLaserRay = content.Load<Texture2D>(@"Texture\SmallLaserRay");
			Content.Texture.BigLaserRay = content.Load<Texture2D>(@"Texture\BigLaserRay");
			Content.Texture.MiniMapRectangle = content.Load<Texture2D>(@"Texture\MiniMapRectangle");
			Content.Texture.Dirt = content.Load<Texture2D>(@"Texture\Dirt");
			Content.Texture.TileSheet = content.Load<Texture2D>(@"Texture\TileSheet");
			Content.Texture.PowerSheet = content.Load<Texture2D>(@"Texture\PowerSheet");
			Content.Texture.BarSheet = content.Load<Texture2D>(@"Texture\BarSheet");
			Content.Texture.SmallFlameSheet = content.Load<Texture2D>(@"Texture\SmallFlameSheet");
			Content.Texture.BigFlameSheet = content.Load<Texture2D>(@"Texture\BigFlameSheet");
			Content.Texture.BulletImpactSheet = content.Load<Texture2D>(@"Texture\BulletImpactSheet");
			Content.Texture.MissileImpactSheet = content.Load<Texture2D>(@"Texture\MissileImpactSheet");
			Content.Texture.LaserImpactSheet = content.Load<Texture2D>(@"Texture\LaserImpactSheet");
			Content.Texture.EnergyImpactSheet = content.Load<Texture2D>(@"Texture\EnergyImpactSheet");
			Content.Texture.ExplosionSheet = content.Load<Texture2D>(@"Texture\ExplosionSheet");
			Content.Texture.Blank = content.Load<Texture2D>(@"Texture\Blank");

			// Load tất cả các Sounds cần thiết
			Content.Audio.BulletFiring = content.Load<SoundEffect>(@"Audio\BulletFiring");
			Content.Audio.BulletImpact = content.Load<SoundEffect>(@"Audio\BulletImpact");
			Content.Audio.MissileFiring = content.Load<SoundEffect>(@"Audio\MissileFiring");
			Content.Audio.MissileImpact = content.Load<SoundEffect>(@"Audio\MissileImpact");
			Content.Audio.SmallLaserRayFiring = content.Load<SoundEffect>(@"Audio\SmallLaserRayFiring");
			Content.Audio.SmallLaserRayImpact = content.Load<SoundEffect>(@"Audio\SmallLaserRayImpact");
			Content.Audio.BigLaserRayFiring = content.Load<SoundEffect>(@"Audio\BigLaserRayFiring");
			Content.Audio.BigLaserRayImpact = content.Load<SoundEffect>(@"Audio\BigLaserRayImpact");
			Content.Audio.Explosion = content.Load<SoundEffect>(@"Audio\Explosion");
			Content.Audio.PowerSwitch = content.Load<SoundEffect>(@"Audio\PowerSwitch");
			Content.Audio.TankMove = content.Load<SoundEffect>(@"Audio\TankMove");
			Content.Audio.TankMoving = Content.Audio.TankMove.CreateInstance();
			Content.Audio.Regenerate = content.Load<SoundEffect>(@"Audio\Regenerate");
			Content.Audio.Regenerating = Content.Audio.Regenerate.CreateInstance();
			Content.Audio.BackgroundBattlefield = content.Load<SoundEffect>(@"Audio\BackgroundBattlefield");
			Content.Audio.BackgroundBattlefieldLoop = Content.Audio.BackgroundBattlefield.CreateInstance();
			Content.Audio.BackgroundBattlefieldLoop.IsLooped = true;
			Content.Audio.BackgroundBattlefieldLoop.Volume = 0.2f;
			Content.Audio.MissionAccomplish = content.Load<SoundEffect>(@"Audio\MissionAccomplish");
			Content.Audio.MissionAccomplished = Content.Audio.MissionAccomplish.CreateInstance();
			Content.Audio.MissionFail = content.Load<SoundEffect>(@"Audio\MissionFail");
			Content.Audio.MissionFailed = Content.Audio.MissionFail.CreateInstance();

			Initialize();

			// once the load has finished, we use ResetElapsedTime to tell the game's
			// timing mechanism that we have just finished a very long frame, and that
			// it should not try to catch up.
			TankWars.ResetElapsedTime();
		}

		public void Initialize()
		{
			EnemyList = new List<Tank>();
			AmmunitionList = new List<Ammunition>();
			EffectList = new List<Sprite>();
			WallList = new List<Wall>();
			PowerIconList = new List<PowerIcon>();
			Texture.TextureImages = new List<Texture2D>();
			Texture.TextureDatas = new List<Color[]>();

			MissionEndDelay = TimeSpan.Zero;

			InitializeMap();

			InitializeAIMap();

			InitializeTanks();

			InitializeCamera();

			InitializeHUD();
		}
		
		/// <summary>
		/// Unload graphics content used by the game.
		/// </summary>
		public override void UnloadContent()
		{
			content.Unload();
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update
			(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);

			// Gradually fade in or out depending on whether we are covered by the pause screen.
			if (coveredByOtherScreen)
				pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
			else
				pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

			if (IsActive)
			{
				Content.Audio.BackgroundBattlefieldLoop.Play();

				Camera.Update(gameTime);

				Cursor.Update(gameTime);

				Minimap.Update(gameTime);

				// Update Sprite người chơi và các thao tác điều khiển
				if (!Player.IsDead)
				{
					Player.Update(gameTime);
					Direction.Update(gameTime);
				}

				// Update các viên đạn đã đc bắn ra
				for (CurrentListItem = 0; CurrentListItem < AmmunitionList.Count; CurrentListItem++)
				{
					Ammunition ammunition = AmmunitionList[CurrentListItem];
					ammunition.Update(gameTime);
				}

				// Update các vụ nổ
				for (CurrentListItem = 0; CurrentListItem < EffectList.Count; CurrentListItem++)
				{
					Sprite effect = EffectList[CurrentListItem];
					effect.Update(gameTime);
				}

				// Update Enemy
				RandomMovableEnemy();

				for (CurrentListItem = 0; CurrentListItem < EnemyList.Count; CurrentListItem++)
				{
					Tank enemy = EnemyList[CurrentListItem];
					enemy.Update(gameTime);
				}

				foreach (PowerIcon power in PowerIconList)
				{
					power.Update(gameTime);
				}

				if (Player.IsDead)
				{
					MissionEndDelay += gameTime.ElapsedGameTime;
					if (MissionEndDelay >= TimeSpan.FromSeconds(2))
					{
						Content.Audio.MissionFailed.Play();
						Content.Audio.BackgroundBattlefieldLoop.Pause();
						TankWars.AddScreen(new MissionFailedMenuScreen());
					}
				}

				if (IsMissionAccomplished)
				{
					MissionEndDelay += gameTime.ElapsedGameTime;
					if (MissionEndDelay >= TimeSpan.FromSeconds(2))
					{
						Content.Audio.MissionAccomplished.Play();
						Content.Audio.BackgroundBattlefieldLoop.Pause();
						if (IsCampaignAccomplished)
						{
							TankWars.AddScreen(new CampaignAccomplishedMenuScreen());
						}
						else
						{
							TankWars.AddScreen(new MissionAccomplishedMenuScreen());
						}
					}
				}
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Draw(GameTime gameTime)
		{
			// Màu nền
			TankWars.GraphicsDevice.Clear(Color.Black);

			// Dùng để vẽ các đối tượng
			SpriteBatch spriteBatch = TankWars.SpriteBatch;

			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend
				/*, null, null, null, null, Camera.get_transformation(Game.GraphicsDevice)*/);

			// Vẽ Màn chơi
			Map.Draw(gameTime, spriteBatch);

			foreach (PowerIcon power in PowerIconList)
			{
				power.Draw(gameTime, spriteBatch);
			}

			// Vẽ Minimap
			Minimap.Draw(gameTime, spriteBatch);

			// Vẽ cursor người chơi
			Cursor.Draw(gameTime, spriteBatch);

			// Vẽ người chơi
			if (!Player.IsDead)
			{
				Player.Draw(gameTime, spriteBatch);
				Direction.Draw(gameTime, spriteBatch);
				Minimap.DrawDot(Player, Color.Blue, spriteBatch);
			}

			// Vẽ Health Bar
			HealthBar.Draw(Player.HitPoints, Player.MaxHitPoints, spriteBatch);

			// Vẽ Energy Bar
			EnergyBar.Draw(Player.EnergyPoints, Player.MaxEnergyPoints, spriteBatch);

			// Vẽ đạn
			foreach (Ammunition ammunition in AmmunitionList)
			{
				ammunition.Draw(gameTime, spriteBatch);
			}

			// Vẽ các vụ nổ
			foreach (Sprite effect in EffectList)
			{
				effect.Draw(gameTime, spriteBatch);
			}

			// Vẽ Enemy
			foreach (Tank enemy in EnemyList)
			{
				enemy.Draw(gameTime, spriteBatch);
				Minimap.DrawDot(enemy, Color.Red, spriteBatch);
			}

			spriteBatch.End();

			// If the game is transitioning on or off, fade it out to black.
			if (TransitionPosition > 0 || pauseAlpha > 0)
			{
				float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

				TankWars.FadeBackBufferToBlack(alpha);
			}
		}

		// Xử lý trường hợp người chơi nhấn Esc
		public override void HandleInput(InputState input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (!Player.IsDead)
			{
				Player.HandleInput(input);
				Cursor.HandleInput(input);
			}

			if (input.IsPauseGame)
			{
				Content.Audio.BackgroundBattlefieldLoop.Pause();
				TankWars.AddScreen(new PauseMenuScreen());
			}
		}

		private void InitializeTanks()
		{
			Vector2 playerStartingPoint = Map.GetSpawningLocation();
			float playerStartingRotation = Map.GetSpawningRotation();

			Player = new Player
				(
					this, Content.Texture.TankBot, Content.Texture.TankTop,
					new Color(200, 180, 140), playerStartingPoint, playerStartingRotation,
					new Vector2(3, 3), 3000, new Point(45, 45), new Point(1, 1),
					new Point(70, 70), new Point(1, 1)
				);

			if (CurrentGameMode == GameMode.Campaign)
			{
				for (int i = 0; i < Map.MapWidth; i++)
					for (int j = 0; j < Map.MapHeight; j++)
					{
						if (Map.IsEliteSpawnTile(i, j))
						{
							EnemyList.Add
								(new Enemy
									(
									this, Content.Texture.TankBot, Content.Texture.TankTop,
									Color.GhostWhite, AItype.StationaryAI, WeaponSet.CannonOnly,
									Map.GetSquareCenter(new Point(i, j)), 0,
									Vector2.Zero, 500, new Point(45, 45), new Point(1, 1),
									new Point(70, 70), new Point(1, 1)
									)
								);
						}
					}
			}
			else if (CurrentGameMode == GameMode.LaserTorture)
			{
				Random random = new Random();
				while (EnemyList.Count < MaximumMovableEnemies)
				{
				Start:
					int A = random.Next(0, Map.MapWidth - 1);
					int B = random.Next(0, Map.MapHeight - 1);
					if (Map.IsWallTile(A, B)
						||
						Camera.ObjectIsVisible(new Rectangle(A * 64, B * 64, 64, 64)))
						goto Start;
					for (int i = -1; i < 2; i++)
						for (int j = -1; j < 2; j++)
						{
							if (Map.IsOccupiedByTank(A + i, B + j))
							{
								goto Start;
							}
						}

					if (random.NextDouble() > 0.5)
					{
						EnemyList.Add
						(
							new Enemy
								(
								this, Content.Texture.TankBot, Content.Texture.TankTop,
								new Color(150, 200, 200), AItype.ForcedOffensiveAI,
								WeaponSet.LaserSet, Map.GetSquareCenter(new Point(A, B)), 0,
								new Vector2(2, 2), 300, new Point(45, 45), new Point(1, 1),
								new Point(70, 70), new Point(1, 1)
								)
						);
					}
					else
					{
						EnemyList.Add
						(
							new Enemy
								(
								this, Content.Texture.HumveeBot, Content.Texture.HumveeTop,
								new Color(255, 230, 180), AItype.ForcedOffensiveAI,
								WeaponSet.GatlingLaserOnly, Map.GetSquareCenter(new Point(A, B)), 0,
								new Vector2(4, 4), 100, new Point(45, 45), new Point(1, 1),
								new Point(45, 45), new Point(1, 1)
								)
						);
					}
				}
			}
		}

		private void RandomMovableEnemy()
		{
			MovableEnemiesCount = 0;
			ImmovableEnemiesCount = 0;
			foreach (Enemy enemy in EnemyList)
			{
				if (enemy.AItypeID != AItype.StationaryAI)
				{
					MovableEnemiesCount++;
				}
			}
			ImmovableEnemiesCount = EnemyList.Count - MovableEnemiesCount;

			Random random = new Random();
			if (ImmovableEnemiesCount > 0
				&&
				MovableEnemiesCount < MaximumMovableEnemies)
			{
			Start:
				int A = random.Next(0, Map.MapWidth - 1);
				int B = random.Next(0, Map.MapHeight - 1);
				if (Map.IsWallTile(A, B)
					||
					Camera.ObjectIsVisible(new Rectangle(A * 64, B * 64, 64, 64)))
					goto Start;
				for (int i = -1; i < 2; i++)
					for (int j = -1; j < 2; j++)
					{
						if (Map.IsOccupiedByTank(A + i, B + j))
						{
							goto Start;
						}
					}

				if (random.NextDouble() > 0.5)
				{
					EnemyList.Add
					(
						new Enemy
							(
							this, Content.Texture.TankBot, Content.Texture.TankTop,
							new Color(150, 200, 200), AItype.ForcedOffensiveAI,
							WeaponSet.StandardSet, Map.GetSquareCenter(new Point(A, B)), 0,
							new Vector2(2, 2), 300, new Point(45, 45), new Point(1, 1),
							new Point(70, 70), new Point(1, 1)
							)
					);
				}
				else
				{
					EnemyList.Add
					(
						new Enemy
							(
							this, Content.Texture.HumveeBot, Content.Texture.HumveeTop,
							new Color(255, 230, 180), AItype.ForcedOffensiveAI,
							WeaponSet.MachineGunOnly, Map.GetSquareCenter(new Point(A, B)), 0,
							new Vector2(4, 4), 100, new Point(45, 45), new Point(1, 1),
							new Point(45, 45), new Point(1, 1)
							)
					);
				}
			}
		}
		private void InitializeHUD()
		{
			Direction = new Direction
					(this, Content.Texture.Direction, Player, Player.RotationAngle,
					new Point(64, 64), new Point(8, 5));

			Cursor = new Cursor
				(this, Content.Texture.Cursor, Color.Red,
				new Point(48, 48), new Point(1, 1));

			Minimap = new Minimap(this, Map.DrawToTexture(TankWars.SpriteBatch, TankWars.Graphics));

			HealthBar = new Bar
				(this, Content.Texture.BarSheet,
				new Rectangle(0, 0, 200, 42), new Rectangle(36, 85, 200, 42),
				new Vector2(10, 5), new Color(135, 200, 35, 200));

			EnergyBar = new Bar
				(this, Content.Texture.BarSheet,
				new Rectangle(0, 43, 200, 42), new Rectangle(36, 85, 200, 42),
				new Vector2(10, 47), new Color(30, 170, 255, 200));

			PowerIconList.Add
				(
				new PowerIcon
					(this, Content.Texture.PowerSheet, new Rectangle(0, 0, 64, 64),
					Power.Armor, "Power Armor", new Point(64, 64), new Point(1, 1))
				);
			PowerIconList.Add
				(
				new PowerIcon
					(this, Content.Texture.PowerSheet, new Rectangle(64, 0, 64, 64),
					Power.Speed, "God Speed", new Point(64, 64), new Point(1, 1))
				);
			PowerIconList.Add
				(
				new PowerIcon
					(this, Content.Texture.PowerSheet, new Rectangle(128, 0, 64, 64),
					Power.Ghost, "Ghost Walk", new Point(64, 64), new Point(1, 1))
				);
			PowerIconList.Add
				(
				new PowerIcon
					(this, Content.Texture.PowerSheet, new Rectangle(192, 0, 64, 64),
					Power.Invisible, "Invisible Cloak", new Point(64, 64), new Point(1, 1))
				);
			PowerIconList.Add
				(
				new PowerIcon
					(this, Content.Texture.PowerSheet, new Rectangle(256, 0, 64, 64),
					Power.Laser, "Laser Weapon", new Point(64, 64), new Point(1, 1))
				);
		}

		private void InitializeAIMap()
		{
			Enemy.Mmap = Map.MapWidth;
			Enemy.Nmap = Map.MapHeight;
			Enemy.AImap = new double[Enemy.Nmap, Enemy.Mmap];
			for (int y = 0; y < Map.MapHeight; y++)
				for (int x = 0; x < Map.MapWidth; x++)
				{
					if (Map.IsWallTile(x, y))
						Enemy.AImap[y, x] = -1;
					else if (Map.IsWaterTile(x, y))
						Enemy.AImap[y, x] = 2;
					else Enemy.AImap[y, x] = 1;
				}
			Enemy.preProcessMap();
		}

		private void InitializeMap()
		{
			Camera = new Camera(this);
			String mapToLoad = null;
			if (CurrentGameMode == GameMode.Campaign)
			{
				switch (CurrentMission)
				{
					case 1:
						mapToLoad = "Level1_TheLovelySideWalks.map";
						CurrentMissionName = "The Lovely SideWalks";
						MaximumMovableEnemies = 3;
						break;
					case 2:
						mapToLoad = "Level2_EscapeToEternity.map";
						CurrentMissionName = "Escape To Eternity";
						MaximumMovableEnemies = 3;
						break;
					case 3:
						mapToLoad = "Level3_MetrosTunnel.map";
						CurrentMissionName = "Metro's Tunnel";
						MaximumMovableEnemies = 3;
						break;
					case 4:
						mapToLoad = "Level4_TheHandsomeDevil.map";
						CurrentMissionName = "The Handsome Devil";
						MaximumMovableEnemies = 4;
						break;
					case 5:
						mapToLoad = "Level5_TheWeirdyDesert.map";
						CurrentMissionName = "The Weirdy Desert";
						MaximumMovableEnemies = 4;
						break;
					case 6:
						mapToLoad = "Level6_SixLanesToTheMoon.map";
						CurrentMissionName = "Six Lanes To The Moon";
						MaximumMovableEnemies = 4;
						break;
					case 7:
						mapToLoad = "Level7_SnowyWarfare.map";
						CurrentMissionName = "Snowy Warfare";
						MaximumMovableEnemies = 5;
						break;
					case 8:
						mapToLoad = "Level8_TheLastStand.map";
						CurrentMissionName = "The LastStand";
						MaximumMovableEnemies = 5;
						break;
				}
			}
			else
			{
				mapToLoad = null;
				CurrentMissionName = "Laser Torture";
				MaximumMovableEnemies = 1 + CurrentMission * 4;
			}

			if (mapToLoad != null)
			{
				// Kh?i t?o map m?i t? File xác d?nh
				Map = new Map(this, mapToLoad);
			}
			else
			{
				// Kh?i t?o map m?i dùng thu?t toán Random
				Map = new Map(this, 60, 60, 5, 20, 0);
			}
		}

		private void InitializeCamera()
		{
			Camera.WholeWorldRectangle = new Rectangle(0, 0,
						Map.MapWidth * Map.TileWidth,
						Map.MapHeight * Map.TileHeight);

			Camera.ViewportWidth = TankWars.Graphics.PreferredBackBufferWidth;
			Camera.ViewportHeight = TankWars.Graphics.PreferredBackBufferHeight;

			Camera.CurrentWorldPosition = new Vector2
				(Player.CurrentWorldPosition.X - Camera.ViewportWidth / 2,
				Player.CurrentWorldPosition.Y - Camera.ViewportHeight / 2);
		}
		#endregion
	}
}