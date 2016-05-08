using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using Libraria.Native;

namespace Libraria {
	public static unsafe class ProcessUtils {
		public static ProcessThread GetCurrentProcessThread() {
			int CurThreadID = (int)Kernel32.GetCurrentThreadId();
			Process CurProc = Process.GetCurrentProcess();
			foreach (ProcessThread PThread in CurProc.Threads)
				if (PThread.Id == CurThreadID)
					return PThread;
			return null;
		}

		public static Process GetParentProcess(this Process P) {
			PROCESS_BASIC_INFORMATION PBI = new PROCESS_BASIC_INFORMATION();
			int RetLen;
			int Status = 0;

			Status = NtDll.NtQueryInformationProcess(P.Handle, ProcessInformationClasss.ProcessBasicInformation, new IntPtr(&PBI),
				Marshal.SizeOf(PBI), out RetLen);

			if (Status != 0)
				throw new Win32Exception(Status);

			try {
				return Process.GetProcessById(PBI.Reserved3.ToInt32());
			} catch (ArgumentException) {
				return null;
			}
		}

		public static void SuspendProcess(this Process Proc, params ProcessThread[] Except) {
			if (string.IsNullOrEmpty(Proc.ProcessName))
				return;

			foreach (ProcessThread ProcessThread in Proc.Threads) {
				if (Except.Contains(ProcessThread))
					continue;

				IntPtr OpenThread = Kernel32.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)ProcessThread.Id);
				if (OpenThread == IntPtr.Zero)
					continue;

				Kernel32.SuspendThread(OpenThread);
				Kernel32.CloseHandle(OpenThread);
			}
		}

		public static void ResumeProcess(this Process Proc, params ProcessThread[] Except) {
			if (string.IsNullOrEmpty(Proc.ProcessName))
				return;

			foreach (ProcessThread ProcessThread in Proc.Threads) {
				if (Except.Contains(ProcessThread))
					continue;

				IntPtr OpenThread = Kernel32.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)ProcessThread.Id);
				if (OpenThread == IntPtr.Zero)
					continue;

				Kernel32.ResumeThread(OpenThread);
				Kernel32.CloseHandle(OpenThread);
			}
		}
	}
}