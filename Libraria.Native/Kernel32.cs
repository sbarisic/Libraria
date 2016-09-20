using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public static class Kernel32 {
		[DllImport("kernel32")]
		public static extern bool AllocConsole();
	}
}
