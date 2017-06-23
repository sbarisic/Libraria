using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Linq;
using System.Runtime.InteropServices;
using Libraria.IO;

namespace Libraria.Rendering {
	public delegate void OnShaderUpdateAction(ShaderProgram Program, string ErrorMsg);

	public unsafe class ShaderProgram : OpenGLGC.Destructable {
		public unsafe class Shader : OpenGLGC.Destructable {
			bool Dirty;
			public int ID;
			public string SourceFile { get; private set; }

			public static Shader CreateShader(string FilePath) {
				string Ext = Path.GetExtension(FilePath).ToLower().Substring(1);
				ShaderType Kind = ShaderType.FragmentShader;

				if (Ext == "frag")
					Kind = ShaderType.FragmentShader;
				else if (Ext == "vert")
					Kind = ShaderType.VertexShader;
				else if (Ext == "geom")
					Kind = ShaderType.GeometryShader;
				else if (Ext == "comp")
					Kind = ShaderType.ComputeShader;
				else
					throw new Exception("Unknown extension: " + Ext);

				if ((Kind == ShaderType.GeometryShader || Kind == ShaderType.ComputeShader) && !File.Exists(FilePath))
					return null;

				Shader S = new Shader(Kind);
				S.Source(FilePath);
				return S;
			}

			public void MarkDirty() {
				Dirty = true;
			}

			public Shader(ShaderType SType) {
				ID = GL.CreateShader(SType);
			}

			public void Source(string SourceFile) {
				this.SourceFile = FilePath.Normalize(SourceFile);
				MarkDirty();
			}

			public void Compile() {
				if (!TryCompile(out string ErrorMsg))
					throw new Exception(ErrorMsg);
			}

			public bool TryCompile(out string ErrorMsg) {
				if (!Dirty) {
					ErrorMsg = "";
					return true;
				}

				if (string.IsNullOrEmpty(SourceFile)) {
					ErrorMsg = "Source file not specified for shader";
					return false;
				}

				while (!FilePath.TryOpenFile(SourceFile, FileAccess.Read))
					;

				string Src = File.ReadAllText(SourceFile);
				GL.ShaderSource(ID, Src);
				GL.CompileShader(ID);

				ErrorMsg = GL.GetShaderInfoLog(ID);
				if (ErrorMsg.Length > 0) {
					ErrorMsg = string.Format("Error in: {0}\n{1}", SourceFile, ErrorMsg);
					return false;
				}

				Dirty = false;
				return true;
			}

			public void Attach(int ProgramID) {
				GL.AttachShader(ProgramID, ID);
			}

			public void Detach(int ProgramID) {
				GL.DetachShader(ProgramID, ID);
			}

			bool WasFinalized;
			~Shader() {
				OpenGLGC.Enqueue(this, ref WasFinalized);
			}

			public void Destroy() {
				GL.DeleteShader(ID);
			}
		}

		public static event OnShaderUpdateAction OnProgramError;
		static Filedog ShaderWatcher = new Filedog(FilePath.GetProcessExeDirectory());

		bool Dirty;
		public int ID;
		Shader[] Shaders;

		public ShaderProgram(string[] ShaderPaths) {
			List<Shader> ShaderList = new List<Shader>();

			for (int i = 0; i < ShaderPaths.Length; i++) {
				Shader S = Shader.CreateShader(ShaderPaths[i]);

				if (S != null) {
					ShaderWatcher.OnFileChanged(S.SourceFile, (Ext, SourceFile, Name, Type) => {
						Dirty = true;
						S.MarkDirty();
					});

					S.Compile();
					ShaderList.Add(S);
				}
			}

			Shaders = ShaderList.ToArray();

			ID = GL.CreateProgram();
			Dirty = true;
			Reload();
			//Bind();
		}

		public ShaderProgram(string Name) : this(new string[] { Name + ".frag", Name + ".vert", Name + ".geom", Name + ".comp" }) {
		}

		bool WasFinalized;
		~ShaderProgram() {
			OpenGLGC.Enqueue(this, ref WasFinalized);
		}

		public void Reload() {
			if (!TryReload(out string ErrorMsg))
				throw new Exception(ErrorMsg);
		}

		public bool TryReload(out string ErrorMsg) {
			ErrorMsg = "";
			if (!Dirty)
				return true;
			Dirty = false;

			for (int i = 0; i < Shaders.Length; i++)
				if (!Shaders[i].TryCompile(out ErrorMsg))
					return false;

			for (int i = 0; i < Shaders.Length; i++)
				Shaders[i].Attach(ID);

			bool Status = true;
			if (!TryLink(out ErrorMsg))
				Status = false;

			for (int i = 0; i < Shaders.Length; i++)
				Shaders[i].Detach(ID);

			return Status;
		}

		public void Link() {
			if (!TryLink(out string ErrorMsg))
				throw new Exception(ErrorMsg);
		}

		public bool TryLink(out string ErrorMsg) {
			int ProgramLen = 0;
			BinaryFormat BinFormat;
			int Len = 1024 * 512;
			byte* BufferPtr = stackalloc byte[Len];
			IntPtr Buffer = new IntPtr(BufferPtr);
			GL.GetProgramBinary(ID, Len, out ProgramLen, out BinFormat, Buffer);

			GL.LinkProgram(ID);

			ErrorMsg = GL.GetProgramInfoLog(ID);
			if (ErrorMsg.Length > 0) {
				GL.ProgramBinary(ID, BinFormat, Buffer, ProgramLen);
				return false;
			}

			return true;
		}

		public void Destroy() {
			GL.DeleteProgram(ID);
		}

		public void Bind(bool DoBindCamera = true) {
			if (!TryReload(out string ErrorMsg))
				OnProgramError?.Invoke(this, ErrorMsg);

			GL.UseProgram(ID);
			if (DoBindCamera)
				BindCamera(Camera.GetCurrent());
		}

		public void Unbind() {
			GL.UseProgram(0);
		}

		public int GetAttribLocation(string Name) {
			/*if (!TryReload(out string ErrorMsg))
				OnProgramError?.Invoke(this, ErrorMsg);*/

			return GL.GetAttribLocation(ID, Name);
		}

		public int GetUniformLocation(string Name, bool ThrowOnFail = true) {
			/*if (!TryReload(out string ErrorMsg))
				OnProgramError?.Invoke(this, ErrorMsg);*/

			int Idx = GL.GetUniformLocation(ID, Name);
			if (Idx < 0 && ThrowOnFail)
				throw new Exception("Invalid index");
			return Idx;
		}

		public int GetUniformBlockIndex(string Name) {
			int Idx = GL.GetUniformBlockIndex(ID, Name);
			if (Idx < 0)
				throw new Exception("Invalid index");
			return Idx;
		}

		public void BindCamera(Camera C) {
			if (C == null)
				throw new Exception("Invalid camera");

			SetUniform("MatTranslation", C.Translation, false);
			SetUniform("MatRotation", C.RotationMat, false);
			SetUniform("MatProjection", C.Projection, false);
		}

		public void SetUniform(string Name, Matrix4 Mtx, bool ThrowOnFail = true) {
			SetUniform(Name, ref Mtx, ThrowOnFail);
		}

		public void SetUniform(string Name, ref Matrix4 Mtx, bool ThrowOnFail = true) {
			int Idx = GetUniformLocation(Name, ThrowOnFail);
			if (Idx >= 0)
				GL.ProgramUniformMatrix4(ID, Idx, false, ref Mtx);
		}

		public void SetUniform(string Name, Vector3 Vec, bool ThrowOnFail = true) {
			SetUniform(Name, ref Vec, ThrowOnFail);
		}

		public void SetUniform(string Name, Vector3[] Vecs) {
			fixed (Vector3* VecsPtr = Vecs)
				GL.ProgramUniform3(ID, GetUniformLocation(Name), Vecs.Length, (float*)VecsPtr);
		}

		public void SetUniform(string Name, ref Vector3 Vec, bool ThrowOnFail = true) {
			GL.ProgramUniform3(ID, GetUniformLocation(Name, ThrowOnFail), ref Vec);
		}

		public void SetUniform(string Name, Vector2 Vec, bool ThrowOnFail = true) {
			SetUniform(Name, ref Vec, ThrowOnFail);
		}

		public void SetUniform(string Name, ref Vector2 Vec, bool ThrowOnFail = true) {
			GL.ProgramUniform2(ID, GetUniformLocation(Name, ThrowOnFail), ref Vec);
		}

		public void SetUniform(string Name, float Val) {
			GL.ProgramUniform1(ID, GetUniformLocation(Name), Val);
		}

		public void SetUniform(string Name, Texture2D Tex) {
			GL.ProgramUniform1(ID, GetUniformLocation(Name), Tex.TexUnit);
		}

		public void SetUniform(string Name, long Handle, bool ThrowOnFail = true) {
			GL.Arb.ProgramUniformHandle(ID, GetUniformLocation(Name, ThrowOnFail), Handle);
		}

		public void SetUniform(string Name, long[] Handles, bool ThrowOnFail = true) {
			GL.Arb.ProgramUniformHandle(ID, GetUniformLocation(Name, ThrowOnFail), Handles.Length, Handles);
		}

		public void SetUniform(string Name, int Index, long Handle, bool ThrowOnFail = true) {
			SetUniform(Name + "[" + Index + "]", Handle, ThrowOnFail);
		}

		public void SetUniform(string Name, int Index, long[] Handles, bool ThrowOnFail = true) {
			SetUniform(Name + "[" + Index + "]", Handles, ThrowOnFail);
		}

		public void SetUniform(string BlockName, UniformBuffer UBO) {
			//int BlockIdx = GetUniformBlockIndex(BlockName);
			int BlockIdx = GetUniformLocation(BlockName);

			Bind();
			GL.BindBufferBase(BufferRangeTarget.UniformBuffer, BlockIdx, UBO.ID);
			Unbind();

			//GL.BindBufferRange(BufferRangeTarget.ShaderStorageBuffer, BlockIndex, UBO.ID, IntPtr.Zero, UBO.Length * Marshal.SizeOf(UBO.DataType));
		}
	}
}
