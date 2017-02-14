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

namespace LibTech {
	public static partial class Engine {
		public static RenderWindow RenderWindow;

		public static bool Headless;
		public static Stopwatch TimeSinceLaunch;
	}

	class Program {
		static Thread UpdateThread;

		static IModule Server, Client, UI;

		static bool Running;
		static TimeSpan UpdateRate, RenderRate;

		static void Main(string[] args) {
			Console.Title = "LibTech";
			SetProcessDPIAware();

			Files.Initialize("basegame");
			// TODO: Load from arguments passed or somethin'
			Files.SetGameFolder("testgame");

			Engine.Headless = false;
			Engine.TimeSinceLaunch = Stopwatch.StartNew();
			Running = true;

			UpdateRate = TimeSpan.FromSeconds(1.0 / 25);
			RenderRate = TimeSpan.FromSeconds(1.0 / (60 * 2));

			if (!Engine.Headless) {
				RenderWindow.InitRenderer();
				SpawnWindow();

				NanoVG.Initialize();
				Engine.RenderWindow.SetWindowSize(-1, -1);

				Client = ModuleLoader.LoadModule(Engine.GameFolder, "Client");
				UI = ModuleLoader.LoadModule(Engine.GameFolder, "UI");
			}
			Server = ModuleLoader.LoadModule(Engine.GameFolder, "Server");

			// Update loop
			UpdateThread = new Thread(() => {
				Clock UpdateClk = new Clock();
				while (Running)
					UpdateClk.AtLeast(UpdateRate, (Dt) => Update(Dt));
			});
			UpdateThread.Start();

			// Render loop
			if (!Engine.Headless) {
				Client?.Init();
				UI?.Init(Client);

				Clock RenderClk = new Clock();
				while (Engine.RenderWindow.IsOpen)
					RenderClk.AtLeast(RenderRate, (Dt) => Render(Dt));
			}

			while (Running)
				Thread.Sleep(10);
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

			Engine.RenderWindow = new RenderWindow("LibTech", W, H);
			Engine.RenderWindow.Init();

			Console.WriteLine("Vendor: {0}", GL.GetString(StringName.Vendor));
			Console.WriteLine("Renderer: {0}", GL.GetString(StringName.Renderer));
			Console.WriteLine("Version: {0}", GL.GetString(StringName.Version));
			Console.WriteLine("Shading language version: {0}", GL.GetString(StringName.ShadingLanguageVersion));
		}

		static void Update(float Dt) {
			Server?.Event(ModuleEvent.UPDATE, Dt);
			Client?.Event(ModuleEvent.UPDATE, Dt);
			UI?.Event(ModuleEvent.UPDATE, Dt);
		}

		static void Render(float Dt) {
			Engine.RenderWindow.PollEvents();
			Engine.RenderWindow.Reset();

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// Perspective 3D
			Client?.Event(ModuleEvent.RENDER, Dt);

			// Ortho 2D
			UI?.Event(ModuleEvent.RENDER, Dt);

			Engine.RenderWindow.Swap();
		}

		static void Exit() {
			Engine.RenderWindow?.Close();
			Running = false;
		}

		[DllImport("user32")]
		private static extern bool SetProcessDPIAware();
	}
}
