using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using IPixelFormat = System.Drawing.Imaging.PixelFormat;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Libraria.Rendering {
	public enum TexParam {
		WrapS = TextureParameterName.TextureWrapS,
		WrapT = TextureParameterName.TextureWrapT,
		WrapR = TextureParameterName.TextureWrapR,
		WrapX = WrapS, WrapY = WrapT, WrapZ = WrapR,
		MinFilter = TextureParameterName.TextureMinFilter,
		MagFilter = TextureParameterName.TextureMagFilter
	}

	public enum TexWrapMode {
		Clamp = 10496,
		Repeat = 10497,
		ClampToBorder = 33069,
		ClampToEdge = 33071,
		MirroredRepeat = 33648
	}

	public enum TexFilterMode {
		Nearest = All.Nearest,
		Linear = All.Linear,
		NearestMipmapNearest = All.NearestMipmapNearest,
		LinearMipmapNearest = All.LinearMipmapNearest,
		LinearMipmapLinear = All.LinearMipmapLinear,
		NearestMipmapLinear = All.NearestMipmapLinear
	}

	public class Texture2D {
		public static Texture2D Mask_Tex0, Mask_Tex1, Mask_Tex2, Mask_Tex3;

		static Texture2D() {
			Mask_Tex0 = FromBitmap(CreateBitmap(Color.FromArgb(255, 0, 0, 0)));
			Mask_Tex1 = FromBitmap(CreateBitmap(Color.FromArgb(255, 255, 0, 0)));
			Mask_Tex2 = FromBitmap(CreateBitmap(Color.FromArgb(255, 0, 255, 0)));
			Mask_Tex3 = FromBitmap(CreateBitmap(Color.FromArgb(255, 0, 0, 255)));
		}

		static Bitmap CreateBitmap(Color Clr) {
			Bitmap Bmp = new Bitmap(1, 1);
			using (Graphics Gfx = Graphics.FromImage(Bmp))
				Gfx.FillRegion(new SolidBrush(Clr), new Region(new Rectangle(0, 0, 1, 1)));
			return Bmp;
		}

		public static Texture2D FromFile(string Pth, TexFilterMode FilterMode = TexFilterMode.Linear,
			TexWrapMode WrapMode = TexWrapMode.ClampToEdge, bool GenerateMipmap = true) {

			return FromBitmap(new Bitmap(Pth), FilterMode, WrapMode, GenerateMipmap);
		}

		public static Texture2D FromBitmap(Bitmap BMap, TexFilterMode FilterMode = TexFilterMode.Linear,
			TexWrapMode WrapMode = TexWrapMode.ClampToEdge, bool GenerateMipmap = true) {

			Texture2D Tex = new Texture2D(FilterMode, WrapMode);
			Tex.LoadDataFromBitmap(BMap);

			if (GenerateMipmap)
				Tex.GenerateMipmap();

			return Tex;
		}

		public int ID;
		public int TexUnit;
		public int Width, Height;
		public TextureTarget Target { get; private set; } = TextureTarget.Texture2D;
		public Vector2 Size { get { return new Vector2(Width, Height); } }

		public Texture2D(TexFilterMode FilterMode, TexWrapMode WrapMode = TexWrapMode.ClampToEdge) {
			ID = GL.GenTexture();
			Bind();

			SetParam(TexParam.WrapS, WrapMode);
			SetParam(TexParam.WrapT, WrapMode);
			SetParam(TexParam.MinFilter, FilterMode);
			SetParam(TexParam.MagFilter, FilterMode);
		}

		public Texture2D(TexFilterMode FilterMode, int W, int H, TexWrapMode WrapMode = TexWrapMode.ClampToEdge) : this(FilterMode, WrapMode) {
			LoadEmptyData(W, H);
		}

		public void Bind(int TexUnit = 0) {
			if (TexUnit < 0 || TexUnit > 31)
				throw new Exception("Invalid texture unit " + TexUnit);

			this.TexUnit = TexUnit;

			GL.ActiveTexture(TextureUnit.Texture0 + TexUnit);
			GL.BindTexture(Target, ID);
		}

		public void Unbind() {
			Unbind(TexUnit);
		}

		public void Unbind(int TexUnit) {
			GL.ActiveTexture(TextureUnit.Texture0 + TexUnit);
			GL.BindTexture(Target, 0);
		}

		public void GenerateMipmap() {
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}

		public void LoadData(Texture2D Tex) {
			if (Tex.Width != Width || Tex.Height != Height)
				throw new Exception("Cannot copy data from two images of different sizes");
			GL.CopyImageSubData(Tex.ID, ImageTarget.Texture2D, 0, 0, 0, 0, ID, ImageTarget.Texture2D, 0, 0, 0, 0, Width, Height, 1);
		}

		public void LoadEmptyData(int W, int H) {
			Width = W;
			Height = H;
			GL.TexImage2D(Target, 0, PixelInternalFormat.Rgba, W, H, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
		}

		public void LoadDataFromFile(string Pth) {
			LoadDataFromBitmap(new Bitmap(Pth));
		}

		public void LoadDataFromBitmap(Bitmap BMap) {
			Width = BMap.Width;
			Height = BMap.Height;

			BitmapData BDta = BMap.LockBits(new Rectangle(0, 0, BMap.Width, BMap.Height),
							ImageLockMode.ReadOnly, IPixelFormat.Format32bppArgb);

			GL.TexImage2D(Target, 0, PixelInternalFormat.Rgba, BMap.Width, BMap.Height, 0,
				PixelFormat.Bgra, PixelType.UnsignedByte, BDta.Scan0);

			BMap.UnlockBits(BDta);
		}

		public void SetParam(TexParam Param, int Val) {
			GL.TexParameter(Target, (TextureParameterName)Param, Val);
		}

		public void SetParam(TexParam Param, TexWrapMode WrapMode) {
			SetParam(Param, (int)WrapMode);
		}

		public void SetParam(TexParam Param, TexFilterMode WrapMode) {
			SetParam(Param, (int)WrapMode);
		}

		public void Destroy() {
			GL.DeleteTexture(ID);
		}
	}
}
