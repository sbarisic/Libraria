using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using Libraria.Timing;
using OpenTK;
using Libraria;
using Libraria.Rendering;
using System.Threading;
using System.Reflection;

namespace LibTech {
	public static class Engine {
		public static RenderWindow RenderWindow;

		public static string GameFolder;
		public static bool Headless;
	}

	class Program {
		static Thread UpdateThread;

		static IModule Server, Client, UI;

		static bool Running;
		static TimeSpan UpdateRate, RenderRate;

		static void Main(string[] args) {
			Console.Title = "LibTech";
			Engine.GameFolder = "testgame";
			Engine.Headless = false;
			Running = true;

			UpdateRate = TimeSpan.FromSeconds(1.0 / 25);
			RenderRate = TimeSpan.FromSeconds(1.0 / 120);

			if (!Engine.Headless) {
				SpawnWindow();

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

		static void SpawnWindow() {
			int W, H;
			Lib.GetScreenResolution(out W, out H, 0.8f);
			Console.WriteLine("Running at {0}x{1}", W, H);
			Engine.RenderWindow = new RenderWindow("LibTech", W, H);
			Engine.RenderWindow.Init();
		}

		static void Update(float Dt) {
			Server?.Event(ModuleEvent.UPDATE, Dt);
			Client?.Event(ModuleEvent.UPDATE, Dt);
			UI?.Event(ModuleEvent.UPDATE, Dt);
		}

		static void Render(float Dt) {
			Engine.RenderWindow.PollEvents();
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
	}
}
