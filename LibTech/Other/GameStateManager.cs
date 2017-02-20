using Libraria.Collections;
using Libraria.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTech {
	public abstract class GameState {
		bool Paused;

		public virtual void Enter() {
		}

		public virtual void Pause() {
			Paused = true;
		}

		public virtual void Resume() {
			Paused = false;
		}

		public virtual void Exit() {
		}

		// Input

		public virtual bool OnMouseMove(int X, int Y, int RelativeX, int RelativeY) {
			return false;
		}

		public virtual bool OnMouseButton(int Clicks, int Button, int X, int Y, bool Pressed) {
			return false;
		}

		public virtual bool OnMouseWheel(int X, int Y) {
			return false;
		}

		public virtual bool OnKey(int Repeat, Scancodes Scancode, int Keycode, int Mod, bool Pressed) {
			return false;
		}

		public virtual bool OnTextInput(string Txt) {
			return false;
		}

		public virtual void Update(float Dt) {
		}

		public virtual void Render(float Dt) {
		}
	}

	public class GameStateManager {
		AdvancedStack<GameState> StateStack;

		public GameStateManager() {
			StateStack = new AdvancedStack<GameState>();
		}

		public void Push(GameState State) {
			StateStack.Peek()?.Pause();
			StateStack.Push(State);
			State.Enter();
		}

		public GameState Pop() {
			GameState S = StateStack.Pop();
			S.Exit();
			StateStack.Peek()?.Resume();
			return S;
		}

		public bool OnMouseMove(int X, int Y, int RelativeX, int RelativeY) {
			return GetInputState()?.OnMouseMove(X, Y, RelativeX, RelativeY) ?? false;
		}

		public bool OnMouseButton(int Clicks, int Button, int X, int Y, bool Pressed) {
			return GetInputState()?.OnMouseButton(Clicks, Button, X, Y, Pressed) ?? false;
		}

		public bool OnMouseWheel(int X, int Y) {
			return GetInputState()?.OnMouseWheel(X, Y) ?? false;
		}

		public bool OnKey(int Repeat, Scancodes Scancode, int Keycode, int Mod, bool Pressed) {
			return GetInputState()?.OnKey(Repeat, Scancode, Keycode, Mod, Pressed) ?? false;
		}

		public bool OnTextInput(string Txt) {
			return GetInputState()?.OnTextInput(Txt) ?? false;
		}

		public void Update(float Dt) {
			for (int i = 0; i < StateStack.Count; i++)
				StateStack[i].Update(Dt);
		}

		public void Render(float Dt) {
			for (int i = 0; i < StateStack.Count; i++)
				StateStack[i].Render(Dt);
		}

		GameState GetInputState() {
			if (StateStack.Count > 0)
				return StateStack.Peek();
			return null;
		}
	}
}
