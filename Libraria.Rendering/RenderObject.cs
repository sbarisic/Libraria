using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Libraria.Rendering {
	public class RenderObject : OpenGLGC.Destructable {
		public int ID;
		public DrawPrimitiveType PrimitiveType;

		IndexBuffer IBuf;
		ShaderProgram Shader;

		Dictionary<int, Texture2D> Textures;

		bool WasFinalized;
		~RenderObject() {
			OpenGLGC.Enqueue(this, ref WasFinalized);
		}

		public RenderObject(DrawPrimitiveType PrimitiveType = DrawPrimitiveType.Triangles) {
			this.PrimitiveType = PrimitiveType;
			Textures = new Dictionary<int, Texture2D>();

			//ID = GL.GenVertexArray();
			GL.CreateVertexArrays(1, out ID);
			Bind();
		}

		public void Destroy() {
			GL.DeleteVertexArray(ID);
		}

		public void Bind() {
			GL.BindVertexArray(ID);
		}

		public void Unbind() {
			GL.BindVertexArray(0);
		}

		public void BindIndexBuffer(GfxBuffer IndexBuffer) {
			IBuf = (IndexBuffer)IndexBuffer;
			GL.VertexArrayElementBuffer(ID, IndexBuffer.ID);
		}

		public void BindShader(ShaderProgram Shader) {
			this.Shader = Shader;
		}

		public int GetAttribLocation(string AttributeName) {
			if (Shader == null)
				throw new Exception("No shader found");
			return Shader.GetAttribLocation(AttributeName);
		}

		public bool BindBuffer(string AttributeName, Func<GfxBuffer> BufferBuilder, int Stride = 0, int Offset = 0) {
			int Attrib = GetAttribLocation(AttributeName);
			if (Attrib == -1)
				return false;

			BindBuffer(Attrib, BufferBuilder(), Stride, Offset);
			return true;
		}

		public void BindBuffer(string AttributeName, GfxBuffer Array, int Stride = 0, int Offset = 0) {
			int AttribLocation = GetAttribLocation(AttributeName);

			if (AttribLocation == -1)
				throw new Exception(string.Format("Attribute '{0}' could not be found", AttributeName));

			BindBuffer(AttribLocation, Array, Stride, Offset);
		}

		public void BindBuffer(int AttributeName, GfxBuffer Array, int Stride = 0, int Offset = 0) {
			GL.EnableVertexArrayAttrib(ID, AttributeName);
			GL.VertexArrayVertexBuffer(ID, AttributeName, Array.ID, (IntPtr)Offset, Array.Size * Marshal.SizeOf(Array.DataType));
			GL.VertexArrayAttribFormat(ID, AttributeName, Array.Size, Array.AttribType, false, Stride);
		}

		public void SetTexture(Texture2D Tex, int Idx = 0) {
			if (Textures.ContainsKey(Idx))
				Textures.Remove(Idx);
			Textures.Add(Idx, Tex);
		}

		public void Draw(ShaderProgram DrawShader = null) {
			if (IBuf == null)
				throw new Exception("No index buffer found");
			if (Shader == null && DrawShader == null)
				throw new Exception("No shader found");

			Bind();
			if (DrawShader != null)
				DrawShader.Bind();
			else
				Shader.Bind();

			foreach (var Tex in Textures) {
				Tex.Value.Bind(Tex.Key);
				Shader.SetUniform("Tex" + Tex.Key, Tex.Value);
			}

			IBuf.Draw(0, PrimitiveType);

			if (DrawShader != null)
				DrawShader.Unbind();
			else
				Shader.Unbind();
			Unbind();
		}
	}
}