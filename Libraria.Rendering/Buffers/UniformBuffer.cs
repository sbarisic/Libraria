using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Libraria.Rendering {
	public class UniformBuffer : GfxBuffer {
		//public long Address;

		public UniformBuffer(int Size, Type DataType) : base(Size, DataType) {
			//GL.GetNamedBufferParameter(ID, (BufferParameterName)All.BufferGpuAddressNv, out Address);
		}
	}
}