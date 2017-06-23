using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace Libraria.Rendering {
	public class PBRMaterial {
		static Bitmap FromFileIfExists(string Pth) {
			if (!File.Exists(Pth))
				return null;

			return new Bitmap(Pth);
		}

		public static PBRMaterial FromDirectory(string Pth) {
			Bitmap Albedo = FromFileIfExists(Path.Combine(Pth, "albedo.png"));
			Bitmap Metallic = FromFileIfExists(Path.Combine(Pth, "metallic.png"));
			Bitmap Normal = FromFileIfExists(Path.Combine(Pth, "normal.png"));
			Bitmap Roughness = FromFileIfExists(Path.Combine(Pth, "roughness.png"));
			Bitmap AO = FromFileIfExists(Path.Combine(Pth, "ao.png"));
			Bitmap Emissive = FromFileIfExists(Path.Combine(Pth, "emissive.png"));
			Bitmap Height = FromFileIfExists(Path.Combine(Pth, "height.png"));
			return new PBRMaterial(Albedo, Metallic, Normal, Roughness, AO, Emissive, Height);
		}

		public Texture2D MaterialAtlasTexture;

		public PBRMaterial(Bitmap Albedo, Bitmap Metallic = null, Bitmap Normal = null, Bitmap Roughness = null, Bitmap AO = null, Bitmap Emissive = null, Bitmap Height = null) {
			if (Albedo == null)
				throw new Exception("Albedo map cannot be null");

			AssertSizeMatch(Albedo, Metallic, "Metallic size does not match Albedo");
			AssertSizeMatch(Albedo, Normal, "Normal size does not match Albedo");
			AssertSizeMatch(Albedo, Roughness, "Roughness size does not match Albedo");
			AssertSizeMatch(Albedo, AO, "AO size does not match Albedo");
			AssertSizeMatch(Albedo, Emissive, "Emissive size does not match Albedo");
			AssertSizeMatch(Albedo, Height, "Height size does not match Albedo");


			Bitmap MaterialAtlas = new Bitmap(Albedo.Width * 3, Albedo.Height * 3);
			using (Graphics Gfx = Graphics.FromImage(MaterialAtlas)) {
				Gfx.Clear(Color.Transparent);
				DrawImage(Gfx, MaterialAtlas.Height, Albedo, 0, 0);

				if (Metallic != null)
					DrawImage(Gfx, MaterialAtlas.Height, Metallic, Albedo.Width * 1, Albedo.Height * 0);

				if (Normal != null)
					DrawImage(Gfx, MaterialAtlas.Height, Normal, Albedo.Width * 2, Albedo.Height * 0);

				if (Roughness != null)
					DrawImage(Gfx, MaterialAtlas.Height, Roughness, Albedo.Width * 0, Albedo.Height * 1);

				if (AO != null)
					DrawImage(Gfx, MaterialAtlas.Height, AO, Albedo.Width * 1, Albedo.Height * 1);

				if (Emissive != null)
					DrawImage(Gfx, MaterialAtlas.Height, Emissive, Albedo.Width * 2, Albedo.Height * 1);

				if (Height != null)
					DrawImage(Gfx, MaterialAtlas.Height, Height, Albedo.Width * 0, Albedo.Height * 2);
			}

			MaterialAtlas.Save("out.png");
			MaterialAtlasTexture = Texture2D.FromBitmap(MaterialAtlas);
		}

		void DrawImage(Graphics Gfx, int H, Image Img, int U, int V) {
			//Gfx.DrawImage(Img, U, H - V - Img.Height, Img.Width, Img.Height);
			Gfx.DrawImage(Img, U, V, Img.Width, Img.Height);
		}

		void AssertSizeMatch(Bitmap A, Bitmap B, string Msg) {
			if (A == null || B == null)
				return;

			if (!(A.Width == B.Width && A.Height == B.Height))
				throw new Exception(Msg);
		}
	}
}
