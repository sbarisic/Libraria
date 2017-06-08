using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Libraria.Rendering {
	public class VertexBuffer : GfxBuffer {
		public VertexBuffer() : base(BufferTarget.ArrayBuffer, 3, typeof(float)) {
		}

		public override void Draw(int First, int Count, DrawPrimitiveType PType = DrawPrimitiveType.Triangles) {
			GL.DrawArrays((PrimitiveType)PType, First, Count);
		}
	}
}