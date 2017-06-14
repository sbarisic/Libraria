using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using System.Runtime.InteropServices;
using OpenTK.Input;

namespace Libraria.Rendering {
	public delegate void OnUpdateAction(float Dt);

	public class RenderWindow : GameWindow {
		public event OnUpdateAction OnUpdate;
		public event OnUpdateAction OnRender;

		public float AspectRatio { get; private set; }
		public bool IsOpen => Visible;

		public static void InitRenderer() {
			ToolkitOptions Options = new ToolkitOptions();
			Options.Backend = PlatformBackend.PreferNative;
			Toolkit.Init(Options);
		}

		public RenderWindow(string Title, int W, int H, bool NoBorder = false) :
			base(W, H, new GraphicsMode(new ColorFormat(32), 24, 8, 0, ColorFormat.Empty, 2, false), Title,
				GameWindowFlags.FixedWindow, DisplayDevice.Default, 4, 5, GraphicsContextFlags.Debug) {
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

		protected override void OnRenderFrame(FrameEventArgs E) {
			MakeCurrent();

			GL.Disable(EnableCap.ScissorTest);
			GL.Disable(EnableCap.StencilTest);

			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);
			//GL.Disable(EnableCap.CullFace);
			GL.FrontFace(FrontFaceDirection.Ccw);

			//GL.Enable(EnableCap.Blend);
			GL.Disable(EnableCap.Blend);

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);

			GL.Viewport(0, 0, Width, Height);
			GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			GL.Clear(ClearBufferMask.StencilBufferBit);

			OnRender?.Invoke((float)E.Time);

			SwapBuffers();
		}
	}
}