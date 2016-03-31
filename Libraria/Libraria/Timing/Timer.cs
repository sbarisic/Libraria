using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

using TimerAction = System.Action<Libraria.Timing.Timer>;

namespace Libraria.Timing {
	public class Timer {
		public static Timer Single(DateTime StartOn, TimerAction A) {
			return Create(DateTime.Now, TimeSpan.Zero, 1, A);
		}

		public static Timer Create(TimeSpan Interval, TimerAction A) {
			return Create(DateTime.Now, Interval, A);
		}

		public static Timer Create(DateTime StartOn, TimeSpan Interval, TimerAction A) {
			return Create(StartOn, Interval, -1, A);
		}

		public static Timer Create(TimeSpan Interval, int LoopCount, TimerAction A) {
			return Create(DateTime.Now, Interval, LoopCount, A);
		}

		public static Timer Create(DateTime StartOn, TimeSpan Interval, int LoopCount, TimerAction A) {
			return new Timer(StartOn, Interval, LoopCount, A);
		}

		public bool Dead { get; private set; }
		public int Loop { get { return CurCount; } }
		public object Userdata;

		DateTime Next;
		TimeSpan Interval;
		int LoopCount, CurCount;
		TimerAction A;

		public Timer(DateTime StartOn, TimeSpan Interval, int LoopCount, TimerAction A) {
			this.A = A;
			Change(StartOn, Interval, LoopCount);
		}

		public void Change(DateTime StartOn, TimeSpan Interval, int LoopCount) {
			this.Interval = Interval;
			this.LoopCount = LoopCount;
			Next = StartOn;
			CurCount = 0;
			Dead = false;
		}

		public void Tick() {
			if (Dead)
				return;
			if (Clock.Elapsed(Next)) {
				if (CurCount < LoopCount || LoopCount == -1) {
					A(this);
					CurCount++;
					Next += Interval;
				} else
					Dead = true;
			}
		}
	}
}