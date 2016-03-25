using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public static class NtDll {
		const string Lib = "ntdll";
		const CharSet CSet = CharSet.Ansi;

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int NtResumeProcess(IntPtr Proc);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int NtSuspendProcess(IntPtr Proc);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int NtResumeThread(IntPtr Thread, IntPtr SuspendCount);

		public static int NtResumeThread(IntPtr Thread) {
			return NtResumeThread(Thread, IntPtr.Zero);
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int NtSuspendThread(IntPtr Thread, IntPtr SuspendCount);

		public static int NtSuspendThread(IntPtr Thread) {
			return NtSuspendThread(Thread, IntPtr.Zero);
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int LdrUnloadDll(IntPtr Module);
	}
}