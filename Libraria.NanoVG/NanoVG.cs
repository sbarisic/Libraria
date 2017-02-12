using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Libraria.NanoVG {
	[StructLayout(LayoutKind.Sequential)]
	public struct NVGcolor {
		public float R;
		public float G;
		public float B;
		public float A;

		public NVGcolor(float R, float G, float B, float A) {
			this.R = R;
			this.G = G;
			this.B = B;
			this.A = A;
		}

		public NVGcolor(byte R, byte G, byte B, byte A) : this(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f) {
		}

		public static implicit operator System.Drawing.Color(NVGcolor Clr) {
			return System.Drawing.Color.FromArgb((int)(Clr.A * 255), (int)(Clr.R * 255), (int)(Clr.G * 255), (int)(Clr.B * 255));
		}

		public static implicit operator NVGcolor(System.Drawing.Color Clr) {
			return new NVGcolor(Clr.R, Clr.G, Clr.B, Clr.A);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct NVGpaint {
		public fixed float XForm[6];
		public fixed float Extent[2];
		public float Radius;
		public float Feather;
		public NVGcolor InnerColor;
		public NVGcolor OuterColor;
		public int Image;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NVGglyphPosition {
		// Position of the glyph in the input string.
		public IntPtr Str;

		// The x-coordinate of the logical glyph position.
		public float X;

		// The bounds of the glyph shape.
		public float MinX, MaxX;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NVGtextRow {
		// Pointer to the input text where the row starts.
		public IntPtr Start;

		// Pointer to the input text where the row ends (one past the last character).
		public IntPtr End;

		// Pointer to the beginning of the next row.
		public IntPtr Next;

		// Logical width of the row.
		public float Width;

		// Actual bounds of the row. Logical with and bounds can differ because of kerning and some parts over extending.
		public float MinX, MaxX;
	}

	public static class NVG {
		const string DllName = "NanoVG";
		const CallingConvention CConv = CallingConvention.Cdecl;
		const CharSet CSet = CharSet.Ansi;

		// Flag indicating if geometry based anti-aliasing is used (may not be needed when using MSAA).
		public const int NVG_ANTIALIAS = 1 << 0;
		// Flag indicating if strokes should be drawn using stencil buffer. The rendering will be a little
		// slower, but path overlaps (i.e. self-intersecting or sharp turns) will be drawn just once.
		public const int NVG_STENCIL_STROKES = 1 << 1;
		// Flag indicating that additional debug checks are done.
		public const int NVG_DEBUG = 1 << 2;

		[DllImport(DllName, EntryPoint = "InitOpenGL", CallingConvention = CConv, CharSet = CSet)]
		public static extern bool InitOpenGL();

		[DllImport(DllName, EntryPoint = "nvgCreateGL3", CallingConvention = CConv, CharSet = CSet)]
		public static extern IntPtr CreateGL3(int flags);

		[DllImport(DllName, EntryPoint = "nvgDeleteGL3", CallingConvention = CConv, CharSet = CSet)]
		public static extern void DeleteGL3(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgBeginFrame", CallingConvention = CConv, CharSet = CSet)]
		public static extern void BeginFrame(IntPtr ctx, int windowWidth, int windowHeight, float devicePixelRatio);
		// NANOVG_EXPORT void nvgBeginFrame(IntPtr ctx, int windowWidth, int windowHeight, float devicePixelRatio);

		[DllImport(DllName, EntryPoint = "nvgCancelFrame", CallingConvention = CConv, CharSet = CSet)]
		public static extern void CancelFrame(IntPtr ctx);
		// NANOVG_EXPORT void nvgCancelFrame(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgEndFrame", CallingConvention = CConv, CharSet = CSet)]
		public static extern void EndFrame(IntPtr ctx);
		// NANOVG_EXPORT void nvgEndFrame(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgGlobalCompositeOperation", CallingConvention = CConv, CharSet = CSet)]
		public static extern void GlobalCompositeOperation(IntPtr ctx, int op);
		// NANOVG_EXPORT void nvgGlobalCompositeOperation(IntPtr ctx, int op);

		[DllImport(DllName, EntryPoint = "nvgGlobalCompositeBlendFunc", CallingConvention = CConv, CharSet = CSet)]
		public static extern void GlobalCompositeBlendFunc(IntPtr ctx, int sfactor, int dfactor);
		// NANOVG_EXPORT void nvgGlobalCompositeBlendFunc(IntPtr ctx, int sfactor, int dfactor);

		[DllImport(DllName, EntryPoint = "nvgGlobalCompositeBlendFuncSeparate", CallingConvention = CConv, CharSet = CSet)]
		public static extern void GlobalCompositeBlendFuncSeparate(IntPtr ctx, int srcRGB, int dstRGB, int srcAlpha, int dstAlpha);
		// NANOVG_EXPORT void nvgGlobalCompositeBlendFuncSeparate(IntPtr ctx, int srcRGB, int dstRGB, int srcAlpha, int dstAlpha);

		[DllImport(DllName, EntryPoint = "nvgRGB", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGcolor RGB(byte r, byte g, byte b);
		// NANOVG_EXPORT NVGcolor nvgRGB(unsigned char r, unsigned char g, unsigned char b);

		[DllImport(DllName, EntryPoint = "nvgRGBf", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGcolor RGBf(float r, float g, float b);
		// NANOVG_EXPORT NVGcolor nvgRGBf(float r, float g, float b);

		[DllImport(DllName, EntryPoint = "nvgRGBA", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGcolor RGBA(byte r, byte g, byte b, byte a);
		// NANOVG_EXPORT NVGcolor nvgRGBA(unsigned char r, unsigned char g, unsigned char b, unsigned char a);

		[DllImport(DllName, EntryPoint = "nvgRGBAf", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGcolor RGBAf(float r, float g, float b, float a);
		// NANOVG_EXPORT NVGcolor nvgRGBAf(float r, float g, float b, float a);

		[DllImport(DllName, EntryPoint = "nvgLerpRGBA", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGcolor LerpRGBA(NVGcolor c0, NVGcolor c1, float u);
		// NANOVG_EXPORT NVGcolor nvgLerpRGBA(NVGcolor c0, NVGcolor c1, float u);

		[DllImport(DllName, EntryPoint = "nvgTransRGBA", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGcolor TransRGBA(NVGcolor c0, byte a);
		// NANOVG_EXPORT NVGcolor nvgTransRGBA(NVGcolor c0, unsigned char a);

		[DllImport(DllName, EntryPoint = "nvgTransRGBAf", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGcolor TransRGBAf(NVGcolor c0, float a);
		// NANOVG_EXPORT NVGcolor nvgTransRGBAf(NVGcolor c0, float a);

		[DllImport(DllName, EntryPoint = "nvgHSL", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGcolor HSL(float h, float s, float l);
		// NANOVG_EXPORT NVGcolor nvgHSL(float h, float s, float l);

		[DllImport(DllName, EntryPoint = "nvgHSLA", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGcolor HSLA(float h, float s, float l, byte a);
		// NANOVG_EXPORT NVGcolor nvgHSLA(float h, float s, float l, unsigned char a);

		[DllImport(DllName, EntryPoint = "nvgSave", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Save(IntPtr ctx);
		// NANOVG_EXPORT void nvgSave(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgRestore", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Restore(IntPtr ctx);
		// NANOVG_EXPORT void nvgRestore(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgReset", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Reset(IntPtr ctx);
		// NANOVG_EXPORT void nvgReset(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgStrokeColor", CallingConvention = CConv, CharSet = CSet)]
		public static extern void StrokeColor(IntPtr ctx, NVGcolor color);
		// NANOVG_EXPORT void nvgStrokeColor(IntPtr ctx, NVGcolor color);

		[DllImport(DllName, EntryPoint = "nvgStrokePaint", CallingConvention = CConv, CharSet = CSet)]
		public static extern void StrokePaint(IntPtr ctx, NVGpaint paint);
		// NANOVG_EXPORT void nvgStrokePaint(IntPtr ctx, NVGpaint paint);

		[DllImport(DllName, EntryPoint = "nvgFillColor", CallingConvention = CConv, CharSet = CSet)]
		public static extern void FillColor(IntPtr ctx, NVGcolor color);
		// NANOVG_EXPORT void nvgFillColor(IntPtr ctx, NVGcolor color);

		[DllImport(DllName, EntryPoint = "nvgFillPaint", CallingConvention = CConv, CharSet = CSet)]
		public static extern void FillPaint(IntPtr ctx, NVGpaint paint);
		// NANOVG_EXPORT void nvgFillPaint(IntPtr ctx, NVGpaint paint);

		[DllImport(DllName, EntryPoint = "nvgMiterLimit", CallingConvention = CConv, CharSet = CSet)]
		public static extern void MiterLimit(IntPtr ctx, float limit);
		// NANOVG_EXPORT void nvgMiterLimit(IntPtr ctx, float limit);

		[DllImport(DllName, EntryPoint = "nvgStrokeWidth", CallingConvention = CConv, CharSet = CSet)]
		public static extern void StrokeWidth(IntPtr ctx, float size);
		// NANOVG_EXPORT void nvgStrokeWidth(IntPtr ctx, float size);

		[DllImport(DllName, EntryPoint = "nvgLineCap", CallingConvention = CConv, CharSet = CSet)]
		public static extern void LineCap(IntPtr ctx, int cap);
		// NANOVG_EXPORT void nvgLineCap(IntPtr ctx, int cap);

		[DllImport(DllName, EntryPoint = "nvgLineJoin", CallingConvention = CConv, CharSet = CSet)]
		public static extern void LineJoin(IntPtr ctx, int join);
		// NANOVG_EXPORT void nvgLineJoin(IntPtr ctx, int join);

		[DllImport(DllName, EntryPoint = "nvgGlobalAlpha", CallingConvention = CConv, CharSet = CSet)]
		public static extern void GlobalAlpha(IntPtr ctx, float alpha);
		// NANOVG_EXPORT void nvgGlobalAlpha(IntPtr ctx, float alpha);

		[DllImport(DllName, EntryPoint = "nvgResetTransform", CallingConvention = CConv, CharSet = CSet)]
		public static extern void ResetTransform(IntPtr ctx);
		// NANOVG_EXPORT void nvgResetTransform(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgTransform", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Transform(IntPtr ctx, float a, float b, float c, float d, float e, float f);
		// NANOVG_EXPORT void nvgTransform(IntPtr ctx, float a, float b, float c, float d, float e, float f);

		[DllImport(DllName, EntryPoint = "nvgTranslate", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Translate(IntPtr ctx, float x, float y);
		// NANOVG_EXPORT void nvgTranslate(IntPtr ctx, float x, float y);

		[DllImport(DllName, EntryPoint = "nvgRotate", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Rotate(IntPtr ctx, float angle);
		// NANOVG_EXPORT void nvgRotate(IntPtr ctx, float angle);

		[DllImport(DllName, EntryPoint = "nvgSkewX", CallingConvention = CConv, CharSet = CSet)]
		public static extern void SkewX(IntPtr ctx, float angle);
		// NANOVG_EXPORT void nvgSkewX(IntPtr ctx, float angle);

		[DllImport(DllName, EntryPoint = "nvgSkewY", CallingConvention = CConv, CharSet = CSet)]
		public static extern void SkewY(IntPtr ctx, float angle);
		// NANOVG_EXPORT void nvgSkewY(IntPtr ctx, float angle);

		[DllImport(DllName, EntryPoint = "nvgScale", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Scale(IntPtr ctx, float x, float y);
		// NANOVG_EXPORT void nvgScale(IntPtr ctx, float x, float y);

		[DllImport(DllName, EntryPoint = "nvgCurrentTransform", CallingConvention = CConv, CharSet = CSet)]
		public static extern void CurrentTransform(IntPtr ctx, float[] xform);
		// NANOVG_EXPORT void nvgCurrentTransform(IntPtr ctx, float* xform);

		[DllImport(DllName, EntryPoint = "nvgTransformIdentity", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TransformIdentity(float[] dst);
		// NANOVG_EXPORT void nvgTransformIdentity(float* dst);

		[DllImport(DllName, EntryPoint = "nvgTransformTranslate", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TransformTranslate(float[] dst, float tx, float ty);
		// NANOVG_EXPORT void nvgTransformTranslate(float* dst, float tx, float ty);

		[DllImport(DllName, EntryPoint = "nvgTransformScale", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TransformScale(float[] dst, float sx, float sy);
		// NANOVG_EXPORT void nvgTransformScale(float* dst, float sx, float sy);

		[DllImport(DllName, EntryPoint = "nvgTransformRotate", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TransformRotate(float[] dst, float a);
		// NANOVG_EXPORT void nvgTransformRotate(float* dst, float a);

		[DllImport(DllName, EntryPoint = "nvgTransformSkewX", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TransformSkewX(float[] dst, float a);
		// NANOVG_EXPORT void nvgTransformSkewX(float* dst, float a);

		[DllImport(DllName, EntryPoint = "nvgTransformSkewY", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TransformSkewY(float[] dst, float a);
		// NANOVG_EXPORT void nvgTransformSkewY(float* dst, float a);

		[DllImport(DllName, EntryPoint = "nvgTransformMultiply", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TransformMultiply(float[] dst, float[] src);
		// NANOVG_EXPORT void nvgTransformMultiply(float* dst, const float* src);

		[DllImport(DllName, EntryPoint = "nvgTransformPremultiply", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TransformPremultiply(float[] dst, float[] src);
		// NANOVG_EXPORT void nvgTransformPremultiply(float* dst, const float* src);

		[DllImport(DllName, EntryPoint = "nvgTransformInverse", CallingConvention = CConv, CharSet = CSet)]
		public static extern int TransformInverse(float[] dst, float[] src);
		// NANOVG_EXPORT int nvgTransformInverse(float* dst, const float* src);

		[DllImport(DllName, EntryPoint = "nvgTransformPoint", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TransformPoint(float[] dstx, float[] dsty, float[] xform, float srcx, float srcy);
		// NANOVG_EXPORT void nvgTransformPoint(float* dstx, float* dsty, const float* xform, float srcx, float srcy);

		[DllImport(DllName, EntryPoint = "nvgDegToRad", CallingConvention = CConv, CharSet = CSet)]
		public static extern float DegToRad(float deg);
		// NANOVG_EXPORT float nvgDegToRad(float deg);

		[DllImport(DllName, EntryPoint = "nvgRadToDeg", CallingConvention = CConv, CharSet = CSet)]
		public static extern float RadToDeg(float rad);
		// NANOVG_EXPORT float nvgRadToDeg(float rad);

		[DllImport(DllName, EntryPoint = "nvgCreateImage", CallingConvention = CConv, CharSet = CSet)]
		public static extern int CreateImage(IntPtr ctx, string filename, int imageFlags);
		// NANOVG_EXPORT int nvgCreateImage(IntPtr ctx, const char* filename, int imageFlags);

		[DllImport(DllName, EntryPoint = "nvgCreateImageMem", CallingConvention = CConv, CharSet = CSet)]
		public static extern int CreateImageMem(IntPtr ctx, int imageFlags, byte[] data, int ndata);
		// NANOVG_EXPORT int nvgCreateImageMem(IntPtr ctx, int imageFlags, unsigned char* data, int ndata);

		[DllImport(DllName, EntryPoint = "nvgCreateImageRGBA", CallingConvention = CConv, CharSet = CSet)]
		public static extern int CreateImageRGBA(IntPtr ctx, int w, int h, int imageFlags, byte[] data);
		// NANOVG_EXPORT 	int nvgCreateImageRGBA(IntPtr ctx, int w, int h, int imageFlags, const unsigned char* data);

		[DllImport(DllName, EntryPoint = "nvgUpdateImage", CallingConvention = CConv, CharSet = CSet)]
		public static extern void UpdateImage(IntPtr ctx, int image, byte[] data);
		// NANOVG_EXPORT void nvgUpdateImage(IntPtr ctx, int image, const unsigned char* data);

		[DllImport(DllName, EntryPoint = "nvgImageSize", CallingConvention = CConv, CharSet = CSet)]
		public static extern void ImageSize(IntPtr ctx, int image, int[] w, int[] h);
		// NANOVG_EXPORT void nvgImageSize(IntPtr ctx, int image, int* w, int* h);

		[DllImport(DllName, EntryPoint = "nvgDeleteImage", CallingConvention = CConv, CharSet = CSet)]
		public static extern void DeleteImage(IntPtr ctx, int image);
		// NANOVG_EXPORT 	void nvgDeleteImage(IntPtr ctx, int image);

		[DllImport(DllName, EntryPoint = "nvgLinearGradient", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGpaint LinearGradient(IntPtr ctx, float sx, float sy, float ex, float ey, NVGcolor icol, NVGcolor ocol);
		// NANOVG_EXPORT NVGpaint nvgLinearGradient(IntPtr ctx, float sx, float sy, float ex, float ey,  		NVGcolor icol, NVGcolor ocol);

		[DllImport(DllName, EntryPoint = "nvgBoxGradient", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGpaint BoxGradient(IntPtr ctx, float x, float y, float w, float h, float r, float f, NVGcolor icol, NVGcolor ocol);
		// NANOVG_EXPORT NVGpaint nvgBoxGradient(IntPtr ctx, float x, float y, float w, float h,  		float r, float f, NVGcolor icol, NVGcolor ocol);

		[DllImport(DllName, EntryPoint = "nvgRadialGradient", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGpaint RadialGradient(IntPtr ctx, float cx, float cy, float inr, float outr, NVGcolor icol, NVGcolor ocol);
		// NANOVG_EXPORT NVGpaint nvgRadialGradient(IntPtr ctx, float cx, float cy, float inr, float outr,  		NVGcolor icol, NVGcolor ocol);

		[DllImport(DllName, EntryPoint = "nvgImagePattern", CallingConvention = CConv, CharSet = CSet)]
		public static extern NVGpaint ImagePattern(IntPtr ctx, float ox, float oy, float ex, float ey, float angle, int image, float alpha);
		// NANOVG_EXPORT NVGpaint nvgImagePattern(IntPtr ctx, float ox, float oy, float ex, float ey,  		float angle, int image, float alpha);

		[DllImport(DllName, EntryPoint = "nvgScissor", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Scissor(IntPtr ctx, float x, float y, float w, float h);
		// NANOVG_EXPORT void nvgScissor(IntPtr ctx, float x, float y, float w, float h);

		[DllImport(DllName, EntryPoint = "nvgIntersectScissor", CallingConvention = CConv, CharSet = CSet)]
		public static extern void IntersectScissor(IntPtr ctx, float x, float y, float w, float h);
		// NANOVG_EXPORT void nvgIntersectScissor(IntPtr ctx, float x, float y, float w, float h);

		[DllImport(DllName, EntryPoint = "nvgResetScissor", CallingConvention = CConv, CharSet = CSet)]
		public static extern void ResetScissor(IntPtr ctx);
		// NANOVG_EXPORT void nvgResetScissor(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgBeginPath", CallingConvention = CConv, CharSet = CSet)]
		public static extern void BeginPath(IntPtr ctx);
		// NANOVG_EXPORT void nvgBeginPath(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgMoveTo", CallingConvention = CConv, CharSet = CSet)]
		public static extern void MoveTo(IntPtr ctx, float x, float y);
		// NANOVG_EXPORT void nvgMoveTo(IntPtr ctx, float x, float y);

		[DllImport(DllName, EntryPoint = "nvgLineTo", CallingConvention = CConv, CharSet = CSet)]
		public static extern void LineTo(IntPtr ctx, float x, float y);
		// NANOVG_EXPORT void nvgLineTo(IntPtr ctx, float x, float y);

		[DllImport(DllName, EntryPoint = "nvgBezierTo", CallingConvention = CConv, CharSet = CSet)]
		public static extern void BezierTo(IntPtr ctx, float c1x, float c1y, float c2x, float c2y, float x, float y);
		// NANOVG_EXPORT void nvgBezierTo(IntPtr ctx, float c1x, float c1y, float c2x, float c2y, float x, float y);

		[DllImport(DllName, EntryPoint = "nvgQuadTo", CallingConvention = CConv, CharSet = CSet)]
		public static extern void QuadTo(IntPtr ctx, float cx, float cy, float x, float y);
		// NANOVG_EXPORT void nvgQuadTo(IntPtr ctx, float cx, float cy, float x, float y);

		[DllImport(DllName, EntryPoint = "nvgArcTo", CallingConvention = CConv, CharSet = CSet)]
		public static extern void ArcTo(IntPtr ctx, float x1, float y1, float x2, float y2, float radius);
		// NANOVG_EXPORT void nvgArcTo(IntPtr ctx, float x1, float y1, float x2, float y2, float radius);

		[DllImport(DllName, EntryPoint = "nvgClosePath", CallingConvention = CConv, CharSet = CSet)]
		public static extern void ClosePath(IntPtr ctx);
		// NANOVG_EXPORT void nvgClosePath(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgPathWinding", CallingConvention = CConv, CharSet = CSet)]
		public static extern void PathWinding(IntPtr ctx, int dir);
		// NANOVG_EXPORT void nvgPathWinding(IntPtr ctx, int dir);

		[DllImport(DllName, EntryPoint = "nvgArc", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Arc(IntPtr ctx, float cx, float cy, float r, float a0, float a1, int dir);
		// NANOVG_EXPORT void nvgArc(IntPtr ctx, float cx, float cy, float r, float a0, float a1, int dir);

		[DllImport(DllName, EntryPoint = "nvgRect", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Rect(IntPtr ctx, float x, float y, float w, float h);
		// NANOVG_EXPORT void nvgRect(IntPtr ctx, float x, float y, float w, float h);

		[DllImport(DllName, EntryPoint = "nvgRoundedRect", CallingConvention = CConv, CharSet = CSet)]
		public static extern void RoundedRect(IntPtr ctx, float x, float y, float w, float h, float r);
		// NANOVG_EXPORT void nvgRoundedRect(IntPtr ctx, float x, float y, float w, float h, float r);

		[DllImport(DllName, EntryPoint = "nvgRoundedRectVarying", CallingConvention = CConv, CharSet = CSet)]
		public static extern void RoundedRectVarying(IntPtr ctx, float x, float y, float w, float h, float radTopLeft, float radTopRight, float radBottomRight, float radBottomLeft);
		// NANOVG_EXPORT void nvgRoundedRectVarying(IntPtr ctx, float x, float y, float w, float h, float radTopLeft, float radTopRight, float radBottomRight, float radBottomLeft);

		[DllImport(DllName, EntryPoint = "nvgEllipse", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Ellipse(IntPtr ctx, float cx, float cy, float rx, float ry);
		// NANOVG_EXPORT void nvgEllipse(IntPtr ctx, float cx, float cy, float rx, float ry);

		[DllImport(DllName, EntryPoint = "nvgCircle", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Circle(IntPtr ctx, float cx, float cy, float r);
		// NANOVG_EXPORT void nvgCircle(IntPtr ctx, float cx, float cy, float r);

		[DllImport(DllName, EntryPoint = "nvgFill", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Fill(IntPtr ctx);
		// NANOVG_EXPORT void nvgFill(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgStroke", CallingConvention = CConv, CharSet = CSet)]
		public static extern void Stroke(IntPtr ctx);
		// NANOVG_EXPORT void nvgStroke(IntPtr ctx);

		[DllImport(DllName, EntryPoint = "nvgCreateFont", CallingConvention = CConv, CharSet = CSet)]
		public static extern int CreateFont(IntPtr ctx, string name, string filename);
		// NANOVG_EXPORT int nvgCreateFont(IntPtr ctx, const char* name, const char* filename);

		[DllImport(DllName, EntryPoint = "nvgCreateFontMem", CallingConvention = CConv, CharSet = CSet)]
		public static extern int CreateFontMem(IntPtr ctx, string name, byte[] data, int ndata, int freeData);
		// NANOVG_EXPORT int nvgCreateFontMem(IntPtr ctx, const char* name, unsigned char* data, int ndata, int freeData);

		[DllImport(DllName, EntryPoint = "nvgFindFont", CallingConvention = CConv, CharSet = CSet)]
		public static extern int FindFont(IntPtr ctx, string name);
		// NANOVG_EXPORT int nvgFindFont(IntPtr ctx, const char* name);

		[DllImport(DllName, EntryPoint = "nvgAddFallbackFontId", CallingConvention = CConv, CharSet = CSet)]
		public static extern int AddFallbackFontId(IntPtr ctx, int baseFont, int fallbackFont);
		// NANOVG_EXPORT int nvgAddFallbackFontId(IntPtr ctx, int baseFont, int fallbackFont);

		[DllImport(DllName, EntryPoint = "nvgAddFallbackFont", CallingConvention = CConv, CharSet = CSet)]
		public static extern int AddFallbackFont(IntPtr ctx, string baseFont, string fallbackFont);
		// NANOVG_EXPORT int nvgAddFallbackFont(IntPtr ctx, const char* baseFont, const char* fallbackFont);

		[DllImport(DllName, EntryPoint = "nvgFontSize", CallingConvention = CConv, CharSet = CSet)]
		public static extern void FontSize(IntPtr ctx, float size);
		// NANOVG_EXPORT void nvgFontSize(IntPtr ctx, float size);

		[DllImport(DllName, EntryPoint = "nvgFontBlur", CallingConvention = CConv, CharSet = CSet)]
		public static extern void FontBlur(IntPtr ctx, float blur);
		// NANOVG_EXPORT void nvgFontBlur(IntPtr ctx, float blur);

		[DllImport(DllName, EntryPoint = "nvgTextLetterSpacing", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TextLetterSpacing(IntPtr ctx, float spacing);
		// NANOVG_EXPORT void nvgTextLetterSpacing(IntPtr ctx, float spacing);

		[DllImport(DllName, EntryPoint = "nvgTextLineHeight", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TextLineHeight(IntPtr ctx, float lineHeight);
		// NANOVG_EXPORT void nvgTextLineHeight(IntPtr ctx, float lineHeight);

		[DllImport(DllName, EntryPoint = "nvgTextAlign", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TextAlign(IntPtr ctx, int align);
		// NANOVG_EXPORT void nvgTextAlign(IntPtr ctx, int align);

		[DllImport(DllName, EntryPoint = "nvgFontFaceId", CallingConvention = CConv, CharSet = CSet)]
		public static extern void FontFaceId(IntPtr ctx, int font);
		// NANOVG_EXPORT void nvgFontFaceId(IntPtr ctx, int font);

		[DllImport(DllName, EntryPoint = "nvgFontFace", CallingConvention = CConv, CharSet = CSet)]
		public static extern void FontFace(IntPtr ctx, string font);
		// NANOVG_EXPORT void nvgFontFace(IntPtr ctx, const char* font);

		[DllImport(DllName, EntryPoint = "nvgText", CallingConvention = CConv, CharSet = CSet)]
		public static extern float Text(IntPtr ctx, float x, float y, string Str, string end);
		// NANOVG_EXPORT float nvgText(IntPtr ctx, float x, float y, const char* string, const char* end);

		[DllImport(DllName, EntryPoint = "nvgTextBox", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TextBox(IntPtr ctx, float x, float y, float breakRowWidth, string Str, string end);
		// NANOVG_EXPORT void nvgTextBox(IntPtr ctx, float x, float y, float breakRowWidth, const char* string, const char* end);

		[DllImport(DllName, EntryPoint = "nvgTextBounds", CallingConvention = CConv, CharSet = CSet)]
		public static extern float TextBounds(IntPtr ctx, float x, float y, string Str, string end, float[] bounds);
		// NANOVG_EXPORT float nvgTextBounds(IntPtr ctx, float x, float y, const char* string, const char* end, float* bounds);

		[DllImport(DllName, EntryPoint = "nvgTextBoxBounds", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TextBoxBounds(IntPtr ctx, float x, float y, float breakRowWidth, string Str, string end, float[] bounds);
		// NANOVG_EXPORT void nvgTextBoxBounds(IntPtr ctx, float x, float y, float breakRowWidth, const char* string, const char* end, float* bounds);

		[DllImport(DllName, EntryPoint = "nvgTextGlyphPositions", CallingConvention = CConv, CharSet = CSet)]
		public static extern int TextGlyphPositions(IntPtr ctx, float x, float y, string Str, string end, ref NVGglyphPosition positions, int maxPositions);
		// NANOVG_EXPORT int nvgTextGlyphPositions(IntPtr ctx, float x, float y, const char* string, const char* end, NVGglyphPosition* positions, int maxPositions);

		[DllImport(DllName, EntryPoint = "nvgTextMetrics", CallingConvention = CConv, CharSet = CSet)]
		public static extern void TextMetrics(IntPtr ctx, float[] ascender, float[] descender, float[] lineh);
		// NANOVG_EXPORT void nvgTextMetrics(IntPtr ctx, float* ascender, float* descender, float* lineh);

		[DllImport(DllName, EntryPoint = "nvgTextBreakLines", CallingConvention = CConv, CharSet = CSet)]
		public static extern int TextBreakLines(IntPtr ctx, string Str, string end, float breakRowWidth, ref NVGtextRow rows, int maxRows);
		// NANOVG_EXPORT int nvgTextBreakLines(IntPtr ctx, const char* string, const char* end, float breakRowWidth, NVGtextRow* rows, int maxRows);
	}
}