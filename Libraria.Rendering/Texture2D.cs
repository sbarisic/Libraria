﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using IPixelFormat = System.Drawing.Imaging.PixelFormat;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System.Runtime.InteropServices;
using System.IO;

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

	public unsafe class Texture2D : OpenGLGC.Destructable {
		const long TextureHandleNotCreated = -2;

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
			TexWrapMode WrapMode = TexWrapMode.ClampToEdge, bool GenerateMipmap = true, bool UseSRGBA = false) {

			if (!File.Exists(Pth))
				throw new FileNotFoundException("Texture file not found", Pth);
			return FromBitmap(new Bitmap(Pth), FilterMode, WrapMode, GenerateMipmap, UseSRGBA);
		}

		public static Texture2D FromBitmap(Bitmap BMap, TexFilterMode FilterMode = TexFilterMode.Linear,
			TexWrapMode WrapMode = TexWrapMode.ClampToEdge, bool GenerateMipmap = true, bool UseSRGBA = false) {

			Texture2D Tex = new Texture2D(FilterMode, WrapMode, UseSRGBA);
			Tex.LoadDataFromBitmap(BMap);

			if (GenerateMipmap)
				Tex.GenerateMipmap();

			Tex.Unbind();
			return Tex;
		}

		public long TextureHandle;
		public int ID;
		public int TexUnit;
		public int Width, Height;
		public TextureTarget Target { get; private set; } = TextureTarget.Texture2D;
		public Vector2 Size { get { return new Vector2(Width, Height); } }
		public bool UseSRGBA;

		public bool Resident {
			get {
				if (TextureHandle == TextureHandleNotCreated)
					return false;
				return GL.Arb.IsTextureHandleResident(TextureHandle);
			}

			set {
				if (TextureHandle == TextureHandleNotCreated)
					TextureHandle = GL.Arb.GetTextureHandle(ID);

				if (value && !Resident)
					GL.Arb.MakeTextureHandleResident(TextureHandle);
				else if (!value && Resident)
					GL.Arb.MakeTextureHandleNonResident(TextureHandle);
			}
		}

		bool WasFinalized;
		~Texture2D() {
			OpenGLGC.Enqueue(this, ref WasFinalized);
		}

		public Texture2D(TexFilterMode FilterMode = TexFilterMode.Nearest, TexWrapMode WrapMode = TexWrapMode.ClampToEdge, bool UseSRGBA = false) {
			this.UseSRGBA = UseSRGBA;
			TextureHandle = TextureHandleNotCreated;

			GL.CreateTextures(Target, 1, out ID);

			Bind();

			SetParam(TexParam.WrapS, WrapMode);
			SetParam(TexParam.WrapT, WrapMode);
			SetFilterMode(FilterMode, FilterMode);
		}

		public Texture2D(TexFilterMode FilterMode, int W, int H, TexWrapMode WrapMode = TexWrapMode.ClampToEdge, bool UseSRGBA = false) : this(FilterMode, WrapMode, UseSRGBA) {
			LoadData(W, H, IntPtr.Zero);
		}

		public void SetFilterMode(TexFilterMode Mag = TexFilterMode.Nearest, TexFilterMode Min = TexFilterMode.Nearest) {
			SetParam(TexParam.MagFilter, Mag);
			SetParam(TexParam.MinFilter, Min);
		}

		public void Bind(int TexUnit = 0) {
			if (TexUnit < 0 || TexUnit > 31)
				throw new Exception("Invalid texture unit " + TexUnit);

			this.TexUnit = TexUnit;

			//GL.ActiveTexture(TextureUnit.Texture0 + TexUnit);
			//GL.BindTexture(Target, ID);
			GL.BindTextureUnit(TexUnit, ID);
		}

		public void Unbind() {
			Unbind(TexUnit);
		}

		public void Unbind(int TexUnit) {
			//GL.ActiveTexture(TextureUnit.Texture0 + TexUnit);
			//GL.BindTexture(Target, 0);
			GL.BindTextureUnit(TexUnit, 0);
		}

		public void GenerateMipmap() {
			//GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			GL.GenerateTextureMipmap(ID);
		}

		public void LoadData(Texture2D Tex) {
			if (Tex.Width != Width || Tex.Height != Height)
				throw new Exception("Cannot copy data from two images of different sizes");
			GL.CopyImageSubData(Tex.ID, ImageTarget.Texture2D, 0, 0, 0, 0, ID, ImageTarget.Texture2D, 0, 0, 0, 0, Width, Height, 1);
		}

		public void LoadData(int W, int H, IntPtr Data, PixelInternalFormat InternalFormat = PixelInternalFormat.Rgba, PixelFormat PixFormat = PixelFormat.Rgba,
			PixelType PixType = PixelType.UnsignedByte) {

			Width = W;
			Height = H;

			GL.TexImage2D(Target, 0, UseSRGBA ? PixelInternalFormat.SrgbAlpha : InternalFormat, W, H, 0, PixFormat, PixType, Data);
		}

		public void LoadDataFromVectorArray(int W, int H, Vector3[] Array) {
			fixed (Vector3* ArrayPtr = Array)
				LoadData(W, H, new IntPtr(ArrayPtr), PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.Float);
		}

		public void LoadDataFromFile(string Pth) {
			LoadDataFromBitmap(new Bitmap(Pth));
		}

		public void LoadDataFromBitmap(Bitmap BMap, PixelInternalFormat InternalFormat = PixelInternalFormat.Rgba,
			PixelFormat PixFormat = PixelFormat.Rgba, PixelType PixType = PixelType.UnsignedByte) {

			LoadData(BMap.Width, BMap.Height, IntPtr.Zero, InternalFormat, PixFormat, PixType);

			BitmapData BDta = BMap.LockBits(new Rectangle(0, 0, BMap.Width, BMap.Height),
							ImageLockMode.ReadOnly, IPixelFormat.Format32bppArgb);
			GL.TexSubImage2D(Target, 0, 0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte, BDta.Scan0);

			BMap.UnlockBits(BDta);
		}

		public void SetParam(TexParam Param, int Val) {
			GL.TextureParameter(ID, (TextureParameterName)Param, Val);
		}

		public void SetParam(TexParam Param, TexWrapMode WrapMode) {
			SetParam(Param, (int)WrapMode);
		}

		public void SetParam(TexParam Param, TexFilterMode WrapMode) {
			SetParam(Param, (int)WrapMode);
		}

		public void Destroy() {
			if (Resident)
				Resident = false;
			GL.DeleteTexture(ID);
		}
	}
}
