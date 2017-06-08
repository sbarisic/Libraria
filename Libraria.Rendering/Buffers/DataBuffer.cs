using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Runtime.InteropServices;
using System.Linq;

namespace Libraria.Rendering {
	public class DataBuffer : GfxBuffer {
		static Type GetTypePrimitiveType(Type T) {
			if (T == typeof(Vector2) || T == typeof(Vector3))
				return typeof(float);
			return T;
		}

		public static GfxBuffer CreateFromData<T>(T[] Data, VertexUsageHint Hint = VertexUsageHint.StaticDraw) where T : struct {
			return new DataBuffer(typeof(T)).SetData(Data, Hint);
		}

		public static GfxBuffer CreateFromData<T>(IEnumerable<T> Data, VertexUsageHint Hint = VertexUsageHint.StaticDraw) where T : struct {
			return CreateFromData(Data.ToArray(), Hint);
		}

		public DataBuffer(int Size, Type T) : base(BufferTarget.ArrayBuffer, Size, T) {
		}

		public DataBuffer(Type T) : this(Marshal.SizeOf(T) / Marshal.SizeOf(GetTypePrimitiveType(T)), GetTypePrimitiveType(T)) {
		}

		public override void Draw(int First, int Count, DrawPrimitiveType PType = DrawPrimitiveType.Triangles) {
			throw new InvalidOperationException();
		}
	}
}