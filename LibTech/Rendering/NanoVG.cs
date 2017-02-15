using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libraria.NanoVG;
using System.Drawing;
using Libraria.Rendering;

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

		public static void BeginFrame(int W, int H, float PxRatio = 1.0f) {
			NVG.BeginFrame(Engine.NVGCtx, W, H, PxRatio);
		}

		public static void BeginFrame(int Size = 600) {
			int W = (int)(Size * Engine.RenderWindow.AspectRatio);
			int H = Size;
			float PxRatio = 0;

			if (RenderTexture.Current == null)
				PxRatio = (float)Engine.RenderWindow.Width / W;
			else
				PxRatio = (float)RenderTexture.Current.Width / W;

			BeginFrame(W, H, PxRatio);
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
	}

	public static partial class NanoVG {
		public static void DrawRectOutline(Color Clr, float LineWidth, float X, float Y, float W, float H, float R = 0) {
			BeginPath();
			StrokeColor(Clr);
			StrokeWidth(LineWidth);
			Rect(X, Y, W, H, R);
			Stroke();
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
