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
	public unsafe class Program {
		static void Main(string[] Args) {
			Console.Title = "LibTests";

			IntPtr NativeTest = Kernel32.LoadLibrary("NativeTest.dll");
			Symbol[] Exports = Dll.GetExports(NativeTest);

			for (int i = 0; i < Exports.Length; i++) {
				string Name = Exports[i].Name;
				string Unmangled = DebugHelp.Demangle(Name);

				Console.WriteLine("{0} - {1}", Name, Unmangled);
			}


			Console.WriteLine("Done!");
			Console.ReadLine();
			Environment.Exit(0);
		}
	}
}