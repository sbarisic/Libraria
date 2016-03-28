using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public unsafe static class Kernel32 {
		const string Lib = "kernel32";
		const CharSet CSet = CharSet.Ansi;
		const CallingConvention CConv = CallingConvention.Winapi;

		public const uint MEM_DECOMMIT = 0x4000;
		public const uint MEM_RELEASE = 0x8000;

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern IntPtr LoadLibrary(string FileName);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool FreeLibrary(IntPtr Module);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern IntPtr GetProcAddress(IntPtr Module, string ProcName);

		public static T GetProcAddress<T>(IntPtr Module, string ProcName) where T : class {
			if (!typeof(Delegate).IsAssignableFrom(typeof(T)))
				throw new Exception("Type has to be a delegate");

			return Marshal.GetDelegateForFunctionPointer(GetProcAddress(Module, ProcName), typeof(T)) as T;
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
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

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern uint GetModuleFileName(IntPtr Mod, StringBuilder FileName, int Size = 80);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool GetModuleHandleEx(ModuleHandleFlags Flags, string ModuleName, out IntPtr Handle);

		public static uint GetModuleFileName(IntPtr Mod, StringBuilder FileName) {
			return GetModuleFileName(Mod, FileName, FileName.Capacity);
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern IntPtr GetModuleHandle(string ModuleName);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool CreateProcess(string AppName, string CmdLine, IntPtr Attributes,
			IntPtr ThreadAttribs, bool InheritHandles, ProcessCreationFlags CFlags,
			IntPtr Env, string Currentdir, ref STARTUPINFO StInfo, out PROCESS_INFORMATION ProcInfo);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool AllocateUserPhysicalPages(IntPtr Process, ref uint NumOfPages, IntPtr PFNArray);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool MapUserPhysicalPages(IntPtr Addr, uint NumOfPages, IntPtr PFNArray);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool AllocConsole();

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool FreeConsole();

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool AttachConsole(int PID);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern int GetProcessId(IntPtr Hnd);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool CloseHandle(IntPtr Hnd);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern IntPtr GetCurrentThread();

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern uint GetCurrentThreadId();

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern IntPtr OpenThread(ThreadAccess Access, bool InheritHandle, uint ThreadID);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern int SuspendThread(IntPtr HThread);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern int ResumeThread(IntPtr Thrd);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern int WaitForSingleObject(IntPtr Handle, uint MS);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern IntPtr OpenProcess(ProcessAccess Access, bool InheritHandle, int PID);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern void FreeLibraryAndExitThread(IntPtr Lib, int Code);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern IntPtr VirtualAllocEx(IntPtr Proc, IntPtr Addr, int Size, AllocType AType = AllocType.Commit,
			MemProtection Prot = MemProtection.ReadWrite);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool VirtualFreeEx(IntPtr Proc, IntPtr Addr, int Size = 0, uint FreeType = MEM_RELEASE);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern IntPtr VirtualAlloc(IntPtr Addr, int Size, AllocType AType = AllocType.Commit, MemProtection Prot = MemProtection.ReadWrite);

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool WriteProcessMemory(IntPtr Proc, IntPtr Addr, byte[] Mem, int Size, ref int BytesWritten);

		public static bool WriteProcessMemory(IntPtr Proc, IntPtr Addr, byte[] Mem) {
			int I = 0;
			return WriteProcessMemory(Proc, Addr, Mem, Mem.Length, ref I);
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool ReadProcessMemory(IntPtr Proc, IntPtr Addr, byte[] Mem, int Size, ref int BytesRead);

		public static byte[] ReadProcessMemory(IntPtr Proc, IntPtr Addr, int Len) {
			int BRead = 0;
			byte[] Ret = new byte[Len];
			if (!ReadProcessMemory(Proc, Addr, Ret, Ret.Length, ref BRead))
				throw new Win32Exception();
			return Ret.Sub(BRead);
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern int GetLastError();

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool TerminateProcess(IntPtr Proc, uint ExitCode);
	}
}