using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Libraria.Maths;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using Libraria.IO;
using NVector2 = System.Numerics.Vector2;

namespace Libraria.Rendering {
	public class GfxFont {
		public Texture2D FontAtlas;

		bool Dirty;
		HashSet<char> Characters;

		Font DrawingFont;

		RectanglePack RectPack;
		Dictionary<char, Rect> CharLocations;

		int FontSize;

		public float LineHeight { get; private set; }
		public int HalfPadding { get; private set; }

		public GfxFont(string FilePath, int Size = 16, int Padding = 2) {
			FontSize = Size;
			HalfPadding = Padding;

			FilePath = Path.GetFullPath(FilePath);
			PrivateFontCollection PFC = new PrivateFontCollection();
			PFC.AddFontFile(FilePath);

			DrawingFont = new Font(PFC.Families[0], FontSize);
			LineHeight = DrawingFont.Height;

			FontAtlas = new Texture2D(TexFilterMode.Linear);
			Characters = new HashSet<char>();
			Dirty = true;
		}

		public void LoadChar(char C) {
			if (char.IsControl(C))
				return;

			if (!Characters.Contains(C)) {
				Characters.Add(C);
				Dirty = true;
			}
		}

		public void LoadChar(string Chars) {
			for (int i = 0; i < Chars.Length; i++)
				LoadChar(Chars[i]);
		}

		/*bool IsMonospace(Graphics Gfx, Font F) {
			return Gfx.MeasureString("iiiii", F).Width == Gfx.MeasureString("MMMMM", F).Width;
		}*/

		public bool Update() {
			if (!Dirty)
				return false;
			Dirty = false;

			Bitmap Bmp = new Bitmap(1, 1);
			using (Graphics Gfx = Graphics.FromImage(Bmp)) {
				RectPack = new RectanglePack();

				//bool Monospace = IsMonospace(Gfx, DrawingFont);
				SizeF Sz = Gfx.MeasureString("X", DrawingFont);

				foreach (var Chr in Characters) {
					if (char.IsWhiteSpace(Chr))
						Sz = Gfx.MeasureString("_", DrawingFont);
					else
						Sz = Gfx.MeasureString(Chr.ToString(), DrawingFont);
			
					RectPack.Add(Chr, new NVector2(Sz.Width + HalfPadding * 2, Sz.Height + HalfPadding * 2));
				}

				CharLocations = RectPack.Pack();
			}

			Bmp = new Bitmap((int)RectPack.Size.X, (int)RectPack.Size.Y);
			using (Graphics Gfx = Graphics.FromImage(Bmp)) {
				Gfx.SmoothingMode = SmoothingMode.HighQuality;
				Gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
				Gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
				Gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				Gfx.Clear(Color.Transparent);
				//Gfx.Clear(Color.Black);

				Random Rnd = new Random(42);

				foreach (var Chr in Characters) {
					Rect R = CharLocations[Chr];

					// TODO: Convar to colorize
					/*Gfx.FillRectangle(new SolidBrush(Color.FromArgb(Rnd.Next(256), Rnd.Next(256), Rnd.Next(256))),
						new Rectangle((int)R.X, (int)R.Y, (int)R.W, (int)R.H));*/

					Gfx.DrawString(Chr.ToString(), DrawingFont, new SolidBrush(Color.FromArgb(255, 255, 255, 255)),
						R.X + HalfPadding, R.Y + HalfPadding);
				}
			}

			// TODO: Make a convar to control dumping the atlas
			FilePath.EnsureDirExists("debug");
			Bmp.Save("debug\\atlas.png");
			FontAtlas.LoadDataFromBitmap(Bmp);
			return true;
		}

		public void GetCharAtlasLocation(char C, out Vector2 Position, out Vector2 Size) {
			if (CharLocations.ContainsKey(C)) {
				Rect Loc = CharLocations[C];
				Position = new Vector2(Loc.X / FontAtlas.Width, Loc.Y / FontAtlas.Height);
				Size = new Vector2(Loc.W / FontAtlas.Width, Loc.H / FontAtlas.Height);
			} else {
				Position = Vector2.Zero;
				Size = Vector2.Zero;
			}
		}

		public float GetKerning(char A, char B) {
			return 0; // TODO
		}

		public float GetDescent(char A) {
			return 0; // TODO
		}

		public float GetCharWidth(char C) {
			if (CharLocations.ContainsKey(C))
				return CharLocations[C].W;
			return 0;
		}

		public float GetCharHeight(char C) {
			if (CharLocations.ContainsKey(C))
				return CharLocations[C].H;
			return 0;
		}
	}
}