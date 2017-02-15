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
using Libraria.NanoVG;
using OpenTK;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace UI {
	public class Entry : IModule {
		IModule Client;

		public void Init(params object[] Args) {
			Client = Args[0] as IModule;

			RT = new RenderTexture((int)(600 * Engine.RenderWindow.AspectRatio), 600, true, true, TexFilterMode.Linear);
			RTImg = NanoVG.CreateImage(RT);
		}

		RenderTexture RT;
		int RTImg;

		public void Event(ModuleEvent Evt, params object[] Args) {
			if (Evt == ModuleEvent.RENDER) {
				float Dt = (float)Args[0];

				RT.Bind();
				{
					RT.Clear(Color.Black);

					NanoVG.BeginFrame();
					{
						const int Sz = 10;
						NanoVG.Save();
						for (int i = 1; i < 10; i++) {
							NanoVG.RotateAround(300 * Engine.RenderWindow.AspectRatio, 300, 0.002f * i);
							NanoVG.DrawRectOutline(Color.DarkGray, (float)Sz / 3, Sz * i, Sz * i, 600 * Engine.RenderWindow.AspectRatio - 2 * Sz * i, 600 - 2 * Sz * i);
						}
						NanoVG.Restore();

						int X = 500;
						int Y = 50;
						NanoVG.DrawTexturedRect(RTImg, X, Y, RT.Width / 2, RT.Height / 2);
						NanoVG.DrawRectOutline(Color.Red, 2.0f, X, Y, RT.Width / 2, RT.Height / 2);

						NanoVG.DrawText("clacon", 180.0f, TextAlign.CenterMiddle, Color.Black, 300 * Engine.RenderWindow.AspectRatio, 300, "こんにちは世界", 6, 5);
						NanoVG.DrawText("clacon", 180.0f, TextAlign.CenterMiddle, Color.White, 300 * Engine.RenderWindow.AspectRatio, 300, "こんにちは世界");
					}
					NanoVG.EndFrame();
				}
				RT.Unbind();

				NanoVG.BeginFrame();
				NanoVG.DrawTexturedRect(RTImg, 0, 0, RT.Width, RT.Height);
				NanoVG.EndFrame();
			}
		}
	}
}