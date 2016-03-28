using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
using Libraria;
using Libraria.Native;
using Libraria.Patterns;
using Libraria.Interop;

namespace LibTests {
	public class Program {
		static void Main(string[] args) {
			Console.Title = "LibTests";

			using (NativeProcess TEST = NativeProcess.Create(null, "TEST.exe", ProcessCreationFlags.CREATE_SUSPENDED)) {
				TEST.ExecEmptyThread();
				TEST.Resume();
				Thread.Sleep(500);
				TEST.TerminateProcess();
			}
			
			Console.WriteLine("Done!");
			Console.ReadLine();
			Environment.Exit(0);
		}
	}
}