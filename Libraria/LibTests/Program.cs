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

namespace LibTests {
	class fage {
		public fage() {
		}
	}

	public class Program {
		static void Main(string[] args) {
			Console.Title = "LibTests";

			string Wate = nameof(Wate);
			object Obj = 4.2f;

			Action<string, int, fage> A = (S, I, F) => {
				Console.WriteLine("Wate: {0}; Obj: {1}; String: {2}; Int: {3}; Fage: {4}", Wate, Obj, S, I, F);
			};
			Delegate B = A.CreateDelegate();

			A("iuahdw", 42, new fage());

			B.DynamicInvoke(Activator.CreateInstance(B.Method.GetParamTypes()[0]).SetFieldValue(new {
				Wate = "wotwat",
				Obj = 4.2d
			}), "inawd", 43, new fage());

			Console.WriteLine("Complete");
			Console.ReadLine();
		}
	}
}
