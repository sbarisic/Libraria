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
using System.Runtime.InteropServices;

namespace Libraria.Rendering {
	public delegate void OnMouseMoveAction(int X, int Y, int RelativeX, int RelativeY);
	public delegate void OnMouseButtonAction(int Clicks, int Button, int X, int Y, bool Pressed);
	public delegate void OnMouseWheelAction(int X, int Y);
	public delegate void OnKeyAction(int Repeat, int Scancode, int Keycode, int Mod, bool Pressed);
	public delegate void OnTextInputAction(string Txt);

	public unsafe class RenderWindow {
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
			SDL.SDL_WindowFlags WFlags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL
				| SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN;

			if (NoBorder)
				WFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS;

			Window = SDL.SDL_CreateWindow(Title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, W, H, WFlags);
			SetWindowSize(W, H);

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

			IsOpen = true;
		}

		public void SetWindowSize(int W, int H) {
			if (W == -1)
				W = Width;
			if (H == -1)
				H = Height;

			SDL.SDL_SetWindowSize(Window, W, H);

			Width = W;
			Height = H;
			AspectRatio = (float)W / H;
		}

		public void Reset() {
			GL.Viewport(0, 0, Width, Height);
			GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
		}

		public int MakeCurrent() {
			return SDL.SDL_GL_MakeCurrent(Window, GLContext);
		}

		public void Swap() {
			SDL.SDL_GL_SwapWindow(Window);
		}

		public void Close() {
			SDL.SDL_GL_DeleteContext(GLContext);
			SDL.SDL_DestroyWindow(Window);
		}

		public void StartTextInput(bool Start) {
			if (Start)
				SDL.SDL_StartTextInput();
			else
				SDL.SDL_StopTextInput();
		}

		public event OnMouseMoveAction OnMouseMove;
		public event OnMouseButtonAction OnMouseButton;
		public event OnMouseWheelAction OnMouseWheel;
		public event OnKeyAction OnKey;
		public event OnTextInputAction OnTextInput;

		public bool PollEvents() {
			SDL.SDL_Event Event;

			while (SDL.SDL_PollEvent(out Event) != 0) {
				switch (Event.type) {
					case SDL.SDL_EventType.SDL_QUIT:
						IsOpen = false;
						return false;

					case SDL.SDL_EventType.SDL_WINDOWEVENT:
						break;

					case SDL.SDL_EventType.SDL_MOUSEMOTION:
						OnMouseMove?.Invoke(Event.motion.x, Event.motion.y, Event.motion.xrel, Event.motion.yrel);
						break;

					case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
						OnMouseButton?.Invoke(Event.button.clicks, Event.button.button, Event.button.x, Event.button.y, true);
						break;

					case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
						OnMouseButton?.Invoke(Event.button.clicks, Event.button.button, Event.button.x, Event.button.y, false);
						break;

					case SDL.SDL_EventType.SDL_MOUSEWHEEL:
						OnMouseWheel?.Invoke(Event.wheel.x, Event.wheel.y);
						break;

					case SDL.SDL_EventType.SDL_KEYDOWN:
						OnKey?.Invoke(Event.key.repeat, (int)Event.key.keysym.scancode, (int)Event.key.keysym.sym, (int)Event.key.keysym.mod, true);
						break;

					case SDL.SDL_EventType.SDL_KEYUP:
						OnKey?.Invoke(Event.key.repeat, (int)Event.key.keysym.scancode, (int)Event.key.keysym.sym, (int)Event.key.keysym.mod, false);
						break;

					case SDL.SDL_EventType.SDL_TEXTINPUT:
						OnTextInput?.Invoke(Marshal.PtrToStringAuto(new IntPtr(Event.text.text)));
						break;

					case SDL.SDL_EventType.SDL_TEXTEDITING:

						break;

					default:
						break;
				}
			}
			
			return true;
		}
	}
}