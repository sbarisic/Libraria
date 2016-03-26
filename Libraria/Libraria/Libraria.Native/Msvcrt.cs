using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public unsafe static class Msvcrt {
		const string Lib = "msvcrt";
		const CharSet CSet = CharSet.Ansi;
		const CallingConvention CConv = CallingConvention.Winapi;

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern IntPtr freopen(string Filename, string Mode, IntPtr Stream);
	}
}