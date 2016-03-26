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
		bool Disposed = false;
		IntPtr Proc;

		public NativeProcess(int PID, ProcessAccess Access = ProcessAccess.AllAccess) {
			Proc = Kernel32.OpenProcess(Access, false, PID);
			if (Proc == IntPtr.Zero)
				throw new Win32Exception();
		}

		public NativeProcess(Process P, ProcessAccess Access = ProcessAccess.AllAccess) : this(P.Id, Access) {
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

		public IntPtr LoadLibrary(string Lib) {
			IntPtr RLib = IntPtr.Zero;

			IntPtr LibName = Allocate(Encoding.ASCII.GetBytes(Lib));
			ExecThread(Kernel32.GetProcAddress(Kernel32.GetModuleHandle("kernel32.dll"), "LoadLibraryA"), LibName, true);
			throw new NotImplementedException();

			return RLib;
		}

		public void FreeLibrary(IntPtr Lib) {
			ExecThread(Kernel32.GetProcAddress(Kernel32.GetModuleHandle("kernel32.dll"), "FreeLibrary"), Lib, true);
		}

		public IntPtr GetProcAddress(IntPtr Mod, string Name) {
			throw new NotImplementedException();
		}
	}
}