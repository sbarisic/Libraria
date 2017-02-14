using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using LibTech;
using LibTech.Rendering;
using Libraria.Rendering;
using OpenTK;
using System.Runtime.InteropServices;

namespace UI {
	public class Entry : IModule {
		IModule Client;

		public void Init(params object[] Args) {
			Client = Args[0] as IModule;

		}

		public void Event(ModuleEvent Evt, params object[] Args) {
			if (Evt == ModuleEvent.RENDER) {
				float Dt = (float)Args[0];

				NanoVG.BeginFrame();

				const int Sz = 10;

				NanoVG.Save();
				for (int i = 1; i < 10; i++) {
					NanoVG.RotateAround(300 * Engine.RenderWindow.AspectRatio, 300, 0.002f * i);
					NanoVG.DrawRectOutline(Color.White, (float)Sz / 3, Sz * i, Sz * i, 600 * Engine.RenderWindow.AspectRatio - 2 * Sz * i, 600 - 2 * Sz * i);
				}
				NanoVG.Restore();

				NanoVG.SetFont("clacon", 128.0f);
				NanoVG.TextAlign(TextAlign.CenterMiddle);
				NanoVG.FillColor(Color.DarkGreen);
				NanoVG.Text(300 * Engine.RenderWindow.AspectRatio, 300, "Hello World!");

				NanoVG.EndFrame();
			}
		}

		//IntPtr Txt2 = IntPtr.Zero;

		/*
	void DrawNVG() {
		string Txt = "Hello NanoVG World! 日本語は面白いです。";
		int TxtX = 100;
		int TxtY = 100;
		float Spacing = 25;
		float Rounding = 20;

		NVG.Translate(Engine.NanoVG, 300, 300);
		NVG.Rotate(Engine.NanoVG, Engine.TimeSinceLaunch.ElapsedMilliseconds / 1000.0f);
		NVG.Translate(Engine.NanoVG, -300, -300);

		NVG.FontSize(Engine.NanoVG, 80.0f);
		NVG.FontFace(Engine.NanoVG, "sans");
		NVG.TextAlign(Engine.NanoVG, NVG.NVG_ALIGN_LEFT | NVG.NVG_ALIGN_MIDDLE);
		float[] Bounds = new float[4];
		NVG.TextBounds(Engine.NanoVG, TxtX, TxtY, Txt, IntPtr.Zero, Bounds);

		float X = Bounds[0] - Spacing;
		float Y = Bounds[1] - Spacing;
		float W = Bounds[2] - Bounds[0] + Spacing * 2;
		float H = Bounds[3] - Bounds[1] + Spacing * 2;

		NVGpaint P = NVG.BoxGradient(Engine.NanoVG, X, Y, W, H, 30, 30, Color.CornflowerBlue, Color.Black);
		NVG.BeginPath(Engine.NanoVG);
		NVG.RoundedRect(Engine.NanoVG, X, Y, W, H, Rounding);
		NVG.FillPaint(Engine.NanoVG, P);
		NVG.Fill(Engine.NanoVG);

		NVG.Save(Engine.NanoVG);
		{
			NVG.FillColor(Engine.NanoVG, Color.Black);
			NVG.FontBlur(Engine.NanoVG, 6);
			NVG.Text(Engine.NanoVG, TxtX, TxtY, Txt, IntPtr.Zero);
			NVG.Text(Engine.NanoVG, TxtX, TxtY, Txt, IntPtr.Zero);
		}
		NVG.Restore(Engine.NanoVG);

		NVG.FillColor(Engine.NanoVG, Color.White);
		NVG.Text(Engine.NanoVG, TxtX, TxtY, Txt, IntPtr.Zero);
	}
	//*/
	}
}