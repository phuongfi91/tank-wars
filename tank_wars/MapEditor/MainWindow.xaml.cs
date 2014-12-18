using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
namespace MapEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public struct texturesItem
	{
		public string name;
		public BitmapImage image;
		public override string ToString()
		{
			return name;
		}
	}
	public partial class MainWindow : Window
	{
		#region variable
		List<List<int>> map = null;
		List<texturesItem> textures = null;
		int pieceSize = 32;
		int pieceChoiced = 5;
		int pieceRotate = 0;
		int n = 20, m = 20;
		BitmapImage piece = null;
		public List<texturesItem> AllTextures
		{
			get
			{
				return textures;
			}
		}
		List<BitmapImage> texturesBitmap = null;
		int mousedown = 0;
		string filename = "";
		#endregion

		public MainWindow()
		{
			initElements();
			InitializeComponent();
			DataContext = this;
		}

		#region MainWindowsFunctions
		private void MenuNew(object sender, RoutedEventArgs e)
		{
			initMap();
			DrawCanvas();
		}
		private void mapArea_MouseMove(object sender, MouseEventArgs e)
		{
			System.Windows.Point mousepos = Mouse.GetPosition(mapArea);
			mousepos.X = (int)(mousepos.X) / pieceSize;
			mousepos.Y = (int)(mousepos.Y) / pieceSize;
			MouseCood.Content = "(" + mousepos.X + ", " + mousepos.Y + ")";
			if (mousedown == 1)
			{
				int x, y, newp;
				y = (int)mousepos.X;
				x = (int)mousepos.Y;
				newp = pieceChoiced * 4 + pieceRotate;
				if (x < n && y < m)
				{
					if (map[x][y] != newp)
					{
						map[x][y] = newp;
						DrawChangedPiece(newp, x, y);
					}
				}
			}
		}
		private void mapArea_MouseDown(object sender, MouseButtonEventArgs e)
		{
			mousedown = 1;
			System.Windows.Point mousepos = Mouse.GetPosition(mapArea);
			int x, y, newp;
			y = (int)(mousepos.X) / pieceSize;
			x = (int)(mousepos.Y) / pieceSize;
			newp = pieceChoiced * 4 + pieceRotate;
			if (x < n && y < m)
			{
				if (map[x][y] != newp)
				{
					map[x][y] = newp;
					DrawChangedPiece(newp, x, y);
				}
			}
		}
		private void mapArea_MouseUp(object sender, MouseButtonEventArgs e)
		{
			mousedown = 0;
		}
		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			int n_old = n, m_old = m;
			n = Int32.Parse(TextboxHeight.Text);
			m = Int32.Parse(TextboxWidth.Text);
			mapArea.Width = (double)m * pieceSize;
			mapArea.Height = (double)n * pieceSize;
			while (map.Count < n) map.Add(new List<int>());
			for (int i = 0; i < n; ++i)
				while (map[i].Count < m)
					map[i].Add(pieceChoiced * 4 + pieceRotate);
			DrawCanvas();
		}
		private void ButtonRotate_Click(object sender, RoutedEventArgs e)
		{
			RotateTextures();
		}
		private void ListboxTextures_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			pieceChoiced = ListboxTextures.SelectedIndex;
			piece = textures[pieceChoiced].image;
			ImagePiece.Source = piece;
			pieceRotate = 0;
		}
		private void mapArea_Initialized(object sender, EventArgs e)
		{
			DrawCanvas();
		}
		private void MenuItemSize100(object sender, RoutedEventArgs e)
		{
			ChangePieceSize(64);
		}
		private void MenuItemSize50(object sender, RoutedEventArgs e)
		{
			ChangePieceSize(32);
		}
		private void MenuItemSize25(object sender, RoutedEventArgs e)
		{
			ChangePieceSize(16);
		}
		private void MenuOpen(object sender, RoutedEventArgs e)
		{
			// Displays an OpenFileDialog and shows the read/only files.
			OpenFileDialog dlgOpenFile = new OpenFileDialog();
			dlgOpenFile.DefaultExt = ".map";
			dlgOpenFile.Filter = "TankWars maps(*.map)|*.map|All files (*.*)|*.*";
			int i, j;
			List<int> adder = null;
			if (dlgOpenFile.ShowDialog().Value)
			{
				filename = dlgOpenFile.FileName;
			}
			if (filename == "") return;
			map = new List<List<int>>();
			FileStream datinp = new FileStream(filename, FileMode.Open, FileAccess.Read);
			StreamReader datstr = new StreamReader(datinp);
			string buffer;
			string[] fields;
			buffer = datstr.ReadLine();
			fields = buffer.Split(' ');
			n = Int32.Parse(fields[0]);
			m = Int32.Parse(fields[1]);
			for (i = 0; i < n; ++i)
			{
				adder = new List<int>();
				buffer = datstr.ReadLine();
				fields = buffer.Split(' ');
				for (j = 0; j < m; ++j)
					adder.Add(Int32.Parse(fields[j]));
				map.Add(adder);
			}
			mapArea.Width = (double)m * pieceSize;
			mapArea.Height = (double)n * pieceSize;
			TextboxHeight.Text = n.ToString();
			TextboxWidth.Text = m.ToString();
			DrawCanvas();
			datstr.Close();
			datstr.Dispose();
			datinp.Close();
			datinp.Dispose();
		}
		private void MenuSave(object sender, RoutedEventArgs e)
		{
			if (filename == "")
				MenuSaveAs(sender, e);
			else
				SaveMapFile(filename);
		}
		private void MenuSaveAs(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dlgSaveFile = new SaveFileDialog();
			dlgSaveFile.DefaultExt = ".map";
			dlgSaveFile.Filter = "TankWars maps(*.map)|*.map";
			if (dlgSaveFile.ShowDialog().Value)
			{
				filename = dlgSaveFile.FileName;
			}
			if (filename == "") return;
			SaveMapFile(filename);
		}
		private void MenuLegendLoad(object sender, RoutedEventArgs e)
		{
			MenuOpen(sender, e);
			int i, j, mpiece, mpiecerotate;
			for (i = 0; i < n; ++i)
				for (j = 0; j < m; ++j)
				{
					mpiece = map[i][j];
					mpiecerotate = mpiece % 4;
					mpiece = mpiece / 4;
					switch (mpiece)
					{
						case 0: mpiece = 0; break;
						case 1: mpiece = 1; break;
						case 2: mpiece = 2; break;
						case 3: mpiece = 4; break;
						case 4: mpiece = 7; break;
						case 5: mpiece = 8; break;
						case 6: mpiece = 10; break;
						case 7: mpiece = 9; break;
						case 8: mpiece = 11; break;
						case 9: mpiece = 12; break;
						case 10: mpiece = 14; break;
						case 11: mpiece = 13; break;
						case 12: mpiece = 15; break;
						case 13: mpiece = 3; break;
						case 14: mpiece = 16; break;
						case 15: mpiece = 17; break;
						case 16: mpiece = 5; break;
						case 17: mpiece = 6; break;
						case 18: mpiece = 18; break;
						case 19: mpiece = 19; break;
						case 20: mpiece = 20; break;
						case 21: mpiece = 21; break;
					}
					mpiece = mpiece * 4 + mpiecerotate;
					map[i][j] = mpiece;
				}
			DrawCanvas();
		}
		private void MenuClose(object sender, RoutedEventArgs e)
		{
			Close();
		}
		#endregion

		#region innerFunctions

		#region initialization
		private void initElements()
		{
			initTexturesList();
			initMap();
		}
		private void initTexturesList()
		{
			FileStream datinp = new FileStream(@"TexturesList.txt", FileMode.Open, FileAccess.Read);
			StreamReader datstr = new StreamReader(datinp);
			int n, i;
			string buffer;
			string[] fields;
			Uri bitmapPath;
			texturesItem adder;
			BitmapImage badder = null;
			textures = new List<texturesItem>();
			texturesBitmap = new List<BitmapImage>();
			buffer = datstr.ReadLine();
			n = Int32.Parse(buffer);
			for (i = 0; i < n; ++i)
			{
				//add texturesItem
				adder = new texturesItem();
				adder.image = new BitmapImage();
				buffer = datstr.ReadLine();
				fields = buffer.Split(' ');
				adder.name = fields[0];
				bitmapPath = new Uri(fields[1], UriKind.Relative);
				adder.name = adder.name.Replace('_', ' ');
				adder.image.BeginInit();
				adder.image.UriSource = bitmapPath;
				adder.image.CacheOption = BitmapCacheOption.OnLoad;
				adder.image.EndInit();
				textures.Add(adder);
				//add texturesBitmap (rotates)
				badder = new BitmapImage();
				badder.BeginInit();
				badder.UriSource = bitmapPath;
				badder.CacheOption = BitmapCacheOption.OnLoad;
				badder.EndInit();
				texturesBitmap.Add(badder);
				badder = new BitmapImage();
				badder.BeginInit();
				badder.UriSource = bitmapPath;
				badder.CacheOption = BitmapCacheOption.OnLoad;
				badder.Rotation = Rotation.Rotate90;
				badder.EndInit();
				texturesBitmap.Add(badder);
				badder = new BitmapImage();
				badder.BeginInit();
				badder.UriSource = bitmapPath;
				badder.CacheOption = BitmapCacheOption.OnLoad;
				badder.Rotation = Rotation.Rotate180;
				badder.EndInit();
				texturesBitmap.Add(badder);
				badder = new BitmapImage();
				badder.BeginInit();
				badder.UriSource = bitmapPath;
				badder.CacheOption = BitmapCacheOption.OnLoad;
				badder.Rotation = Rotation.Rotate270;
				badder.EndInit();
				texturesBitmap.Add(badder);
			}
		}
		private void initMap()
		{
			int i, j;
			List<int> adder = null;
			if (map != null)
			{
				map = null;
				System.GC.Collect();
			}
			map = new List<List<int>>();
			for (i = 0; i < n; ++i)
			{
				adder = new List<int>();
				for (j = 0; j < m; ++j) adder.Add(pieceChoiced * 4 + pieceRotate);
				map.Add(adder);
			}
		}
		#endregion
		private void RotateTextures()
		{
			pieceRotate = (pieceRotate + 1) % 4;
			BitmapImage temp = new BitmapImage();
			temp.BeginInit();
			temp.UriSource = piece.UriSource;
			temp.CacheOption = BitmapCacheOption.OnLoad;
			switch (pieceRotate)
			{
				case 0: break;
				case 1: temp.Rotation = Rotation.Rotate90; break;
				case 2: temp.Rotation = Rotation.Rotate180; break;
				case 3: temp.Rotation = Rotation.Rotate270; break;
			}
			temp.EndInit();
			piece = temp;
			ImagePiece.Source = piece;
		}
		private void DrawCanvas()
		{
			int i, j;
			mapArea.Children.Clear();
			ImageBrush pbrush = null;
			Rectangle prect = null;
			for (i = 0; i < n; ++i)
			{
				for (j = 0; j < m; ++j)
				{
					prect = new Rectangle();
					pbrush = new ImageBrush();
					prect.Width = pieceSize;
					prect.Height = pieceSize;
					pbrush.ImageSource = texturesBitmap[map[i][j]];
					prect.Fill = pbrush;
					Canvas.SetTop(prect, i * pieceSize);
					Canvas.SetLeft(prect, j * pieceSize);
					mapArea.Children.Add(prect);
				}
			}
		}
		private void DrawChangedPiece(int newpiece, int x, int y)
		{
			ImageBrush pbrush = new ImageBrush();
			Rectangle prect = new Rectangle();
			prect.Width = pieceSize;
			prect.Height = pieceSize;
			pbrush.ImageSource = texturesBitmap[newpiece];
			prect.Fill = pbrush;
			Canvas.SetTop(prect, x * pieceSize);
			Canvas.SetLeft(prect, y * pieceSize);
			mapArea.Children.Add(prect);
		}
		private void ChangePieceSize(int size)
		{
			pieceSize = size;
			mapArea.Width = (double)m * pieceSize;
			mapArea.Height = (double)n * pieceSize;
			DrawCanvas();
		}
		private void SaveMapFile(string filename)
		{
			int i, j;
			TextWriter fout = new StreamWriter(filename);
			fout.WriteLine(n + " " + m);
			for (i = 0; i < n; ++i)
			{
				fout.Write(map[i][0]);
				for (j = 1; j < m; ++j)
					fout.Write(" " + map[i][j]);
				fout.WriteLine();
			}
			fout.Close();
			fout.Dispose();
		}
		#endregion
	}
}
