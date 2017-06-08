using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Linq;

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
			if (Kind == ShaderKind.GeometryShader && !File.Exists(FilePath))
				return -1;

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

			//GL.BindFragDataLocation(ID, 0, "Color");
			GL.LinkProgram(ID);

			string Log = GL.GetProgramInfoLog(ID);
			if (Log.Length > 0)
				throw new Exception(Log);

			for (int i = 0; i < Shaders.Length; i++)
				GL.DetachShader(ID, Shaders[i]);

			return ID;
		}

		public static ShaderProgram CreateProgram(string Name) {
			return new ShaderProgram(Name + ".frag", Name + ".vert", Name + ".geom");
		}

		public int ID;

		public ShaderProgram(params string[] ShaderPaths) {
			int[] Shaders = new int[ShaderPaths.Length];

			for (int i = 0; i < ShaderPaths.Length; i++) 
				Shaders[i] = CreateShader(ShaderPaths[i]);
	
			ID = CreateProgram(Shaders.Where((I) => I != -1).ToArray());
			Bind();
		}

		public void Bind() {
			GL.UseProgram(ID);
			BindCamera(Camera.GetCurrent());
		}

		public void Unbind() {
			GL.UseProgram(0);
		}

		public int GetAttribLocation(string Name) {
			return GL.GetAttribLocation(ID, Name);
		}

		public int GetUniformLocation(string Name) {
			return GL.GetUniformLocation(ID, Name);
		}

		public void BindCamera(Camera C) {
			if (C == null)
				throw new Exception("Invalid camera");

			SetUniform("MatTranslation", C.Translation);
			SetUniform("MatRotation", C.RotationMat);
			SetUniform("MatProjection", C.Projection);
		}

		public void SetUniform(string Name, Matrix4 Mtx) {
			SetUniform(Name, ref Mtx);
		}

		public void SetUniform(string Name, ref Matrix4 Mtx) {
			GL.ProgramUniformMatrix4(ID, GetUniformLocation(Name), false, ref Mtx);
		}

		public void SetUniform(string Name, Vector3 Vec) {
			SetUniform(Name, ref Vec);
		}

		public void SetUniform(string Name, ref Vector3 Vec) {
			GL.ProgramUniform3(ID, GetUniformLocation(Name), ref Vec);
		}

		public void SetUniform(string Name, Vector2 Vec) {
			SetUniform(Name, ref Vec);
		}

		public void SetUniform(string Name, ref Vector2 Vec) {
			GL.ProgramUniform2(ID, GetUniformLocation(Name), ref Vec);
		}

		public void SetUniform(string Name, float Val) {
			GL.ProgramUniform1(ID, GetUniformLocation(Name), Val);
		}

		public void SetUniform(string Name, Texture2D Tex) {
			GL.ProgramUniform1(ID, GetUniformLocation(Name), Tex.TexUnit);
		}
	}
}
