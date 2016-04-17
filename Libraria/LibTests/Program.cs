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
using Libraria.Interop;
using Libraria.Native;

namespace LibTests {
	static unsafe class NativeTestLib {
		[DllImport("NativeTest.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void NativeTest();
	}


	public unsafe class Program {
		static void Main(string[] Args) {
			Console.Title = "LibTests";

			/*Process[] Processes = Process.GetProcesses().OrderBy((P) => P.ProcessName).ToArray();
			for (int i = 0; i < Processes.Length; i++)
				if (Processes[i].ProcessName == "test.elf") {
					Hax(Processes[i]);
					break;
				}*/

			NativeTestLib.NativeTest();

			Console.WriteLine("Done!");
			Console.ReadLine();
			Environment.Exit(0);
		}

		static void Hax(Process P) {
			using (NativeProcess NP = new NativeProcess(P)) {
				NP.ExecEmptyThread();
			}
		}
	}
}