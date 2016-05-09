using System;
using System.Collections.Generic;
using System.Text;

namespace Libraria {
	public class EventPool {
		Queue<Action> Events;

		public EventPool() {
			Events = new Queue<Action>();
		}

		public void Add(Action A) {
			Events.Enqueue(A);
		}

		public void Poll() {
			while (Events.Count > 0)
				Events.Dequeue()();
		}
	}
}
