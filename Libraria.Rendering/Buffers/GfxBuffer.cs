using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Libraria.Rendering {
	public enum VertexUsageHint {
		StreamDraw = 35040,
		StreamRead = 35041,
		StreamCopy = 35042,
		StaticDraw = 35044,
		StaticRead = 35045,
		StaticCopy = 35046,
		DynamicDraw = 35048,
		DynamicRead = 35049,
		DynamicCopy = 35050
	}

	public enum AttribPointerType {
		Byte = 5120,
		UnsignedByte = 5121,
		Short = 5122,
		UnsignedShort = 5123,
		Int = 5124,
		UnsignedInt = 5125,
		Float = 5126,
		Double = 5130,
		HalfFloat = 5131,
		Fixed = 5132,
		UnsignedInt2101010Rev = 33640,
		Int2101010Rev = 36255
	}

	public enum DrawPrimitiveType {
		Points = 0,
		Lines = 1,
		LineLoop = 2,
		LineStrip = 3,
		Triangles = 4,
		TriangleStrip = 5,
		TriangleFan = 6,
		Quads = 7,
		QuadsExt = 7,
		QuadStrip = 8,
		Polygon = 9,
		LinesAdjacency = 10,
		LinesAdjacencyArb = 10,
		LinesAdjacencyExt = 10,
		LineStripAdjacency = 11,
		LineStripAdjacencyArb = 11,
		LineStripAdjacencyExt = 11,
		TrianglesAdjacency = 12,
		TrianglesAdjacencyArb = 12,
		TrianglesAdjacencyExt = 12,
		TriangleStripAdjacency = 13,
		TriangleStripAdjacencyArb = 13,
		TriangleStripAdjacencyExt = 13,
		Patches = 14,
		PatchesExt = 14
	}

	public abstract class OpenGLBuffer<T> {
		public int ID;

		public abstract T Bind();
		public abstract T Unbind();
		public abstract void Destroy();
	}

	public abstract class GfxBuffer : OpenGLBuffer<GfxBuffer> {
		public int Stride;
		public BufferTarget Tgt;

		int Length;

		public GfxBuffer(BufferTarget Tgt, int Stride) {
			this.Tgt = Tgt;
			this.Stride = Stride;
			ID = GL.GenBuffer();
			Bind();
		}

		public override GfxBuffer Bind() {
			GL.BindBuffer(Tgt, ID);
			return this;
		}

		public override GfxBuffer Unbind() {
			GL.BindBuffer(Tgt, 0);
			return this;
		}

		public override void Destroy() {
			GL.DeleteBuffer(ID);
		}

		public virtual GfxBuffer SetData<T>(T[] Data, VertexUsageHint Hint = VertexUsageHint.StaticDraw) where T : struct {
			return SetData<T>(Data.Length * Marshal.SizeOf<T>(), Data, Hint);
		}

		public virtual GfxBuffer SetData<T>(int Size, T[] Data, VertexUsageHint Hint = VertexUsageHint.StaticDraw) where T : struct {
			Length = Data.Length;
			Bind();
			GL.BufferData<T>(Tgt, Size, Data, (BufferUsageHint)Hint);
			return this;
		}

		public virtual void Draw(int First = 0, DrawPrimitiveType PType = DrawPrimitiveType.Triangles) {
			Draw(First, Length, PType);
		}

		public abstract void Draw(int First, int Count, DrawPrimitiveType PType = DrawPrimitiveType.Triangles);
	}
}