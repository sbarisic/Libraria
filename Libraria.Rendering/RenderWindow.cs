using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Libraria.Maths;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using System.Drawing;
using OpenTK.Platform;
using System.Runtime.InteropServices;
using OpenTK.Input;
using Libraria;
using Libraria.Rendering;
using System.Diagnostics;

namespace Libraria.Rendering {
	public delegate void OnUpdateAction(float Dt);

	public class RenderWindow : GameWindow {
		const int GL_CONSERVATIVE_RASTERIZATION_NV = 0x9346;

		public event OnUpdateAction OnUpdate;
		public event OnUpdateAction OnRenderClient;
		public event OnUpdateAction OnRenderUI;

		public float AspectRatio { get; private set; }
		public bool IsOpen => Visible;

		FrameBuffer GFrameBuffer;
		Texture2D PositionBuffer;
		Texture2D ColorBuffer;
		Texture2D MetallicRoughnessAOHeightBuffer;
		Texture2D NormalBuffer;
		Texture2D EmissiveBuffer;
		RenderBuffer DepthBuffer;

		FrameBuffer SSAOFrameBuffer;
		Texture2D SSAOBuffer;
		Vector3[] SSAOKernel;

		ShaderProgram SSAOShader;
		ShaderProgram PostShader;
		RenderObject ScreenQuad;

		Camera UICam;

		public static void InitRenderer() {
			ToolkitOptions Options = new ToolkitOptions();
			Options.Backend = PlatformBackend.PreferNative;
			Toolkit.Init(Options);
		}

		public RenderWindow(string Title, int W, int H, bool NoBorder = false) :
			base(W, H, new GraphicsMode(new ColorFormat(32), 24, 8, 4, ColorFormat.Empty, 2, false), Title,
				GameWindowFlags.FixedWindow, DisplayDevice.Default, 4, 5, GraphicsContextFlags.Debug | GraphicsContextFlags.ForwardCompatible) {

			GL.Enable(EnableCap.FramebufferSrgb);
			//GL.ClampColor(ClampColorTarget.ClampReadColor, ClampColorMode.FixedOnly);
			//GL.Enable((EnableCap)GL_CONSERVATIVE_RASTERIZATION_NV);
			CheckError();

			UICam = new Camera();
			UICam.Projection = Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -2, 2);

			SSAOShader = new ShaderProgram("basegame\\shaders\\ssao");
			PostShader = new ShaderProgram("basegame\\shaders\\post");

			ScreenQuad = new RenderObject(DrawPrimitiveType.Triangles);

			ScreenQuad.BindBuffer(0, new VertexBuffer().SetData(new Vector3[] {
				new Vector3(0, 0, 0),
				new Vector3(0, 1, 0),
				new Vector3(1, 1, 0),
				new Vector3(0, 0, 0),
				new Vector3(1, 1, 0),
				new Vector3(1, 0, 0)
			}));

			ScreenQuad.BindBuffer(1, DataBuffer.CreateFromData(new Vector2[] {
				new Vector2(0, 0),
				new Vector2(0, 1),
				new Vector2(1, 1),
				new Vector2(0, 0),
				new Vector2(1, 1),
				new Vector2(1, 0)
			}));

			ScreenQuad.BindIndexBuffer(new IndexBuffer().SetSequence(6));

			GFrameBuffer = new FrameBuffer();
			{
				PositionBuffer = new Texture2D(TexFilterMode.Nearest);
				PositionBuffer.LoadData(W, H, IntPtr.Zero, PixelInternalFormat.Rgb32f, PixelFormat.Rgb, PixelType.Float);
				PositionBuffer.Resident = true;
				GFrameBuffer.BindTexture(PositionBuffer, FramebufferAttachment.ColorAttachment0);

				ColorBuffer = new Texture2D(TexFilterMode.Nearest);
				ColorBuffer.LoadData(W, H, IntPtr.Zero);
				ColorBuffer.Resident = true;
				GFrameBuffer.BindTexture(ColorBuffer, FramebufferAttachment.ColorAttachment1);

				MetallicRoughnessAOHeightBuffer = new Texture2D(TexFilterMode.Nearest);
				MetallicRoughnessAOHeightBuffer.LoadData(W, H, IntPtr.Zero, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float);
				MetallicRoughnessAOHeightBuffer.Resident = true;
				GFrameBuffer.BindTexture(MetallicRoughnessAOHeightBuffer, FramebufferAttachment.ColorAttachment2);

				NormalBuffer = new Texture2D(TexFilterMode.Nearest);
				NormalBuffer.LoadData(W, H, IntPtr.Zero, PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.Float);
				NormalBuffer.Resident = true;
				GFrameBuffer.BindTexture(NormalBuffer, FramebufferAttachment.ColorAttachment3);
				
				EmissiveBuffer = new Texture2D(TexFilterMode.Nearest);
				EmissiveBuffer.LoadData(W, H, IntPtr.Zero, PixelInternalFormat.Rgb, PixelFormat.Rgb);
				EmissiveBuffer.Resident = true;
				GFrameBuffer.BindTexture(EmissiveBuffer, FramebufferAttachment.ColorAttachment4);

				/*HeightBuffer = new Texture2D(TexFilterMode.Nearest);
				HeightBuffer.LoadData(W, H, IntPtr.Zero, PixelInternalFormat.R16f, PixelFormat.Red, PixelType.Float);
				HeightBuffer.Resident = true;
				GFrameBuffer.BindTexture(HeightBuffer, FramebufferAttachment.ColorAttachment7);*/

				DepthBuffer = new RenderBuffer(RenderbufferStorage.DepthComponent, W, H);
				GFrameBuffer.BindRenderBuffer(DepthBuffer, FramebufferAttachment.DepthAttachment);
				GFrameBuffer.DrawBuffer(DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3,
					DrawBuffersEnum.ColorAttachment4);
			}

			SSAOFrameBuffer = new FrameBuffer();
			{
				SSAOBuffer = new Texture2D(TexFilterMode.Linear, TexWrapMode.ClampToEdge);
				SSAOBuffer.LoadData(W, H, IntPtr.Zero, PixelInternalFormat.R32f, PixelFormat.Red, PixelType.Float);
				SSAOBuffer.Resident = true;
				SSAOFrameBuffer.BindTexture(SSAOBuffer, FramebufferAttachment.ColorAttachment0);
			}

			Random Rnd = new Random();
			SSAOKernel = new Vector3[64];
			for (int i = 0; i < SSAOKernel.Length; i++) {
				Vector3 Sample = new Vector3((float)Rnd.NextDouble() * 2 - 1, (float)Rnd.NextDouble() * 2 - 1, (float)Rnd.NextDouble());
				Sample.Normalize();
				Sample *= (float)Rnd.NextDouble();

				float Scale = (float)i / SSAOKernel.Length;
				Scale = MathFuncs.Lerp(0.1f, 1.0f, Scale * Scale);
				Sample *= Scale;

				SSAOKernel[i] = Sample;
			}

			CheckError();
		}

		Stopwatch SWatch = Stopwatch.StartNew();

		public void SetWindowSize(int W, int H) {
			if (W == -1)
				W = Width;
			if (H == -1)
				H = Height;

			Width = W;
			Height = H;
			AspectRatio = (float)W / H;
		}

		protected override void OnUpdateFrame(FrameEventArgs E) {
			OnUpdate?.Invoke((float)E.Time);
		}

		public void CheckError() {
			ErrorCode EC = GL.GetError();
			if (EC != ErrorCode.NoError)
				Console.WriteLine(">> {0}", EC);
		}

		void ClearBuffers() {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
		}
		
		void SetShaderUniforms() {
			SSAOShader.SetUniform("Textures", 0, PositionBuffer.TextureHandle);
			SSAOShader.SetUniform("Textures", 1, NormalBuffer.TextureHandle);
			SSAOShader.SetUniform("Textures", 2, ColorBuffer.TextureHandle);
			SSAOShader.SetUniform("SSAOKernel", SSAOKernel);

			PostShader.SetUniform("Resolution", new Vector2(Width, Height), false);
			PostShader.SetUniform("SSAOBuffer", SSAOBuffer.TextureHandle, false);
			PostShader.SetUniform("PositionBuffer", PositionBuffer.TextureHandle, false);
			PostShader.SetUniform("ColorBuffer", ColorBuffer.TextureHandle, false);
			PostShader.SetUniform("MetallicRoughnessAOHeightBuffer", MetallicRoughnessAOHeightBuffer.TextureHandle, false);
			PostShader.SetUniform("NormalBuffer", NormalBuffer.TextureHandle, false);
			//PostShader.SetUniform("RoughnessBuffer", RoughnessBuffer.TextureHandle, false);
			//PostShader.SetUniform("AOBuffer", AOBuffer.TextureHandle, false);
			PostShader.SetUniform("EmissiveBuffer", EmissiveBuffer.TextureHandle, false);
			//PostShader.SetUniform("HeightBuffer", HeightBuffer.TextureHandle, false);
		}

		protected override void OnRenderFrame(FrameEventArgs E) {
			MakeCurrent();
			OpenGLGC.CollectAll();
			SetShaderUniforms();

			GL.Disable(EnableCap.ScissorTest);
			GL.Disable(EnableCap.StencilTest);
			GL.Disable(EnableCap.Blend);

			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);
			//GL.Disable(EnableCap.CullFace);
			GL.FrontFace(FrontFaceDirection.Ccw);

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);

			GL.Viewport(0, 0, Width, Height);
			GL.ClearColor(0.01f, 0.01f, 0.01f, 1.0f);
			ClearBuffers();

			// World geometry
			GFrameBuffer.Bind();
			{
				ClearBuffers();
				OnRenderClient?.Invoke((float)E.Time);
			}
			GFrameBuffer.Unbind();

			// Screen quad
			{
				Camera CurCam = Camera.GetCurrent();
				Camera.Push(UICam);
				GL.FrontFace(FrontFaceDirection.Cw);

				SSAOFrameBuffer.Bind();
				//GL.Viewport(0, 0, Width / 2, Height / 2);
				{
					SSAOShader.SetUniform("MatProjection2", CurCam.Projection, false);
					ScreenQuad.Draw(SSAOShader);
				}
				//GL.Viewport(0, 0, Width, Height);
				SSAOFrameBuffer.Unbind();

				float T = (float)SWatch.ElapsedMilliseconds / 1000;
				PostShader.SetUniform("LightDir[0]", new Vector3((float)Math.Cos(T), 1, (float)Math.Sin(T)).Normalized(), false);

				PostShader.SetUniform("MatTranslation2", CurCam.Translation, false);
				PostShader.SetUniform("MatRotation2", CurCam.RotationMat, false);
				PostShader.SetUniform("MatProjection2", CurCam.Projection, false);
				ScreenQuad.Draw(PostShader);
				Camera.Pop();
				CheckError();
			}

			// UI
			OnRenderUI?.Invoke((float)E.Time);
			SwapBuffers();
		}
	}
}