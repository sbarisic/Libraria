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
using Libraria.Timing;
using Timer = Libraria.Timing.Timer;
using TTimer = System.Threading.Timer;

namespace LibTests {
	public unsafe class Program {
		static void Main(string[] Args) {
			Console.Title = "LibTests";
			Console.WriteLine("Running!");
			
			Console.WriteLine("Done!");
			Console.ReadLine();
			Environment.Exit(0);
		}
	}
}