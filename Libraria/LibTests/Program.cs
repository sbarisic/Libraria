using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Libraria;

namespace LibTests {
	static class Test {
		public static void Assert(bool B, string ErrorMsg = "Assertion failed") {
			if (!B)
				throw new Exception(ErrorMsg);
		}
	}

	class Program {
		static void Main(string[] args) {
			Console.Title = "LibTests";

			new ValTests();
			new IsValidTests();

			Console.WriteLine("All tests passed");
			Console.ReadLine();
		}
	}
}