using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public static class Kernel32 {
		const string Lib = "kernel32";
		const CharSet CSet = CharSet.Ansi;

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern IntPtr LoadLibrary(string FileName);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern bool FreeLibrary(IntPtr Module);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern IntPtr GetProcAddress(IntPtr Module, string ProcName);
	}
}