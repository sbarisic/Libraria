using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Libraria.Rendering {
	public class DataBuffer : GfxBuffer {
		public DataBuffer(int Stride) : base(BufferTarget.ArrayBuffer, Stride) {
		}

		public override void Draw(int First, int Count, DrawPrimitiveType PType = DrawPrimitiveType.Triangles) {
			throw new InvalidOperationException();
		}
	}
}