using Microsoft.Xna.Framework;

namespace Tank_Wars
{
	/// <summary>
	/// Camera của game. Hiển thị một phần của màn chơi lên màn hình và di
	/// chuyển theo người chơi. Khung hiển thị của Camera (gọi là Viewport)
	/// là một hình chữ nhật.
	/// </summary>
	public class Camera
	{
		#region Declarations
		// Tọa độ của Camera trong màn chơi, tọa độ này gắn liền với góc trên
		// bên trái của Viewport.
		private Vector2 position;
		// Kích thước của Viewport. Với .X là chiều dài, .Y là chiều rộng.
		private Vector2 viewportSize;

		// Hình chữ nhật bao quanh cả màn chơi. Viewport phải luôn luôn nằm
		// bên trong hình chữ nhật này.
		private Rectangle worldRectangle = new Rectangle(0, 0, 0, 0);

		private GameplayScreen GameplayScreen { get; set; }
		#endregion

		#region Constructor
		public Camera(GameplayScreen gameplayScreen)
		{
			this.GameplayScreen = gameplayScreen;
		}

		public Camera(GameplayScreen gameplayScreen, Vector2 position, Rectangle worldRectangle)
		{
			this.GameplayScreen = gameplayScreen;
			this.position = position;
			this.viewportSize = Vector2.Zero;
			this.worldRectangle = worldRectangle;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Truy xuất và thay đổi vị trí của Viewport.
		/// </summary>
		public Vector2 CurrentWorldPosition
		{
			get { return position; }
			set
			{
				// Giữ Viewport nằm bên trong worldRectangle.
				position = new Vector2(
					MathHelper.Clamp(value.X,
						worldRectangle.X,
						worldRectangle.Width - ViewportWidth),
					MathHelper.Clamp(value.Y,
						worldRectangle.Y,
						worldRectangle.Height - ViewportHeight));
			}
		}

		/// <summary>
		/// Truy xuất và thay đổi hình chữ nhật WorldRectangle bao quanh cả
		/// màn chơi.
		/// </summary>
		public Rectangle WholeWorldRectangle
		{
			get { return worldRectangle; }
			set { worldRectangle = value; }
		}

		/// <summary>
		/// Truy xuất và thay đổi chiều dài của Viewport.
		/// </summary>
		public int ViewportWidth
		{
			get { return (int)viewportSize.X; }
			set { viewportSize.X = value; }
		}

		/// <summary>
		/// Truy xuất và thay đổi chiều rộng của Viewport.
		/// </summary>
		public int ViewportHeight
		{
			get { return (int)viewportSize.Y; }
			set { viewportSize.Y = value; }
		}

		/// <summary>
		/// Truy xuất Viewport.
		/// </summary>
		public Rectangle Viewport
		{
			get
			{
				return new Rectangle
					((int)CurrentWorldPosition.X, (int)CurrentWorldPosition.Y,
					ViewportWidth, ViewportHeight);
			}
		}
		#endregion

		#region Public Methods
		public void Update(GameTime gameTime)
		{
			this.ViewportWidth = GameplayScreen.TankWars.Graphics.PreferredBackBufferWidth;
			this.ViewportHeight = GameplayScreen.TankWars.Graphics.PreferredBackBufferHeight;
			//this.Move(GameplayScreen.Player.MovingOffset);
		}

		/// <summary>
		/// Di chuyển Viewport.
		/// </summary>
		/// <param name="offset">Di chuyển Viewport với một offset xác định.
		/// </param>
		public void Move(Vector2 movingOffset)
		{
			// Ví dụ: Tọa độ hiện tại Position(2, 3) += offset(5, 7) sẽ trả
			// về tọa độ mới Position(7, 10).
			CurrentWorldPosition += movingOffset;
		}

		/// <summary>
		/// Xác định xem một đối tượng nào đó có nằm trong tầm nhìn của
		/// Viewport hay không.
		/// </summary>
		/// <param name="boundingRectangle">Hình chữ nhật bao quanh đối
		/// tượng cần xét.</param>
		/// <returns></returns>
		public bool ObjectIsVisible(Rectangle boundingRectangle)
		{
			// Nếu hình chữ nhật Viewport giao với hình chữ nhật bao quanh
			// đối tượng thì trả về True hoặc ngược lại.
			return (Viewport.Intersects(boundingRectangle));
		}

		/// <summary>
		/// Thực hiện phép Transform (ở đây là biến đổi tọa độ) của một đối
		/// tượng bất kì về tọa độ hiển thị được trong Viewport.
		/// 
		/// *Lưu ý: Tại sao phải thực hiện việc biến đổi tọa độ này ? Bởi vì
		/// khi ta vẽ một đối tượng lên màn hình, ta sẽ truyền vào hàm vẽ
		/// tọa độ trên một Viewport khác của Graphics Device, Viewport này
		/// có gốc tọa độ (0, 0) ở góc trên bên trái màn hình và chiều dài,
		/// chiều rộng tương ứng với độ phân giải của game. Do đó, dù Viewport
		/// của Camera ở đâu, có những đối tượng nào đang hiện hữu trên Camera,
		/// ta đều phải sử dụng phép biến đổi này để vẽ đối tượng một cách
		/// chính xác.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public Vector2 Transform(Vector2 targetPosition)
		{
			// Lấy tọa độ của đối tượng trừ cho tọa độ của Camera Viewport, 
			// ta được tọa độ trên Viewport của Graphics Device.
			// 
			// *Lưu ý: Tại sao phải thực hiện ép kiểu (int) ? Bởi vì độ chính
			// xác của các giá trị float lưu trong Vector2 là không cao, luôn
			// luôn xảy ra sự sai lệch trong tọa độ nếu Viewport phải di chuyển
			// theo người chơi (tọa độ của người chơi cũng là Vector2 với 2
			// biến float) nếu ta vẽ tất cả các đối tượng với tọa độ float 
			// thì sẽ xảy ra hiện tượng các tiles trên màn hình hiển thị đè
			// nhau ngoài rìa gây ra cảm giác xé hình. Do vậy, nếu ta vẽ tất
			// cả các đối tượng bằng tọa độ int, sẽ không có sai lệch bởi
			// phần dư của biến float nữa, đảm bảo về mặt đồ họa, bù lại hi
			// sinh một phần sự chính xác khi vẽ đối tượng (không đáng kể)
			// và đồng thời làm cho Sprite của người chơi và các đối tượng 
			// di chuyển có hiện tượng rung nhẹ (do ép từ float xuống int), 
			// hiện tượng rung này có thể xem là lỗi hoặc là một hiệu ứng 
			// do Tank di chuyển gây ra.
			return new Vector2
				((int)targetPosition.X - (int)position.X,
				(int)targetPosition.Y - (int)position.Y);
		}

		/// <summary>
		/// Tương tự với phép Transform tọa độ. Ở đây ta sẽ transform cả
		/// hình chữ nhật bao quanh đối tượng.
		/// </summary>
		/// <param name="boundingRectangle">Hình chữ nhật bao quanh đối tượng
		/// </param>
		/// <returns></returns>
		public Rectangle Transform(Rectangle boundingRectangle)
		{
			return new Rectangle
				(boundingRectangle.Left - (int)position.X,
				boundingRectangle.Top - (int)position.Y,
				boundingRectangle.Width, boundingRectangle.Height);
		}
		#endregion
	}
}

// Thử nghiệm Transform Camera: Xoay, Zoom,... Viewport của Camera.

/*private float rotation;
private Matrix transform;
public float Rotation
{
	get { return rotation; }
	set { rotation = value; }
}*/

/*public Matrix get_transformation(GraphicsDevice graphicsDevice)
{
	transform = 
	  Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
								 Matrix.CreateRotationZ(Rotation) *
		/*Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
								 Matrix.CreateTranslation(new Vector3(0, 0, 0));
	return transform;
}*/