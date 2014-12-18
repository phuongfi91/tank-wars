using Microsoft.Xna.Framework.Input;

namespace Tank_Wars
{
	public class InputState
	{
		#region Declarations
		public KeyboardState CurrentKeyboardState;
		public KeyboardState PreviousKeyboardState;
		public MouseState CurrentMouseState;
		public MouseState PreviousMouseState;

		#endregion

		#region Constructor
		/// <summary>
		/// Khởi tạo InputState. Bao gồm trạng thái trước đó và hiện tại 
		/// của Keyboard.
		/// </summary>
		public InputState()
		{
			CurrentKeyboardState = new KeyboardState();
			PreviousKeyboardState = new KeyboardState();
			CurrentMouseState = new MouseState();
			PreviousMouseState = new MouseState();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Update trạng thái Input của Keyboard.
		/// </summary>
		public void Update()
		{
			PreviousKeyboardState = CurrentKeyboardState;
			CurrentKeyboardState = Keyboard.GetState();
			PreviousMouseState = CurrentMouseState;
			CurrentMouseState = Mouse.GetState();
		}

		public bool IsNewKeyPress(Keys key)
		{
			return
				(CurrentKeyboardState.IsKeyDown(key)
				&&
				PreviousKeyboardState.IsKeyUp(key));
		}

		public bool IsCurrentKeyPress(Keys key)
		{
			return CurrentKeyboardState.IsKeyDown(key);
		}
		#endregion

		#region Input States
		#region Menu Input
		/// <summary>
		/// Kiểm tra xem người chơi có chọn Menu Item không.
		/// </summary>
		public bool IsMenuSelect
		{
			get
			{
				return IsNewKeyPress(Keys.Space) || IsNewKeyPress(Keys.Enter);
			}
		}

		/// <summary>
		/// Kiểm tra xem người chơi có cancel Menu không.
		/// </summary>
		public bool IsMenuCancel
		{
			get
			{
				return IsNewKeyPress(Keys.Escape);
			}
		}

		/// <summary>
		/// Kiểm tra xem người chơi có "Move up" trong Menu không.
		/// </summary>
		public bool IsMenuUp
		{
			get
			{
				return IsNewKeyPress(Keys.Up) || IsNewKeyPress(Keys.W);
			}
		}

		/// <summary>
		/// Kiểm tra xem người chơi có "Move down" trong Menu không.
		/// </summary>
		public bool IsMenuDown
		{
			get
			{
				return IsNewKeyPress(Keys.Down) || IsNewKeyPress(Keys.S);
			}
		}
		#endregion

		#region Gameplay Input
		/// <summary>
		/// Kiểm tra xem người chơi có Pause Game không.
		/// </summary>
		public bool IsPauseGame
		{
			get
			{
				return IsNewKeyPress(Keys.Escape);
			}
		}

		public bool IsRotateRight
		{
			get
			{
				return IsCurrentKeyPress(Keys.Right) || IsCurrentKeyPress(Keys.D);
			}
		}

		public bool IsRotateLeft
		{
			get
			{
				return IsCurrentKeyPress(Keys.Left) || IsCurrentKeyPress(Keys.A);
			}
		}

		public bool IsMoveForward
		{
			get
			{
				return IsCurrentKeyPress(Keys.Up) || IsCurrentKeyPress(Keys.W);
			}
		}

		public bool IsMoveBackward
		{
			get
			{
				return IsCurrentKeyPress(Keys.Down) || IsCurrentKeyPress(Keys.S);
			}
		}

		public bool IsFireMainGun
		{
			get
			{
				return CurrentMouseState.LeftButton == ButtonState.Pressed;
			}
		}

		public bool IsFireSecondaryGun
		{
			get
			{
				return CurrentMouseState.RightButton == ButtonState.Pressed;
			}
		}

		public bool IsSelectNextPower
		{
			get
			{
				return IsNewKeyPress(Keys.Tab) || IsNewKeyPress(Keys.RightControl);
			}
		}

		public bool IsSelectPowerNo1
		{
			get
			{
				return IsNewKeyPress(Keys.D1) || IsNewKeyPress(Keys.NumPad1);
			}
		}

		public bool IsSelectPowerNo2
		{
			get
			{
				return IsNewKeyPress(Keys.D2) || IsNewKeyPress(Keys.NumPad2);
			}
		}

		public bool IsSelectPowerNo3
		{
			get
			{
				return IsNewKeyPress(Keys.D3) || IsNewKeyPress(Keys.NumPad3);
			}
		}

		public bool IsSelectPowerNo4
		{
			get
			{
				return IsNewKeyPress(Keys.D4) || IsNewKeyPress(Keys.NumPad4);
			}
		}

		public bool IsSelectPowerNo5
		{
			get
			{
				return IsNewKeyPress(Keys.D5) || IsNewKeyPress(Keys.NumPad5);
			}
		}
		#endregion
		#endregion
	}
}