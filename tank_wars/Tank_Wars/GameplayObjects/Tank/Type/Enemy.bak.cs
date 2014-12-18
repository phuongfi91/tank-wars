using System;
using System.Collections;  // adder
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	public class Enemy : Tank
	{
        //adder
        public static double[,] AImap = null;
        public static double[, , ,] AIpathdis = null;
        public static Point[, , ,] AIpathnext = null;
        public static int Nmap,Mmap;
        public static int[] dx = {0,1,0,-1};
        public static int[] dy = {1,0,-1,0};
        public int AItype = 0; // need constructor
        private Point AIcommingposition; //need constructor
        private double AIrotate = 0.0; // need constructor
        //adder
		#region Constructors
		// Nếu ko truyền vào tham số millisecondsPerFrame
		// Sử dụng defaultMillisecondsPerFrame = 16 (mặc định)
		public Enemy
			(
			Texture2D botTexture, Texture2D topTexture, Color color,
			Vector2 position, float rotationAngle, Vector2 maximumSpeed,
			int hitPoints, Point frameSize, Point sheetSize
			)
			: this
			(
			botTexture, topTexture, color, position, rotationAngle, maximumSpeed,
			hitPoints, frameSize, sheetSize, defaultMillisecondsPerFrame
			)
		{

		}

		// Ngược lại, sử dụng custom millisecondsPerFrame
		public Enemy
			(
			Texture2D botTexture, Texture2D topTexture, Color color,
			Vector2 position, float rotationAngle, Vector2 maximumSpeed,
			int hitPoints, Point frameSize, Point sheetSize,
			int millisecondsPerFrame
			)
			: base
			(
			botTexture, topTexture, color, position, rotationAngle, maximumSpeed,
			hitPoints, frameSize, sheetSize, millisecondsPerFrame
			)
		{
			// Test, cho Tank một movingSpeed xác định, rồi tính toán direction
			// dựa vào movingSpeed này
			movingSpeed = new Vector2(1, 0); // Tốc độ đi tới = 1

			currentMapPosition = TileMap.GetSquareAtPixel(position);
			TileMap.MapSquares[(int)currentMapPosition.X, (int)currentMapPosition.Y][2, 0] = 89;
			TileMap.MapSquares[(int)currentMapPosition.X, (int)currentMapPosition.Y][2, 1] = GameplayScreen.EnemyList.Count;
			currentMapPositionIndex = 2;
		} 
		#endregion

		#region Methods
		public override void Update(GameTime gameTime)
		{
			// Cập nhật vị trí trên bản đồ
			UpdateMapPosition();

			// Check va chạm
			CollisionCheck(gameTime);

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




			// Ví dụ trỏ vào player
			Player player = GameplayScreen.Player;

			// Những thông tin có thể khai thác từ player để tối ưu AI
			/* player.IsAlive;
			 * player.CurrentMapPosition;
			 * player.Position_World;
			 * player.Position_Screen;
			 * player.RotationAngle;
			 * player.Direction;
			 * player.CurrentHitPoints;
			 * player.CurrentEnergyShieldPoints;
			 * player.EnergyShieldIsSteady;
			 */


			// Những thông tin nên dùng để tùy biến hành vi của AI
			/* this.direction;
			 * this.movingSpeed;
			 * this.rotationAngle;
			 * this.rotationAngleTop;
			 * this.position;
			 * this.currentMapPosition;
			 * this.hitPoints;
			 * this.origin;
			 */

			// Để khai hỏa, sử dụng hàm FireMainGun() hoặc FireMachineGun().
			// Ví dụ đã có bên dưới.
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
			 * const TileMap.FloorTileStart: từ 12 (19 loại Tile còn lại)
			 * const TileMap.FloorTileEnd: đến 87 là Floor Tile, có thể đi xuyên qua
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
			 */




			// Thông tin có thể sử dụng từ Camera để hỗ trợ AI
			/* Camera.ObjectIsVisible(this.BoundingRectangle_World): Kiểm tra
			 * xem this Enemy có hiện hữu trong Viewport của người chơi hay ko
			 * (nói cách khác: người chơi có nhìn thấy this Enemy hay ko)
			 */



			// Ngoài ra có thể sử dụng thêm các hàm trong Math và MathHelper
			// để trợ giúp tính toán



			if (hitPoints <= 0)
			{
				GameplayScreen.EnemyList.Remove(this);
				GameplayScreen.CurrentListItem--;
			}
			
			base.Update(gameTime);
		}

		protected override void UpdateMapPosition()
		{
			currentMapPosition = TileMap.GetSquareAtPixel(position);
			/*if (TileMap.MapSquares[(int)currentMapPosition.X, (int)currentMapPosition.Y][2, 0] == 0)
			{*/
				TileMap.MapSquares[(int)currentMapPosition.X, (int)currentMapPosition.Y][2, 0] = 89;
				TileMap.MapSquares[(int)currentMapPosition.X, (int)currentMapPosition.Y][2, 1] = GameplayScreen.EnemyList.IndexOf((Enemy)this);
				currentMapPositionIndex = 2;
			/*}
			else
			{
				TileMap.MapSquares[(int)currentMapPosition.X, (int)currentMapPosition.Y][3, 0] = 89;
				TileMap.MapSquares[(int)currentMapPosition.X, (int)currentMapPosition.Y][3, 1] = GameplayScreen.EnemyList.IndexOf((Enemy)this);
				currentMapPositionIndex = 3;
			}*/

			if (previousMapPosition != currentMapPosition)
			{
				TileMap.MapSquares[(int)previousMapPosition.X, (int)previousMapPosition.Y][previousMapPositionIndex, 0] = 
				TileMap.MapSquares[(int)previousMapPosition.X, (int)previousMapPosition.Y][previousMapPositionIndex, 1] = 0;
				previousMapPosition = currentMapPosition;
				previousMapPositionIndex = currentMapPositionIndex;
			}
		}

		private void CollisionCheck(GameTime gameTime)
		{
			// Tạm thời lưu lại trạng thái góc quay và vị trí của AI, nếu 
			// trạng thái, hành vi mới đc thiết lập bởi hàm Action gây ra 
			// va chạm thì trả lại trạng thái này.
			float tempRotationAngle = rotationAngle;
			Vector2 tempPosition = position;

			// AI tiến hành vài hành động thay đổi vị trí, góc quay
			Action(gameTime);

			// Sau đó, check vị trí, góc quay mới. Nếu vị trí, góc quay mới
			// gây ra va chạm với tường hay các Tanks khác thì đưa ra xử lý
			// thích hợp. Cách xử lý hiện tại là khá đơn giản: Trả về trạng
			// thái chưa có va chạm và tự động đổi chiều di chuyển từ đi tới
			// sang đi lùi.

			// Tiến hành check va chạm với tất cả các ô xung quanh
			for (int i = -1; i < 2; i++)
				for (int j = -1; j < 2; j++)
					for (int k = 0; k < 4; k++)
					{
						// Không check với những ô ko nằm trong bản đồ
						if ((int)currentMapPosition.X + i < 0
							||
							(int)currentMapPosition.Y + j < 0
							||
							(int)currentMapPosition.X + i >= TileMap.MapWidth
							||
							(int)currentMapPosition.Y + j >= TileMap.MapHeight)
						{
							break;
						}

						// Lấy loại đối tượng (Là địa hình hay Tank, Wall,...)
						int tempType =
							TileMap.MapSquares[(int)currentMapPosition.X + i,
							(int)currentMapPosition.Y + j][k, 0];

						// Lấy Index của đối tượng trong mảng chứa nó
						int tempIndex =
							TileMap.MapSquares[(int)currentMapPosition.X + i,
							(int)currentMapPosition.Y + j][k, 1];

						// Va chạm với Wall
						if (tempType >= 0 && tempType <= TileMap.WallTileEnd)
						{
							Wall wall = GameplayScreen.WallList[tempIndex];
							// Nếu va chạm
							if (this.CollideWith(wall))
							{
								// Trả lại trạng thái trước đó
								position = tempPosition;
								rotationAngle = tempRotationAngle;

								// Đổi chiều di chuyển
								// Trong ví dụ này thì đổi movingSpeed từ (1, 0)
								// là đi tới sang (0, 1) là đi lùi
								float t = movingSpeed.X;
								movingSpeed.X = movingSpeed.Y;
								movingSpeed.Y = t;
							}
						}

						// Va chạm với Enemy
						else if (tempType == 89)
						{
							// Do Tank bị hủy diệt trong quá trình chơi,
							// đôi khi gây mất đồng bộ trong quá trình
							// update, dẫn đến tempIndex >= EnemyList.Count.
							// Khi đó ta sẽ không check va chạm.
							// Trong trường hợp this is Enemy, thì ko check 
							// với bản thân.
							if (tempIndex >= GameplayScreen.EnemyList.Count
								||
								this == GameplayScreen.EnemyList[tempIndex])
							{
								break;
							}

							Tank enemy = GameplayScreen.EnemyList[tempIndex];
							// Nếu va chạm
							if (this.CollideWith(enemy))
							{
								// Trả lại trạng thái trước đó
								position = tempPosition;
								rotationAngle = tempRotationAngle;

								// Đổi chiều di chuyển
								// Trong ví dụ này thì đổi movingSpeed từ (1, 0)
								// là đi tới sang (0, 1) là đi lùi
								float t = movingSpeed.X;
								movingSpeed.X = movingSpeed.Y;
								movingSpeed.Y = t;
							}
						}

						// Va chạm với Player
						else if (tempType == 88)
						{
							// Nếu va chạm
							if (this.CollideWith(GameplayScreen.Player))
							{
								// Trả lại trạng thái trước đó
								position = tempPosition;
								rotationAngle = tempRotationAngle;

								// Đổi chiều di chuyển
								// Trong ví dụ này thì đổi movingSpeed từ (1, 0)
								// là đi tới sang (0, 1) là đi lùi
								float t = movingSpeed.X;
								movingSpeed.X = movingSpeed.Y;
								movingSpeed.Y = t;
							}
						}
					}
		}

		private void Action(GameTime gameTime)
		{
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

			// Trên constructor đã cho Tank movingSpeed = new Vector2(1, 0); 
			// Hay tốc độ đi tới = 1. Sau đó ta sẽ tiến hành tính toán offset
			// theo movingSpeed này.

			// Nên giữ nguyên tính toán này, ko cần bận tâm thay đổi công thức
			// tính toán direction (hay offset)
			direction.X = (float)
				(
				movingSpeed.X * Math.Cos(rotationAngle)
				-
				movingSpeed.Y * Math.Cos(rotationAngle)
				);
			direction.Y = (float)
				(
				movingSpeed.X * Math.Sin(rotationAngle)
				-
				movingSpeed.Y * Math.Sin(rotationAngle)
				);

			// Thay đổi trạng thái vị trí, góc quay của Tank.
			// Do hàm đc gọi 60 lần/s, nên sau 1s:
			rotationAngleTop -= 0.01f; // -> Top sẽ xoay -0.6 rads
			rotationAngle += 0.01f; // -> Bot sẽ xoay +0.6 rads
			position += direction;	// -> position sẽ cứ thay đổi dựa vào
									// sự tính toán của direction theo movingSpeed

			// Tiến hành khai hỏa. 
			// Repeat Rate của MainGun = 60 -> Loop 1s bắn chỉ dc 1 phát
			// Repeat Rate của MachineGun = 5 -> Loop 1s bắn đc 12 phát
			FireMachineGun();
			FireMainGun();
		}
        public static void preProcessMap() //adder
        {
            int i,j,ii,jj;
            Point p;
            int nx,ny;
            double traveltime;
            AIpathdis = new double[Nmap, Mmap, Nmap, Mmap];
            AIpathnext = new Point[Nmap, Mmap, Nmap, Mmap];
            Queue AIqueue = new Queue();
            for (i = 0; i < Nmap; ++i)
                for (j = 0; j < Mmap; ++j)
                {
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
                            for (int k = 0;k < 3;++k)
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
        public void AIAction()
        {
            //AI will load the action from its memory and act like that

            //the point of it's comming is AIcommingposition
            //the point of 
        }
        public int AIshootcheck(double x, double y)
        {
            return AIshootcheck(this.currentMapPosition.X, this.currentMapPosition.Y, x, y);
        }
        public int AIshootcheck(double fromX, double fromY, double toX, double toY)
        {
            // Lấy góc địa điểm hiện tại của tank so với tọa độ (x, y).
            float angle;
            float temp = (float)Math.Sqrt
                ((toX - this.position.X) * (toX - this.position.X)
                +
                (toY - this.position.Y) * (toY - this.position.Y));
            temp = MathHelper.Clamp((float)(toX - this.position.X) / temp, -1.0f, 1.0f);
            if (toY > this.position.Y)
                angle = (float)Math.Acos(temp);
            else
                angle = -(float)Math.Acos(temp);

            // Độ chính xác trong khi check.
            const int checkingAccuracy = 64;

            Vector2 checkingSquare = Vector2.Zero;
            Vector2 offset = Vector2.Zero;
            int i = 1;

            int Allyflag = 0,Enemyflag = 0,Wallflag = 0;

            // Check cho đến khi ô đang check trùng với ô đích.
            while (checkingSquare != TileMap.GetSquareAtPixel(new Vector2((float)toX, (float)toY)))
            {
                // Tính tọa độ của ô sắp check
                offset.X = i * checkingAccuracy * (float)Math.Cos(angle);
                offset.Y = i * checkingAccuracy * (float)Math.Sin(angle);
                i++;
                checkingSquare = TileMap.GetSquareAtPixel(this.position + offset);

                // Đọc dữ liệu của ô.
                // [0,0] chứa thông tin địa hình.
                // [1,0] chứa sự tồn tại của người chơi.
                // [2,0] và [3,0] chứa sự tồn tại của Enemy. ([3,0] để dự
                // phòng trường hợp xấu nhất có 2 Enemy Tanks trong cùng
                // một ô)
                if (TileMap.MapSquares[(int)checkingSquare.X, (int)checkingSquare.Y][0, 0] <= TileMap.WallTileEnd)
                {
                    // Return sự hiện diện của người chơi, tránh trường hợp
                    // người chơi lợi dụng đứng trong tường và bắn ra.
                    if (TileMap.MapSquares[(int)checkingSquare.X, (int)checkingSquare.Y][1, 0] != 0)
                    {
                        Enemyflag = 1;
                        break;
                    }
                    Wallflag = 1;
                    break;
                }

                if (TileMap.MapSquares[(int)checkingSquare.X, (int)checkingSquare.Y][2, 0] != 0)
                {
                    Allyflag = 1;
                }

                if (TileMap.MapSquares[(int)checkingSquare.X, (int)checkingSquare.Y][1, 0] != 0)
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
            throw new Exception("A bug happened: No wall No enemy ?");
        }
        public double AIAngle(double px1, double py1, double px2, double py2)
        {
            // Negate X and Y values
            double rx = px2 - px1;
            double ry = py2 - py1;
            double angle = 0.0;
            // Calculate the angle
            if (rx == 0.0)
            {
                if (ry == 0.0) angle = 0.0;
                else if (ry > 0.0) angle = System.Math.PI / 2.0;
                else angle = System.Math.PI * 3.0 / 2.0;
            }
            else if (ry == 0.0)
            {
                if (rx > 0.0) angle = 0.0; else angle = System.Math.PI;
            }
            else
            {
                if (rx < 0.0) angle = System.Math.Atan(ry / rx) + System.Math.PI;
                else if (ry < 0.0) angle = System.Math.Atan(ry / rx) + (2 * System.Math.PI);
                else angle = System.Math.Atan(ry / rx);
            }
            return angle;
        }
        public int AIPath(int fx, int fy, int dx, int dy) //coding
        {
            int nx, ny;
            if (AIpathdis[dx, dy, fx, fy] == 0) return 0;
            nx = AIpathnext[dx, dy, fx, fy].X;
            ny = AIpathnext[dx, dy, fx, fy].Y;
            while (nx != dx || nx != dy)
            {
                //if any object in the way return 0;
                nx = AIpathnext[dx, dy, fx, fy].X;
                ny = AIpathnext[dx, dy, fx, fy].Y;
            }
            return 1;
        }
        public void AI1() //save mode
        {
            Player player = GameplayScreen.Player;
            switch (AIshootcheck(player.CurrentMapPosition.X, player.CurrentMapPosition.Y))
            {
                case 1:
                    if (Math.Sqrt(Math.Pow((this.currentMapPosition.X - player.CurrentMapPosition.X), 2.0) + Math.Pow((this.currentMapPosition.Y - player.CurrentMapPosition.Y), 2.0)) < 500 || AItype == 11)
                    {
                        if (AIpathdis[(int)(Math.Round(player.CurrentMapPosition.X)), (int)(Math.Round(player.CurrentMapPosition.Y)), (int)(Math.Round(this.currentMapPosition.X)), (int)(Math.Round(this.currentMapPosition.Y))] > 0)
                        {
                            AIcommingposition.X = AIpathnext[(int)(Math.Round(player.CurrentMapPosition.X)), (int)(Math.Round(player.CurrentMapPosition.Y)), (int)(Math.Round(this.currentMapPosition.X)), (int)(Math.Round(this.currentMapPosition.Y))].X;
                            AIcommingposition.Y = AIpathnext[(int)(Math.Round(player.CurrentMapPosition.X)), (int)(Math.Round(player.CurrentMapPosition.Y)), (int)(Math.Round(this.currentMapPosition.X)), (int)(Math.Round(this.currentMapPosition.Y))].Y;
                        }
                    }
                    break;
                case 2:
                    AIrotate = AIAngle(this.currentMapPosition.X, this.currentMapPosition.Y, player.CurrentMapPosition.X, player.CurrentMapPosition.Y);
                    FireMachineGun();
                    FireMainGun();
                    break;
                case 3:
                    for (int i = -2;i <= 2;++i)
                        for (int j = -2; j <= 2; ++j)
                        {
                            if (AIshootcheck(this.currentMapPosition.X + i,this.currentMapPosition.Y + j,player.CurrentMapPosition.X, player.CurrentMapPosition.Y) == 1)
                                if (AIPath((int)this.currentMapPosition.X, (int)this.currentMapPosition.Y, (int)this.currentMapPosition.X, (int)this.currentMapPosition.Y) == 1)
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
        public void AI2() //save mode
        {
            Player player = GameplayScreen.Player;
            switch (AIshootcheck(player.CurrentMapPosition.X, player.CurrentMapPosition.Y))
            {
                case 1:
                    if (Math.Sqrt(Math.Pow((this.currentMapPosition.X - player.CurrentMapPosition.X), 2.0) + Math.Pow((this.currentMapPosition.Y - player.CurrentMapPosition.Y), 2.0)) < 500)
                    {
                        int nx = (int) player.CurrentMapPosition.X, ny = (int) player.CurrentMapPosition.Y,tx,ty;
                        for (int i = 0; i < 5; ++i)
                        {
                            //save mode
                            if (AIpathdis[nx,ny,(int) (this.currentMapPosition.X),(int)(this.currentMapPosition.Y)] == 0) break;
                            tx = AIpathnext[nx, ny, (int)(this.currentMapPosition.X), (int)(this.currentMapPosition.Y)].X;
                            ty = AIpathnext[nx, ny, (int)(this.currentMapPosition.X), (int)(this.currentMapPosition.Y)].Y;
                            nx = tx; ny = ty;
                            if (AIshootcheck(nx,ny) == 3)
                            {
                                AIrotate = AIAngle(this.currentMapPosition.X, this.currentMapPosition.Y, player.CurrentMapPosition.X, player.CurrentMapPosition.Y);
                                FireMachineGun();
                                FireMainGun();
                                break;
                            }
                        }
                    }
                    break;
                case 2:
                    AIrotate = AIAngle(this.currentMapPosition.X, this.currentMapPosition.Y, player.CurrentMapPosition.X, player.CurrentMapPosition.Y);
                    FireMachineGun();
                    FireMainGun();
                    break;
                case 3:
                    for (int i = -2; i <= 2; ++i)
                        for (int j = -2; j <= 2; ++j)
                        {
                            
                            if (AIshootcheck(this.currentMapPosition.X + i, this.currentMapPosition.Y + j, player.CurrentMapPosition.X, player.CurrentMapPosition.Y) == 1)
                                if (AIPath((int)this.currentMapPosition.X , (int)this.currentMapPosition.Y, (int)this.currentMapPosition.X + i, (int)this.currentMapPosition.Y + j) == 1)
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
        public void AI3()
        {
            //this AI will race
        }
        public void AI4()
        {
            //this AI will hold the race
        }
		#endregion
	}
}
