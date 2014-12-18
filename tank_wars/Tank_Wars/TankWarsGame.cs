using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Wars
{
	/// <summary>
	/// Class chính của game.
	/// </summary>
	public class TankWarsGame : Game
	{
		#region Properties
		#region Public
		/// <summary>
		/// Font menu dành cho tất cả các Screen.
		/// </summary>
		public SpriteFont Font { get; private set; }

		// Cho phép truy cập thông số và tùy chỉnh thiết lập của
		// Graphics Device.
		public GraphicsDeviceManager Graphics { get; set; }

		/// <summary>
		/// SpriteBatch của ScreenManager sẽ được share với các Screen con,
		/// do đó các Screen con sẽ không phải tạo SpriteBatch của riêng nó
		/// để vẽ cái gì đó nữa.
		/// </summary>
		public SpriteBatch SpriteBatch { get; private set; }
		#endregion

		#region Private
		Texture2D blankTexture;

		// Lưu giữ và kiểm soát trạng thái Input của người chơi.
		InputState Input { get; set; }

		// Danh sách các Screen hiện hành.
		List<GameScreen> Screens { get; set; }

		// Danh sách các Screen cần được Update.
		List<GameScreen> ScreensToUpdate { get; set; }

		// Danh sách các bản nhạc nền được chơi xuyên suốt trong game.
		List<Song> BackgroundSongs { get; set; }

		// Bản nhạc đang được chơi.
		int CurrentBackgroundSong { get; set; }

		// Xác định xem Graphics Device có được Initialize chưa.
		bool IsInitialized { get; set; }
		#endregion
		#endregion

		#region Constructor
		/// <summary>
		/// Khởi tạo game với các thông số cơ bản.
		/// </summary>
		public TankWarsGame()
		{
			Input = new InputState();
			Screens = new List<GameScreen>();
			ScreensToUpdate = new List<GameScreen>();
			BackgroundSongs = new List<Song>();
			Graphics = new GraphicsDeviceManager(this);

			// Tên thư mục chứa dữ liệu của game.
			Content.RootDirectory = "Content";

			// Tên game hiển thị trên Title Bar. (Window Mode)
			this.Window.Title = "Tank Wars";

			// Kích hoạt Screen mặc định: Menu và Background của nó.
			this.AddScreen(new BackgroundScreen());
			this.AddScreen(new MainMenuScreen());
		}
		#endregion

		#region Methods
		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// Thiết lập độ phân giải của game.
			Graphics.IsFullScreen = true;
			Graphics.PreferredBackBufferWidth = /*1024*/
				Graphics.GraphicsDevice.DisplayMode.Width;
			Graphics.PreferredBackBufferHeight = /*768*/
				Graphics.GraphicsDevice.DisplayMode.Height;
			Graphics.ApplyChanges();

			SpriteBatch = new SpriteBatch(GraphicsDevice);

			IsInitialized = true;

			base.Initialize();
		}

		/// <summary>
		/// Load các dữ liệu Texture, Font, Audio,... cần thiết.
		/// </summary>
		protected override void LoadContent()
		{
			Font = Content.Load<SpriteFont>(@"Fonts\MenuFont");
			blankTexture = Content.Load<Texture2D>(@"Texture\Blank");

			BackgroundSongs.Add(Content.Load<Song>(@"Audio\BackgroundSong1"));
			BackgroundSongs.Add(Content.Load<Song>(@"Audio\BackgroundSong2"));
			MediaPlayer.Volume = 0.6f;
			CurrentBackgroundSong = 0;

			// Gọi hàm load của các Screen
			foreach (GameScreen screen in Screens)
			{
				screen.LoadContent();
			}
		}

		/// <summary>
		/// Unload your graphics content.
		/// </summary>
		protected override void UnloadContent()
		{
			// Gọi hàm unload của các Screen
			foreach (GameScreen screen in Screens)
			{
				screen.UnloadContent();
			}
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (MediaPlayer.State != MediaState.Playing)
			{
				CurrentBackgroundSong++;
				if (CurrentBackgroundSong > BackgroundSongs.Count - 1)
				{
					CurrentBackgroundSong = 0;
				}
				MediaPlayer.Play(BackgroundSongs[CurrentBackgroundSong]);
			}

			// Đọc Input
			Input.Update();

			ScreensToUpdate.Clear();

			foreach (GameScreen screen in Screens)
				ScreensToUpdate.Add(screen);

			bool otherScreenHasFocus = !this.IsActive;
			bool coveredByOtherScreen = false;

			while (ScreensToUpdate.Count > 0)
			{
				GameScreen screen = ScreensToUpdate[ScreensToUpdate.Count - 1];

				ScreensToUpdate.RemoveAt(ScreensToUpdate.Count - 1);

				screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

				if (screen.ScreenState == ScreenState.TransitionOn ||
					screen.ScreenState == ScreenState.Active)
				{
					if (!otherScreenHasFocus)
					{
						screen.HandleInput(Input);

						otherScreenHasFocus = true;
					}

					if (!screen.IsPopup)
						coveredByOtherScreen = true;
				}
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			// Clear cả màn hình với màu nền xác định. (Màu này sẽ ảnh hưởng
			// đến Background của Menu vì Background của Menu có một phần
			// transparent. Có thể thử thay đổi giá trị màu dưới đây để thấy
			// rõ sự thay đổi.
			this.Graphics.GraphicsDevice.Clear(Color.Black);

			foreach (GameScreen screen in Screens)
			{
				if (screen.ScreenState == ScreenState.Hidden)
					continue;

				screen.Draw(gameTime);
			}

			base.Draw(gameTime);
		}

		public void AddScreen(GameScreen screen)
		{
			screen.TankWars = this;
			screen.IsExiting = false;

			if (IsInitialized)
			{
				screen.LoadContent();
			}

			Screens.Add(screen);
		}

		public void RemoveScreen(GameScreen screen)
		{
			if (IsInitialized)
			{
				screen.UnloadContent();
			}

			Screens.Remove(screen);
			ScreensToUpdate.Remove(screen);
		}

		public GameScreen[] GetScreens()
		{
			return Screens.ToArray();
		}

		/// <summary>
		/// Trả về loại Screen xác định trong danh sách Screens của game.
		/// </summary>
		/// <param name="targetScreenType">Loại Screen.</param>
		/// <returns>Trả về Screen nếu tìm thấy hoặc null.</returns>
		public GameScreen GetScreen(Type targetScreenType)
		{
			foreach (GameScreen gameScreen in Screens)
			{
				if (gameScreen.GetType() == targetScreenType)
				{
					return gameScreen;
				}
			}
			return null;
		}

		public void FadeBackBufferToBlack(float alpha)
		{
			Viewport viewport = GraphicsDevice.Viewport;

			SpriteBatch.Begin();

			SpriteBatch.Draw(blankTexture,
							 new Rectangle(0, 0, viewport.Width, viewport.Height),
							 Color.Black * alpha);

			SpriteBatch.End();
		}
		#endregion
	}
}