using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;
using Libraria.IO;
using Libraria.Net;

namespace Test {
	class Program {
		static void Main(string[] Args) {
			Tube A = new Tube();
			A.ByteReceived += (Tube, Data) => Console.WriteLine("Received: {0}", Data);

			Tube B = new Tube();
			B.ByteReceived += (Tube, Data) => {
				Console.WriteLine("Received: {0}", Data);
				Tube.WriteByte((byte)(Data + 1));
			};

			A.Connect(B);

			A.WriteByte(24);

			//Console.WriteLine("Done!");
			Console.ReadLine();
		}
	}
}