using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;
using Libraria.IO;
using Libraria.Net;

using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace Test {
	class Program {
		static float[] Data;
		static float SampleFrequency;

		static float Tolerance = 0.1f;
		static float Highest, Lowest, Middle, DataFrequency;

		static void Main(string[] Args) {
			Data = new float[800];

			Thread T = new Thread(Update);
			T.Priority = ThreadPriority.AboveNormal;
			T.IsBackground = true;
			T.Start();

			RenderWindow RW = new RenderWindow(new VideoMode(800, 600), "Oscilloscope");
			RW.SetView(new View(new FloatRect(0, RW.Size.Y, RW.Size.X, -RW.Size.Y)));
			RW.Closed += (S, E) => RW.Close();

			Font F = new Font("C:\\Windows\\Fonts\\consola.ttf");
			Text InfoText = new Text("", F, 16);
			InfoText.Position = new Vector2f(0, RW.Size.Y);
			InfoText.Color = Color.White;
			InfoText.Scale = new Vector2f(1.0f, -1.0f);

			Vertex[] DataPoints = new Vertex[RW.Size.X];
			for (int i = 0; i < DataPoints.Length; i++) {
				DataPoints[i].Color = Color.Green;
				DataPoints[i].Position = new Vector2f(i, 0);
			}

			Color GridClr = new Color(25, 25, 25);
			float VertVolts = 6;
			float Scale = RW.Size.Y / VertVolts;

			while (RW.IsOpen) {
				RW.DispatchEvents();

				Highest = float.NegativeInfinity;
				Lowest = float.PositiveInfinity;
				for (int i = 0; i < Data.Length; i++) {
					Highest = Math.Max(Highest, Data[i]);
					Lowest = Math.Min(Lowest, Data[i]);
				}

				if (DoRefresh) {
					DoRefresh = false;

					for (int i = 0; i < DataPoints.Length; i++)
						DataPoints[i].Position.Y = Data[Data.Length - DataPoints.Length + i] * Scale;
				}


				InfoText.DisplayedString = string.Format("High: {0:0.00} V\nLow: {1:0.00} V\nMiddle: {2:0.00} V\nFrequency: {3:0.0} Hz\n\n", Highest, Lowest, Middle, DataFrequency);

				float VerticalSample = 1.0f / (int)(RW.Size.X / Scale);
				InfoText.DisplayedString += string.Format("Horizontal: {0} V\nVertical: {1} ms, {2} Hz", (RW.Size.Y / Scale) / VertVolts / 2, VerticalSample, 1.0f / VerticalSample);
				RW.Clear(Color.Black);


				for (int i = 0; i < (int)(RW.Size.Y / Scale * 2) + 1; i++)
					DrawHorizontal(RW, i * Scale / 2, GridClr);
				for (int i = 0; i < (int)(RW.Size.X / Scale) + 1; i++)
					DrawVertical(RW, i * Scale, GridClr);

				DrawHorizontal(RW, Highest * Scale, Color.Red);
				DrawHorizontal(RW, Lowest * Scale, Color.Blue);
				DrawHorizontal(RW, Middle * Scale, Color.White);

				RW.Draw(DataPoints, PrimitiveType.LinesStrip);
				RW.Draw(InfoText);
				RW.Display();

				RW.SetTitle("Sample: " + SampleFrequency + " Hz");
				//Console.WriteLine(SampleFrequency);
			}

			/*Console.WriteLine("Done!");
			Console.ReadLine();*/
		}

		static void DrawHorizontal(RenderWindow RW, float Y, Color Clr) {
			if (Y > RW.Size.Y || Y < 0)
				return;
			RW.Draw(new Vertex[] { new Vertex(new Vector2f(0, Y), Clr), new Vertex(new Vector2f(RW.Size.X, Y), Clr) }, PrimitiveType.Lines);
		}

		static void DrawVertical(RenderWindow RW, float X, Color Clr) {
			if (X > RW.Size.X || X < 0)
				return;
			RW.Draw(new Vertex[] { new Vertex(new Vector2f(X, 0), Clr), new Vertex(new Vector2f(X, RW.Size.Y), Clr) }, PrimitiveType.Lines);
		}

		static bool DoRefresh;
		static Stopwatch FreqTimer;
		static void OnProbe(float Prev, float Cur) {
			bool DoTrigger = false;

			Middle = ((Highest - Lowest) / 2) + Lowest;
			if (((Prev > Middle) && (Cur < Middle)) || ((Prev < Middle) && (Cur > Middle)))
				DoTrigger = true;

			if (DoTrigger && Cur > Middle) {
				DoRefresh = true;

				DataFrequency = (float)((double)Stopwatch.Frequency / FreqTimer.ElapsedTicks);
				FreqTimer.Restart();
			}
		}

		static void Update() {
			BusPirate BP = BusPirate.OpenBBIO1("COM42");
			Stopwatch SWatch = Stopwatch.StartNew();
			FreqTimer = Stopwatch.StartNew();

			while (true) {
				for (int i = 1; i < Data.Length; i++)
					Data[i - 1] = Data[i];
				Data[Data.Length - 1] = BP.ProbeVoltage();
				OnProbe(Data[Data.Length - 2], Data[Data.Length - 1]);

				while (((double)Stopwatch.Frequency / SWatch.ElapsedTicks) > 1000)
					;
				SampleFrequency = (float)((double)Stopwatch.Frequency / SWatch.ElapsedTicks);
				SWatch.Restart();
			}
		}


	}
}