using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

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

	public class VertexArray {
		List<int> AttribArrays;
		public int ID;

		public VertexArray() {
			AttribArrays = new List<int>();
			ID = GL.GenVertexArray();
			Bind();
		}

		public void Destroy() {
			GL.DeleteVertexArray(ID);
		}

		public void Bind() {
			GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
		}

		public void Unbind() {
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void SetData<T>(T[] Data, VertexUsageHint Hint = VertexUsageHint.DynamicDraw) where T : struct {
			SetData<T>(Data.Length * Marshal.SizeOf<T>(), Data, Hint);
		}

		public void SetData<T>(int Size, T[] Data, VertexUsageHint Hint = VertexUsageHint.DynamicDraw) where T : struct {
			GL.BufferData<T>(BufferTarget.ArrayBuffer, Size, Data, (BufferUsageHint)Hint);
		}

		public void ClearAttribArrays() {
			AttribArrays.Clear();
		}

		public void AddAttribArray(int Num) {
			AttribArrays.Add(Num);
		}

		public void VertexAttribPointer(int Idx, int Size, AttribPointerType PType,
			bool Normalized = false, int Stride = 0, int Offset = 0) {
			GL.VertexAttribPointer(Idx, Size, (VertexAttribPointerType)PType, Normalized, Stride, Offset);
		}

		public void Draw(int First, int Count, DrawPrimitiveType PType = DrawPrimitiveType.Triangles) {
			foreach (var AttribArray in AttribArrays)
				GL.EnableVertexAttribArray(AttribArray);

			Bind();

			GL.DrawArrays((PrimitiveType)PType, First, Count);

			foreach (var AttribArray in AttribArrays)
				GL.DisableVertexAttribArray(AttribArray);

			Unbind();
		}
	}
}