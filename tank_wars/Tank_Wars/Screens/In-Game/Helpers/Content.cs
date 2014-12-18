using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Tank_Wars
{
	/// <summary>
	/// Do nhu cầu sử dụng dữ liệu hình ảnh, âm thanh và font rải rác trong
	/// cả game. Class được ra đời nhằm giảm bớt các khai báo rải rác đó và
	/// mở rộng phạm vi sử dụng của dữ liệu. Bù lại, lập trình viên cần phải
	/// kiểm soát tốt dữ liệu nào sẽ được Load và UnLoad ở những thời điểm
	/// phù hợp, tránh UnLoad những dữ liệu đã hết dùng ở nơi này nhưng vẫn
	/// còn dùng ở nơi khác.
	/// </summary>
	public class Content
	{
		public Font Font { get; set; }
		public Audio Audio { get; set; }
		public Texture Texture { get; set; }

		public Content()
		{
			Font = new Font();
			Audio = new Audio();
			Texture = new Texture();
		}
	}

	/// <summary>
	/// Class con chứa truy xuất và lưu trữ các dữ liệu âm thanh.
	/// </summary>
	public class Audio
	{
		/// <summary>
		/// Phát âm thanh với hiệu ứng xa gần.
		/// </summary>
		/// <param name="soundEffect">Tên hiệu ứng âm thanh cần phát.</param>
		/// <param name="emitterPosition">Vị trí của nguồn phát.</param>
		/// <param name="listenerPosition">Vị trí của máy thu.</param>
		/// <param name="amplifier">Chỉ số khuếch đại. Chỉ số càng lớn thì 
		/// người ở xa nghe càng lớn và ngược lại.</param>
		/// <param name="minVolume">Âm lượng tối thiểu.</param>
		/// <param name="maxVolume">Âm lượng tối đa.</param>
		static public void Play(SoundEffect soundEffect,
			Vector2 emitterPosition, Vector2 listenerPosition,
			int amplifier, float minVolume, float maxVolume)
		{
			// Tính toán âm lượng theo chỉ số khuếch đại và khoảng cách
			// giữa nguồn và máy thu.
			float volume = amplifier / Vector2.Distance
				(emitterPosition, listenerPosition);

			// Giữ âm lượng nằm trong khoảng tối thiểu và tối đa cho trước.
			volume = MathHelper.Clamp(volume, minVolume, maxVolume);

			// Phát âm thanh.
			soundEffect.Play(volume, 0, 0);
		}

		// Các dữ liệu Audio có thể được Load xuyên suốt trong game.
		public SoundEffect BulletFiring;
		public SoundEffect BulletImpact;
		public SoundEffect MissileFiring;
		public SoundEffect MissileImpact;
		public SoundEffect SmallLaserRayFiring;
		public SoundEffect SmallLaserRayImpact;
		public SoundEffect BigLaserRayFiring;
		public SoundEffect BigLaserRayImpact;
		public SoundEffect Explosion;
		public SoundEffect PowerSwitch;
		public SoundEffect TankMove;
		public SoundEffectInstance TankMoving;
		public SoundEffect Regenerate;
		public SoundEffectInstance Regenerating;
		public SoundEffect BackgroundBattlefield;
		public SoundEffectInstance BackgroundBattlefieldLoop;
		public SoundEffect MissionAccomplish;
		public SoundEffectInstance MissionAccomplished;
		public SoundEffect MissionFail;
		public SoundEffectInstance MissionFailed;
	}

	/// <summary>
	/// Class con chứa truy xuất và lưu trữ các dữ liệu hình ảnh.
	/// </summary>
	public class Texture
	{
		/// <summary>
		/// Lấy dữ liệu màu từ Texture (lấy của cả Texture hoặc chỉ một
		/// phần tùy theo frameSize) nhằm sử dụng cho việc xử lý va chạm
		/// ở cấp độ Per-Pixel.
		/// </summary>
		/// <param name="textureImage">Texture cần lấy dữ liệu màu.</param>
		/// <param name="frameSize">Kích thước frame cần lấy dữ liệu.</param>
		/// <returns></returns>
		static public Color[] GetTextureData(Texture2D textureImage, Rectangle sourceRectangle)
		{
			// Nếu list textureImages không chứa textureImage thì mới
			// tiến hành suy xét và thêm vào danh sách dữ liệu màu. Bằng
			// cách này, ta tránh được sự trùng lắp các dữ liệu màu đã
			// tồn tại, tiết kiệm không gian nhớ và thời gian loading.
			if (!TextureImages.Contains(textureImage))
			{
				// Tạo một mảng Color lưu dữ liệu của Texture.
				Color[] textureData = new Color[sourceRectangle.Width * sourceRectangle.Height];

				// Lấy dữ liệu màu từ textureImage và gán vào textureData.
				textureImage.GetData
					(
					0, sourceRectangle,
					textureData, 0, textureData.Length
					);

				// Thêm dữ liệu vừa trích xuất vào danh sách textureDatas.
				TextureDatas.Add(textureData);

				// Đồng thời thêm textureImage vào danh sách textureImages
				// phục vụ việc kiểm tra trùng lắp sau này.
				// *Lưu ý: Tại sao phải tạo thêm danh sách textureImages
				// để phục vụ kiểm tra trùng lắp ? Bởi vì trong trường hợp
				// này nếu muốn kiểm tra xem dữ liệu màu vừa tạo ra có nằm
				// trong danh sách textureDatas hay chưa, ta sẽ phải tiến
				// hành so sánh từng ô màu với từng khối dữ liệu trong danh
				// sách, do chi phí cao và không hiệu quả, ta nên sử dụng
				// thêm một danh sách khác lưu reference đến những textureImage
				// đã lấy dữ liệu, chính vì thế, bằng cách so sách reference
				// tốc độ kiểm tra trùng lắp được cải thiện đáng kể.
				TextureImages.Add(textureImage);

				// Return về textureData tương ứng trong danh sách.
				return TextureDatas[TextureDatas.Count - 1];
			}
			return TextureDatas[TextureImages.IndexOf(textureImage)];
		}

		// Danh sách lưu trữ các textureImage đã lấy dữ liệu.
		static public List<Texture2D> TextureImages;

		// Danh sách lưu trữ các textureData được trích xuất từ textureImage
		// tương ứng trong danh sách textureImages.
		static public List<Color[]> TextureDatas;

		// Các dữ liệu Texture có thể được Load xuyên suốt trong game.
		public Texture2D Cursor;
		public Texture2D Direction;
		public Texture2D TankBot;
		public Texture2D TankTop;
		public Texture2D HumveeBot;
		public Texture2D HumveeTop;
		public Texture2D Bullet;
		public Texture2D Missile;
		public Texture2D SmallLaserRay;
		public Texture2D BigLaserRay;
		public Texture2D MiniMapRectangle;
		public Texture2D Dirt;
		public Texture2D TileSheet;
		public Texture2D PowerSheet;
		public Texture2D BarSheet;
		public Texture2D SmallFlameSheet;
		public Texture2D BigFlameSheet;
		public Texture2D BulletImpactSheet;
		public Texture2D MissileImpactSheet;
		public Texture2D LaserImpactSheet;
		public Texture2D EnergyImpactSheet;
		public Texture2D ExplosionSheet;
		public Texture2D Blank;
	}

	/// <summary>
	/// Class con chứa truy xuất và lưu trữ các dữ liệu Font.
	/// </summary>
	public class Font
	{
		// Các dữ liệu Font có thể được Load xuyên suốt trong game.
		public SpriteFont HUDFont;
	}
}
