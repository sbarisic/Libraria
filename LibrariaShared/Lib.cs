using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Libraria {
	public static class Lib {
		public static string GetProcessExePath() {
			return Assembly.GetEntryAssembly().Location;
		}
	}
}