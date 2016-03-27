using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using Libraria.Native;

namespace Libraria.Interop {
	public class NativeProcess : IDisposable {
		public static NativeProcess Create(string File, string CmdLine, ProcessCreationFlags Flags,
			bool InheritHandles = false, string CurrentDir = null) {

			STARTUPINFO SInf = new STARTUPINFO();
			PROCESS_INFORMATION PInf = new PROCESS_INFORMATION();
			if (!Kernel32.CreateProcess(File, CmdLine, IntPtr.Zero, IntPtr.Zero, InheritHandles, Flags, IntPtr.Zero, CurrentDir, ref SInf, out PInf))
				throw new Win32Exception();
			return new NativeProcess((int)PInf.ClientID.ProcessID);
		}

		bool Disposed = false;
		IntPtr Proc;

		public Process Process { get; private set; }

		public NativeProcess(Process P, ProcessAccess Access = ProcessAccess.AllAccess) {
			Process = P;
			Proc = Kernel32.OpenProcess(Access, false, P.Id);
			if (Proc == IntPtr.Zero)
				throw new Win32Exception();
		}

		public NativeProcess(int PID, ProcessAccess Access = ProcessAccess.AllAccess) : this(Process.GetProcessById(PID), Access) {
		}

		public void Dispose() {
			if (Disposed)
				return;
			Disposed = true;
			Kernel32.CloseHandle(Proc);
		}

		public bool WriteProcessMemory(IntPtr Addr, byte[] Bytes) {
			return Kernel32.WriteProcessMemory(Proc, Addr, Bytes);
		}

		public bool WriteProcessMemory(IntPtr Addr, MemoryStream MS) {
			return WriteProcessMemory(Addr, MS.ToArray());
		}

		public int ExecThread(IntPtr Func, IntPtr Param, bool Wait = false) {
			int Ret = 0;
			IntPtr Thread;
			NtDll.RtlCreateUserThread(Proc, Func, Param, out Thread);

			if (Wait)
				Ret = Kernel32.WaitForSingleObject(Thread, uint.MaxValue);
			if (!Kernel32.CloseHandle(Thread))
				throw new Win32Exception();

			return Ret;
		}

		public IntPtr Allocate(byte[] Bytes) {
			IntPtr Mem = Kernel32.VirtualAllocEx(Proc, IntPtr.Zero, Bytes.Length);
			WriteProcessMemory(Mem, Bytes);
			return Mem;
		}

		public bool Free(IntPtr P) {
			return Kernel32.VirtualFreeEx(Proc, P);
		}

		public IntPtr LoadLibrary(string Lib) {
			IntPtr RLib = IntPtr.Zero;

			IntPtr LibName = Allocate(Encoding.ASCII.GetBytes(Lib));
			ExecThread(Kernel32.GetProcAddress(Kernel32.GetModuleHandle("kernel32.dll"), "LoadLibraryA"), LibName, true);
			Free(LibName);

			throw new NotImplementedException();
			return RLib;
		}

		public void FreeLibrary(IntPtr Lib) {
			ExecThread(Kernel32.GetProcAddress(Kernel32.GetModuleHandle("kernel32.dll"), "FreeLibrary"), Lib, true);
		}

		public IntPtr GetProcAddress(IntPtr Mod, string Name) {
			throw new NotImplementedException();
		}

		public IntPtr[] EnumProcessModules(int MaxQuery = 4096) {
			IntPtr[] Mods = new IntPtr[MaxQuery];
			uint SizeNeeded = 0;

			if (Kernel32.EnumProcessModules(Proc, Mods, out SizeNeeded)) {
				return Mods.Sub(SizeNeeded / IntPtr.Size);
			}
			throw new Win32Exception();
		}

		public void Resume() {
			Process.ResumeProcess();
		}

		public void Suspend() {
			Process.SuspendProcess();
		}
	}
}