using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Libraria.Rendering {
	public enum ShaderKind {
		FragmentShader = 35632,
		VertexShader = 35633,
		GeometryShader = 36313,
		GeometryShaderExt = 36313,
		TessEvaluationShader = 36487,
		TessControlShader = 36488,
		ComputeShader = 37305
	}

	public class ShaderProgram {
		static int CreateShader(string FilePath, ShaderKind Kind) {
			string Src = File.ReadAllText(FilePath);

			int ID = GL.CreateShader((ShaderType)Kind);
			GL.ShaderSource(ID, Src);
			GL.CompileShader(ID);

			string Log = GL.GetShaderInfoLog(ID);
			if (Log.Length > 0)
				throw new Exception(Log);

			return ID;
		}

		static int CreateShader(string FilePath) {
			string Ext = Path.GetExtension(FilePath).ToLower().Substring(1);
			ShaderKind Kind = ShaderKind.FragmentShader;

			if (Ext == "frag")
				Kind = ShaderKind.FragmentShader;
			else if (Ext == "vert")
				Kind = ShaderKind.VertexShader;
			else if (Ext == "geom")
				Kind = ShaderKind.GeometryShader;
			else if (Ext == "comp")
				Kind = ShaderKind.ComputeShader;
			else
				throw new Exception("Unknown extension: " + Ext);

			return CreateShader(FilePath, Kind);
		}

		static int CreateProgram(int[] Shaders) {
			int ID = GL.CreateProgram();

			for (int i = 0; i < Shaders.Length; i++)
				GL.AttachShader(ID, Shaders[i]);

			GL.LinkProgram(ID);

			string Log = GL.GetProgramInfoLog(ID);
			if (Log.Length > 0)
				throw new Exception(Log);

			for (int i = 0; i < Shaders.Length; i++)
				GL.DetachShader(ID, Shaders[i]);

			return ID;
		}

		public static ShaderProgram CreateProgram(string Name) {
			return new ShaderProgram(Name + ".frag", Name + ".vert");
		}

		public int ID;

		public ShaderProgram(params string[] ShaderPaths) {
			int[] Shaders = new int[ShaderPaths.Length];

			for (int i = 0; i < ShaderPaths.Length; i++)
				Shaders[i] = CreateShader(ShaderPaths[i]);

			ID = CreateProgram(Shaders);
			Bind();
		}

		public void Bind() {
			GL.UseProgram(ID);
		}

		public void Unbind() {
			GL.UseProgram(0);
		}

		public int GetUniformLocation(string Name) {
			return GL.GetUniformLocation(ID, Name);
		}

		public void SetUniform(string Name, ref Matrix4 Mtx) {
			GL.ProgramUniformMatrix4(ID, GetUniformLocation(Name), false, ref Mtx);
		}
	}
}
