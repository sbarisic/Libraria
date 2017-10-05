using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	[Flags]
	public enum MemProtection : uint {
		NoAccess = 0x01,
		ReadOnly = 0x02,
		ReadWrite = 0x04,
		WriteCopy = 0x08,
		Exec = 0x10,
		ExecRead = 0x20,
		ExecReadWrite = 0x40,
		ExecWriteCopy = 0x80,
		PageGuard = 0x100,
		NoCache = 0x200,
		WriteCombine = 0x400
	}

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
		
		[DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr GetProcAddress(IntPtr Lib, string ProcName);

		[DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr GetModuleHandle(string ModuleName);

		[DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		public static extern bool VirtualProtect(IntPtr Addr, uint Size, MemProtection NewProtect, out MemProtection OldProtect);

		[DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr LoadLibrary(string Name);

		[DllImport("kernel32")]
		public static extern IntPtr RtlPcToFileHeader(IntPtr PC, out IntPtr Base);

		public static bool VirtualProtect(IntPtr Addr, int Size, MemProtection NewProtect, out MemProtection OldProtect) {
			return VirtualProtect(Addr, (uint)Size, NewProtect, out OldProtect);
		}

		public static bool VirtualProtect(IntPtr Addr, uint Size, MemProtection NewProtect) {
			MemProtection Old;
			return VirtualProtect(Addr, Size, NewProtect, out Old);
		}

		public static bool VirtualProtect(IntPtr Addr, int Size, MemProtection NewProtect) {
			return VirtualProtect(Addr, (uint)Size, NewProtect);
		}

		public static bool VirtualFree(IntPtr Addr) {
			return VirtualFree(Addr, IntPtr.Zero, AllocationType.RELEASE);
		}
	}
}