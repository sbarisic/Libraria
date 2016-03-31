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
using Libraria.AI;

namespace LibTests {
	public unsafe class Program {
		static void Main(string[] Args) {
			Console.Title = "LibTests";

			BehaviorNetwork N = new BehaviorNetwork();
			N["Hunger"] = 0;

			N.Add((This) => Console.WriteLine("Doin' nothing"), (This) => 42);

			N.Add((This) => {
				Console.WriteLine("Eating; {0}", This["Hunger"]);
				This["Hunger"] = 0;
			}, (This) => {
				return (int)This["Hunger"];
			});

			while (true) {
				Thread.Sleep(100);
				N["Hunger"] = (int)N["Hunger"] + 1;
				N.Decide();
			}

			Console.WriteLine("Done!");
			Console.ReadLine();
			Environment.Exit(0);
		}
	}
}