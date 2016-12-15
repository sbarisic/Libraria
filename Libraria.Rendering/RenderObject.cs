using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Libraria.Rendering {
	public class RenderObject {
		public int ID;
		public DrawPrimitiveType PrimitiveType;

		IndexBuffer IBuf;
		ShaderProgram Shader;

		Dictionary<int, Texture2D> Textures;

		public RenderObject(DrawPrimitiveType PrimitiveType = DrawPrimitiveType.Triangles) {
			this.PrimitiveType = PrimitiveType;
			Textures = new Dictionary<int, Texture2D>();

			ID = GL.GenVertexArray();
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

		public void BindIndexBuffer(IndexBuffer IndexBuffer) {
			IBuf = IndexBuffer;
			IBuf.Bind();
		}

		public void BindShader(ShaderProgram Shader) {
			this.Shader = Shader;
		}

		public void BindBuffer(string AttributeName, GfxBuffer Array, int Stride = -1, int Offset = 0) {
			if (Shader == null)
				throw new Exception("No shader bound");
			int AttribLocation = Shader.GetAttribLocation(AttributeName);
			if (AttribLocation == -1)
				throw new Exception(string.Format("Attribute '{0}' could not be found", AttributeName));

			BindBuffer(AttribLocation, Array, Stride, Offset);
		}

		public void BindBuffer(int AttributeName, GfxBuffer Array, int Stride = -1, int Offset = 0) {
			if (Stride == -1)
				Stride = Array.Stride;

			GL.EnableVertexAttribArray(AttributeName);
			GL.BindVertexBuffer(AttributeName, Array.ID, (IntPtr)Offset, Stride);
		}

		public void SetTexture(Texture2D Tex, int Idx = 0) {
			if (Textures.ContainsKey(Idx))
				Textures.Remove(Idx);
			Textures.Add(Idx, Tex);
		}

		public void Draw() {
			if (IBuf == null)
				throw new Exception("No index buffer found");
			if (Shader == null)
				throw new Exception("No shader found");

			Bind();
			Shader.Bind();

			foreach (var Tex in Textures) {
				Tex.Value.Bind(Tex.Key);
				Shader.SetUniform("Tex" + Tex.Key, Tex.Value);
			}

			IBuf.Draw(0, PrimitiveType);
			Shader.Unbind();
			Unbind();
		}
	}
}