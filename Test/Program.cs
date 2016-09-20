using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Libraria.Hacks;
using System.Threading;
using System.Runtime.InteropServices;
using Libraria.Security;
using Libraria.Native;

namespace Test {
	class Program {
		static void Main(string[] Args) {
			PrivEsc.Escalate(Args);
			Kernel32.AllocConsole();

			Console.WriteLine("{0}", string.Join(", ", Privileges.GetRoles()));

			Console.WriteLine("Done!");
			Console.ReadLine();
		}
	}
}