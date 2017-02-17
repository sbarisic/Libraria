using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libraria.NanoVG;
using Libraria.Rendering;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Globalization;

namespace LibTech {
	public static partial class Engine {
		internal static IntPtr NVGCtx;
	}
}

namespace LibTech.Rendering {
	[Flags]
	public enum TextAlign : int {
		Left = 1 << 0,  // Default, align text horizontally to left.
		Center = 1 << 1, // Align text horizontally to center.
		Right = 1 << 2, // Align text horizontally to right.
						// Vertical align
		Top = 1 << 3,   // Align text vertically to top.
		Middle = 1 << 4,// Align text vertically to middle.
		Bottom = 1 << 5, // Align text vertically to bottom.
		Baseline = 1 << 6,// Default, align text vertically to baseline.

		TopLeft = Top | Left,
		BottomLeft = Bottom | Left,
		CenterMiddle = Center | Middle,
	}

	public static partial class NanoVG {
		public static float AspectRatio;

		public static int Width {
			get {
				return (int)(Height * Engine.RenderWindow.AspectRatio);
			}
		}

		public static int Height {
			get {
				return 600;
			}
		}

		internal static void Initialize() {
			NVG.InitOpenGL();
			Engine.NVGCtx = NVG.CreateGL3(NVG.NVG_DEBUG | NVG.NVG_ANTIALIAS);

			string[] Fonts = ContentManager.GetAllFonts();
			List<string> LoadedFonts = new List<string>();

			foreach (var Font in Fonts) {
				string FontName = Files.GetFileName(Font);
				NVG.CreateFont(Engine.NVGCtx, FontName, Font);
				LoadedFonts.Add(FontName);
			}

			// Every font is the fallback for every other font. TODO: Is this even a good idea?
			for (int i = 0; i < LoadedFonts.Count; i++) {
				for (int j = 0; j < LoadedFonts.Count; j++) {
					if (i == j)
						continue;

					NVG.AddFallbackFont(Engine.NVGCtx, LoadedFonts[i], LoadedFonts[j]);
				}
			}

			NVG.ImagePattern(Engine.NVGCtx, 0, 0, 100, 100, 0, 0, 0);
		}

		public static int CreateImage(Texture2D Tex, bool FlipY = false) {
			int Flags = 0;
			if (FlipY)
				Flags = NVG.NVG_IMAGE_FLIPY;

			return NVG.CreateImageFromHandleGL3(Engine.NVGCtx, Tex.ID, Tex.Width, Tex.Height, Flags);
		}

		public static int CreateImage(RenderTexture RT) {
			return CreateImage(RT.Texture, true);
		}

		public static void BeginFrame(int W, int H, float PxRatio) {
			NVG.BeginFrame(Engine.NVGCtx, W, H, PxRatio);
		}

		public static void BeginFrame(int W, int H) {
			float PxRatio = 0;

			if (RenderTexture.Current == null)
				PxRatio = (float)Engine.RenderWindow.Width / W;
			else
				PxRatio = (float)RenderTexture.Current.Width / W;

			BeginFrame(W, H, PxRatio);
		}

		public static void BeginFrame(int H) {
			BeginFrame((int)(H * Engine.RenderWindow.AspectRatio), H);
		}

		public static void BeginFrame() {
			//int W = (int)(Size * Engine.RenderWindow.AspectRatio);
			//int H = Size;
			BeginFrame(Width, Height);
		}

		public static void EndFrame() {
			NVG.EndFrame(Engine.NVGCtx);
		}

		public static void FontFace(string Name) {
			NVG.FontFace(Engine.NVGCtx, Name);
		}

		public static void FontSize(float Size = 16.0f) {
			NVG.FontSize(Engine.NVGCtx, Size);
		}

		public static void TextAlign(TextAlign Align) {
			NVG.TextAlign(Engine.NVGCtx, (int)Align);
		}

		public static void Text(float X, float Y, string Txt) {
			NVG.Text(Engine.NVGCtx, X, Y, Txt, IntPtr.Zero);
		}

		public static void FillColor(Color Clr) {
			NVG.FillColor(Engine.NVGCtx, Clr);
		}

		public static void FillColor(byte R, byte G, byte B, byte A) {
			NVG.FillColor(Engine.NVGCtx, new NVGcolor(R, G, B, A));
		}

		public static void FillColor(int RGBA) {
			FillColor((byte)((RGBA >> 24) & 0xFF), (byte)((RGBA >> 16) & 0xFF), (byte)((RGBA >> 8) & 0xFF), (byte)((RGBA) & 0xFF));
		}

		public static void FillPaint(NVGpaint Paint) {
			NVG.FillPaint(Engine.NVGCtx, Paint);
		}

		public static void Fill() {
			NVG.Fill(Engine.NVGCtx);
		}

		public static void StrokeColor(Color Clr) {
			NVG.StrokeColor(Engine.NVGCtx, Clr);
		}

		public static void StrokeWidth(float W) {
			NVG.StrokeWidth(Engine.NVGCtx, W);
		}

		public static void Stroke() {
			NVG.Stroke(Engine.NVGCtx);
		}

		public static void BeginPath() {
			NVG.BeginPath(Engine.NVGCtx);
		}

		public static void Rect(float X, float Y, float W, float H, float R = 0) {
			if (R == 0)
				NVG.Rect(Engine.NVGCtx, X, Y, W, H);
			else
				NVG.RoundedRect(Engine.NVGCtx, X, Y, W, H, R);
		}

		public static void Save() {
			NVG.Save(Engine.NVGCtx);
		}

		public static void Restore() {
			NVG.Restore(Engine.NVGCtx);
		}

		public static void Rotate(float Rad) {
			NVG.Rotate(Engine.NVGCtx, Rad);
		}

		public static void Translate(float X, float Y) {
			NVG.Translate(Engine.NVGCtx, X, Y);
		}

		public static NVGpaint ImagePattern(int OriginX, int OriginY, int Width, int Height, float Angle, int Image, float Alpha = 1.0f) {
			return NVG.ImagePattern(Engine.NVGCtx, OriginX, OriginY, Width, Height, Angle, Image, Alpha);
		}

		public static void FontBlur(float Amt) {
			NVG.FontBlur(Engine.NVGCtx, Amt);
		}

		public static void TextMetrics(float[] Ascender, float[] Descender, float[] LineH) {
			NVG.TextMetrics(Engine.NVGCtx, Ascender, Descender, LineH);
		}

		public static NVGglyphPosition[] TextGlyphPositions(float X, float Y, string Txt) {
			NVGglyphPosition[] GlyphPositions = new NVGglyphPosition[Txt.Length];
			NVG.TextGlyphPositions(Engine.NVGCtx, X, Y, Txt, IntPtr.Zero, GlyphPositions, GlyphPositions.Length);
			return GlyphPositions;
		}

		public static float[] TextBounds(float X, float Y, string Txt, out float W, out float H) {
			float[] Bounds = new float[4];
			NVG.TextBounds(Engine.NVGCtx, X, Y, Txt, IntPtr.Zero, Bounds);

			W = Bounds[2] - Bounds[0];
			H = Bounds[3] - Bounds[1];
			return Bounds;
		}
	}

	public static partial class NanoVG {
		public static void DrawRectOutline(Color Clr, float LineWidth, float X, float Y, float W, float H, float R = 0) {
			BeginPath();
			StrokeColor(Clr);
			StrokeWidth(LineWidth);
			Rect(X, Y, W, H, R);
			Stroke();
		}

		public static void DrawRect(Color Clr, float X, float Y, float W, float H, float R = 0) {
			BeginPath();
			FillColor(Clr);
			Rect(X, Y, W, H, R);
			Fill();
		}

		public static void DrawTexturedRect(int Img, int X, int Y, int W, int H, float R = 0) {
			BeginPath();
			Rect(X, Y, W, H, R);

			NVGpaint Paint = ImagePattern(X, Y, W, H, 0, Img);
			FillPaint(Paint);
			Fill();
		}

		public static void DrawText(string Font, float Size, TextAlign Align, Color Color, float X, float Y, string Str, float Blur = 0, int BlurPasses = 0) {
			SetFont(Font, Size);
			TextAlign(Align);
			FillColor(Color);

			if (Blur != 0)
				FontBlur(Blur);

			Text(X, Y, Str);

			if (Blur != 0) {
				FontBlur(0);
				if (BlurPasses > 0)
					DrawText(Font, Size, Align, Color, X, Y, Str, Blur, BlurPasses - 1);
			}
		}

		public static void StyledText(float X, float Y, string Txt) {
			const string ColorRegex = "\\^[0-9a-fA-F]{6}";

			Match[] Matches = Regex.Matches(Txt, ColorRegex).Cast<Match>().ToArray();
			if (Matches.Length == 0) {
				Text(X, Y, Txt);
				return;
			}

			string[] TxtParts = Regex.Split(Txt, ColorRegex, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);
			//Txt = Regex.Replace(Txt, ColorRegex, "");

			for (int i = 1; i < TxtParts.Length; i++) {
				Match M = Matches[i - 1];
				string Str = TxtParts[i];
				if (Str.Length == 0)
					continue;

				FillColor((int.Parse(M.Value.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture) << 8) | 0xFF);
				Text(X, Y, Str);

				float W, H;
				TextBounds(X, Y, Str, out W, out H);
				X += W;
			}
		}

		public static float GetLineHeight() {
			float[] LineH = new float[1];
			TextMetrics(null, null, LineH);
			return LineH[0];
		}

		public static void DrawParagraph(string Txt, float X, float Y, float Width, int LineStart = 0, int LineCount = -1) {
			Save();

			TextAlign(Rendering.TextAlign.TopLeft);
			float LineH = GetLineHeight();

			string[] Lines = Txt.Split('\n');
			if (LineCount == -1)
				LineCount = Lines.Length;

			for (int i = 0; i < LineCount; i++) {
				StyledText(X, Y + LineH * i, Lines[i]);
			}

			Restore();
		}

		public static void RotateAround(float X, float Y, float Rad) {
			Translate(X, Y);
			Rotate(Rad);
			Translate(-X, -Y);
		}

		public static void SetFont(string Name, float Size) {
			FontFace(Name);
			FontSize(Size);
		}
	}
}
