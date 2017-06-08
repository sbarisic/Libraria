using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libraria.Collections;
using LibTech.Rendering;
using System.Drawing;
using Libraria.Rendering;

namespace LibTech {
	public static class Console {
		public const string Black = "^000000";
		public const string DarkBlue = "^00008B";
		public const string DarkGreen = "^006400";
		public const string DarkCyan = "^008B8B";
		public const string DarkRed = "^8B0000";
		public const string DarkMagenta = "^8B008B";
		public const string DarkYellow = "^D7C32A";
		public const string Gray = "^808080";
		public const string DarkGray = "^A9A9A9";
		public const string Blue = "^0000FF";
		public const string Green = "^008000";
		public const string Cyan = "^00FFFF";
		public const string Red = "^FF0000";
		public const string Magenta = "^FF00FF";
		public const string Yellow = "^FFFF00";
		public const string White = "^FFFFFF";

		const int LineHeight = 14;
		const int MaxLines = 128;
		const float ConsoleHeightRatio = 0.6f;
		const string InputPrefix = "> ";

		static AdvancedStack<string> ConsoleLines;
		static RenderTexture RT;
		static int RT_NVG;
		static float ScrollAmt;
		static Color ClearColor;

		public static string Input;
		public static bool IsOpen;

		static Console() {
			ConsoleLines = new AdvancedStack<string>();
			Clear();
			ScrollAmt = 0;
			Input = "";
		}

		static void AppendString(string Str) {
			ConsoleLines.Push(ConsoleLines.Pop() + Str);
			System.Console.Write(Str);
		}

		static void AppendLine(string Str) {
			ConsoleLines.Push(Str);
			while (ConsoleLines.Count > MaxLines)
				ConsoleLines.PopBottom();

			System.Console.WriteLine(Str);
		}

		public static void Clear() {
			for (int i = 0; i < MaxLines; i++)
				ConsoleLines.Push("");
			while (ConsoleLines.Count > MaxLines)
				ConsoleLines.PopBottom();
			Scroll(0, true);
		}

		public static void Write(string Str) {
			string[] Lines = Str.Split('\n');

			for (int i = 0; i < Lines.Length; i++) {
				if (i != 0)
					AppendLine("");
				AppendString(Lines[i]);
			}
		}

		public static void WriteLine(string Str) {
			Write(Str + "\n");
		}

		public static void WriteLine(string Fmt, params object[] Args) {
			WriteLine(string.Format(Fmt, Args));
		}

		public static void WriteLine(object Obj) {
			WriteLine(Obj.ToString());
		}
		
		public static void ParseInput() {
			string In = Input;
			Input = "";
			ParseInput(In);
		}

		public static void ParseInput(string In) {
			// TODO: Proper way

			if (In.Length == 0)
				return;

			if (In.StartsWith("echo ")) {
				In = In.Substring(5);
				Console.WriteLine(In);
			} else if (In == "clear")
				Clear();
			else
				Console.WriteLine("Unknown command '{0}'", In);
		}

		public static void Scroll(int Amt, bool Reset = false) {
			if (Reset)
				ScrollAmt = 0;
			ScrollAmt += (Amt * 10);
		}

		public static void InitGraphics() {
			RT = new RenderTexture(Engine.RenderWindow.Width, (MaxLines + 1) * LineHeight);
			RT_NVG = NanoVG.CreateImage(RT);
			ClearColor = Color.FromArgb(200, 16, 19, 25);
		}

		static void RenderContent(float Dt) {
			NanoVG.BeginFrame(RT.Width, RT.Height);
			//NanoVG.DrawRect(ClearColor, 0, 0, RT.Width, RT.Height);

			NanoVG.SetFont("clacon", LineHeight);
			NanoVG.TextAlign(TextAlign.TopLeft);

			string[] Lines = ConsoleLines.ToArray();
			for (int i = 0; i < Lines.Length; i++) {
				NanoVG.FillColor(Color.White);
				NanoVG.StyledText(0, LineHeight * i, Lines[i]);
			}

			NanoVG.FillColor(Color.Wheat);
			NanoVG.Text(0, LineHeight * MaxLines + 1, InputPrefix + Input + "_");
			NanoVG.EndFrame();
		}

		public static void Render(float Dt) {
			if (!IsOpen)
				return;

			if (RT == null)
				InitGraphics();

			RT.Bind();
			RT.Clear(ClearColor);
			RenderContent(Dt);
			RT.Unbind();

			float Y = (0 - RT.Height) + (NanoVG.Height * ConsoleHeightRatio) + ScrollAmt;
			NanoVG.BeginFrame();
			NanoVG.DrawTexturedRect(RT_NVG, 0, (int)Y, RT.Width, RT.Height);
			NanoVG.EndFrame();
		}
	}
}
