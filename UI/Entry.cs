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
using Libraria.Collections;

namespace UI {
	class MainMenuState : GameState {
		public override void Render(float Dt) {
			const float Offset = 20;

			NanoVG.DrawRectOutline(Color.DarkGray, 6.0f, Offset, Offset, NanoVG.Width - Offset * 2, NanoVG.Height - Offset * 2);

			/*NanoVG.SetFont("clacon", 24);
			NanoVG.DrawParagraph("^FF0000Red^00FF00Green^0000FFBlue^FFFFFFWhite\nRedGreenBlueWhite", 100, 100, 100);*/
		}
	}

	public class Entry : ModuleBase {
		GameStateManager StateMgr;

		public override void Open(ModuleBase Client, ModuleBase Server, ModuleBase UI) {
			StateMgr = new GameStateManager();
			StateMgr.Push(new MainMenuState());

			Engine.Print("Hello UI!");
		}
		
		public override bool OnMouseMove(int X, int Y, int RelativeX, int RelativeY) {
			return StateMgr.OnMouseMove(X, Y, RelativeX, RelativeY);
		}

		public override bool OnMouseButton(int Clicks, int Button, int X, int Y, bool Pressed) {
			return StateMgr.OnMouseButton(Clicks, Button, X, Y, Pressed);
		}

		public override bool OnMouseWheel(int X, int Y) {
			return StateMgr.OnMouseWheel(X, Y);
		}

		public override bool OnKey(int Repeat, Scancodes Scancode, int Keycode, int Mod, bool Pressed) {
			return StateMgr.OnKey(Repeat, Scancode, Keycode, Mod, Pressed);
		}

		public override bool OnTextInput(string Txt) {
			return StateMgr.OnTextInput(Txt);
		}

		public override void Update(float Dt) {
			StateMgr.Update(Dt);
		}

		public override void Render(float Dt) {
			NanoVG.BeginFrame();
			StateMgr.Render(Dt);
			NanoVG.EndFrame();
		}
	}
}