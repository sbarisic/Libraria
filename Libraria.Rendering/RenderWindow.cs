using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using SDL2;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;

namespace Libraria.Rendering {
	public class RenderWindow {
		public bool IsOpen { get; private set; }

		IntPtr Window;
		IntPtr GLContext;
		public GraphicsContext TKContext;

		public float AspectRatio { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public static void InitRenderer() {
			ToolkitOptions Options = new ToolkitOptions();
			Options.Backend = PlatformBackend.PreferNative;
			Toolkit.Init(Options);

			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
				throw new Exception("Failed to initialize SDL2");
		}

		public RenderWindow(string Title, int W, int H, bool NoBorder = true) {
			Width = W;
			Height = H;
			AspectRatio = (float)W / H;

			SDL.SDL_WindowFlags WFlags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL
				| SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN;

			if (NoBorder)
				WFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS;

			Window = SDL.SDL_CreateWindow(Title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, W, H, WFlags);
			//SDL.SDL_SetWindowPosition(Window, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED);

			GLContext = SDL.SDL_GL_CreateContext(Window);
			MakeCurrent();
		}

		public void Init() {
			TKContext = new GraphicsContext(new ContextHandle(IntPtr.Zero),
						SDL.SDL_GL_GetProcAddress, () => new ContextHandle(SDL.SDL_GL_GetCurrentContext()));
			TKContext.LoadAll();

			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, (int)SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 4);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 5);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ACCELERATED_VISUAL, 1); // Require hardware acceleration

			GL.Disable(EnableCap.DepthTest); //Disable Z-Buffer, 2D Rendering
			GL.Disable(EnableCap.CullFace);

			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

			//GL.RenderMode(RenderingMode.Render);
			//GL.MatrixMode(MatrixMode.Projection);

			GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			IsOpen = true;
		}

		public void SetWindowSize(int W, int H) {
			if (W == -1)
				W = Width;
			if (H == -1)
				H = Height;


			//SDL.SDL_SetWindowSize(Window, (int)(W * 1.2f), (int)(H * 1.2f));
			//GL.Viewport(0, 0, (int)(W * 1.2f), (int)(H * 1.2f));

			SDL.SDL_SetWindowSize(Window, W, H);
			Reset();

			Width = W;
			Height = H;
		}

		public void Reset() {
			/*GL.Viewport(0, 0, Width, Height);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, 1, 0, 1, 0.01, 1);*/
		}

		public int MakeCurrent() {
			return SDL.SDL_GL_MakeCurrent(Window, GLContext);
		}

		public void Swap() {
			SDL.SDL_GL_SwapWindow(Window);
		}

		public void Close() {
			IsOpen = false;
		}

		public void PollEvents() {
			SDL.SDL_Event Event;

			while (SDL.SDL_PollEvent(out Event) != 0) {
				switch (Event.type) {
					case SDL.SDL_EventType.SDL_QUIT:
						Close();
						break;

					default:
						break;
				}
			}
		}
	}
}