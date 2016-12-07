using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public static class Kernel32 {
		[DllImport("kernel32")]
		public static extern bool AllocConsole();

		[DllImport("kernel32")]
		public static extern IntPtr GetConsoleWindow();

		[DllImport("kernel32", SetLastError = true)]
		public static extern IntPtr VirtualAlloc(IntPtr Addr, IntPtr Size,
			AllocationType AllocType = AllocationType.COMMIT, MemoryProtection MemProtect = MemoryProtection.EXECUTE_READWRITE);

		[DllImport("kernel32", SetLastError = true)]
		public static extern bool VirtualFree(IntPtr Addr, IntPtr Size, AllocationType FreeType);

		public static bool VirtualFree(IntPtr Addr) {
			return VirtualFree(Addr, IntPtr.Zero, AllocationType.RELEASE);
		}
	}
}