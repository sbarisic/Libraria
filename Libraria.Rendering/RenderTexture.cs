using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Libraria.Rendering {
	public class RenderTexture : OpenGLBuffer<RenderTexture> {
		public static RenderTexture Current {
			get {
				if (RTStack.Count == 0)
					return null;
				return RTStack.Peek();
			}
		}

		static Stack<RenderTexture> RTStack = new Stack<RenderTexture>();

		public FrameBuffer FrameBuffer;
		public RenderBuffer DepthBuffer;
		public Texture2D Texture {
			get {
				if (IsDoubleBuffered)
					return OldBuffer;
				return NewBuffer;
			}
		}

		public bool HasDepth { get; private set; }
		public bool IsDoubleBuffered { get; private set; }
		public int Width { get { return Texture.Width; } }
		public int Height { get { return Texture.Height; } }

		int[] OldViewport = new int[4];
		Texture2D OldBuffer = null;
		Texture2D NewBuffer = null;

		public RenderTexture(int W, int H, bool Depth = true, bool DoubleBuffered = false, TexFilterMode FilterMode = TexFilterMode.Linear) {
			HasDepth = Depth;
			IsDoubleBuffered = DoubleBuffered;

			FrameBuffer = new FrameBuffer();

			if (Depth) {
				DepthBuffer = new RenderBuffer(RenderBufferStorage.DepthComponent, W, H);
				FrameBuffer.BindRenderBuffer(DepthBuffer, FramebufferAttachment.DepthAttachment);
			}

			NewBuffer = new Texture2D(FilterMode, W, H);
			if (IsDoubleBuffered)
				OldBuffer = new Texture2D(FilterMode, W, H);

			FrameBuffer.BindTexture(NewBuffer, FramebufferAttachment.ColorAttachment0);
			FrameBuffer.DrawBuffer(DrawBufferMode.ColorAttachment0);

			Bind();
			Unbind();
		}

		public void Clear(Color Clr) {
			GL.ClearColor(Clr);
			Clear();
		}

		public void Clear() {
			ClearBufferMask ClearMask = ClearBufferMask.ColorBufferBit;
			if (HasDepth)
				ClearMask |= ClearBufferMask.DepthBufferBit;
			GL.Clear(ClearMask);
		}

		public override RenderTexture Bind() {
			GL.GetInteger(GetPName.Viewport, OldViewport);
			RTStack.Push(this);

			if (IsDoubleBuffered)
				OldBuffer.LoadData(NewBuffer);

			FrameBuffer.Bind();
			GL.Viewport(0, 0, Width, Height);
			return this;
		}

		public override RenderTexture Unbind() {
			RTStack.Pop();
			FrameBuffer.Unbind();
			if (RTStack.Count > 0)
				RTStack.Peek().Bind();
			GL.Viewport(OldViewport[0], OldViewport[1], OldViewport[2], OldViewport[3]);
			return this;
		}

		public override void Destroy() {
		}
	}
}
