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
using System.Windows.Input;
using System.IO;
using System.Reflection;

namespace LibTech {
	public static partial class Engine {
		internal static ModuleBase Server, Client, UI;

		public static RenderWindow RenderWindow;
		public static bool DrawFPSCounter;
		public static bool DedicatedServer;
		public static Stopwatch TimeSinceLaunch;

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

	static class Program {
		static Thread UpdateThread;
		static TimeSpan UpdateRate, RenderRate;

		static void Main(string[] args) {
			SetProcessDPIAware();

			Files.Initialize("basegame");
			// TODO: Load from arguments passed or somethin'
			Files.SetGameFolder("testgame");

			Console.IsOpen = false;
			Engine.DedicatedServer = false;
			Engine.DrawFPSCounter = true;
			Engine.TimeSinceLaunch = Stopwatch.StartNew();

			bool Running = true;
			UpdateRate = TimeSpan.FromSeconds(1.0 / 25);
			RenderRate = TimeSpan.FromSeconds(1.0 / 120);

			if (!Engine.DedicatedServer) {
				RenderWindow.InitRenderer();
				SpawnWindow();

				NanoVG.Initialize();
				Engine.RenderWindow.SetWindowSize(-1, -1);

				Engine.Client = ModuleLoader.LoadModule(Engine.GameFolder, "Client");
				Engine.UI = ModuleLoader.LoadModule(Engine.GameFolder, "UI");
			} else {
				// TODO: Spawn a console here
			}

			Engine.Server = ModuleLoader.LoadModule(Engine.GameFolder, "Server");
			Engine.Server?.Open(null, null, null);

			// Update loop
			UpdateThread = new Thread(() => {
				Clock UpdateClk = new Clock();
				while (Running)
					UpdateClk.AtLeast(UpdateRate, (Dt) => Update(Dt));
			});
			UpdateThread.Start();

			// Render loop
			if (!Engine.DedicatedServer) {
				Engine.Client?.Open(null, Engine.Server, Engine.UI);
				Engine.UI?.Open(Engine.Client, Engine.Server, null);

				Clock RenderClk = new Clock();
				while (Engine.RenderWindow.IsOpen)
					RenderClk.AtLeast(RenderRate, (Dt) => Render(Dt));

				Running = false;
				Engine.RenderWindow.Close();
			}

			while (Running)
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

			Console.WriteLine("Running at {0}x{1}", W, H);

			Engine.RenderWindow = new RenderWindow("LibTech", W, H, false);
			Engine.RenderWindow.Init();
			Engine.RenderWindow.OnMouseMove += OnMouseMove;
			Engine.RenderWindow.OnMouseButton += OnMouseButton;
			Engine.RenderWindow.OnMouseWheel += OnMouseWheel;
			Engine.RenderWindow.OnKey += OnKey;
			Engine.RenderWindow.OnTextInput += OnTextInput;

			Console.WriteLine("Vendor: {0}", GL.GetString(StringName.Vendor));
			Console.WriteLine("Renderer: {0}", GL.GetString(StringName.Renderer));
			Console.WriteLine("Version: {0}", GL.GetString(StringName.Version));
			Console.WriteLine("Shading language version: {0}", GL.GetString(StringName.ShadingLanguageVersion));
		}

		static void OnMouseMove(int X, int Y, int RelX, int RelY) {
			if (Console.IsOpen)
				return;

			if (!(Engine.UI?.OnMouseMove(X, Y, RelX, RelY) ?? false))
				Engine.Client?.OnMouseMove(X, Y, RelX, RelY);
		}

		static void OnMouseButton(int Clicks, int Button, int X, int Y, bool Pressed) {
			if (Console.IsOpen)
				return;

			if (!(Engine.UI?.OnMouseButton(Clicks, Button, X, Y, Pressed) ?? false))
				Engine.Client?.OnMouseButton(Clicks, Button, X, Y, Pressed);
		}

		static void OnMouseWheel(int X, int Y) {
			if (Console.IsOpen) {
				Console.Scroll(Y);
				return;
			}

			if (!(Engine.UI?.OnMouseWheel(X, Y) ?? false))
				Engine.Client?.OnMouseWheel(X, Y);
		}

		static void OnKey(int Repeat, Scancodes Scancode, int Keycode, int Mod, bool Pressed) {
			if (Pressed && Scancode == Scancodes.F1) {
				Console.IsOpen = !Console.IsOpen;
				Console.Scroll(0, true);
				return;
			}

			if (Console.IsOpen) {
				if (Pressed) {
					if ((Scancode == Scancodes.Backspace || Scancode == Scancodes.Kp_Backspace) && (Console.Input.Length > 0))
						Console.Input = Console.Input.Substring(0, Console.Input.Length - 1);
					else if (Scancode == Scancodes.Kp_Enter || Scancode == Scancodes.Return)
						Console.ParseInput();
				}
				return;
			}

			if (!(Engine.UI?.OnKey(Repeat, Scancode, Keycode, Mod, Pressed) ?? false))
				Engine.Client?.OnKey(Repeat, Scancode, Keycode, Mod, Pressed);
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
			Engine.Server?.Update(Dt);
			Engine.Client?.Update(Dt);
			Engine.UI?.Update(Dt);
		}

		static void Render(float Dt) {
			if (!Engine.RenderWindow.PollEvents())
				return;

			Engine.RenderWindow.Reset();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

			Engine.Client?.Render(Dt);
			Engine.UI?.Render(Dt);
			Console.Render(Dt);
			if (Engine.DrawFPSCounter) {
				NanoVG.BeginFrame();
				NanoVG.DrawText("clacon", 12, TextAlign.TopLeft, Color.White, 0, 0, string.Format("FPS: {0} - {1} ms", 1.0f / Dt, Dt));
				NanoVG.EndFrame();
			}

			Engine.RenderWindow.Swap();
		}

		[DllImport("user32")]
		private static extern bool SetProcessDPIAware();
	}
}
