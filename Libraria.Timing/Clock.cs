using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Libraria.Timing {
	public class Clock {
		public static bool Elapsed(DateTime DTime) {
			return (DateTime.Now - DTime).TotalMilliseconds >= 0;
		}

		Stopwatch SWatch;
		DateTime CreationTime;
		float Dt;

		public Clock() {
			SWatch = new Stopwatch();
			CreationTime = DateTime.Now;
			Dt = 0;
		}

		public TimeSpan SinceCreation() {
			return DateTime.Now - CreationTime;
		}

		public TimeSpan Wait(TimeSpan TSpan) {
			SWatch.Restart();
			while (SWatch.Elapsed < TSpan)
				;
			SWatch.Stop();
			return SWatch.Elapsed;
		}

		public TimeSpan AtLeast(TimeSpan TSpan, Action<float> A) {
			SWatch.Restart();

			A(Dt);

			while (SWatch.Elapsed < TSpan)
				;
			SWatch.Stop();
			Dt = (float)SWatch.Elapsed.TotalSeconds;
			return SWatch.Elapsed;
		}
	}
}