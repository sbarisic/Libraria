using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Libraria.Rendering {
	public class IndexBuffer : GfxBuffer {
		public IndexBuffer() : base(BufferTarget.ElementArrayBuffer, 1 * sizeof(uint)) {
		}

		public GfxBuffer SetSequence(uint Len, VertexUsageHint Hint = VertexUsageHint.StaticDraw) {
			uint[] Arr = new uint[Len];
			for (uint i = 0; i < Len; i++)
				Arr[i] = i;
			return SetData(Arr, Hint);
		}

		public override void Draw(int First, int Count, DrawPrimitiveType PType = DrawPrimitiveType.Triangles) {
			GL.DrawElements((PrimitiveType)PType, Count, DrawElementsType.UnsignedInt, First);
		}
	}
}