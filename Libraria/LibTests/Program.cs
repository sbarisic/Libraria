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

			using (NativeProcess LibTests2 = NativeProcess.Create("LibTests2.exe", "", ProcessCreationFlags.CREATE_SUSPENDED)) {
				LibTests2.Resume();
			}

			Console.WriteLine("Done!");
			Console.ReadLine();
			Environment.Exit(0);
		}
	}
}