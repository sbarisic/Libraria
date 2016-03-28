using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Libraria.Native;

namespace Libraria.Interop {
	public static unsafe class Fork {
		public static void spoon(out int Res) {
			Res = -1;
			ProcessInfo* PInf = (ProcessInfo*)Marshal.AllocHGlobal(sizeof(ProcessInfo));

			if (NtDll.RtlCloneUserProcess(CloneProcessFlags.CreateSuspended | CloneProcessFlags.InheritHandles,
				IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, PInf) == CloneStatus.Parent) {
				Res = Kernel32.GetProcessId(PInf->ProcInfo.Process);

				Kernel32.ResumeThread(PInf->ProcInfo.Thread);
				Kernel32.CloseHandle(PInf->ProcInfo.Process);
				Kernel32.CloseHandle(PInf->ProcInfo.Thread);
				Marshal.FreeHGlobal(new IntPtr(PInf));
			} else {
				Res = 0;

				if (Kernel32.FreeConsole()) {
					Kernel32.AllocConsole();

					StreamWriter OutWriter = new StreamWriter(Console.OpenStandardOutput());
					OutWriter.AutoFlush = true;
					Console.SetOut(OutWriter);

					StreamWriter ErrWriter = new StreamWriter(Console.OpenStandardError());
					ErrWriter.AutoFlush = true;
					Console.SetError(ErrWriter);

					StreamReader InReader = new StreamReader(Console.OpenStandardInput());
					Console.SetIn(InReader);
				}
			}
		}

		public static int fork() {
			int Val = -2;
			spoon(out Val);
			return Val;
		}
	}
}