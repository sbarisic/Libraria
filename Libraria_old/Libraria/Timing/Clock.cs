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

		public Clock() {
			SWatch = new Stopwatch();
			CreationTime = DateTime.Now;
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

		public TimeSpan AtLeast(TimeSpan TSpan, Action A) {
			SWatch.Restart();
			A();
			while (SWatch.Elapsed < TSpan)
				;
			SWatch.Stop();
			return SWatch.Elapsed;
		}
	}
}