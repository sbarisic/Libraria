using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		const string Lib = "kernel32";
		const CharSet CSet = CharSet.Ansi;

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern IntPtr LoadLibrary(string FileName);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern bool FreeLibrary(IntPtr Module);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern IntPtr GetProcAddress(IntPtr Module, string ProcName);

		public static T GetProcAddress<T>(IntPtr Module, string ProcName) where T : class {
			if (!typeof(Delegate).IsAssignableFrom(typeof(T)))
				throw new Exception("Type has to be a delegate");

			return Marshal.GetDelegateForFunctionPointer(GetProcAddress(Module, ProcName), typeof(T)) as T;
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern bool VirtualProtect(IntPtr Addr, uint Size, MemProtection NewProtect, out MemProtection OldProtect);

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

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern uint GetModuleFileName(IntPtr Mod, StringBuilder FileName, int Size = 80);

		public static uint GetModuleFileName(IntPtr Mod, StringBuilder FileName) {
			return GetModuleFileName(Mod, FileName, FileName.Capacity);
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern IntPtr GetModuleHandle(string ModuleName);
	}
}