using Libraria.Rendering;
using LibTech;
using LibTech.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibTech.Networking;
using Libraria.Maths;
using OpenTK.Input;

namespace UILib {
	class MainMenuState : GameState {
		AABB? DemoGameButton = null, ExitButton = null;
		Color DemoGameColor = Color.White, ExitColor = Color.White;

		public override void Render(float Dt) {
			if (StateManager.GetTopState() != this)
				return;

			const float Offset = 20;
			NanoVG.DrawRectOutline(Color.DarkGray, 6.0f, Offset, Offset, NanoVG.Width - Offset * 2, NanoVG.Height - Offset * 2);

			NanoVG.DrawText("clacon", 24, TextAlign.TopLeft, DemoGameColor, 100, NanoVG.Height - 200, "Demo Game", ref DemoGameButton);
			NanoVG.DrawText("clacon", 24, TextAlign.TopLeft, ExitColor, 100, NanoVG.Height - 170, "Exit", ref ExitButton);
		}

		public override bool OnMouseButton(MouseButtonEventArgs E, bool Pressed) {
			int X = NanoVG.ScaleX(E.X);
			int Y = NanoVG.ScaleY(E.Y);

			if (Pressed && DemoGameButton?.Collide(X, Y) == true) {
				StateManager.Push(new DemoGameState());
				return true;
			}

			if (Pressed && ExitButton?.Collide(X, Y) == true) {
				StateManager.Pop();
				Engine.Running = false;
				return true;
			}

			return base.OnMouseButton(E, Pressed);
		}

		public override bool OnMouseMove(MouseMoveEventArgs E) {
			int X = NanoVG.ScaleX(E.X);
			int Y = NanoVG.ScaleY(E.Y);

			if (DemoGameButton?.Collide(X, Y) == true)
				DemoGameColor = Color.Green;
			else
				DemoGameColor = Color.White;

			if (ExitButton?.Collide(X, Y) == true)
				ExitColor = Color.Green;
			else
				ExitColor = Color.White;

			return base.OnMouseMove(E);
		}
	}

	class DemoGameState : GameState {
		public override void Enter() {
			Engine.Client.Message("EnterWorld");
		}

		public override void Exit() {
			Engine.Client.Message("ExitWorld");
		}

		public override void Render(float Dt) {

		}

		public override bool OnKey(KeyboardKeyEventArgs E, bool Pressed) {
			if (E.Key == Key.Escape) {
				StateManager.Pop();
				return true;
			}

			return base.OnKey(E, Pressed);
		}
	}

	public class Entry : ModuleBase {
		GameStateManager StateMgr;

		public override void Open() {
			StateMgr = new GameStateManager();
			StateMgr.Push(new MainMenuState());

		}

		public override bool OnMouseMove(MouseMoveEventArgs E) {
			return StateMgr.OnMouseMove(E);
		}

		public override bool OnMouseButton(MouseButtonEventArgs E, bool Pressed) {
			return StateMgr.OnMouseButton(E, Pressed);
		}

		public override bool OnMouseWheel(MouseWheelEventArgs E) {
			return StateMgr.OnMouseWheel(E);
		}

		public override bool OnKey(KeyboardKeyEventArgs E, bool Pressed) {
			return StateMgr.OnKey(E, Pressed);
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
