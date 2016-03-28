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
using Libraria.Interop.Memory;
using Libraria.Interop;

namespace LibTests {
	public unsafe class Program {
		static void Main(string[] Args) {
			Console.Title = "LibTests";
			Console.WriteLine("Running!");

			int PID = -1;
			PID = Fork.fork();

			if (PID != 0) {
				Console.WriteLine("Parent! Child: {0}", PID);
			} else if (PID == 0) {
				Console.WriteLine("Child!");
			}

			Console.WriteLine("Almost done!");
			Random Rnd = new Random();
			Console.WriteLine("Random is: {0}", Rnd);
			Console.WriteLine(Rnd.NextDouble());

			Console.WriteLine("Done!");
			Console.ReadLine();
			Environment.Exit(0);
		}
	}
}