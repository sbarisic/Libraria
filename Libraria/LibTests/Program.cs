using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;

using Libraria;
using Libraria.Reflection;
using Libraria.Interop;

namespace LibTests {
	public class Program {
		static void Main(string[] args) {
			Console.Title = "LibTests";

			dynamic Native = new NativeBinder(CallingConvention.Winapi);
			Native.Kernel32.Beep((uint)2000, (uint)500);
			Native.User32.MessageBoxA(IntPtr.Zero, "Text", "Caption", 0);

			Console.WriteLine("Done!");
			Console.ReadLine();
		}
	}
}
