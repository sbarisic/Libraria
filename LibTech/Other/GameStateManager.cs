using Libraria.Collections;
using Libraria.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace LibTech {
	public abstract class GameState {
		public GameStateManager StateManager;
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

		public virtual bool OnMouseMove(MouseMoveEventArgs E) {
			return false;
		}

		public virtual bool OnMouseButton(MouseButtonEventArgs E, bool Pressed) {
			return false;
		}

		public virtual bool OnMouseWheel(MouseWheelEventArgs E) {
			return false;
		}

		public virtual bool OnKey(KeyboardKeyEventArgs E, bool Pressed) {
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
			State.StateManager = this;
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

		public bool OnMouseMove(MouseMoveEventArgs E) {
			return GetTopState()?.OnMouseMove(E) ?? false;
		}

		public bool OnMouseButton(MouseButtonEventArgs E, bool Pressed) {
			return GetTopState()?.OnMouseButton(E, Pressed) ?? false;
		}

		public bool OnMouseWheel(MouseWheelEventArgs E) {
			return GetTopState()?.OnMouseWheel(E) ?? false;
		}

		public bool OnKey(KeyboardKeyEventArgs E, bool Pressed) {
			return GetTopState()?.OnKey(E, Pressed) ?? false;
		}

		public bool OnTextInput(string Txt) {
			return GetTopState()?.OnTextInput(Txt) ?? false;
		}

		public void Update(float Dt) {
			for (int i = 0; i < StateStack.Count; i++)
				StateStack[i].Update(Dt);
		}

		public void Render(float Dt) {
			for (int i = 0; i < StateStack.Count; i++)
				StateStack[i].Render(Dt);
		}

		public GameState GetTopState() {
			if (StateStack.Count > 0)
				return StateStack.Peek();
			return null;
		}
	}
}
