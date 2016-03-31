using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

namespace Libraria.Timing {
	public class TimerManager {
		HashSet<Timer> Timers;
		Queue<Timer> RemoveQueue;

		public int ActiveTimers
		{
			get
			{
				return Timers.Count;
			}
		}

		public event Action<Timer> TimerRemoved;
		public event Action<Timer> TimerAdded;

		public TimerManager() {
			Timers = new HashSet<Timer>();
			RemoveQueue = new Queue<Timer>();
		}

		public void Add(Timer T) {
			if (Timers.Contains(T))
				throw new Exception("Timer already exists");

			Timers.Add(T);
			if (TimerAdded != null)
				TimerAdded(T);
		}

		public void Add(IEnumerable<Timer> Timers) {
			foreach (var T in Timers)
				Add(T);
		}

		public void Remove(Timer T) {
			if (!Timers.Contains(T))
				throw new Exception("Timer doesn't exist");

			Timers.Remove(T);
			if (TimerRemoved != null)
				TimerRemoved(T);
		}

		public void Remove(IEnumerable<Timer> Timers) {
			foreach (var T in Timers)
				Remove(T);
		}

		public void Tick() {
			RemoveQueue.Clear();

			foreach (var T in Timers) {
				T.Tick();
				if (T.Dead)
					RemoveQueue.Enqueue(T);
			}

			if (RemoveQueue.Count > 0)
				Remove(RemoveQueue);
		}
	}
}