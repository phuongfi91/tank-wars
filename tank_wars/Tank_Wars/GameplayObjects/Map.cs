using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	/// <summary>
	/// Class TileMap giữ vai trò khởi tạo Map trong màn chơi. Đồng thời 
	/// cung cấp thông tin của Map cho lập trình viên.
	/// </summary>
	public class Map
	{
		#region Declarations
		// Ma trận Map. Đây là một mảng 2 chiều, với mỗi ô chứa một ma trận
		// con, ma trận con này sẽ chứa thông tin địa hình và các đối tượng
		// đang chiếm giữ không gian của ô.
		public int[,][,] MapSquares;

		// Chiều dài và chiều rộng của Map. Tuy nhiên map không nhất thiết
		// có hình chữ nhật nằm ngang, nó có thể là hình chữ nhật dọc hoặc
		// hình vuông, do đó Width và Height ở đây chỉ mang tính chất tham
		// khảo. Hoặc có thể hiểu với một nghĩa khác: Width tượng trưng cho
		// trục hoành, Height tượng trưng cho trục tung.
		public int MapWidth;
		public int MapHeight;

		// Kích thước ngang dọc của một Tile trong TileSheet.
		public int TileWidth = 64;
		public int TileHeight = 64;

		// Xác định xem trong TileSheet, theo thứ tự từ trái sang phải, từ
		// trên xuống dưới, những Tile nào là Wall, Ground hoặc Water.
		public int WallTileStart = 0;
		public int WallTileEnd = 11;
		public int GroundTileStart = 12;
		public int GroundTileEnd = 71;
		public int WaterTileStart = 72;
		public int WaterTileEnd = 87;

		// Biến random phục vụ việc Random Map và Môi trường.
		private Random random = new Random();

		// Kích thước của TileSheet. (bao nhiêu cột, bao nhiêu dòng)
		private Point sheetSize = new Point(11, 2);

		// Texture TileSheet.
		private Texture2D tileSheet;

		// Danh sách chứa các hình chữ nhật bao quanh mỗi tile trong texture
		// TileSheet. Hình chữ nhật này sẽ phục vụ cho việc lấy sourceRectangle
		// trong hàm vẽ.
		private List<Rectangle> tileList;

		// Danh sách các địa điểm Player có thể Spawn trong màn chơi.
		private List<Point> spawningLocationList;

		// Địa điểm Player sẽ Spawn.
		private Vector2 spawningLocation;

		// Góc quay ban đầu của Player sau khi Spawn.
		private float spawningRotation;



		private GameplayScreen GameplayScreen { get; set; }
		#endregion

		#region Constructor
		public Map(GameplayScreen gameplayScreen, String mapName)
		{
			this.GameplayScreen = gameplayScreen;
			Initialize(mapName);
		}

		public Map(GameplayScreen gameplayScreen, int mapWidth, int mapHeight,
			int wallPercentage, int groundTileID, int wallTileID)
		{
			this.GameplayScreen = gameplayScreen;
			Initialize(mapWidth, mapHeight, wallPercentage, groundTileID, wallTileID);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Initialize màn chơi dựa theo một map có sẵn trên đĩa.
		/// </summary>
		/// <param name="mapName">Đường dẫn đến map hoặc tên map.</param>
		public void Initialize(String mapName)
		{
			tileSheet = GameplayScreen.Content.Texture.TileSheet;

			// Khởi tạo danh sách các hiệu ứng môi trường, danh sách này cũng
			// đồng thời phục vụ việc lưu giữ các hiệu ứng cháy nổ trong màn
			// chơi.
			//GameplayScreen.EffectList = new List<Sprite>();

			// Khởi tạo danh sách chứa các Wall. (phục vụ việc xử lý va chạm)
			//GameplayScreen.WallList = new List<Wall>();

			// Khởi tạo danh sách các địa điểm Player có thể Spawn.
			spawningLocationList = new List<Point>();

			// Khởi tạo Tile List.
			InitializeTileList();

			// Đọc dữ liệu map từ File map được truyền vào.
			string[] tempString = new string[2];

			// Đọc kích thước map từ file.
			StreamReader streamReader = new StreamReader(mapName);
			tempString = streamReader.ReadLine().Split(' ');
			MapHeight = Int32.Parse(tempString[0]);
			MapWidth = Int32.Parse(tempString[1]);

			// Khởi tạo map với kích thước trên.
			MapSquares = new int[MapWidth, MapHeight][,];

			// Đọc dữ liệu địa hình từ file vào map.
			string currentLine;
			for (int y = 0; y < MapHeight; y++)
			{
				currentLine = streamReader.ReadLine();
				tempString = currentLine.Split(' ');

				for (int x = 0; x < MapWidth; x++)
				{
					// Tạo mảng con bên trong ô để phục vụ lưu giữ địa hình, đối tượng.
					MapSquares[x, y] = new int[4, 2];

					MapSquares[x, y][0, 0] = Int32.Parse(tempString[x]);

					// Nếu địa hình là Wall thì add vào WallList để phục vụ
					// xử lý va chạm sau này.
					if (IsWallTile(x, y))
					{
						MapSquares[x, y][0, 1] = GameplayScreen.WallList.Count;
						GameplayScreen.WallList.Add
							(
								new Wall
								(
								this.GameplayScreen, tileSheet, tileList[GetTileAtSquare(x, y) / 4],
								new Vector2
									(x * TileWidth + TileWidth / 2,
									y * TileHeight + TileHeight / 2),
								(GetTileAtSquare(x, y) % 4) * MathHelper.PiOver2,
								new Point(TileWidth, TileHeight), new Point(11, 2)
								)
							);
					}

					// Nếu đây là địa điểm Spawn của Player thì add vào List.
					if (IsPlayerSpawnTile(x, y))
					{
						spawningLocationList.Add(new Point(x, y));
					}
				}
			}

			// Random môi trường trong map (hiệu ứng khói, bụi,...)
			GenerateRandomEnvironment();
		}

		/// <summary>
		/// Initialize màn chơi một cách Random dựa trên một số thông số
		/// có sẵn.
		/// </summary>
		/// <param name="mapWidth">Kích thước mapWidth.</param>
		/// <param name="mapHeight">Kích thước mapHeight.</param>
		/// <param name="wallPercentage">Xác suất xuất hiện Wall.</param>
		/// <param name="groundTile">ID của địa hình ground trong TileSheet
		/// </param>
		/// <param name="wallTile">ID của địa hình wall trong TileSheet</param>
		public void Initialize(int mapWidth, int mapHeight, 
			int wallPercentage, int groundTileID, int wallTileID)
		{
			tileSheet = GameplayScreen.Content.Texture.TileSheet;

			// Khởi tạo danh sách các hiệu ứng môi trường, danh sách này cũng
			// đồng thời phục vụ việc lưu giữ các hiệu ứng cháy nổ trong màn
			// chơi.
			//GameplayScreen.EffectList = new List<Sprite>();

			// Khởi tạo danh sách chứa các Wall. (phục vụ việc xử lý va chạm)
			//GameplayScreen.WallList = new List<Wall>();

			// Khởi tạo danh sách các địa điểm Player có thể Spawn.
			spawningLocationList = new List<Point>();

			// Khởi tạo Tile List.
			InitializeTileList();

			MapWidth = mapWidth;
			MapHeight = mapHeight;

			// Khởi tạo map với kích thước trên.
			MapSquares = new int[MapWidth, MapHeight][,];

			// Tiến hành Random map.
			for (int x = 0; x < MapWidth; x++)
				for (int y = 0; y < MapHeight; y++)
				{
					// Tạo mảng con bên trong ô để phục vụ lưu giữ địa hình, đối tượng.
					MapSquares[x, y] = new int[4, 2];

					// Mặc định, ô chứa địa hình Ground.
					MapSquares[x, y][0, 0] = groundTileID;

					// Nếu ô nằm ở rìa map, ô đó sẽ là Wall.
					if ((x == 0) || (y == 0) ||
						(x == MapWidth - 1) || (y == MapHeight - 1))
					{
						MapSquares[x, y][0, 0] = wallTileID;
						MapSquares[x, y][0, 1] = GameplayScreen.WallList.Count;

						// Add vào WallList để phục vụ xử lý va chạm sau này.
						GameplayScreen.WallList.Add
						(
							new Wall
							(
							this.GameplayScreen, tileSheet, tileList[GetTileAtSquare(x, y) / 4],
							new Vector2
								(x * TileWidth + TileWidth / 2,
								y * TileHeight + TileHeight / 2),
							(GetTileAtSquare(x, y) % 4) * MathHelper.PiOver2,
							new Point(TileWidth, TileHeight), new Point(11, 2)
							)
						);
						continue;
					}

					// Nếu ô nằm ở kế rìa map thì sẽ chắc chắn là Ground.
					if ((x == 1) || (y == 1) ||
						(x == MapWidth - 2) || (y == MapHeight - 2))
					{
						continue;
					}

					// Còn lại các ô nằm bên trong sẽ có xác suất Wall là
					// wallPercentage.
					if (random.Next(0, 100) <= wallPercentage)
					{
						MapSquares[x, y][0, 0] = wallTileID;
						MapSquares[x, y][0, 1] = GameplayScreen.WallList.Count;

						// Add vào WallList để phục vụ xử lý va chạm sau này.
						GameplayScreen.WallList.Add
						(
							new Wall
							(
							this.GameplayScreen, tileSheet, tileList[GetTileAtSquare(x, y) / 4],
							new Vector2
								(x * TileWidth + TileWidth / 2,
								y * TileHeight + TileHeight / 2),
							(GetTileAtSquare(x, y) % 4) * MathHelper.PiOver2,
							new Point(TileWidth, TileWidth), new Point(11, 2)
							)
						);
					}
				}

			// Lấy vị trí (1, 1) trên map cho vào danh sách các địa điểm
			// Spawn của Player.
			spawningLocationList.Add(new Point(1, 1));

			// Random môi trường trong map (hiệu ứng khói, bụi,...)
			GenerateRandomEnvironment();
		}

		// Lấy tọa độ ô trên map theo tọa độ Pixel. (trục hoành)
		public int GetSquareByPixelX(int pixelX)
		{
			return pixelX / TileWidth;
		}

		// Lấy tọa độ ô trên map theo tọa độ Pixel. (trục tung)
		public int GetSquareByPixelY(int pixelY)
		{
			return pixelY / TileHeight;
		}

		/************************************************************************/
		/*                            Tương tự                                  */
		/************************************************************************/
		public Point GetSquareAtPixel(Vector2 pixelLocation)
		{
			return new Point(
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		public Vector2 GetSquareCenter(int squareX, int squareY)
		{
			return new Vector2(
				(squareX * TileWidth) + (TileWidth / 2),
				(squareY * TileHeight) + (TileHeight / 2));
		}

		public Vector2 GetSquareCenter(Point square)
		{
			return GetSquareCenter(square.X, square.Y);
		}

		public Rectangle SquareWorldRectangle(int x, int y)
		{
			return new Rectangle(
				x * TileWidth + TileWidth / 2,
				y * TileHeight + TileWidth / 2,
				TileWidth,
				TileHeight);
		}

		public Rectangle SquareWorldRectangle(Point square)
		{
			return SquareWorldRectangle(square.X, square.Y);
		}

		public Rectangle SquareScreenRectangle(int x, int y)
		{
			return GameplayScreen.Camera.Transform(SquareWorldRectangle(x, y));
		}

		public Rectangle SquareScreenRectangle(Point square)
		{
			return SquareScreenRectangle((int)square.X, (int)square.Y);
		}

		public int GetTileAtSquare(int tileX, int tileY)
		{
			if ((tileX >= 0) && (tileX < MapWidth) &&
				(tileY >= 0) && (tileY < MapHeight))
			{
				return MapSquares[tileX, tileY][0, 0];
			}
			else
			{
				return -1;
			}
		}

		public void SetTileAtSquare(int tileX, int tileY, int tile)
		{
			if ((tileX >= 0) && (tileX < MapWidth) &&
				(tileY >= 0) && (tileY < MapHeight))
			{
				MapSquares[tileX, tileY][0, 0] = tile;
			}
		}

		public int GetTileAtPixel(int pixelX, int pixelY)
		{
			return GetTileAtSquare(
				GetSquareByPixelX(pixelX),
				GetSquareByPixelY(pixelY));
		}

		public int GetTileAtPixel(Vector2 pixelLocation)
		{
			return GetTileAtPixel(
				(int)pixelLocation.X,
				(int)pixelLocation.Y);
		}

		public bool IsWallTile(int tileX, int tileY)
		{
			int tileIndex = GetTileAtSquare(tileX, tileY);

			if (tileIndex == -1)
			{
				return false;
			}

			return WallTileStart <= tileIndex && tileIndex <= WallTileEnd;
		}

		public bool IsWallTile(Point square)
		{
			return IsWallTile((int)square.X, (int)square.Y);
		}

		public bool IsWallTileByPixel(Vector2 pixelLocation)
		{
			return IsWallTile(
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		public bool IsGroundTile(int tileX, int tileY)
		{
			int tileIndex = GetTileAtSquare(tileX, tileY);

			if (tileIndex == -1)
			{
				return false;
			}

			return GroundTileStart <= tileIndex && tileIndex <= GroundTileEnd;
		}

		public bool IsGroundTile(Point square)
		{
			return IsGroundTile((int)square.X, (int)square.Y);
		}

		public bool IsGroundTileByPixel(Vector2 pixelLocation)
		{
			return IsGroundTile(
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		public bool IsWaterTile(int tileX, int tileY)
		{
			int tileIndex = GetTileAtSquare(tileX, tileY);

			if (tileIndex == -1)
			{
				return false;
			}

			return WaterTileStart <= tileIndex && tileIndex <= WaterTileEnd;
		}

		public bool IsWaterTile(Point square)
		{
			return IsWaterTile((int)square.X, (int)square.Y);
		}

		public bool IsWaterTileByPixel(Vector2 pixelLocation)
		{
			return IsWaterTile(
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		public bool IsRoadTile(int tileX, int tileY)
		{
			int tileIndex = GetTileAtSquare(tileX, tileY);

			if (tileIndex == -1)
			{
				return false;
			}

			return 52 <= tileIndex && tileIndex <= 55;
		}

		public bool IsRoadTile(Point square)
		{
			return IsRoadTile((int)square.X, (int)square.Y);
		}

		public bool IsRoadTileByPixel(Vector2 pixelLocation)
		{
			return IsRoadTile(
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		public bool IsPlayerSpawnTile(int tileX, int tileY)
		{
			int tileIndex = GetTileAtSquare(tileX, tileY);

			if (tileIndex == -1)
			{
				return false;
			}

			return 64 <= tileIndex && tileIndex <= 67;
		}

		public bool IsPlayerSpawnTile(Point square)
		{
			return IsPlayerSpawnTile(square.X, square.Y);
		}

		public bool IsPlayerSpawnTileByPixel(Vector2 pixelLocation)
		{
			return IsPlayerSpawnTile(
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		public bool IsEliteSpawnTile(int tileX, int tileY)
		{
			int tileIndex = GetTileAtSquare(tileX, tileY);

			if (tileIndex == -1)
			{
				return false;
			}

			return 12 <= tileIndex && tileIndex <= 15;
		}

		public bool IsEliteSpawnTile(Point square)
		{
			return IsEliteSpawnTile((int)square.X, (int)square.Y);
		}

		public bool IsEliteSpawnTileByPixel(Vector2 pixelLocation)
		{
			return IsEliteSpawnTile(
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		public bool IsOccupiedByTank(int tileX, int tileY)
		{
			int tileIndex = GetTileAtSquare(tileX, tileY);

			if (tileIndex == -1)
			{
				return false;
			}

			for (int i = 1; i < 4; i++)
			{
				if (MapSquares[tileX, tileY][i, 0] != 0)
				{
					return true;
				}
			}

			return false;
		}

		public bool IsOccupiedByTank(Point square)
		{
			return IsOccupiedByTank((int)square.X, (int)square.Y);
		}

		public bool IsOccupiedByTankByPixel(Vector2 pixelLocation)
		{
			return IsOccupiedByTank( 
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		public bool IsOccupiedByEnemy(int tileX, int tileY)
		{
			int tileIndex = GetTileAtSquare(tileX, tileY);

			if (tileIndex == -1)
			{
				return false;
			}

			for (int i = 1; i < 4; i++)
			{
				if (MapSquares[tileX, tileY][i, 0] == 89)
				{
					return true;
				}
			}

			return false;
		}

		public bool IsOccupiedByEnemy(Point square)
		{
			return IsOccupiedByEnemy((int)square.X, (int)square.Y);
		}

		public bool IsOccupiedByEnemyByPixel(Vector2 pixelLocation)
		{
			return IsOccupiedByEnemy(
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		public bool IsOccupiedByPlayer(int tileX, int tileY)
		{
			int tileIndex = GetTileAtSquare(tileX, tileY);

			if (tileIndex == -1)
			{
				return false;
			}

			for (int i = 1; i < 4; i++)
			{
				if (MapSquares[tileX, tileY][i, 0] == 88)
				{
					return true;
				}
			}

			return false;
		}

		public bool IsOccupiedByPlayer(Point square)
		{
			return IsOccupiedByPlayer((int)square.X, (int)square.Y);
		}

		public bool IsOccupiedByPlayerByPixel(Vector2 pixelLocation)
		{
			return IsOccupiedByPlayer(
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		public bool IsAvailable(int tileX, int tileY)
		{
			int tileIndex = GetTileAtSquare(tileX, tileY);

			if (tileIndex == -1)
			{
				return false;
			}

			for (int i = 1; i < 4; i++ )
			{
				if (MapSquares[tileX, tileY][i, 0] == 0)
				{
					return true;
				}
			}

			return false;
		}

		public bool IsAvailable(Point square)
		{
			return IsAvailable(square.X, square.Y);
		}

		public bool IsAvailableByPixel(Vector2 pixelLocation)
		{
			return IsAvailable(
				GetSquareByPixelX((int)pixelLocation.X),
				GetSquareByPixelY((int)pixelLocation.Y));
		}

		

		public Vector2 GetSpawningLocation()
		{
			if (spawningLocationList.Count != 0)
			{
				int index = random.Next(0, spawningLocationList.Count - 1);
				spawningLocation = GetSquareCenter(spawningLocationList[index]);
			}
			else
			{
				spawningLocation = new Vector2(97, 97);
			}
			return spawningLocation;
		}

		public float GetSpawningRotation()
		{
			if (spawningLocationList.Count != 0)
			{
				spawningRotation =
					GetTileAtPixel(spawningLocation) % 4 * MathHelper.PiOver2;
			}
			else
				spawningRotation = 0;
			return spawningRotation;
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			Camera camera = GameplayScreen.Camera;
			int startX =
				GetSquareByPixelX((int)camera.CurrentWorldPosition.X);
			int endX =
				GetSquareByPixelX((int)camera.CurrentWorldPosition.X + camera.ViewportWidth);

			int startY =
				GetSquareByPixelY((int)camera.CurrentWorldPosition.Y);
			int endY =
				GetSquareByPixelY((int)camera.CurrentWorldPosition.Y + camera.ViewportHeight);

			for (int x = startX; x <= endX; x++)
				for (int y = startY; y <= endY; y++)
				{
					Rectangle currentSquare = SquareScreenRectangle(x, y);
					if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight))
					{
						if (IsWallTile(x, y))
						{
							spriteBatch.Draw
								(
								tileSheet, camera.Transform(new Vector2
									(x * TileWidth + TileWidth / 2,
									y * TileHeight + TileHeight / 2)),
								tileList[GetTileAtSquare(x, y) / 4], 
								new Color(220, 220, 220, 255),
								(GetTileAtSquare(x, y) % 4) * MathHelper.PiOver2,
								new Vector2(TileWidth / 2, TileHeight / 2), 1,
								SpriteEffects.None, 0.4f
								);
							spriteBatch.Draw
								(
								tileSheet, camera.Transform(new Vector2
									(x * TileWidth + 2,
									y * TileHeight + 2)),
								tileList[GetTileAtSquare(x, y) / 4],
								new Color(0, 0, 0, 200), 0, Vector2.Zero, 1,
								SpriteEffects.None, 0.35f
								);
							/*spriteBatch.Draw
								(
								tileSheet, new Rectangle
									(currentSquare.X,
									currentSquare.Y,
									currentSquare.Width,
									currentSquare.Height),
								tileList[GetTileAtSquare(x, y) / 4], 
								new Color(220, 220, 220, 255),
								(GetTileAtSquare(x, y) % 4) * MathHelper.PiOver2,
								new Vector2(TileWidth / 2, TileHeight / 2),
								SpriteEffects.None, 0.4f
								);
							spriteBatch.Draw
								(
								tileSheet, new Rectangle
									(currentSquare.X - TileWidth / 2 + 2,
									currentSquare.Y - TileHeight / 2 + 2,
									currentSquare.Width,
									currentSquare.Height),
								tileList[GetTileAtSquare(x, y) / 4],
								new Color(0, 0, 0, 200), 0, Vector2.Zero,
								SpriteEffects.None, 0.35f
								);*/
						}
						else
						{
							spriteBatch.Draw
								(
								tileSheet, camera.Transform(new Vector2
									(x * TileWidth + TileWidth / 2,
									y * TileHeight + TileHeight / 2)),
								tileList[GetTileAtSquare(x, y) / 4], 
								new Color(220, 220, 220, 255),
								(GetTileAtSquare(x, y) % 4) * MathHelper.PiOver2,
								new Vector2(TileWidth / 2, TileHeight / 2), 1,
								SpriteEffects.None, 0.0f
								);
							/*spriteBatch.Draw
								(
								tileSheet, new Rectangle
									(SquareScreenRectangle(x, y).X,
									SquareScreenRectangle(x, y).Y,
									SquareScreenRectangle(x, y).Width,
									SquareScreenRectangle(x, y).Height),
								tileList[GetTileAtSquare(x, y) / 4], 
								new Color(220, 220, 220, 255),
								(GetTileAtSquare(x, y) % 4) * MathHelper.PiOver2,
								new Vector2(TileWidth / 2, TileHeight / 2),
								SpriteEffects.None, 0.0f
								);*/
						}
					}
				}
		}

		public Texture2D DrawToTexture(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
		{
			RenderTarget2D renderTarget = new RenderTarget2D
				(graphics.GraphicsDevice, MapWidth * 4, MapHeight * 4);
				graphics.GraphicsDevice.SetRenderTarget(renderTarget);

			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

			for (int x = 0; x < MapWidth; x++)
				for (int y = 0; y < MapHeight; y++)
				{
					Rectangle currentSquare = SquareWorldRectangle(x, y);
					if (IsWallTile(x, y))
					{
						spriteBatch.Draw
							(
							tileSheet, new Rectangle
								(currentSquare.X / 16,
								currentSquare.Y / 16,
								currentSquare.Width / 16,
								currentSquare.Height / 16),
							tileList[GetTileAtSquare(x, y) / 4], Color.White,
							(GetTileAtSquare(x, y) % 4) * MathHelper.PiOver2,
							new Vector2(TileWidth / 2, TileHeight / 2),
							SpriteEffects.None, 0.4f
							);
					}
					else
					{
						spriteBatch.Draw
							(
							tileSheet, new Rectangle
								(currentSquare.X / 16,
								currentSquare.Y / 16,
								currentSquare.Width / 16,
								currentSquare.Height / 16),
							tileList[GetTileAtSquare(x, y) / 4], Color.White,
							(GetTileAtSquare(x, y) % 4) * MathHelper.PiOver2,
							new Vector2(TileWidth / 2, TileHeight / 2),
							SpriteEffects.None, 0.0f
							);
					}
				}

			spriteBatch.End();

			graphics.GraphicsDevice.SetRenderTarget(null);

			Texture2D miniMap = new Texture2D
				(graphics.GraphicsDevice, MapWidth * 4, MapHeight * 4);
			Color[] renderTargetData = new Color[MapWidth * 4 * MapHeight * 4];
			renderTarget.GetData(renderTargetData);
			miniMap.SetData(renderTargetData);

			return miniMap;
		}

		private void InitializeTileList()
		{
			// Khởi tạo danh sách quản lý hình chữ nhật của các Tile.
			tileList = new List<Rectangle>();

			Point currentTile = new Point(0, 0);
			while (currentTile.Y < sheetSize.Y)
			{
				tileList.Add
					(
					new Rectangle
						(currentTile.X * TileWidth,
						currentTile.Y * TileHeight,
						TileWidth, TileHeight)
					);
				currentTile.X++;
				if (currentTile.X >= sheetSize.X)
				{
					currentTile.X = 0;
					currentTile.Y++;
				}
			}
		}

		private void GenerateRandomEnvironment()
		{
			GameplayScreen.EffectList = new List<Sprite>();
			for (int y = 0; y < MapHeight; y++)
				for (int x = 0; x < MapWidth; x++)
					if (IsWallTile(x, y))
					{
						if (random.Next(0, 100) <= 10)
						{
							GameplayScreen.EffectList.Add
							(
								new EnvironmentFlame
								(
								this.GameplayScreen, GameplayScreen.Content.Texture.BigFlameSheet,
								new Vector2
									(x * TileWidth + TileWidth / 2,
									y * TileHeight),
								new Point(128, 128), new Point(6, 5), 60
								)
							);
							GameplayScreen.EffectList.Add
							(
								new EnvironmentDirt
								(
								this.GameplayScreen, GameplayScreen.Content.Texture.Dirt,
								new Vector2
									(x * TileWidth + TileWidth / 2,
									y * TileHeight + TileHeight / 2),
								0.51f,
								new Point(64, 64), new Point(1, 1)
								)
							);
						}
					}
					else if (GetTileAtSquare(x, y) < 72)
					{
						if (random.Next(0, 100) <= 20)
						{
							GameplayScreen.EffectList.Add
							(
								new EnvironmentFlame
								(
								this.GameplayScreen, GameplayScreen.Content.Texture.SmallFlameSheet,
								new Vector2
									(x * TileWidth + TileWidth / 2,
									y * TileHeight),
								new Point(128, 128), new Point(10, 6)
								)
							);
							GameplayScreen.EffectList.Add
							(
								new EnvironmentDirt
								(
								this.GameplayScreen, GameplayScreen.Content.Texture.Dirt,
								new Vector2
									(x * TileWidth + TileWidth / 2,
									y * TileHeight + TileHeight / 2),
								0.1f,
								new Point(64, 64), new Point(1, 1)
								)
							);
						}
					}
		}
		#endregion
	}
}