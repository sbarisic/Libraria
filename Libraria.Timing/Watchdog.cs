using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace Libraria.Timing {
	public enum WatchdogResult {
		Completed = 0,
		Aborted
	}

	public static class Watchdog {
		public static WatchdogResult Run(TimeSpan Time, out TimeSpan CompletedTime, Action A) {
			bool Completed = false;

			Thread WorkerThread = new Thread(() => {
				A();
				Completed = true;
			});
			WorkerThread.IsBackground = true;

			Stopwatch SWatch = Stopwatch.StartNew();
			WorkerThread.Start();
			while (!Completed && SWatch.Elapsed < Time)
				Thread.Sleep(0);
			SWatch.Stop();
			CompletedTime = SWatch.Elapsed;

			if (Completed) {
				return WatchdogResult.Completed;
			} else {
				WorkerThread.Abort();
				return WatchdogResult.Aborted;
			}
		}

		public static WatchdogResult Run(TimeSpan Time, Action A) {
			TimeSpan CompletedTime;
			return Run(Time, out CompletedTime, A);
		}
	}
}