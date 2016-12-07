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
		GraphicsContext TKContext;
		public float AspectRatio { get; private set; }

		public RenderWindow(string Title, int W, int H, bool NoBorder = true) {
			ToolkitOptions Options = new ToolkitOptions();
			Options.EnableHighResolution = false;
			Options.Backend = PlatformBackend.PreferNative;
			Toolkit.Init(Options);

			AspectRatio = (float)W / H;

			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
				throw new Exception("Failed to initialize SDL2");

			SDL.SDL_WindowFlags WFlags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN;
			if (NoBorder)
				WFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS;

			Window = SDL.SDL_CreateWindow(Title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, W, H, WFlags);

			GLContext = SDL.SDL_GL_CreateContext(Window);
			MakeCurrent();
		}

		public void Init() {
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);

			TKContext = new GraphicsContext(new ContextHandle(IntPtr.Zero),
						SDL.SDL_GL_GetProcAddress, () => new ContextHandle(SDL.SDL_GL_GetCurrentContext()));
			TKContext.LoadAll();

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

		public int MakeCurrent() {
			return SDL.SDL_GL_MakeCurrent(Window, GLContext);
		}

		public void Swap() {
			SDL.SDL_GL_SwapWindow(Window);
		}

		public void PollEvents() {
			SDL.SDL_Event Event;

			while (SDL.SDL_PollEvent(out Event) != 0) {
				switch (Event.type) {
					case SDL.SDL_EventType.SDL_QUIT:
						IsOpen = false;
						break;

					default:
						break;
				}
			}
		}
	}
}