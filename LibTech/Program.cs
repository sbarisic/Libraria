using Libraria;
using Libraria.NanoVG;
using Libraria.Rendering;
using Libraria.Timing;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using LibTech.Rendering;
using Libraria.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Input;
using OpenTK.Graphics;

namespace LibTech {
	public static partial class Engine {
		public static ModuleBase Server, Client, UI;

		public static bool Running;
		public static bool DedicatedServer;
		public static Stopwatch TimeSinceLaunch;

		public static Camera Camera;
		public static float UpdateDelta;
		public static RenderWindow RenderWindow;
		public static bool DrawFPSCounter;

		public static void Print(params object[] Args) {
			const string _SV = "^00FFFF";
			const string _CL = "^FFFF00";
			const string _UI = "^00FF33";

			string Str = string.Join("", Args);
			Assembly IntroAsm = Assembly.GetCallingAssembly();

			if (IntroAsm == Server?.GetType().Assembly)
				Console.WriteLine(_SV + Str);
			else if (IntroAsm == Client?.GetType().Assembly)
				Console.WriteLine(_CL + Str);
			else if (IntroAsm == UI?.GetType().Assembly)
				Console.WriteLine(_UI + Str);
			else
				Console.WriteLine(Str);
		}
	}

	public static class Input {
		public static KeyboardDevice Keyboard;

		internal static void Init() {
			Keyboard = Engine.RenderWindow.Keyboard;
		}
	}

	static class Program {
		static Thread UpdateThread;
		static TimeSpan UpdateRate, RenderRate;

		static void Main(string[] args) {
			SetProcessDPIAware();
			//Thread.Sleep(2000);

			Files.Initialize("basegame");

			// TODO: Load from arguments passed or somethin'
			//Files.SetGameFolder("testgame");
			Files.SetGameFolder("Blochs");

			AppDomain.CurrentDomain.AssemblyResolve += (S, E) => {
				string DllName = E.Name.Split(',')[0] + ".dll";

				string DllPath = Files.GetFilePath(DllName);
				if (!string.IsNullOrEmpty(DllPath))
					return Assembly.LoadFile(DllPath);

				return null;
			};

			Console.IsOpen = false;
			Engine.DedicatedServer = false;
			Engine.DrawFPSCounter = true;
			Engine.TimeSinceLaunch = Stopwatch.StartNew();
			Engine.Running = true;

			UpdateRate = TimeSpan.FromSeconds(1.0 / 60);
			RenderRate = TimeSpan.FromSeconds(1.0 / 120);

			Engine.Server = ModuleLoader.LoadModule(Engine.GameFolder, "Server");
			Engine.Server?.Open();

			// Update loop
			UpdateThread = new Thread(() => {
				Clock UpdateClk = new Clock();

				while (Engine.Running)
					UpdateClk.AtLeast(UpdateRate, (Dt) => Update(Dt));
			});

			if (Engine.DedicatedServer)
				UpdateThread.Start();

			// Render loop
			if (!Engine.DedicatedServer) {
				RenderWindow.InitRenderer();
				SpawnWindow();
				
				NanoVG.Initialize();
				Engine.RenderWindow.SetWindowSize(-1, -1);

				Engine.Camera = new Camera();
				Engine.Camera.Projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2, Engine.RenderWindow.AspectRatio, 0.01f, 5000.0f);
				Engine.Camera.SetPosition(new Vector3(0, 0, -100));
				Camera.Push(Engine.Camera);

				ModuleBase Client = ModuleLoader.LoadModule(Engine.GameFolder, "Client");
				Client?.Open();
				Engine.Client = Client;

				ModuleBase UI = ModuleLoader.LoadModule(Engine.GameFolder, "UI");
				UI?.Open();
				Engine.UI = UI;

				Engine.RenderWindow.OnUpdate += Update;
				Engine.RenderWindow.OnRender += Render;
				Engine.RenderWindow.Run(60);
				Engine.Running = false;
			} else {
				// TODO: Spawn console
			}

			while (Engine.Running)
				Thread.Sleep(10);

			ModuleLoader.UnloadModule(Engine.Server);
			ModuleLoader.UnloadModule(Engine.Client);
			ModuleLoader.UnloadModule(Engine.UI);
			Environment.Exit(0);
		}

		static void SpawnWindow(int PrefW = -1, int PrefH = -1) {
			int W, H;
			Lib.GetScreenResolution(out W, out H, 0.8f);
			if (PrefW != -1)
				W = PrefW;
			if (PrefH != -1)
				H = PrefH;

			Console.WriteLine(Console.Cyan + "Running at {0}x{1}", W, H);

			Engine.RenderWindow = new RenderWindow("LibTech", W, H, false);
			Engine.RenderWindow.MouseMove += (S, E) => OnMouseMove(E);
			Engine.RenderWindow.MouseDown += (S, E) => OnMouseButton(E, true);
			Engine.RenderWindow.MouseUp += (S, E) => OnMouseButton(E, false);
			Engine.RenderWindow.MouseWheel += (S, E) => OnMouseWheel(E);
			Engine.RenderWindow.KeyDown += (S, E) => OnKey(E, true);
			Engine.RenderWindow.KeyUp += (S, E) => OnKey(E, false);
			Engine.RenderWindow.KeyPress += (S, E) => OnTextInput(E.KeyChar.ToString());

			Input.Init();

			Console.WriteLine(Console.Cyan + "Vendor: {0}", GL.GetString(StringName.Vendor));
			Console.WriteLine(Console.Cyan + "Renderer: {0}", GL.GetString(StringName.Renderer));
			Console.WriteLine(Console.Cyan + "Version: {0}", GL.GetString(StringName.Version));
			Console.WriteLine(Console.Cyan + "Shading language version: {0}", GL.GetString(StringName.ShadingLanguageVersion));

			GraphicsMode GMode = Engine.RenderWindow.Context.GraphicsMode;
			Console.WriteLine(Console.Cyan + "Color/Depth/Stencil: {0}/{1}/{2} bpp", GMode.ColorFormat.BitsPerPixel, GMode.Depth, GMode.Stencil);
		}

		static void OnMouseMove(MouseMoveEventArgs E) {
			if (Console.IsOpen)
				return;

			if (!(Engine.UI?.OnMouseMove(E) ?? false))
				if (Engine.Client?.OnMouseMove(E) == false)
					Engine.Camera.MouseRotate(Engine.UpdateDelta, -E.XDelta, -E.YDelta); // TODO: Add delta time
		}

		static void OnMouseButton(MouseButtonEventArgs E, bool Pressed) {
			if (Console.IsOpen)
				return;

			if (!(Engine.UI?.OnMouseButton(E, Pressed) ?? false))
				Engine.Client?.OnMouseButton(E, Pressed);
		}

		static void OnMouseWheel(MouseWheelEventArgs E) {
			if (Console.IsOpen) {
				Console.Scroll(E.Value);
				return;
			}

			if (!(Engine.UI?.OnMouseWheel(E) ?? false))
				Engine.Client?.OnMouseWheel(E);
		}

		static void OnKey(KeyboardKeyEventArgs E, bool Pressed) {
			if (Pressed && E.Key == Key.F1) {
				Console.IsOpen = !Console.IsOpen;
				Console.Scroll(0, true);
				return;
			}

			if (Console.IsOpen) {
				if (Pressed) {
					if (E.Key == Key.BackSpace && Console.Input.Length > 0)
						Console.Input = Console.Input.Substring(0, Console.Input.Length - 1);
					else if (E.Key == Key.Enter || E.Key == Key.KeypadEnter)
						Console.ParseInput();
				}
				return;
			}

			if (!(Engine.UI?.OnKey(E, Pressed) ?? false))
				Engine.Client?.OnKey(E, Pressed);
		}

		static void OnTextInput(string Txt) {
			if (Console.IsOpen) {
				Console.Input += Txt;
				return;
			}

			if (!(Engine.UI?.OnTextInput(Txt) ?? false))
				Engine.Client?.OnTextInput(Txt);
		}

		static void Update(float Dt) {
			Engine.UpdateDelta = Dt;
			Engine.Server?.Update(Dt);
			Engine.Client?.Update(Dt);
			Engine.UI?.Update(Dt);
		}

		static void Render(float Dt) {
			if (!Engine.Running) {
				Engine.RenderWindow.Close();
				return;
			}

			Engine.Client?.Render(Dt);
			Engine.UI?.Render(Dt);
			Console.Render(Dt);
			if (Engine.DrawFPSCounter) {
				NanoVG.BeginFrame();
				NanoVG.DrawText("clacon", 12, TextAlign.TopLeft, Color.White, 0, 0, string.Format("FPS: {0} - {1} ms", 1.0f / Dt, Dt));
				NanoVG.EndFrame();
			}
		}

		[DllImport("user32")]
		private static extern bool SetProcessDPIAware();
	}
}
