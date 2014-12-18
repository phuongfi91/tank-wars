using System;
using System.IO;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public enum AItype
	{
		NoAI, StationaryAI,
		ImbaDummyAI, DummyAI,
		OffensiveAI, DefensiveAI, RacerAI, AntiRacerAI,
		ForcedOffensiveAI
	}

	public enum WeaponSet
	{
		StandardSet, LaserSet, 
		CannonOnly, PowerLaserOnly, 
		MachineGunOnly, GatlingLaserOnly
	}

	public class Enemy : Tank
	{
		#region Declarations
		// Need: AIRacer, AI Antiracer
		//TextWriter fout = new StreamWriter("AIlog.txt");
		const double offensiveRange = 30;
		const double defensiveRange = 10;
		public static double[,] AImap = null;
		public static double[, , ,] AIpathdis = null;
		public static Point[, , ,] AIpathnext = null;
		public static int Nmap, Mmap;
		public static int[] dx = { 0, 1, 0, -1 };
		public static int[] dy = { 1, 0, -1, 0 };
		public AItype AItypeID = AItype.OffensiveAI; // need constructor (Who is it ?)
		private Point AIcommingposition; //need constructor (Where is it at the beginning ?)
		private double AIrotate = 0.0; // need constructor (which direction it is at in the beginning ?)
		private int AIfireatwill = 0; // no need: always = 0 (not need to fire)
		//adder 
		private WeaponSet weaponSet;
		#endregion

		#region Constructors
		/// <summary>
		/// Nếu ko truyền vào tham số millisecondsPerFrame thì sử dụng 
		/// defaultMillisecondsPerFrame = 16 (mặc định)
		/// </summary>
		public Enemy
			(
			GameplayScreen gameplayScreen, Texture2D botTexture, Texture2D topTexture, Color color, AItype AItypeID,
			WeaponSet weaponSet, Vector2 position, float rotationAngle, Vector2 maximumSpeed,
			int hitPoints, Point frameSize, Point sheetSize, Point frameSizeTop,
			Point sheetSizeTop
			)
			: this
			(
			gameplayScreen, botTexture, topTexture, color, AItypeID, weaponSet, position, rotationAngle, maximumSpeed,
			hitPoints, frameSize, sheetSize, frameSizeTop, sheetSizeTop, 
			defaultMillisecondsPerFrame
			)
		{
			
		}

		/// <summary>
		/// Ngược lại, sử dụng millisecondsPerFrame từ Constructor.
		/// </summary>
		public Enemy
			(
			GameplayScreen gameplayScreen, Texture2D botTexture, Texture2D topTexture, Color color, AItype AItypeID,
			WeaponSet weaponSet, Vector2 position, float rotationAngle, Vector2 maximumSpeed,
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
			this.AItypeID = AItypeID;
			this.typeID = 89;
			this.weaponSet = weaponSet;
			this.typeIndex = GameplayScreen.EnemyList.Count;

			AIrotate = RotationAngle;
			AIcommingposition = CurrentMapPosition;

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
			base.Update(gameTime);

			// Nếu người chơi không tàng hình thì tiến hành suy nghĩ và
			// hành động.
			if (GameplayScreen.Player.PowerState != Power.Invisible)
			{
				Think(gameTime);
				Action(gameTime);
			}

			this.typeIndex = GameplayScreen.EnemyList.IndexOf(this);

			if (!IsAlive)
			{
				GameplayScreen.EnemyList.Remove(this);
				GameplayScreen.CurrentListItem--;
				Map.MapSquares[CurrentMapPosition.X, CurrentMapPosition.Y]
					[CurrentMapPositionIndex, 0] =
				Map.MapSquares[CurrentMapPosition.X, CurrentMapPosition.Y]
					[CurrentMapPositionIndex, 1] = 0;
				foreach (Enemy enemy in GameplayScreen.EnemyList)
				{
					enemy.TypeIndex = GameplayScreen.EnemyList.IndexOf(enemy);
					enemy.UpdateMapPosition();
				}
			}

			/************************************************************************/
			/*                        Dành cho người viết AI                        */
			/************************************************************************/
			// Sơ lược cấu trúc Game:
			// MainGame với 1 Module chính:
			// - ScreenManager: Quản lý tất cả các màn hình trong game từ
			// Menu cho đến Gameplay.
			// 
			// Trong số các Screen, GameplayScreen đóng vai trò cốt lõi trong
			// game, chứa các thiết lập, load texture và gọi lệnh Update, Draw 
			// các đối tượng. GameplayScreen sẽ tự động random N bots vào
			// màn chơi, số lượng tùy ý có thể chỉnh trong GameplayScreen
			// -> Update() -> RandomTestDummies(N).
			// 
			// Các đối tượng trong Gameplay nằm trong GameplayObjects. Tất cả
			// đều kế thừa từ class Sprite, và đc phân loại, tùy biến dựa 
			// theo mục đích.
			// 
			// Tạm thời chỉ tập trung vào Class Enemy (Kế thừa từ Tank), 
			// và tất cả các code AI nên viết trong hàm Action (đc gọi bởi
			// CollisionCheck) và CollisionCheck lại đc gọi bởi Update.
			// Vì game sẽ loop qua hàm Update này 60 lần / giây nên các hàm
			// con đc gọi bởi Update cũng sẽ loop với số lần tương tự.
			// Chi tiết ví dụ nên xem trong hàm CollisionCheck() và Action().

			// Những thông tin có thể khai thác từ player để tối ưu AI
			// Player player = GameplayScreen.Player;
			/* player.IsAlive;
			 * player.CurrentMapPosition;
			 * player.PreviousMapPosition
			 * player.CurrentWorldPosition;
			 * player.CurrentScreenPosition;
			 * player.RotationAngle;
			 * player.HitPoints;
			 * player.EnergyPoints;
			 * player.EnergyIsSteady;
			 */

			// Những thông tin nên dùng để tùy biến hành vi của AI
			/* this.movingSpeed;
			 * this.maximumSpeed
			 * this.RotationAngle;
			 * this.RotationAngleTop;
			 * this.CurrentMapPosition;
			 * this.PreviousMapPosition
			 * this.CurrentWorldPosition;
			 * this.CurrentScreenPosition;
			 * this.HitPoints;
			 * this.PowerState
			 * this.EnergyPoints;
			 * this.EnergyIsSteady;
			 */

			// Để khai hỏa, sử dụng hàm FireMainGun() hoặc FireMachineGun().
			// 
			// Có thể tìm hiểu thêm các thông tin trên thông qua Comment
			// trong file Player.cs, Tank.cs (có thể bắt chước từ Player) 
			// và Sprite.cs.
			// 
			// -> Tạm thời ko cần hiểu thêm về các lớp hay đối tượng khác.

			// Những thông tin có thể sử dụng từ TileMap để tìm đường đi
			/* TileMap.MapSquares: Ma trận Map, đc đánh số y như Map đc tạo
			 * ra từ MapEditor. Hiện tại, Map này có kích thước Width*Height,
			 * với mỗi ô chứa thêm 1 ma trận con 4x2, chứa 4 thông tin:
			 *					ID						Index
			 * + Địa hình: ID của định hình		Nếu là Wall thì nhận Index
			 *									trong GameplayScreen.WallList
			 *									
			 * + Đối tượng: Tank của Player			Index trống
			 * 
			 * + Đối tượng: Tank 1 của Enemy		Nhận Index trong 
			 *									GameplayScreen.EnemyList
			 *									
			 * + Đối tượng: Tank 2 của Enemy		Tương tự, dùng để dự
			 *									phòng trường hợp có 3 Tank
			 *									trong cùng 1 ô (1 Player, 2 AI)
			 *									
			 * -> Do 1 ô có kích thước 64x64 và tương tự đối với Tank, trong
			 * trường hợp tương đối xấu sẽ có 2 Enemy trong cùng 1 ô, ngoài
			 * ra Player có khả năng sử dụng Ghost Power nên sẽ dẫn đến trường
			 * hợp có đến 3 Tank trong cùng một ô, do vậy tạm thời sử dụng
			 * ma trận 4x2 cho mỗi ô. Định dạng: TileMap.MapSquares[x,y][a,b].
			 * 
			 * -> Hiện tại chỉ dùng 1 ô cho Enemy, ô cuối cùng chỉ để dự phòng.
			 * 
			 * const TileMap.WallTileStart: từ 0 (3 loại Tile đầu tiên, 12 kiểu rotation)
			 * const TileMap.WallTileEnd: đến 11 là Wall Tile, ko để đi xuyên qua
			 * const TileMap.GroundTileStart: từ 12
			 * const TileMap.GroundTileEnd: đến 71 là Ground Tile, có thể đi xuyên qua
			 * const int WaterTileStart: = từ 72 
			 * const int WaterTileEnd: = đến 87 là Water Tile, có thể đi qua, nhưng chậm.
			 * 
			 * TileMap.GetSquareAtPixel: Get ô ở Pixel xác định. Trả về vị trí
			 * trong ma trận MapSquares.
			 * 
			 * TileMap.GetTileAtPixel: Get loại Tile từ Pixel xác định. Trả về
			 * kiểu Tile từ 0 -> 87 (ứng với 22 loại Tile)
			 * TileMap.GetTileAtSquare: Get loại Tile từ Square xác định
			 * (Trong ma trận MapSquares). Trả về tương tự như trên.
			 * 
			 * TileMap.IsWallTile (2 Overloads): Trả về có phải Wall Tile hay
			 * ko, Input là tọa độ Square trong ma trận MapSquares.
			 * TileMap.IsWallTileByPixel: Trả về có phải Wall Tile hay
			 * ko, Input là Pixel.
			 * 
			 * TileMap.WallList: chứa tất cả các Wall có trong màn chơi, chủ
			 * yếu dùng để xử lý va chạm. Có thể bỏ qua và sử dụng MapSquares
			 * hoặc IsWallTile để check Wall thay vì WallList.
			 * 
			 * Và một số thông tin khác: TileMap.IsWaterTile; TileMap.IsOccupiedByTank;
			 * TileMap.IsAvailable; TileMap.IsGroundTile;...
			 */

			// Thông tin có thể sử dụng từ Camera để hỗ trợ AI
			/* Camera.ObjectIsVisible(this.BoundingWorldRectangle): Kiểm tra
			 * xem this Enemy có hiện hữu trong Viewport của người chơi hay ko
			 * (nói cách khác: người chơi có nhìn thấy this Enemy hay ko)
			 */

			// Ngoài ra có thể sử dụng thêm các hàm trong Math và MathHelper
			// để trợ giúp tính toán
			//////////////////////////////////////////////////////////////////////////
		}

		private void Think(GameTime gameTime)
		{
			switch (AItypeID)
			{
				case AItype.OffensiveAI: AIOffensive(); break;
				case AItype.DefensiveAI: AIDefensive(); break;
				case AItype.ForcedOffensiveAI: AIOffensive(); break;
				case AItype.RacerAI: AIRacer(); break;
				case AItype.AntiRacerAI: AIAntiRacer(); break;
				case AItype.StationaryAI: AIStationary(); break;
				default: break;
			}
		}

		private void Action(GameTime gameTime)
		{
			//AI will load the action from its memory and act like that
			switch (AItypeID)
			{
				case AItype.ImbaDummyAI: AIImbaDummy(); break;
				case AItype.DummyAI: AIDummy(); break;
				case AItype.NoAI: break;
				default:
					MoveTo(AIcommingposition);
					RotationAngleTop = Rotate(this.RotationAngleTop, (float)AIrotate, 0.05f);
					AIFireAtWill(gameTime);
					break;
			}

			/* Khi 1 Tank đc khởi tạo, nó đc truyền vào maximumSpeed, tuy
			 * nhiên, maximumSpeed chỉ thực sự có ý nghĩa đối với player vì
			 * nó sẽ đc dùng để làm ngưỡng của tốc độ chuyển động movingSpeed.
			 * Đối với Bots do AI điều khiển, lập trình viên ko cần quan tâm
			 * đến maximumSpeed mà chỉ sử dụng movingSpeed. Kết hợp với
			 * direction (nên hiểu direction ở đây là một offset của position)
			 * và rotationAngle để thực hiện cơ chế chuyển động tới lui, xoay
			 * chuyển 360 độ.
			 * 
			 * 
			 * + movingSpeed là 1 Vector2 chứa 2 giá trị float X và Y. Với 
			 * giá trị .X là tốc độ đi tới và .Y là tốc độ đi lùi.
			 * 
			 * + Sau khi cho movingSpeed một giá trị nào đó, tiến hành tính
			 * toán direction như bên dưới để ra được offset chuyển động,
			 * cộng offset này với position hiện tại của Tank sẽ ra được vị
			 * trí position mới.
			 */
			/* AI demo
			// Trên constructor đã cho Tank movingSpeed = new Vector2(1, 0); 
			// Hay tốc độ đi tới = 1. Sau đó ta sẽ tiến hành tính toán offset
			// theo movingSpeed này.

			// Nên giữ nguyên tính toán này, ko cần bận tâm thay đổi công thức
			// tính toán direction (hay offset)
			direction.X = (float)(movingSpeed.X * Math.Cos(rotationAngle) - movingSpeed.Y * Math.Cos(rotationAngle));
			direction.Y = (float)(movingSpeed.X * Math.Sin(rotationAngle) - movingSpeed.Y * Math.Sin(rotationAngle));

			// Thay đổi trạng thái vị trí, góc quay của Tank.
			// Do hàm đc gọi 60 lần/s, nên sau 1s:
			rotationAngleTop = RotateTo(rotationAngleTop, GameplayScreen.Player, 0.05f); // -> Top sẽ xoay -0.6 rads
			rotationAngle += 0.01f; // -> Bot sẽ xoay +0.6 rads
			position += direction;	// -> position sẽ cứ thay đổi dựa vào
			// sự tính toán của direction theo movingSpeed

			// Tiến hành khai hỏa. 
			// Repeat Rate của MainGun = 60 -> Loop 1s bắn chỉ dc 1 phát
			// Repeat Rate của MachineGun = 5 -> Loop 1s bắn đc 12 phát
			FireMachineGun();
			FireMainGun();
			 */
		}

		// AI sẽ move đến ô destinationSquare kế cận.
		private void MoveTo(Point destinationSquare)
		{
			if (destinationSquare.X <= 0 || destinationSquare.Y <= 0) return;

			//1234AI bug: if (!Map.IsOccupiedByTank(destinationSquare))
			if (!Map.IsOccupiedByTank(destinationSquare)
				||
				destinationSquare == CurrentMapPosition)
			{
				Vector2 destination = Map.GetSquareCenter
					(destinationSquare.X, destinationSquare.Y);

				float newRotationAngle = (float)GetAngle
					(this.CurrentWorldPosition.X, this.CurrentWorldPosition.Y, 
					destination.X, destination.Y);

				RotationAngle = Rotate(RotationAngle, newRotationAngle, 0.05f);

				MovingSpeed += new Vector2(0.5f, 0.0f);

				CurrentWorldPosition += MovingOffset;
			}
		}
		
		#endregion
		#region more
		public static void preProcessMap()
		{
			int i, j;
			Point p;
			int nx, ny;
			double traveltime;
			AIpathdis = new double[Nmap, Mmap, Nmap, Mmap];
			AIpathnext = new Point[Nmap, Mmap, Nmap, Mmap];
			Queue AIqueue = new Queue();
			for (i = 0; i < Nmap; ++i)
				for (j = 0; j < Mmap; ++j)
				{
					{
						if (i == 18 && j == 5)
						{
							++i;
							--i;
						}
					}
					if (AImap[i, j] != -1.0)
					/*
					 * if ==
						{
							for (ii = 0; ii < Nmap; ++ii)
								for (jj = 0; jj < Mmap; ++jj)
								{
									AIpathdis[i, j, ii, jj] = 0;
									AIpathnext[i, j, ii, jj].x = 0;
									AIpathnext[i, j, ii, jj].y = 0;
								}
						}
						else
					 **/
					{
						AIqueue.Enqueue(new Point(i, j));
						while (AIqueue.Count != 0)
						{
							p = (Point)AIqueue.Dequeue();
							for (int k = 0; k < 4; ++k)
							{
								nx = p.X + dx[k];
								ny = p.Y + dy[k];
								if (nx >= 0 && nx < Nmap && ny >= 0 && ny < Mmap && AImap[nx, ny] != -1.0)
								{
									traveltime = AIpathdis[i, j, p.X, p.Y] + AImap[p.X, p.Y] / 2 + AImap[nx, ny] / 2;
									if (traveltime < AIpathdis[i, j, nx, ny] || AIpathdis[i, j, nx, ny] == 0)
									{
										AIpathdis[i, j, nx, ny] = traveltime;
										AIpathnext[i, j, nx, ny].X = p.X;
										AIpathnext[i, j, nx, ny].Y = p.Y;
										AIqueue.Enqueue(new Point(nx, ny));
									}
								}
							}
						}
					}
				}
		}

		public void AIFireAtWill(GameTime gameTime)
		{
			if (AIfireatwill == 1)
			{
				if (weaponSet == WeaponSet.StandardSet 
					||
					weaponSet == WeaponSet.CannonOnly)
				{
					FireCannon(0);
				}
				else if 
					(weaponSet == WeaponSet.LaserSet 
					|| 
					weaponSet == WeaponSet.PowerLaserOnly)
				{
					FirePowerLaser(0);
				}

				float secondaryGunLatency = ((float)Math.Sin
					(gameTime.TotalGameTime.TotalSeconds * 3));
				if (secondaryGunLatency > 0.5)
				{
					if (weaponSet == WeaponSet.StandardSet 
						|| 
						weaponSet == WeaponSet.MachineGunOnly)
					{
						FireMachineGun(0);
					}
					else if 
						(weaponSet == WeaponSet.LaserSet 
						||
						weaponSet == WeaponSet.GatlingLaserOnly)
					{
						FireGatlingLaser(0);
					}
				}
			}
		}

		public int AIshootcheck(double x, double y)
		{
			return AIshootcheck(this.CurrentWorldPosition.X, this.CurrentWorldPosition.Y, x, y);
		}

		public int AIshootcheck(double fromX, double fromY, double toX, double toY)
		{
			// Lấy góc địa điểm hiện tại của tank so với tọa độ (x, y).
			double angle = GetAngle(fromX, fromY, toX, toY);

			if (angle.Equals(double.NaN))
			{
				return 2;
			}

			// Độ chính xác trong khi check.
			const int checkingAccuracy = 64;

			Point checkingSquare = Point.Zero;
			Vector2 offset = Vector2.Zero;
			int i = 1;

			int Allyflag = 0, Enemyflag = 0, Wallflag = 0;

			// Check cho đến khi ô đang check trùng với ô đích.
			while 
				(checkingSquare != Map.GetSquareAtPixel(new Vector2((float)toX, (float)toY))
				&&
				checkingSquare.X >= 0 && checkingSquare.Y >= 0
				&&
				checkingSquare.X < Map.MapWidth && checkingSquare.Y < Map.MapHeight)
			{
				// Tính tọa độ của ô sắp check
				offset.X = i * checkingAccuracy * (float)Math.Cos(angle);
				offset.Y = i * checkingAccuracy * (float)Math.Sin(angle);
				i++;
				checkingSquare = Map.GetSquareAtPixel(this.CurrentWorldPosition + offset);

				// Đọc dữ liệu của ô.
				// [0,0] chứa thông tin địa hình.
				// [1,0] chứa sự tồn tại của người chơi.
				// [2,0] và [3,0] chứa sự tồn tại của Enemy. ([3,0] để dự
				// phòng trường hợp xấu nhất có 2 Enemy Tanks trong cùng
				// một ô)
				if (Map.IsWallTile(checkingSquare))
				{
					// Return sự hiện diện của người chơi, tránh trường hợp
					// người chơi lợi dụng đứng trong tường và bắn ra.
					if (Map.IsOccupiedByPlayer(checkingSquare))
					{
						Enemyflag = 1;
						break;
					}
					Wallflag = 1;
					break;
				}

				if (Map.IsOccupiedByEnemy(checkingSquare))
				{
					Allyflag = 1;
				}

				if (Map.IsOccupiedByPlayer(checkingSquare))
				{
					Enemyflag = 1;
					break;
				}
			}
			if (Wallflag == 1) return 1;
			else if (Enemyflag == 1)
			{
				if (Allyflag == 1) return 3; else return 2;
			}
			return 0;
		}

		public void AIOffensive()
		{
			Player player = GameplayScreen.Player;
			switch (AIshootcheck(player.CurrentWorldPosition.X, player.CurrentWorldPosition.Y))
			{
				case 0: AIfireatwill = 0; break;
				case 1:
					AIfireatwill = 0;
					//if ((((int)this.currentMapPosition.X) != AIcommingposition.X) || (((int)this.currentMapPosition.Y) != AIcommingposition.Y)) break;
					if (Math.Sqrt(Math.Pow((this.CurrentWorldPosition.X - player.CurrentWorldPosition.X), 2.0) + Math.Pow((this.CurrentWorldPosition.Y - player.CurrentWorldPosition.Y), 2.0)) < offensiveRange * 64 || AItypeID == AItype.ForcedOffensiveAI)
					{
						if (AIpathdis[(int)player.CurrentMapPosition.Y, (int)player.CurrentMapPosition.X, (int)this.currentMapPosition.Y, (int)this.currentMapPosition.X] > 0)
						{
							AIcommingposition.X = AIpathnext[(int)player.CurrentMapPosition.Y, (int)player.CurrentMapPosition.X, (int)this.currentMapPosition.Y, (int)this.currentMapPosition.X].Y;
							AIcommingposition.Y = AIpathnext[(int)player.CurrentMapPosition.Y, (int)player.CurrentMapPosition.X, (int)this.currentMapPosition.Y, (int)this.currentMapPosition.X].X;
						}
					}
					break;
				case 2:
					AIrotate = GetAngle(this.CurrentWorldPosition.X, this.CurrentWorldPosition.Y, player.CurrentWorldPosition.X, player.CurrentWorldPosition.Y);
					AIfireatwill = 1;
					break;
				case 3:
					AIfireatwill = 0;
					for (int i = -2; i <= 2; ++i)
						for (int j = -2; j <= 2; ++j)
						{
							if (AIshootcheck(this.CurrentWorldPosition.X + i * 64, this.CurrentWorldPosition.Y + j * 64, player.CurrentWorldPosition.X, player.CurrentWorldPosition.Y) == 1)
								if (AIshootcheck(this.CurrentWorldPosition.X, this.CurrentWorldPosition.Y, (int)this.CurrentWorldPosition.X + i * 64, (int)this.CurrentWorldPosition.Y + j * 64) == 0)
								{
									AIcommingposition.X = (int)this.currentMapPosition.X + i;
									AIcommingposition.Y = (int)this.currentMapPosition.Y + j;
									i = 2;
									j = 2;
								}
						}
					break;
				default: throw (new Exception("Unexpected case of shoot check"));
			}
		}

		public void AIDefensive()
		{
			Player player = GameplayScreen.Player;
			switch (AIshootcheck(player.CurrentWorldPosition.X, player.CurrentWorldPosition.Y))
			{
				case 0: //the player is dead
					AIfireatwill = 0;
					break;
				case 1:
					AIfireatwill = 0;
					if (Math.Sqrt(Math.Pow((this.CurrentWorldPosition.X - player.CurrentWorldPosition.X), 2.0) + Math.Pow((this.CurrentWorldPosition.Y - player.CurrentWorldPosition.Y), 2.0)) < defensiveRange * 64)
					{
						//if ((((int)this.currentMapPosition.X) == AIcommingposition.X) || (((int)this.currentMapPosition.Y) == AIcommingposition.Y))
						{
							if (AIpathdis[(int)player.CurrentMapPosition.Y, (int)player.CurrentMapPosition.X, (int)this.currentMapPosition.Y, (int)this.currentMapPosition.X] > 0)
							{
								AIcommingposition.X = AIpathnext[(int)player.CurrentMapPosition.Y, (int)player.CurrentMapPosition.X, (int)this.currentMapPosition.Y, (int)this.currentMapPosition.X].Y;
								AIcommingposition.Y = AIpathnext[(int)player.CurrentMapPosition.Y, (int)player.CurrentMapPosition.X, (int)this.currentMapPosition.Y, (int)this.currentMapPosition.X].X;
							}
						}
					}
					int nx = (int)player.CurrentMapPosition.X, ny = (int)player.CurrentMapPosition.Y, tx, ty;
					for (int i = 0; i < 2; ++i)
					{
						AIfireatwill = 0;
						if (AIpathdis[(int)this.currentMapPosition.Y, (int)this.currentMapPosition.X, ny, nx] == 0) break;
						tx = AIpathnext[(int)this.currentMapPosition.Y, (int)this.currentMapPosition.X, ny, nx].Y;
						ty = AIpathnext[(int)this.currentMapPosition.Y, (int)this.currentMapPosition.X, ny, nx].X;
						nx = tx; ny = ty;
						if (AIshootcheck(nx * 64, ny * 64) == 0 || AIshootcheck(nx * 64 + 16, ny * 64 + 16) == 0 || AIshootcheck(nx * 64 + 32, ny * 64 + 32) == 0 || AIshootcheck(nx * 64 + 48, ny * 64 + 48) == 0 || AIshootcheck(nx * 64 + 63, ny * 64 + 63) == 0)
						{
							AIrotate = GetAngle(this.CurrentWorldPosition.X, this.CurrentWorldPosition.Y, player.CurrentWorldPosition.X, player.CurrentWorldPosition.Y);
							AIfireatwill = 1;
							break;
						}
					}
					break;
				case 2:
					AIrotate = GetAngle(this.CurrentWorldPosition.X, this.CurrentWorldPosition.Y, player.CurrentWorldPosition.X, player.CurrentWorldPosition.Y);
					AIfireatwill = 1;
					break;
				case 3:
					AIfireatwill = 0;
					for (int i = -2; i <= 2; ++i)
						for (int j = -2; j <= 2; ++j)
						{
							if (AIshootcheck(this.CurrentWorldPosition.X + i * 64, this.CurrentWorldPosition.Y + j * 64, player.CurrentWorldPosition.X, player.CurrentWorldPosition.Y) == 1)
								if (AIshootcheck(this.CurrentWorldPosition.X, this.CurrentWorldPosition.Y, (int)this.CurrentWorldPosition.X + i * 64, (int)this.CurrentWorldPosition.Y + j * 64) == 0)
								{
									AIcommingposition.X = (int)this.currentMapPosition.X + i;
									AIcommingposition.Y = (int)this.currentMapPosition.Y + j;
									i = 2;
									j = 2;
								}
						}
					break;
				default: throw (new Exception("Unexpected case of shoot check"));
			}
		}

		public void AIStationary()
		{
			Player player = GameplayScreen.Player;
			switch (AIshootcheck(player.CurrentWorldPosition.X, player.CurrentWorldPosition.Y))
			{
				case 2:
					AIrotate = GetAngle(this.CurrentWorldPosition.X, this.CurrentWorldPosition.Y, player.CurrentWorldPosition.X, player.CurrentWorldPosition.Y);
					AIfireatwill = 1;
					break;
				default:
					AIfireatwill = 0;
					break;
			}
		}

		public void AIRacer()
		{
			//this AI will race
		}

		public void AIAntiRacer()
		{
			//this AI will hold the race
		}

		public void AIImbaDummy()
		{
			RotationAngleTop = Rotate(RotationAngleTop, GameplayScreen.Player, 0.05f);
			RotationAngle += 0.01f;
			MovingSpeed += new Vector2(0.5f, 0.0f);
			CurrentWorldPosition += MovingOffset;
			FireGatlingLaser(0);
			FirePowerLaser(0);
		}

		public void AIDummy()
		{
			RotationAngleTop -= 0.01f;
			RotationAngle += 0.01f;
			MovingSpeed += new Vector2(0.5f,0.0f);
			CurrentWorldPosition += MovingOffset;
			FireMachineGun(0);
			FireCannon(0);
		}
		#endregion
	}
}