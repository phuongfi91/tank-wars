using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tank_Wars
{
	/// <summary>
	/// Sprite là class cơ bản. Tất cả các đối tượng trong game (Tank, Wall,...)
	/// đều kế thừa từ class này.
	/// </summary>
	public abstract class Sprite
	{
		#region Declarations
		protected GameplayScreen GameplayScreen { get; set;}
		protected Camera Camera { get; set; }
		protected Map Map { get; set; }

		/************************************************************************/
		/*                      Texture (hoặc Sprite Sheet)                     */
		/************************************************************************/
		// Texture của Sprite. Texture này có thể đơn thuần là một file hình,
		// hoặc nó cũng có thể là một Sheet (Sprite Sheet), bao gồm nhiều hình
		// tạo thành một chuỗi chuyển động animation.
		protected Texture2D textureImage;

		// Mảng màu chứa dữ liệu màu của Texture. Mảng này được dùng cho việc
		// xử lý va chạm giữa các Sprite với độ chính xác từng Pixel.
		protected Color[] textureImageData;
		//////////////////////////////////////////////////////////////////////////



		/************************************************************************/
		/*                  Các thông tin để sử dụng Sprite Sheet               */
		/************************************************************************/
		// Do Texture có thể là một Sprite Sheet chứa các hình nối tiếp nhau 
		// tạo thành một chuỗi chuyển động animation:

		// Sprite Sheet sẽ có kích thước sheetSize (Ví dụ: Sheet với 5 cột,
		// và 4 dòng sẽ có sheetSize là 5x4)
		protected Point sheetSize;

		// Mỗi Sprite trong Sheet sẽ có kích thước frameSize pixel AxB 
		// (Ví dụ: Sheet có sheetSize 5x4 và frameSize 64x64 thì tổng kích thước
		// sẽ là (5x64)x(4x64) = 320x256)
		protected Point frameSize;

		// currentFrame sẽ đại diện cho frame hiện tại đang được vẽ trong
		// chuỗi animation.
		protected Point currentFrame;

		// isLooped sẽ quyết định xem chuỗi animation này có lặp lại hay không.
		protected bool isLooped;

		// Nếu isLooped = false thì isFinishedAnimating sẽ xác định xem chuỗi
		// animation có kết thúc hay chưa.
		protected bool isFinishedAnimating;
		//////////////////////////////////////////////////////////////////////////



		/************************************************************************/
		/*                     Các thông số của đối tượng:                      */
		/************************************************************************/
		// position: tọa độ (Pixel) hiện tại của đối tượng (trong cả màn chơi)
		protected Vector2 position;

		// origin: trọng tâm của đối tượng. (Ví dụ: với frameSize là 64x64,
		// ta có thể set origin = 32x32 -> trọng tâm nằm ngay chính giữa đối
		// tượng). Đây là một thông số quan trọng liên quan đến việc xác định
		// tọa độ và khả năng xoay của đối tượng.
		protected Vector2 origin;

		// rotationAngle: góc quay của đối tượng.
		protected float rotationAngle;

		// scale: kích thước của đối tượng. Mặc định có giá trị 1.0f.
		// -> Tăng scale <=> Phóng lớn.
		// -> Giảm scale <=> Thu nhỏ.
		protected float scale;

		// drawLayer: do có rất nhiều đối tượng đc vẽ lên màn hình trong cùng
		// một lúc, phải có một chỉ số để xác định đối tượng nào nằm trên, đối
		// tượng nào nằm dưới, đó chính là drawLayer, có giá trị từ 0.0f -> 1.0f.
		protected float drawLayer;

		// Xác định xem đối tượng này có khả năng gây ra va chạm hay không.
		// Giá trị này sẽ được gán là true cho các đối tượng có khả năng di
		// chuyển (như tank, đạn,...) do các đối tượng đó sẽ chủ động check
		// va chạm, thay cho những đối tượng bị động (như tường chẳng hạn).
		protected bool isCollidable;

		// previousMapPosition: Tọa độ trước đó của đối tượng trên Map.
		// Lưu ý: Đây là tọa độ trên Map, khác với tọa độ Pixel.
		protected Point previousMapPosition;

		// currentMapPosition: Tọa độ hiện tại của đối tượng trên Map.
		// Lưu ý: Đây là tọa độ trên Map, khác với tọa độ Pixel.
		protected Point currentMapPosition;

		/* 
		 * Do mỗi ô trên map có chứa nhiều tầng thông tin (địa hình, các đối
		 * tượng đang chiếm giữ ô map,...) ta cần biết được thông tin của 
		 * đối tượng này được lưu ở tầng nào, chỉ số Index được sử dụng
		 * cho mục đích đó.
		 */

		// previousMapPositionIndex: Index của đối tượng trong previousMapPosition.
		protected int previousMapPositionIndex;

		// currentMapPositionIndex: Index của đối tượng trong currentMapPosition.
		protected int currentMapPositionIndex;

		// Các thông số điều chỉnh tốc độ khung hình của 1 Sprite xác định
		// (Không phải tốc độ khung hình FPS của cả Game).
		// Nếu millisecondsPerFrame = defaultMillisecondsPerFrame = 16 thì
		// trong 1 giây, Sprite sẽ được vẽ khoảng 62 lần (1000 / 16 = 62.5)
		// -> millisecondsPerFrame càng lớn thì cử động animation của Sprite
		// càng chậm và ngược lại.
		protected int timeSinceLastFrame = 0;
		protected int millisecondsPerFrame;
		protected const int defaultMillisecondsPerFrame = 16;
		//////////////////////////////////////////////////////////////////////////
		#endregion

		#region Properties
		#region Public
		/// <summary>
		/// Truy xuất kích thước frameSize.
		/// </summary>
		public Point FrameSize
		{
			get { return frameSize; }
		}

		/// <summary>
		/// Truy xuất và thay đổi góc quay rotationAngle.
		/// </summary>
		public virtual float RotationAngle
		{
			get { return rotationAngle; }
			// Khi set góc quay, nó sẽ luôn được Wrap lại, để giá trị luôn
			// nằm trong khoảng từ -Pi -> Pi.
			set { rotationAngle = MathHelper.WrapAngle(value); }
		}

		/* 
		 * BoundingRectangle (Hình chữ nhật bao quanh Sprite): HCN này sẽ dùng
		 * để bổ trợ cho thuật toán xử lý va chạm và một số hàm xử lý khác 
		 * trong game.
		 */

		/// <summary>
		/// Truy xuất BoundingWorldRectangle: HCN này có hệ tọa độ là cả màn chơi.
		/// </summary>
		public Rectangle BoundingWorldRectangle
		{
			get
			{
				return CalculateBoundingRectangle
					(new Rectangle(0, 0, frameSize.X, frameSize.Y), Transform);
			}
		}

		/// <summary>
		/// Truy xuất BoundingScreenRectangle, HCN này có hệ tọa độ chỉ nằm 
		/// trong Viewport hiện tại. (Viewport là những gì người chơi đang 
		/// nhìn thấy trên màn hình)
		/// </summary>
		public Rectangle BoundingScreenRectangle
		{
			get { return Camera.Transform(BoundingWorldRectangle); }
		}

		/// <summary>
		/// Truy xuất và thay đổi CurrentWorldPosition: Tọa độ hiện tại 
		/// trong toàn màn chơi.
		/// </summary>
		public virtual Vector2 CurrentWorldPosition
		{
			get { return position; }
			// Khi set tọa độ, nó sẽ bị kiểm soát để không vượt ra khỏi màn
			// chơi.
			set
			{
				position.X = MathHelper.Clamp
					(value.X, frameSize.X / 2,
					Camera.WholeWorldRectangle.Width - frameSize.X / 2);
				position.Y = MathHelper.Clamp
					(value.Y, frameSize.Y / 2,
					Camera.WholeWorldRectangle.Height - frameSize.Y / 2);
			}
		}

		/// <summary>
		/// Truy xuất CurrentScreenPosition: Tọa độ hiện tại trong Viewport.
		/// </summary>
		public Vector2 CurrentScreenPosition
		{
			get { return Camera.Transform(CurrentWorldPosition); }
		}

		/// <summary>
		/// Truy xuất và thay đổi tọa độ trước đó trên Map.
		/// </summary>
		public Point PreviousMapPosition
		{
			get { return previousMapPosition; }
			set { previousMapPosition = value; }
		}

		/// <summary>
		/// Truy xuất và thay đổi tọa độ hiện tại trên Map.
		/// </summary>
		public Point CurrentMapPosition
		{
			get { return currentMapPosition; }
			set { currentMapPosition = value; }
		}

		/// <summary>
		/// Truy xuất và thay đổi Index trong PreviousMapPosition.
		/// </summary>
		public int PreviousMapPositionIndex
		{
			get { return previousMapPositionIndex; }
			set { previousMapPositionIndex = value; }
		}

		/// <summary>
		/// Truy xuất và thay đổi Index trong CurrentMapPosition
		/// </summary>
		public int CurrentMapPositionIndex
		{
			get { return currentMapPositionIndex; }
			set { currentMapPositionIndex = value; }
		}
		#endregion

		#region Private
		/// <summary>
		/// Transform Matrix. (dùng để bổ trợ cho các thao tác Scale, Rotate,...)
		/// </summary>
		protected Matrix Transform
		{
			get
			{
				return
					Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
					/*Matrix.CreateScale(scale) **/
					Matrix.CreateRotationZ(RotationAngle) *
					Matrix.CreateTranslation(new Vector3(CurrentWorldPosition, 0.0f));
			}
		}

		/// <summary>
		/// Truy xuất dữ liệu màu của Sprite, dùng để xử lý va chạm.
		/// </summary>
		protected Color[] TextureData
		{
			get { return textureImageData; }
		}
		#endregion
		#endregion

		#region Constructors
		/// <summary>
		/// Nếu ko truyền vào tham số millisecondsPerFrame thì sử dụng 
		/// defaultMillisecondsPerFrame = 16 (mặc định)
		/// </summary>
		public Sprite
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, float rotationAngle, 
			Point frameSize, Point sheetSize
			)
			: this
			(
			gameplayScreen, textureImage, position, rotationAngle, frameSize, sheetSize, 
			defaultMillisecondsPerFrame
			)
		{
			// Bỏ trống vì đã gọi Constructor bên dưới.
		}

		/// <summary>
		/// Ngược lại, sử dụng millisecondsPerFrame từ Constructor.
		/// </summary>
		public Sprite
			(
			GameplayScreen gameplayScreen, Texture2D textureImage, Vector2 position, float rotationAngle, 
			Point frameSize, Point sheetSize, int millisecondsPerFrame
			)
		{
			this.GameplayScreen = gameplayScreen;
			this.Camera = gameplayScreen.Camera;
			this.Map = gameplayScreen.Map;


			/************************************************************************/
			/*                 Truyền thông tin vào từ Constructor                  */
			/************************************************************************/
			this.textureImage = textureImage;
			this.position = position;
			this.rotationAngle = rotationAngle;
			this.frameSize = frameSize;
			this.sheetSize = sheetSize;
			this.isLooped = true;
			this.millisecondsPerFrame = millisecondsPerFrame;
			//////////////////////////////////////////////////////////////////////////



			/************************************************************************/
			/*                     Khởi tạo các thông tin khác                      */
			/************************************************************************/
			// Trọng tâm của đối tượng sẽ nằm ngay chính giữa đối tượng
			this.origin = new Vector2(frameSize.X / 2, frameSize.Y / 2);

			// scale mặc định = 1.0f.
			this.scale = 1.0f;

			// Mặc định đối tượng không có khả năng va chạm.
			this.isCollidable = false;
			//////////////////////////////////////////////////////////////////////////
		}
		#endregion

		#region Methods
		/// <summary>
		/// Update các giá trị, thông số của đối tượng.
		/// </summary>
		public virtual void Update(GameTime gameTime)
		{
			// Check va chạm (Nếu isCollidable == true)
			CollisionCheck();

			// Update currentFrame để Sprite chuyển động theo mô hình trong
			// Sprite Sheet, tạo thành 1 chuỗi animation. (currentFrame chạy
			// từ trái sáng phải, rồi xuống dòng, sau đó lặp lại cho đến khi
			// hết Sprite Sheet thì quay về dòng đầu tiên, bằng cách này ta
			// tạo ra chuỗi chuyển động khép kín, lặp vô tận)
			timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
			if (timeSinceLastFrame > millisecondsPerFrame)
			{
				timeSinceLastFrame = 0;
				++currentFrame.X;
				if (currentFrame.X >= sheetSize.X)
				{
					currentFrame.X = 0;
					++currentFrame.Y;
					if (currentFrame.Y >= sheetSize.Y)
						if (isLooped)
						{
							currentFrame.Y = 0;
						}
						else
						{
							isFinishedAnimating = true;
						}
				}
			}
		}

		/// <summary>
		/// Vẽ đối tượng dựa trên các thông số của nó.
		/// </summary>
		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Chỉ vẽ đối tượng nếu đối tượng đang hiện hữu trong Viewport
			// của Camera.
			if (Camera.ObjectIsVisible(BoundingWorldRectangle))
			{
				// Lấy tọa độ của bóng.
				Vector2 shadowPosition = new Vector2
					(CurrentScreenPosition.X + 5.0f, CurrentScreenPosition.Y + 5.0f);

				// Vẽ đối tượng.
				spriteBatch.Draw
					(
					textureImage, CurrentScreenPosition, new Rectangle
						(
						currentFrame.X * frameSize.X,
						currentFrame.Y * frameSize.Y,
						frameSize.X, frameSize.Y
						),
					Color.White, RotationAngle, origin, 1.0f,
					SpriteEffects.None, drawLayer
					);

				// Vẽ bóng của đối tượng.
				spriteBatch.Draw
					(
					textureImage, shadowPosition, new Rectangle
						(
						currentFrame.X * frameSize.X,
						currentFrame.Y * frameSize.Y,
						frameSize.X, frameSize.Y
						),
					new Color(0, 0, 0, 100), RotationAngle, origin, 1.0f,
					SpriteEffects.None, drawLayer - 0.05f
					);
			}
		}

		#region Helpers
		protected Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
		{
			Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
			Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
			Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
			Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

			Vector2.Transform(ref leftTop, ref transform, out leftTop);
			Vector2.Transform(ref rightTop, ref transform, out rightTop);
			Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
			Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

			Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
									  Vector2.Min(leftBottom, rightBottom));
			Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
									  Vector2.Max(leftBottom, rightBottom));

			return new Rectangle((int)min.X, (int)min.Y,
								 (int)(max.X - min.X), (int)(max.Y - min.Y));
		}

		protected bool IntersectPixels(
							Matrix transformA, int widthA, int heightA, Color[] dataA,
							Matrix transformB, int widthB, int heightB, Color[] dataB)
		{
			Matrix transformAToB = transformA * Matrix.Invert(transformB);

			Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
			Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

			Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

			for (int yA = 0; yA < heightA; yA++)
			{
				Vector2 posInB = yPosInB;

				for (int xA = 0; xA < widthA; xA++)
				{
					int xB = (int)Math.Round(posInB.X);
					int yB = (int)Math.Round(posInB.Y);

					if (0 <= xB && xB < widthB &&
						0 <= yB && yB < heightB)
					{
						Color colorA = dataA[xA + yA * widthA];
						Color colorB = dataB[xB + yB * widthB];

						if (colorA.A != 0 && colorB.A != 0)
						{
							return true;
						}
					}

					posInB += stepX;
				}

				yPosInB += stepY;
			}

			return false;
		}

		public virtual void UpdateMapPosition()
		{
			if (CurrentMapPosition != Map.GetSquareAtPixel(CurrentWorldPosition))
			{
				PreviousMapPosition = CurrentMapPosition;
				CurrentMapPosition = Map.GetSquareAtPixel(CurrentWorldPosition);
			}
		}

		// Nếu A va chạm với B thì trả về true hoặc ngược lại
		protected bool CollideWith(Sprite sprite)
		{
			// Kiểm tra xem BoundingWorldRectangle của 2 đối tượng có cắt
			// nhau hay không trước khi thực hiện việc kiểm tra va chạm
			// bằng Pixel, từ đây tránh được phí tổn không cần thiết.
			if (this.BoundingWorldRectangle.Intersects(sprite.BoundingWorldRectangle))
				if  (
						IntersectPixels
						(
						Transform, FrameSize.X, FrameSize.Y, TextureData,
						sprite.Transform, sprite.FrameSize.X,
						sprite.FrameSize.Y, sprite.TextureData
						)
					)
				{
					return true;
				}
			return false;
		}

		/// <summary>
		/// Kiểm tra va chạm và đưa ra hành động phản ứng thích hợp.
		/// </summary>
		/// <returns>Trả về giá trị true nếu có va chạm hoặc false nếu 
		/// không có va chạm.</returns>
		protected bool CollisionCheck()
		{
			if (this.isCollidable)
			{
				// Tiến hành check va chạm với tất cả các ô xung quanh và kể
				// cả ô mà đối tượng đang chiếm giữ.
				for (int i = -1; i < 2; i++)
					for (int j = -1; j < 2; j++)
						for (int k = 0; k < 4; k++)
						{
							// Không check với những ô ko nằm trong bản đồ
							if ((int)currentMapPosition.X + i < 0
								||
								(int)currentMapPosition.Y + j < 0
								||
								(int)currentMapPosition.X + i >= Map.MapWidth
								||
								(int)currentMapPosition.Y + j >= Map.MapHeight)
							{
								break;
							}

							// Lấy loại đối tượng (Là địa hình hay Tank, Wall,...)
							int tempType =
								Map.MapSquares[(int)currentMapPosition.X + i,
								(int)currentMapPosition.Y + j][k, 0];

							// Lấy Index của đối tượng trong List quản lý nó
							// (Ví dụ: EnemyList, WallList,...)
							int tempIndex =
								Map.MapSquares[(int)currentMapPosition.X + i,
								(int)currentMapPosition.Y + j][k, 1];

							// Đối tượng có khả năng gây ra va chạm.
							Sprite possibleCollisionTarget = null;

							// Va chạm với Wall.
							if (Map.WallTileStart <= tempType
								&&
								tempType <= Map.WallTileEnd
								/*&&
								i != -2 && i != 2 && j != -2 && j != 2*/)
							{
								possibleCollisionTarget = 
									GameplayScreen.WallList[tempIndex];
								if (this.GetType() == typeof(Tank_Wars.Enemy))
								{
									Enemy enemy = (Enemy)this;
									if (enemy.AItypeID == AItype.StationaryAI)
									{
										possibleCollisionTarget = null;
									}
								}
							}

							// Va chạm với Tank.
							else if (tempType > Map.WaterTileEnd)
							{
								switch (tempType)
								{
									// ID số 88 đại diện cho Player.
									case 88:
										possibleCollisionTarget = 
											GameplayScreen.Player;
										break;

									// ID số 89 đại diện cho Enemy.
									case 89:
										// Do Tank bị hủy diệt trong quá trình chơi,
										// đôi khi gây mất đồng bộ trong quá trình
										// update, dẫn đến tempIndex >= EnemyList.Count.
										// Khi đó ta sẽ không check va chạm.
										if (tempIndex >= GameplayScreen.EnemyList.Count)
										{
											break;
										}
										possibleCollisionTarget = 
											GameplayScreen.EnemyList[tempIndex];
										break;
								}
							}

							// Nếu possibleCollisionTarget != null và khác
							// bản thân đối tượng đang check thì ta mới tiến
							// hành check và xử lý.
							if (possibleCollisionTarget != null
								&&
								possibleCollisionTarget != this
								&&
								this.CollideWith(possibleCollisionTarget))
							{
								HandleCollision(possibleCollisionTarget);
								return true;
							}
						}
			}
			return false;
		}

		/// <summary>
		/// Xử lý va chạm. Hàm được tùy biến tùy theo thể loại đối tượng kế
		/// thừa từ class Sprite.
		/// </summary>
		/// <param name="possibleCollisionTarget">Đối tượng mà ta sẽ check
		/// va chạm với đối tượng this</param>
		protected virtual void HandleCollision(Sprite possibleCollisionTarget)
		{

		}
		#endregion
		#endregion
	}
}