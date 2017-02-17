using Libraria.Collections;
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

		public void Update(float Dt) {
			for (int i = 0; i < StateStack.Count; i++)
				StateStack[i].Update(Dt);
		}

		public void Render(float Dt) {
			for (int i = 0; i < StateStack.Count; i++)
				StateStack[i].Render(Dt);
		}
	}
}
