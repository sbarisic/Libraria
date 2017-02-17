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

namespace LibTech {
	public static partial class Engine {
		public static RenderWindow RenderWindow;

		public static bool DrawFPSCounter;
		public static bool DedicatedServer;
		public static Stopwatch TimeSinceLaunch;
	}

	class Program {
		static Thread UpdateThread;
		static TimeSpan UpdateRate, RenderRate;

		static IModule Server, Client, UI;

		static void Main(string[] args) {
			SetProcessDPIAware();

			Files.Initialize("basegame");
			// TODO: Load from arguments passed or somethin'
			Files.SetGameFolder("testgame");

			Console.Visible = true;
			Engine.DedicatedServer = false;
			Engine.DrawFPSCounter = true;
			Engine.TimeSinceLaunch = Stopwatch.StartNew();

			bool Running = true;
			UpdateRate = TimeSpan.FromSeconds(1.0 / 25);
			RenderRate = TimeSpan.FromSeconds(1.0 / (60 * 2));

			if (!Engine.DedicatedServer) {
				RenderWindow.InitRenderer();
				SpawnWindow();

				NanoVG.Initialize();
				Engine.RenderWindow.SetWindowSize(-1, -1);

				Client = ModuleLoader.LoadModule(Engine.GameFolder, "Client");
				UI = ModuleLoader.LoadModule(Engine.GameFolder, "UI");
			} else {
				// TODO: Spawn a console here
			}

			Server = ModuleLoader.LoadModule(Engine.GameFolder, "Server");
			Server?.Open(null, null, null);

			// Update loop
			UpdateThread = new Thread(() => {
				Clock UpdateClk = new Clock();
				while (Running)
					UpdateClk.AtLeast(UpdateRate, (Dt) => Update(Dt));
			});
			UpdateThread.Start();

			// Render loop
			if (!Engine.DedicatedServer) {
				Client?.Open(null, Server, UI);
				UI?.Open(Client, Server, null);

				Clock RenderClk = new Clock();
				while (Engine.RenderWindow.IsOpen)
					RenderClk.AtLeast(RenderRate, (Dt) => Render(Dt));

				Running = false;
				Engine.RenderWindow.Close();
			}

			while (Running)
				Thread.Sleep(10);
			Server?.Close();
			Client?.Close();
			UI?.Close();
			Environment.Exit(0);
		}

		static void SpawnWindow(int PrefW = -1, int PrefH = -1) {
			int W, H;
			Lib.GetScreenResolution(out W, out H, 0.8f);
			if (PrefW != -1)
				W = PrefW;
			if (PrefH != -1)
				H = PrefH;

			Console.WriteLine(Console.Yellow + "Running at {0}x{1}", W, H);

			Engine.RenderWindow = new RenderWindow("LibTech", W, H, false);
			Engine.RenderWindow.Init();

			Console.WriteLine("Vendor: {0}", GL.GetString(StringName.Vendor));
			Console.WriteLine("Renderer: {0}", GL.GetString(StringName.Renderer));
			Console.WriteLine("Version: {0}", GL.GetString(StringName.Version));
			Console.WriteLine("Shading language version: {0}", GL.GetString(StringName.ShadingLanguageVersion));
		}

		static void Update(float Dt) {
			Server?.Update(Dt);
			Client?.Update(Dt);
			UI?.Update(Dt);
		}

		static void Render(float Dt) {
			if (!Engine.RenderWindow.PollEvents())
				return;

			Engine.RenderWindow.Reset();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

			Client?.Render(Dt);
			UI?.Render(Dt);
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
