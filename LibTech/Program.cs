using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Libraria;
using Libraria.Rendering;

namespace LibTech {
	class Program {
		static RenderWindow RWind;

		static void Main(string[] args) {
			Console.Title = "LibTech";

			int W, H;
			Lib.GetScreenResolution(out W, out H, 0.8f);
			Console.WriteLine("Running at {0}x{1}", W, H);
			RWind = new RenderWindow("LibTech", W, H);
			RWind.Init();


			Matrix4 ViewMatrix = Matrix4.CreateOrthographicOffCenter(0, RWind.AspectRatio, 0, 1, -100, 100);

			float Off = 0.1f;
			VertexArray Triangle = new VertexArray();
			Triangle.SetData(new float[] {
				Off, Off, 0.0f,
				RWind.AspectRatio / 2, 1.0f - Off, 0.0f,
				RWind.AspectRatio - Off, Off, 0.0f
			}, VertexUsageHint.StaticDraw);
			Triangle.VertexAttribPointer(0, 3, AttribPointerType.Float);
			Triangle.AddAttribArray(0);

			ShaderProgram TestShader = ShaderProgram.CreateProgram("shaders\\test");
			TestShader.SetUniform("ViewMatrix", ref ViewMatrix);

			while (RWind.IsOpen) {
				RWind.PollEvents();
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				Triangle.Draw(0, 3, DrawPrimitiveType.Triangles);

				RWind.Swap();
			}

			Environment.Exit(0);
		}
	}
}
