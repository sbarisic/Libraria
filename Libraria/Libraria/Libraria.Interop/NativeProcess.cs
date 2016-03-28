using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using Libraria.Native;
using Libraria.IO;

namespace Libraria.Interop {
	public class NativeProcess : IDisposable {
		public static NativeProcess Create(string File, string CmdLine, ProcessCreationFlags Flags,
			bool InheritHandles = false, string CurrentDir = null) {

			STARTUPINFO SInf = new STARTUPINFO();
			PROCESS_INFORMATION PInf = new PROCESS_INFORMATION();
			if (!Kernel32.CreateProcess(File, CmdLine, IntPtr.Zero, IntPtr.Zero, InheritHandles, Flags, IntPtr.Zero, CurrentDir, ref SInf, out PInf))
				throw new Win32Exception();

			int PID = (int)PInf.ClientID.ProcessID;
			Kernel32.CloseHandle(PInf.Process);
			Kernel32.CloseHandle(PInf.Thread);
			return new NativeProcess(PID);
		}

		bool Disposed = false;
		IntPtr Proc;

		public Process Process { get; private set; }
		public IntPtr Handle { get { return Proc; } }

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

		public byte[] ReadProcessMemory(IntPtr Addr, int Len) {
			return Kernel32.ReadProcessMemory(Proc, Addr, Len);
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

		public void ExecEmptyThread(bool Wait = true) {
			ExecThread(Kernel32.GetProcAddress(Kernel32.GetModuleHandle("kernel32.dll"), "ExitThread"), IntPtr.Zero, Wait);
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
			IntPtr LibName = Allocate(Encoding.ASCII.GetBytes(Lib));
			ExecThread(Kernel32.GetProcAddress(Kernel32.GetModuleHandle("kernel32.dll"), "LoadLibraryA"), LibName, true);
			Free(LibName);

			ProcessModule Mod = GetModulesByFileName(Lib).FirstOrDefault();
			if (Mod != null)
				return Mod.BaseAddress;
			return IntPtr.Zero;
		}

		public void FreeLibrary(IntPtr Lib) {
			ExecThread(Kernel32.GetProcAddress(Kernel32.GetModuleHandle("kernel32.dll"), "FreeLibrary"), Lib, true);
		}

		public IEnumerable<ProcessModule> GetModulesByFileName(string FileName) {
			IEnumerable<ProcessModule> Modules = GetProcessModules();
			foreach (var M in Modules) {
				if (PathExtended.IsSameFile(M.FileName, FileName))
					yield return M;
			}
		}

		public IEnumerable<ProcessModule> GetProcessModules() {
			foreach (ProcessModule M in Process.Modules) {
				yield return M;
			}
		}

		public string GetModuleFileName(IntPtr Module) {
			StringBuilder FileName = new StringBuilder(2048);
			PsAPI.GetModuleFileNameEx(Proc, Module, FileName, FileName.MaxCapacity);
			return FileName.ToString();
		}

		public void Resume() {
			Process.ResumeProcess();
		}

		public void Suspend() {
			Process.SuspendProcess();
		}

		public void Suspended(Action A) {
			Suspend();
			A();
			Resume();
		}

		public void WaitForExit() {
			Kernel32.WaitForSingleObject(Proc, uint.MaxValue);
		}

		public bool TerminateProcess(uint ExitCode = 0) {
			return Kernel32.TerminateProcess(Proc, ExitCode);
		}
	}
}