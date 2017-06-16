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

namespace Libraria.Rendering {
	public delegate void OnUpdateAction(float Dt);

	public class RenderWindow : GameWindow {
		public event OnUpdateAction OnUpdate;
		public event OnUpdateAction OnRenderClient;
		public event OnUpdateAction OnRenderUI;

		public float AspectRatio { get; private set; }
		public bool IsOpen => Visible;

		FrameBuffer GFrameBuffer;
		Texture2D PositionBuffer;
		Texture2D NormalBuffer;
		Texture2D ColorBuffer;
		RenderBuffer DepthBuffer;

		FrameBuffer SSAOFrameBuffer;
		Texture2D SSAOBuffer;

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
				GameWindowFlags.FixedWindow, DisplayDevice.Default, 4, 5, GraphicsContextFlags.Debug) {

			GL.Enable(EnableCap.FramebufferSrgb);

			UICam = new Camera();
			UICam.Projection = Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -2, 2);

			SSAOShader = ShaderProgram.CreateProgram("basegame\\shaders\\ssao");
			PostShader = ShaderProgram.CreateProgram("basegame\\shaders\\post");
			PostShader.SetUniform("Resolution", new Vector2(W, H), false);

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
				PositionBuffer.LoadData(W, H, IntPtr.Zero, PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.Float);
				PositionBuffer.Resident = true;
				GFrameBuffer.BindTexture(PositionBuffer, FramebufferAttachment.ColorAttachment0);

				NormalBuffer = new Texture2D(TexFilterMode.Nearest);
				NormalBuffer.LoadData(W, H, IntPtr.Zero, PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.Float);
				NormalBuffer.Resident = true;
				GFrameBuffer.BindTexture(NormalBuffer, FramebufferAttachment.ColorAttachment1);

				ColorBuffer = new Texture2D(TexFilterMode.Nearest);
				ColorBuffer.LoadData(W, H, IntPtr.Zero);
				ColorBuffer.Resident = true;
				GFrameBuffer.BindTexture(ColorBuffer, FramebufferAttachment.ColorAttachment2);

				DepthBuffer = new RenderBuffer(RenderbufferStorage.DepthComponent, W, H);
				GFrameBuffer.BindRenderBuffer(DepthBuffer, FramebufferAttachment.DepthAttachment);
				GFrameBuffer.DrawBuffer(DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2);
			}

			SSAOFrameBuffer = new FrameBuffer();
			{
				SSAOBuffer = new Texture2D(TexFilterMode.Linear, TexWrapMode.ClampToEdge);
				SSAOBuffer.LoadData(W, H, IntPtr.Zero, PixelInternalFormat.R32f, PixelFormat.Red, PixelType.Float);
				SSAOBuffer.Resident = true;
				SSAOFrameBuffer.BindTexture(SSAOBuffer, FramebufferAttachment.ColorAttachment0);
			}

			SSAOShader.SetUniform("Textures", 0, PositionBuffer.TextureHandle);
			SSAOShader.SetUniform("Textures", 1, NormalBuffer.TextureHandle);
			SSAOShader.SetUniform("Textures", 2, ColorBuffer.TextureHandle);

			Random Rnd = new Random();
			Vector3[] SSAOKernel = new Vector3[64];
			for (int i = 0; i < SSAOKernel.Length; i++) {
				Vector3 Sample = new Vector3((float)Rnd.NextDouble() * 2 - 1, (float)Rnd.NextDouble() * 2 - 1, (float)Rnd.NextDouble());
				Sample.Normalize();
				Sample *= (float)Rnd.NextDouble();

				float Scale = (float)i / SSAOKernel.Length;
				Scale = MathFuncs.Lerp(0.1f, 1.0f, Scale * Scale);
				Sample *= Scale;

				SSAOKernel[i] = Sample;
			}
			SSAOShader.SetUniform("SSAOKernel", SSAOKernel);

			PostShader.SetUniform("Textures", 0, ColorBuffer.TextureHandle, false);
			PostShader.SetUniform("Textures", 1, SSAOBuffer.TextureHandle, false);

			CheckError();
		}

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

		protected override void OnRenderFrame(FrameEventArgs E) {
			MakeCurrent();
			OpenGLGC.CollectAll();

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
				Matrix4 ClientProjection = Camera.GetCurrent().Projection;
				Camera.Push(UICam);
				GL.FrontFace(FrontFaceDirection.Cw);

				SSAOFrameBuffer.Bind();
				//GL.Viewport(0, 0, Width / 2, Height / 2);
				{
					SSAOShader.SetUniform("MatProjection2", ClientProjection);
					ScreenQuad.Draw(SSAOShader);
				}
				//GL.Viewport(0, 0, Width, Height);
				SSAOFrameBuffer.Unbind();

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