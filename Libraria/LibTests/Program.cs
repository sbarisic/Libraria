using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using Libraria;
using Libraria.Native;
using Libraria.Patterns;
using Libraria.Interop;

namespace LibTests {
	public unsafe class Program {
		static CloneStatus Spoon(out int ChildPID) {
			ProcessInfo PInf = new ProcessInfo();
			CloneStatus Stat = NtDll.RtlCloneUserProcess(CloneProcessFlags.InheritHandles | CloneProcessFlags.CreateSuspended, ref PInf);
			ChildPID = -1;

			if (Stat == CloneStatus.Parent) {
				ChildPID = Kernel32.GetProcessId(PInf.ProcInfo.Process);
				Kernel32.ResumeThread(PInf.ProcInfo.Thread);
				Kernel32.CloseHandle(PInf.ProcInfo.Process);
				Kernel32.CloseHandle(PInf.ProcInfo.Thread);
			} else {
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

			return Stat;
		}

		static int fork() {
			int PID;
			if (Spoon(out PID) == CloneStatus.Parent)
				return PID;
			return -1;
		}

		static void Main(string[] args) {
			Console.Title = "LibTests";
			Console.WriteLine("Running!");

			int PID;
			if ((PID = fork()) != -1) {
				Console.WriteLine("Parent! Child PID: {0}", PID);
			} else {
				Console.WriteLine("Child! PID: {0}", Process.GetCurrentProcess().Id);
			}

			Console.WriteLine("Done!");
			Console.ReadLine();
			Environment.Exit(0);
		}
	}
}